﻿namespace Fixie.Tests.Cases
{
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using Assertions;
    using static System.Environment;
    using static Utility;

    public class AsyncCaseTests
    {
        public async Task ShouldAwaitTaskReturningTestsToEnsureCompleteExecution()
        {
            (await RunAsync<SampleTestClass>())
                .ShouldBe(
                    For<SampleTestClass>(
                        ".AwaitTaskThenPass passed",
                        ".AwaitTaskWithResultThenPass passed",
                        ".AwaitValueTaskThenPass passed",
                        ".AwaitValueTaskWithResultThenPass passed",
                        ".CompleteTaskThenPass passed",
                        ".CompleteTaskWithResultThenPass passed",
                        ".FailAfterAwaitTask failed: Expected: 0" + NewLine + "Actual:   3",
                        ".FailAfterAwaitValueTask failed: Expected: 0" + NewLine + "Actual:   3",
                        ".FailBeforeAwaitTask failed: 'FailBeforeAwaitTask' failed!",
                        ".FailBeforeAwaitValueTask failed: 'FailBeforeAwaitValueTask' failed!",
                        ".FailDuringAwaitTask failed: Attempted to divide by zero.",
                        ".FailDuringAwaitValueTask failed: Attempted to divide by zero."
                        ));
        }

        public async Task ShouldPassForNullTask()
        {
            (await RunAsync<NullTaskTestClass>())
                .ShouldBe(
                    For<NullTaskTestClass>(".Test passed"));
        }

        public async Task ShouldFailWithClearExplanationWhenAsyncCaseMethodReturnsNonStartedTask()
        {
            (await RunAsync<FailDueToNonStartedTaskTestClass>())
                .ShouldBe(
                    For<FailDueToNonStartedTaskTestClass>(
                        ".Test failed: The test returned a non-started task, which cannot " +
                        "be awaited. Consider using Task.Run or Task.Factory.StartNew."));
        }

        public async Task ShouldExecuteReturnedTaskDeclaredAsObject()
        {
            (await RunAsync<CompleteTaskDeclaredAsObjectTestClass>())
                .ShouldBe(
                    For<CompleteTaskDeclaredAsObjectTestClass>(
                        ".Test failed: Expected: 0" + NewLine +
                        "Actual:   3"));
        }

        public async Task ShouldFailUnsupportedAsyncVoidCases()
        {
            (await RunAsync<UnsupportedAsyncVoidTestTestClass>())
                .ShouldBe(
                    For<UnsupportedAsyncVoidTestTestClass>(
                        ".Test failed: " +
                        "`async void` test methods are not supported. Declare " +
                        "the test method as `async Task` to ensure the task " +
                        "actually runs to completion."));
        }

        abstract class SampleTestClassBase
        {
            protected static void ThrowException([CallerMemberName] string member = default!)
            {
                throw new FailureException(member);
            }

            protected static Task<int> DivideAsync(int numerator, int denominator)
            {
                return Task.Run(() => numerator/denominator);
            }
        }

        class SampleTestClass : SampleTestClassBase
        {
            public async Task AwaitTaskThenPass()
            {
                var result = await DivideAsync(15, 5);

                result.ShouldBe(3);
            }

            public async Task<bool> AwaitTaskWithResultThenPass()
            {
                var result = await DivideAsync(15, 5);

                result.ShouldBe(3);

                return true;
            }

            public async ValueTask AwaitValueTaskThenPass()
            {
                var result = await DivideAsync(15, 5);

                result.ShouldBe(3);
            }

            public async ValueTask<bool> AwaitValueTaskWithResultThenPass()
            {
                var result = await DivideAsync(15, 5);

                result.ShouldBe(3);

                return true;
            }

            public Task CompleteTaskThenPass()
            {
                var divide = DivideAsync(15, 5);

                return divide.ContinueWith(division =>
                {
                    division.Result.ShouldBe(3);
                });
            }

            public Task<bool> CompleteTaskWithResultThenPass()
            {
                var divide = DivideAsync(15, 5);

                return divide.ContinueWith(division =>
                {
                    division.Result.ShouldBe(3);

                    return true;
                });
            }

            public async Task FailAfterAwaitTask()
            {
                var result = await DivideAsync(15, 5);

                result.ShouldBe(0);
            }

            public async ValueTask FailAfterAwaitValueTask()
            {
                var result = await DivideAsync(15, 5);

                result.ShouldBe(0);
            }

            public async Task FailBeforeAwaitTask()
            {
                ThrowException();

                await DivideAsync(15, 5);
            }

            public async ValueTask FailBeforeAwaitValueTask()
            {
                ThrowException();

                await DivideAsync(15, 5);
            }

            public async Task FailDuringAwaitTask()
            {
                await DivideAsync(15, 0);

                throw new ShouldBeUnreachableException();
            }

            public async ValueTask FailDuringAwaitValueTask()
            {
                await DivideAsync(15, 0);

                throw new ShouldBeUnreachableException();
            }
        }

        class NullTaskTestClass
        {
            public Task? Test()
            {
                // Although unlikely, we must ensure that
                // we don't attempt to wait on a Task that
                // is in fact null.
                return null;
            }
        }

        class FailDueToNonStartedTaskTestClass
        {
            public Task Test()
            {
                return new Task(() => throw new ShouldBeUnreachableException());
            }
        }

        class CompleteTaskDeclaredAsObjectTestClass : SampleTestClassBase
        {
            public object Test()
            {
                var divide = DivideAsync(15, 5);

                return divide.ContinueWith(division =>
                {
                    // Fail within the continuation, so that we can prove
                    // that the task object was fully executed.
                    division.Result.ShouldBe(0);
                });
            }
        }

        class UnsupportedAsyncVoidTestTestClass : SampleTestClassBase
        {
            public async void Test()
            {
                await DivideAsync(15, 5);

                throw new ShouldBeUnreachableException();
            }
        }
    }
}