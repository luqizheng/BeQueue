using System;
using System.Collections.Generic;

namespace BeQueue
{
    public class ExecutionPool<TFatory, TService>
        where TService : IDisposable
        where TFatory : ExecutionServiceFactory<TService>
    {
        private readonly List<ExecuteQueue<TFatory, TService>> _pools =
            new List<ExecuteQueue<TFatory, TService>>();

        private readonly PoolSetting _setting;

        private int _currentIndex;


        private bool _disposing;

        public ExecutionPool(TFatory factoryService, PoolSetting setting)
        {
            if (factoryService == null) throw new ArgumentNullException(nameof(factoryService));

            _setting = setting ?? throw new ArgumentNullException(nameof(setting));
            for (var i = 0; i < setting.PoolSize; i++)
                _pools.Add(new ExecuteQueue<TFatory, TService>(factoryService)
                {
                    ExecuteIntervalTime = setting.ExecuteIntervalTime,
                    ExecuteTimeout = setting.ExecuteTimeout
                });
        }

        /// <summary>
        ///     获取执行队列
        /// </summary>
        /// <returns></returns>
        public ExecuteQueue<TFatory, TService> Get()
        {
            //return item;
            if (_disposing) throw new ObjectDisposedException("ExcutionPools");

            var result = _pools[_currentIndex];

            _currentIndex = _setting.PoolSize - 1 > _currentIndex ? _currentIndex + 1 : 0;

            return result;
        }

        public TResult Invoke<TResult>(Func<TService, TResult> func)
        {
            var queue = Get();
            var item = new ExecuteItem<TService>
            {
                Action = service => func(service)
            };
            var result = queue.Invoke<TResult>(item);
            if (item.Exception != null)
                throw item.Exception;
         
            return result;
        }

        public void Dispose()
        {
            _disposing = true;
            foreach (var item in _pools) item.Dispose();
        }
    }
}