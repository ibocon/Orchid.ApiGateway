using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Orchid.ApiGateway.Middleware;
using Xunit;

namespace Orchid.UnitTests.Gateway;

/// <summary>
/// <see cref="CorrelationIdMiddleware"/>??怨좎쑀 ?앸퀎??諛쒓툒 諛?湲곗〈 ?앸퀎???꾪뙆瑜?寃利앺븯???⑥쐞 ?뚯뒪?몄엯?덈떎.
/// </summary>
public sealed class CorrelationIdMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_WhenHeaderMissing_ShouldGenerateAndAttachCorrelationId()
    {
        // Given
        var context = new DefaultHttpContext();
        var middleware = new CorrelationIdMiddleware(next: (_) => Task.CompletedTask);

        // When
        await middleware.InvokeAsync(context);

        // Then
        context.Request.Headers.ContainsKey("X-Correlation-ID").Should().BeTrue();
        context.Response.Headers.ContainsKey("X-Correlation-ID").Should().BeTrue();
        Guid.TryParse(context.Response.Headers["X-Correlation-ID"].ToString(), out _).Should().BeTrue();
    }

    [Fact]
    public async Task InvokeAsync_WhenHeaderPresent_ShouldPreserveCorrelationId()
    {
        // Given
        string existingId = "existing-custom-trace-id";
        var context = new DefaultHttpContext();
        context.Request.Headers["X-Correlation-ID"] = existingId;

        var middleware = new CorrelationIdMiddleware(next: (_) => Task.CompletedTask);

        // When
        await middleware.InvokeAsync(context);

        // Then
        context.Response.Headers["X-Correlation-ID"].ToString().Should().Be(existingId);
    }
}
