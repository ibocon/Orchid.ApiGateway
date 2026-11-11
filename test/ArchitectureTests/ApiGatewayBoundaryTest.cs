using System.Reflection;
using FluentAssertions;
using Orchid.ApiGateway.Middleware;
using Xunit;

namespace Orchid.ApiGateway.ArchTests;

public sealed class ApiGatewayBoundaryTest
{
    [Fact]
    public void ApiGatewayAssemblyShouldNotReferenceOtherOrchidAssemblies()
    {
        // Given
        Assembly gatewayAssembly = typeof(CorrelationIdMiddleware).Assembly;

        // When
        string[] orchidRefs = gatewayAssembly
            .GetReferencedAssemblies()
            .Select(a => a.Name ?? string.Empty)
            .Where(name => name.StartsWith("Orchid.", StringComparison.Ordinal))
            .ToArray();

        // Then
        orchidRefs.Should().BeEmpty();
    }

    [Fact]
    public void DockerfileAndAppSettingsShouldUseStandalonePathAndOrchidSubpathTransform()
    {
        // Given
        string repoRoot = FindRepositoryRoot();
        string dockerfile = File.ReadAllText(Path.Combine(repoRoot, "src/Dockerfile"));
        string appSettings = File.ReadAllText(Path.Combine(repoRoot, "src/appsettings.json"));

        // When & Then
        dockerfile.Should().Contain("src/Orchid.ApiGateway.csproj");
        dockerfile.Should().NotContain("src/Service/ApiGateway/");
        appSettings.Should().Contain("/orchid/hubs/clipboard/{**catch-all}");
        appSettings.Should().Contain("\"PathRemovePrefix\": \"/orchid\"");
    }

    [Fact]
    public void AppSettingsShouldDeclareWebSubpathRouteAndWebCluster()
    {
        // Given
        string repoRoot = FindRepositoryRoot();
        string appSettings = File.ReadAllText(Path.Combine(repoRoot, "src/appsettings.json"));

        // When & Then
        appSettings.Should().Contain("\"web-subpath-route\"");
        appSettings.Should().Contain("\"/orchid/{**catch-all}\"");
        appSettings.Should().Contain("http://orchid-web-service:8080");
    }

    private static string FindRepositoryRoot()
    {
        DirectoryInfo? dir = new(AppContext.BaseDirectory);
        while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "Orchid.ApiGateway.slnx")))
        {
            dir = dir.Parent;
        }

        return dir?.FullName ?? throw new InvalidOperationException("Orchid.ApiGateway.slnx not found.");
    }
}
