using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Logging;

namespace DParserverTests.Util
{
    class InMemoryLoggerProvider : ILoggerProvider
    {
        public void Dispose()
        {
            InMemoryLogger.loggedThings.Clear();
        }

        class InMemoryLogger : ILogger
        {
            public static readonly List<string> loggedThings = new List<string>();

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
                Func<TState, Exception, string> formatter)
            {
                var msg = formatter.Invoke(state, exception);
                Console.WriteLine(msg);
                loggedThings.Add(msg);
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