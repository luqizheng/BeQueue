using System;
using Microsoft.Extensions.DependencyInjection;

namespace BeQueue.ServiceFactories
{
    public class ExecutionServiceIoCFactory<TService> : ExecutionServiceFactory<TService>
    {
        private readonly IServiceProvider _sp;

        public ExecutionServiceIoCFactory(IServiceProvider sp)
        {
            _sp = sp;
        }

        public override TService CreateService()
        {
            return _sp.CreateScope().ServiceProvider.GetRequiredService<TService>();
        }

        public override bool ServiceReady(TService service)
        {
            return true;
        }
    }
}