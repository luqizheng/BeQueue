using System;

namespace BeQueue
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    public interface IExecutionPool<TService>
    {
        IExecuteItem Invoke(Action<TService> func);
    }
}