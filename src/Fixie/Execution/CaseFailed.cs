﻿namespace Fixie.Execution
{
    using System;

    public class CaseFailed : CaseCompleted
    {
        public CaseFailed(Case @case, AssertionLibraryFilter filter)
            : base(@case)
        {
            var exception = @case.Exception;

            Exception = new CompoundException(exception, filter);
            Type = exception.GetType().FullName;
            Message = exception.Message;
            FailedAssertion = filter.IsFailedAssertion(exception);
            StackTrace = Exception.StackTrace;
        }

        [Obsolete]
        public CompoundException Exception { get; }

        public string Type { get; }
        public string Message { get; }
        public bool FailedAssertion { get; }
        public string StackTrace { get; }

        public string TypedStackTrace()
        {
            if (FailedAssertion)
                return StackTrace;

            return Type + Environment.NewLine + StackTrace;
        }
    }
}