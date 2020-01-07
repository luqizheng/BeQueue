using System;

namespace BeQueue
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    public interface IExecutionPool<TService>
    {
        void Invoke(Action<TService> func);
    }
}