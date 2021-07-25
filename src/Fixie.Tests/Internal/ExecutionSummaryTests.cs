﻿namespace Fixie.Tests.Internal
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Assertions;
    using Fixie.Reports;
    using static Utility;

    public class ExecutionSummaryTests
    {
        public async Task ShouldAccumulateTestResultCounts()
        {
            var report = new StubExecutionSummaryReport();
            var discovery = new SelfTestDiscovery();
            var execution = new CreateInstancePerCase();

            await Run(report, discovery, execution, typeof(FirstSampleTestClass), typeof(SecondSampleTestClass));

            report.AssemblySummary.Count.ShouldBe(1);

            var assembly = report.AssemblySummary[0];

            assembly.Passed.ShouldBe(2);
            assembly.Failed.ShouldBe(3);
            assembly.Skipped.ShouldBe(4);
            assembly.Total.ShouldBe(9);
        }

        class StubExecutionSummaryReport :
            IHandler<AssemblyCompleted>
        {
            public List<AssemblyCompleted> AssemblySummary { get; } = new List<AssemblyCompleted>();

            public Task Handle(AssemblyCompleted message)
            {
                AssemblySummary.Add(message);
                return Task.CompletedTask;
            }
        }

        class FirstSampleTestClass
        {
            public void Pass() { }
            public void Fail() { throw new FailureException(); }
            public void Skip() { }
        }

        class SecondSampleTestClass
        {
            public void Pass() { }
            public void FailA() { throw new FailureException(); }
            public void FailB() { throw new FailureException(); }
            public void SkipA() { }
            public void SkipB() { }
            public void SkipC() { }
        }

        class CreateInstancePerCase : IExecution
        {
            public async Task Run(TestSuite testSuite)
            {
                foreach (var test in testSuite.Tests)
                    if (!test.Name.Contains("Skip"))
                        await test.Run();
            }
        }
    }
}