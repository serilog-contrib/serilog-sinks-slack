using System;
using System.Text;

namespace Serilog.Sinks.Slack.Helpers
{
    public static class ExceptionHelper
    {
        public static string GetFlattenedMessage(this Exception exception)
        {
            return GetFlattenedProperty(exception, ex => ex.Message);
        }

        public static string GetFlattenedStackTrace(this Exception exception)
        {
            return GetFlattenedProperty(exception, ex => ex.StackTrace);
        }

        public static string GetFlattenedType(this Exception exception)
        {
            return GetFlattenedProperty(exception, ex => ex.GetType().Name);
        }

        private static string GetFlattenedProperty(Exception exception, Func<Exception, string> exceptionTextSelector)
        {
            var exceptionBuilder = new StringBuilder();
            ExceptionIterator(exception, ref exceptionBuilder, exceptionTextSelector);
            return exceptionBuilder.ToString();
        }

        private static void ExceptionIterator(Exception exception, ref StringBuilder stringBuilder, Func<Exception, string> exceptionTextSelector)
        {
            stringBuilder.Append(exceptionTextSelector(exception));

            if (exception is AggregateException aex)
            {
                var innerExceptions = aex.Flatten().InnerExceptions;
                for (int i = 0; i < (innerExceptions?.Count ?? 0); i++)
                {
                    if (i == 0)
                        stringBuilder.Append(" ---> ");
                    else if (i < innerExceptions.Count - 1)
                        stringBuilder.Append(" | ");

                    ExceptionIterator(innerExceptions[i], ref stringBuilder, exceptionTextSelector);
                }
            }
            else if (exception.InnerException != null)
            {
                stringBuilder.Append(" ---> ");
                ExceptionIterator(exception.InnerException, ref stringBuilder, exceptionTextSelector);
            }
        }
    }
}
