namespace Fixie.VisualStudio.TestAdapter
{
    using System;
    using Execution;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;

    public interface IExecutionRecorder
    {
        void RecordResult(
            string fullyQualifiedName,
            string displayName,
            string outcome,
            TimeSpan duration,
            string output,
            string errorMessage,
            string errorStackTrace);
    }

    public class ExecutionRecorder : LongLivedMarshalByRefObject, IExecutionRecorder
    {
        readonly ITestExecutionRecorder log;
        readonly string assemblyPath;

        public ExecutionRecorder(ITestExecutionRecorder log, string assemblyPath)
        {
            this.log = log;
            this.assemblyPath = assemblyPath;
        }

        public void RecordResult(
            string fullyQualifiedName,
            string displayName,
            string outcome,
            TimeSpan duration,
            string output,
            string errorMessage,
            string errorStackTrace)
        {
            var testCase = new TestCase(fullyQualifiedName, VsTestExecutor.Uri, assemblyPath);

            var testResult = new TestResult(testCase)
            {
                DisplayName = displayName,
                Outcome = Parse(outcome),
                Duration = duration,
                ComputerName = Environment.MachineName,

                ErrorMessage = errorMessage,
                ErrorStackTrace = errorStackTrace
            };

            AttachCapturedConsoleOutput(output, testResult);

            log.RecordResult(testResult);
        }

        static TestOutcome Parse(string outcome)
        {
            TestOutcome parsed;

            if (Enum.TryParse(outcome, out parsed))
                return parsed;

            return TestOutcome.None;
        }

        static void AttachCapturedConsoleOutput(string output, TestResult testResult)
        {
            if (!String.IsNullOrEmpty(output))
                testResult.Messages.Add(new TestResultMessage(TestResultMessage.StandardOutCategory, output));
        }
    }
}