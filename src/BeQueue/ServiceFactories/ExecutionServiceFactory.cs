namespace BeQueue.ServiceFactories
{
    public abstract class ExecutionServiceFactory<TService>
    {
        /// <summary>
        ///     create instance
        /// </summary>
        /// <returns></returns>
        public abstract TService CreateService();

        /// <summary>
        ///     检查服务是否具备可以状态，如果不可用，会调用 dispose，并且 再次调用 <see cref="CreateService" />  初始化
        /// </summary>
        /// <returns></returns>
        public abstract bool ServiceReady(TService service);
    }
}