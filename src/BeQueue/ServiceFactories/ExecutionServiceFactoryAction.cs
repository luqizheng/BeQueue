namespace BeQueue.ServiceFactories
{
    public class ExecutionServiceFactoryAction<TService> : ExecutionServiceFactory<TService>
    {
        private readonly ServiceFactoryBuilder<TService> _builder;
      

        public ExecutionServiceFactoryAction(ServiceFactoryBuilder<TService> builder)
        {
            _builder = builder;
       
        }

        public override TService CreateService()
        {
            return _builder.CreateFunc();
        }

        public override bool ServiceReady(TService service)
        {
            if (_builder == null)
                return true;
            return _builder.IsReady(service);
        }
    }
}