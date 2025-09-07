using System;
using cAlgo.API;

namespace Ultimatum.Utils
{
    public static class Logger
    {
        private static Robot _robot;

        /// <summary>
        /// Initializes the Logger with the cBot instance. Must be called in OnStart.
        /// </summary>
        /// <param name="robot">The current robot instance (this)</param>
        public static void Initialize(Robot robot)
        {
            _robot = robot;
        }

        /// <summary>
        /// Logs an informational message.
        /// </summary>
        public static void Info(string message)
        {
            Log("INFO", message);
        }

        /// <summary>
        /// Logs a warning message.
        /// </summary>
        public static void Warning(string message)
        {
            Log("WARNING", message);
        }

        /// <summary>
        /// Logs an error message, optionally with an exception.
        /// </summary>
        public static void Error(string message, Exception ex = null)
        {
            string finalMessage = message;
            if (ex != null)
            {
                finalMessage = $"{message} | Exception: {ex.Message}";
            }
            Log("ERROR", finalMessage);
        }

        private static void Log(string level, string message)
        {
            if (_robot == null)
            {
                // Failsafe, but initialization should always happen in OnStart.
                return;
            }

            // The Print method is thread-safe.
            _robot.Print($"[{DateTime.UtcNow:HH:mm:ss.fff}] [{level}] {message}");
        }
    }
}
