using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Logging;

namespace DParserverTests.Util
{
    public class InMemoryLoggerProvider : ILoggerProvider
    {
        public static readonly List<string> LoggedThings = new List<string>();
        
        public void Dispose()
        {
            LoggedThings.Clear();
        }

        class InMemoryLogger : ILogger
        {
            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
                Func<TState, Exception, string> formatter)
            {
                var msg = formatter.Invoke(state, exception);
                if (msg == "Read 0 bytes from input stream." || msg == "Reading response headers...")
                {
                    return;
                }
                Console.WriteLine(msg);
                LoggedThings.Add(msg);
            }

            public bool IsEnabled(LogLevel logLevel)
            {
                return true;
            }

            public IDisposable BeginScope<TState>(TState state)
            {
                return new StringReader("");
            }
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new InMemoryLogger();
        }
    }
}