using System;

namespace BeQueue
{
    public class ServiceFactoryBuilder<TService>
    {
        public Func<TService> CreateFunc { get; set; }
        public Func<TService, bool> IsReady { get; set; }
        public int MaxThread { get; set; }
    }
}