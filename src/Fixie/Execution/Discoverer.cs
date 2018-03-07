﻿namespace Fixie.Execution
{
    using System.Reflection;

    class Discoverer
    {
        readonly Bus bus;
        readonly Filter filter;
        readonly string[] conventionArguments;

        public Discoverer(Bus bus)
            : this(bus, new Filter(), new string[] {}) { }

        public Discoverer(Bus bus, Filter filter, string[] conventionArguments)
        {
            this.bus = bus;
            this.filter = filter;
            this.conventionArguments = conventionArguments;
        }

        public void DiscoverMethods(Assembly assembly)
        {
            var conventions = new ConventionDiscoverer(assembly, conventionArguments).GetConventions();

            DiscoverMethods(assembly, conventions);
        }

        public void DiscoverMethods(Assembly assembly, Convention convention)
        {
            var conventions = new[] { convention };

            DiscoverMethods(assembly, conventions);
        }

        void DiscoverMethods(Assembly assembly, Convention[] conventions)
        {
            foreach (var convention in conventions)
            {
                var classDiscoverer = new ClassDiscoverer(convention);
                var candidateTypes = assembly.GetTypes();
                var testClasses = classDiscoverer.TestClasses(candidateTypes);

                var methodDiscoverer = new MethodDiscoverer(filter, convention);
                foreach (var testClass in testClasses)
                    foreach (var testMethod in methodDiscoverer.TestMethods(testClass))
                        bus.Publish(new MethodDiscovered(testMethod));
            }
        }
    }
}