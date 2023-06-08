using System;
using System.Threading;
using NLog;
using System.Threading.Tasks;

namespace DWOS.Shared.Utilities
{
    /// <summary>
    /// Provides utility methods for sound-related functionality.
    /// </summary>
    public static class Sound
    {
        /// <summary>
        /// Console speaker beep.
        /// </summary>
        public static void Beep()
        {
            const int count = 1;
            const int tone = 262;

            try
            {

                Task.Factory.StartNew(() => Beep(tone, 400, count));
            }
            catch(Exception exc)
            {
                var errorMsg = "Error beeping.";
                LogManager.GetCurrentClassLogger().Error(exc, errorMsg);
            }
        }

        /// <summary>
        /// Console speaker beep that represents an error.
        /// </summary>
        public static void BeepError()
        {
            const int count = 2;
            const int tone = 392;

            try
            {

                Task.Factory.StartNew(() => Beep(tone, 400, count));
            }
            catch(Exception exc)
            {
                var errorMsg = "Error beeping.";
                LogManager.GetCurrentClassLogger().Error(exc, errorMsg);
            }
        }

        private static void Beep(int tone, int duration, int count)
        {
            const int timeBeforeBeep = 250; // 1/4 second

            for(int i = 0; i < count; i++)
            {
                Thread.Sleep(timeBeforeBeep);
                Console.Beep(tone, duration);
            }
        }
    }
}