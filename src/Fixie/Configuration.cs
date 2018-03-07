﻿namespace Fixie
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    class Configuration
    {
        readonly List<Func<Type, bool>> testClassConditions;
        readonly List<Func<MethodInfo, bool>> testMethodConditions;
        readonly List<ParameterSource> parameterSources;

        public Configuration()
        {
            OrderMethods = methods => methods;
            Lifecycle = new DefaultLifecycle();

            testClassConditions = new List<Func<Type, bool>>();
            testMethodConditions = new List<Func<MethodInfo, bool>>();
            parameterSources = new List<ParameterSource>();
        }

        public Func<IReadOnlyList<MethodInfo>, IReadOnlyList<MethodInfo>> OrderMethods { get; set; }
        public Lifecycle Lifecycle { get; set; }

        class DefaultLifecycle : Lifecycle
        {
            public void Execute(TestClass testClass, Action<CaseAction> runCases)
            {
                runCases(@case =>
                {
                    var instance = testClass.Construct();

                    @case.Execute(instance);

                    instance.Dispose();
                });
            }
        }

        public void AddTestClassCondition(Func<Type, bool> testClassCondition)
            => testClassConditions.Add(testClassCondition);

        public void AddTestMethodCondition(Func<MethodInfo, bool> testMethodCondition)
            => testMethodConditions.Add(testMethodCondition);

        public void AddParameterSource(ParameterSource parameterSource)
            => parameterSources.Add(parameterSource);

        public IReadOnlyList<Func<Type, bool>> TestClassConditions => testClassConditions;
        public IReadOnlyList<Func<MethodInfo, bool>> TestMethodConditions => testMethodConditions;
        public IReadOnlyList<ParameterSource> ParameterSources => parameterSources;
    }
}