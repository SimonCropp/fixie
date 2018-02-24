﻿namespace Fixie.Samples.IoC
{
    using System;

    public class CustomConvention : Convention
    {
        public CustomConvention()
        {
            Methods
                .OrderBy(x => x.Name, StringComparer.Ordinal);

            Classes
                .InTheSameNamespaceAs(typeof(CustomConvention))
                .NameEndsWith("Tests");

            ClassExecution
                .Lifecycle<IocLifecycle>();
        }

        public class IocLifecycle : Lifecycle
        {
            public void Execute(Type testClass, Action<CaseAction> runCases)
            {
                using (var container = InitContainerForIntegrationTests())
                {
                    var instance = container.Get(testClass);

                    runCases(@case => @case.Execute(instance));
                }
            }

            static IoCContainer InitContainerForIntegrationTests()
            {
                var container = new IoCContainer();
                container.Add(typeof(IDatabase), typeof(RealDatabase));
                container.Add(typeof(IThirdPartyService), typeof(FakeThirdPartyService));
                return container;
            }
        }
    }
}