using System.Reflection;
using FluentAssertions;
using Orchid.ApiGateway.Middleware;
using Xunit;

namespace Orchid.ApiGateway.ArchTest;

public sealed class ApiGatewayBoundaryTest
{
    [Fact]
    public void ApiGatewayAssemblyShouldNotReferenceOtherOrchidAssemblies()
    {
        Assembly gatewayAssembly = typeof(CorrelationIdMiddleware).Assembly;
        string[] orchidRefs = gatewayAssembly
            .GetReferencedAssemblies()
            .Select(a => a.Name ?? string.Empty)
            .Where(name => name.StartsWith("Orchid.", StringComparison.Ordinal))
            .ToArray();

        orchidRefs.Should().BeEmpty();
    }
}