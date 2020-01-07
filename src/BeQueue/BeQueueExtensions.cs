using System;
using BeQueue.ServiceFactories;
using Microsoft.Extensions.DependencyInjection;

namespace BeQueue
{
    public static class BeQueueExtensions
    {
        /// <summary>
        /// </summary>
        /// <typeparam name="TServiceFactory"></typeparam>
        /// <typeparam name="TService"></typeparam>
        /// <param name="services"></param>
        /// <param name="setting"></param>
        /// <returns></returns>
        public static IServiceCollection AddBeQueue<TServiceFactory, TService>(this IServiceCollection services,
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
                var result = new ExecutionPool<TServiceFactory, TService>(factory, option);


                return result;
            });

            return services;
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="services"></param>
        /// <param name="setting"></param>
        /// <returns></returns>
        public static IServiceCollection AddBeQueue<TService>(this IServiceCollection services, Action<PoolSetting> setting = null)
        {
            services.AddSingleton(sp =>
            {
                var r = new ExecutionServiceIoCFactory<TService>(sp);
                return (ExecutionServiceFactory<TService>)r;
            });

            services.AddSingleton(sp =>
            {
                PoolSetting pol = new PoolSetting();
                var factory = sp.GetService<ExecutionServiceFactory<TService>>();
                setting?.Invoke(pol);
                return (IExecutionPool<TService>)new ExecutionPool<ExecutionServiceFactory<TService>, TService>(
                    factory, pol);
            });

            return services;
        }

        public static ServiceFactoryBuilder<TService> AddBeQueue<TService>(this IServiceCollection services,
            Func<TService> createServiceFunc, Action<PoolSetting> setting = null)
        {
            var builder = new ServiceFactoryBuilder<TService> { CreateFunc = createServiceFunc };
            services.AddSingleton(builder);
            services.AddSingleton(sp =>
            {
                var builder1 = sp.GetService<ServiceFactoryBuilder<TService>>();
                var result = new ExecutionServiceFactoryAction<TService>(builder1);

                return (ExecutionServiceFactory<TService>)result;
            });

            services.AddSingleton(sp =>
            {
                var option = new PoolSetting();
                if (setting == null) setting(option);
                var factory = sp.GetService<ExecutionServiceFactory<TService>>();
                var result = new ExecutionPool<ExecutionServiceFactory<TService>, TService>(factory, option);


                return (IExecutionPool<TService>)result;
            });

            return builder;
        }

        public static ServiceFactoryBuilder<TService> SetMaxThread<TService>(
            this ServiceFactoryBuilder<TService> builder, int max)
        {
            builder.MaxThread = max;
            return builder;
        }

        public static ServiceFactoryBuilder<TService> SetCreateService<TService>(
            this ServiceFactoryBuilder<TService> builder, Func<TService> createFunc)
        {
            builder.CreateFunc = createFunc;
            return builder;
        }

        public static ServiceFactoryBuilder<TService> SetIsReady<TService>(this ServiceFactoryBuilder<TService> builder,
            Func<TService, bool> isReady)
        {
            builder.IsReady = isReady;
            return builder;
        }
    }
}