using System;
using System.Threading;
using System.Threading.Tasks;

namespace BeQueue
{
    public interface IExecuteItem
    {
        Exception Exception { get; set; }
        void WaitResult();

        bool? Success { get; set; }

    }
    /// <summary>
    /// </summary>
    /// <typeparam name="T">Services</typeparam>
    public class ExecuteItem<T> : IExecuteItem
    {
        private readonly AutoResetEvent _waitResult = new AutoResetEvent(false);

        /// <summary>
        /// </summary>
        private Thread _thread;



        /// <summary>
        /// </summary>
        public Action<T> Action { get; set; }

        /// <summary>
        /// </summary>
        public bool? Success { get; set; } = false;

        /// <summary>
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public void WaitResult()
        {
            if (!Success.HasValue) _waitResult.WaitOne();

            if (Success != false) return;
            if (Exception != null) throw Exception;
        }

        /// <summary>
        ///     执行。
        /// </summary>
        /// <param name="clrWrapper"></param>
        /// <returns></returns>
        internal Task Execute(T clrWrapper)
        {
            return Task.Run(() =>
            {
                _thread = Thread.CurrentThread;
                try
                {
                    Action.Invoke(clrWrapper);
                    Success = true;
                }
                catch (Exception ex)
                {
                    Exception = ex;
                }
                finally
                {
                    _waitResult.Set();
                }
            });
        }

        internal void Abort()
        {
            Success = false;
            if (_thread.IsAlive) _thread.Abort();

            _waitResult.Set();
        }
    }
}