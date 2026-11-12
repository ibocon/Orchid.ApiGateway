using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Orchid.ApiGateway.Middleware;
using Xunit;

namespace Orchid.UnitTest.Gateway;

/// <summary>
/// <see cref="CorrelationIdMiddleware"/>??怨좎쑀 ?앸퀎??諛쒓툒 諛?湲곗〈 ?앸퀎???꾪뙆瑜?寃利앺븯???⑥쐞 ?뚯뒪?몄엯?덈떎.
/// </summary>
public sealed class CorrelationIdMiddlewareTest
{
    [Fact]
    public async Task GivenHeaderMissingWhenInvokedThenGeneratesAndAttachesCorrelationId()
    {
        // Given
        DefaultHttpContext context = new();
        CorrelationIdMiddleware middleware = new(next: (_) => Task.CompletedTask);

        // When
        await middleware.InvokeAsync(context);

        // Then
        context.Request.Headers.ContainsKey("X-Correlation-ID").Should().BeTrue();
        context.Response.Headers.ContainsKey("X-Correlation-ID").Should().BeTrue();
        bool parsed = Guid.TryParse(context.Response.Headers["X-Correlation-ID"].ToString(), out _);
        parsed.Should().BeTrue();
    }

    [Fact]
    public async Task GivenHeaderPresentWhenInvokedThenPreservesCorrelationId()
    {
        // Given
        string existingId = "existing-custom-trace-id";
        DefaultHttpContext context = new();
        context.Request.Headers["X-Correlation-ID"] = existingId;

        CorrelationIdMiddleware middleware = new(next: (_) => Task.CompletedTask);

        // When
        await middleware.InvokeAsync(context);

        // Then
        context.Response.Headers["X-Correlation-ID"].ToString().Should().Be(existingId);
    }
}
