using System;
using System.Threading;
using System.Threading.Tasks;

namespace DWOS.Utilities
{
    /// <summary>
    /// Basic Timer implementation for missing Timer in PCL for 4.5
    /// </summary>
    internal sealed class Timer : CancellationTokenSource, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Timer"/> class.
        /// </summary>
        /// <param name="callback">The method called on timer interval.</param>
        /// <param name="state">The current state.</param>
        /// <param name="millisecondsDueTime">Delay time in milliseconds.</param>
        internal Timer(Action<object> callback, object state, int millisecondsDueTime)
        {
            Task.Delay(millisecondsDueTime, Token).ContinueWith((continueTask, continueState) =>
            {
                var tuple = (Tuple<Action<object>, object>)continueState;
                tuple.Item1(tuple.Item2);
            }, Tuple.Create(callback, state), CancellationToken.None,
                TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnRanToCompletion,
                TaskScheduler.Default);
        }

        public new void Dispose() { base.Cancel(); }
    }
}
