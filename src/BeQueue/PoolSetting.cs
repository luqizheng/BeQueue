using System;

namespace BeQueue
{
    public class PoolSetting
    {
        /// <summary>
        ///     创建多少个执行队列，默认12个
        /// </summary>
        public int PoolSize { get; set; } = 12;

        /// <summary>
        ///     队列中，每个执行单元执行的时间间隔，默认 200毫秒
        /// </summary>
        public int ExecuteIntervalTime { get; set; } = 200;

        /// <summary>
        ///     超时时间 默认 30秒
        /// </summary>
        public TimeSpan ExecuteTimeout { get; set; } = new TimeSpan(0, 0, 30);
    }
}