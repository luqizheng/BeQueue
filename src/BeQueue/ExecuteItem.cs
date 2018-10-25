using System;
using System.Threading;
using System.Threading.Tasks;

namespace BeQueue
{
    /// <summary>
    /// </summary>
    /// <typeparam name="T">Services</typeparam>
    public class ExecuteItem<T> where T : IDisposable
    {
        private readonly AutoResetEvent _waitResult = new AutoResetEvent(false);

        /// <summary>
        /// </summary>
        private Thread _thread;

        /// <summary>
        ///     执行结果
        /// </summary>
        private object Result { get; set; }

        /// <summary>
        /// </summary>
        public Func<T, object> Action { get; set; }

        /// <summary>
        /// </summary>
        public bool? Success { get; set; } = false;

        public Exception Exception { get; set; }

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T WaitResult<T>()
        {
            _waitResult.WaitOne();
            if (Success == false)
            {
                if (Exception != null) throw Exception;

                return default(T);
            }

            return (T) Result;
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
                    Result = Action.Invoke(clrWrapper);
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