using System;
namespace MigrationTutorial.Utils
{
    public static class Logger
    {
        public static void LogDebug(string message)
        {
#if DEBUG
            Console.WriteLine($"Debug: {message}");
#endif
        }

        public static void LogWarning(string message)
        {
            Console.WriteLine($"Warning: {message}");
        }

        public static void LogError(string message)
        {
            Console.WriteLine($"Error: {message}");
        }
    }
}
