using System;
using System.Collections.Generic;
using BeQueue.ServiceFactories;

namespace BeQueue
{
    public class ExecutionPool<TFactory, TService> : IExecutionPool<TService>

        where TFactory : ExecutionServiceFactory<TService>
    {
        private readonly List<ExecuteQueue<TFactory, TService>> _pools =
            new List<ExecuteQueue<TFactory, TService>>();

        private readonly PoolSetting _setting;

        private int _currentIndex;


        private bool _disposing;

        public ExecutionPool(TFactory factoryService, PoolSetting setting)
        {
            if (factoryService == null) throw new ArgumentNullException(nameof(factoryService));

            _setting = setting ?? throw new ArgumentNullException(nameof(setting));
            for (var i = 0; i < setting.PoolSize; i++)
                _pools.Add(new ExecuteQueue<TFactory, TService>(factoryService)
                {
                    ExecuteIntervalTime = setting.ExecuteIntervalTime,
                    ExecuteTimeout = setting.ExecuteTimeout
                });
        }

        /// <summary>
        ///     获取执行队列
        /// </summary>
        /// <returns></returns>
        public ExecuteQueue<TFactory, TService> Get()
        {
            //return item;
            if (_disposing) throw new ObjectDisposedException("ExcutionPools");

            var result = _pools[_currentIndex];

            _currentIndex = _setting.PoolSize - 1 > _currentIndex ? _currentIndex + 1 : 0;

            return result;
        }

        public IExecuteItem Invoke(Action<TService> func)
        {
            var queue = Get();
            var item = new ExecuteItem<TService>
            {
                Action = service => func(service)
            };
            queue.Invoke(item);
            if (item.Exception != null)
                throw item.Exception;

            return item;
        }

        public void Dispose()
        {
            _disposing = true;
            foreach (var item in _pools) item.Dispose();
        }
    }
}