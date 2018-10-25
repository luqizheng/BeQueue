using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BeQueue
{
    /// <summary>
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    /// <typeparam name="TFactory"></typeparam>
    public class ExecuteQueue<TFactory, TService> where TService : IDisposable
        where TFactory : ExecutionServiceFactory<TService>

    {
        private readonly object _lockExecutTask = new object();

        private readonly ConcurrentQueue<ExecuteItem<TService>> _pools = new ConcurrentQueue<ExecuteItem<TService>>();
        private readonly TFactory _serviceFactory;
        private bool _disposing;


        private Task _executeTask;
        private TService _service;

        public ExecuteQueue(TFactory serviceFactory)
        {
            _serviceFactory = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));
        }

        public TimeSpan ExecuteTimeout { get; set; } = new TimeSpan(0, 0, 10);

        /// <summary>
        ///     毫秒
        /// </summary>
        public int ExecuteIntervalTime { get; set; } = 200;

        /// <summary>
        /// </summary>
        public int Id { get; internal set; }

        private void Execute()
        {
            if (!_serviceFactory.ServiceReady(_service))
            {
                _service?.Dispose();

                _service = _serviceFactory.CreateService();
            }

            if (_executeTask == null || _executeTask.IsCompleted)
                lock (_lockExecutTask)
                {
                    if (_executeTask == null || _executeTask.IsCompleted)
                        _executeTask = Task.Run(() =>
                        {
                            while (_disposing == false && _pools.TryDequeue(out var result))
                            {
                                var executSuccess = ExecuteResultSuccess(_service, result);
                                if (!executSuccess)
                                {
                                    _service?.Dispose();
                                    _service = _serviceFactory.CreateService();
                                }

                                if (_pools.Any()) Thread.Sleep(ExecuteIntervalTime);
                            }
                        });
                }
        }

        private bool ExecuteResultSuccess(TService clrWrapper, ExecuteItem<TService> result)
        {
            try
            {
                var task = result.Execute(clrWrapper).Wait(ExecuteTimeout);

                if (!task) result.Abort();

                return task;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public void Invoke(ExecuteItem<TService> item)
        {
            _pools.Enqueue(item);
        }

        public void Dispose()
        {
            _disposing = true;


            while (_pools.TryDequeue(out var executeItem)) executeItem.Abort();
        }
    }
}