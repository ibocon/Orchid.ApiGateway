using System.Reflection;
using FluentAssertions;
using Orchid.ApiGateway.Middleware;
using Xunit;

namespace Orchid.ApiGateway.ArchTests;

public sealed class ApiGatewayBoundaryTests
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