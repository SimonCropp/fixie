﻿using System.Reflection;

namespace Fixie.Internal;

class MethodDiscoverer(IDiscovery discovery)
{
    public IReadOnlyList<MethodInfo> TestMethods(Type testClass)
    {
        try
        {
            return discovery.TestMethods(
                    testClass
                        .GetMethods()
                        .Where(method => method.DeclaringType != typeof(object)))
                .ToList();
        }
        catch (Exception exception)
        {
            throw new Exception(
                "Exception thrown during test method discovery. " +
                "Check the inner exception for more details.", exception);
        }
    }
}