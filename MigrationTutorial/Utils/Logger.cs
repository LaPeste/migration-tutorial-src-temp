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
    }
}
