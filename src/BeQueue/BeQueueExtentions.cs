using System;
using Microsoft.Extensions.DependencyInjection;

namespace BeQueue
{
    public static class BeQueueExtentions
    {
        public static IServiceCollection AddBeQueueSerivce<TServiceFactory, TService>(this IServiceCollection services,
            Action<PoolSetting> setting = null
        )
            where TServiceFactory : ExecutionServiceFactory<TService>
            where TService : IDisposable
        {
            services.AddSingleton<TServiceFactory>();
            services.AddSingleton(sp =>
            {
                var option = new PoolSetting();
                setting?.Invoke(option);
                var factory = sp.GetService<TServiceFactory>();
                return new ExecutionPool<TServiceFactory, TService>(factory, option);
            });

            return services;
        }
    }
}