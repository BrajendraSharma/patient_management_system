using ClinicalPatientManagement.Api.DTOs;
using ClinicalPatientManagement.Api.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace ClinicalPatientManagement.Api.Tests.Middleware;

/// <summary>
/// Tests for GlobalExceptionHandlingMiddleware
/// Phase 2: Architectural Improvements - Exception Handling Tests
/// Verifies that exceptions are caught and standardized error responses are returned
/// </summary>
public class ExceptionMiddlewareTests
{
    /// <summary>
    /// Helper method to create an HttpContext with a properly configured service provider
    /// </summary>
    private DefaultHttpContext CreateContextWithServices()
    {
        var services = new ServiceCollection();
        var mockEnvironment = new Mock<IWebHostEnvironment>();
        mockEnvironment.Setup(e => e.EnvironmentName).Returns("Development");
        services.AddSingleton(mockEnvironment.Object);
        
        var context = new DefaultHttpContext
        {
            RequestServices = services.BuildServiceProvider(),
            Response =
            {
                Body = new MemoryStream(),
                ContentType = "application/json"
            }
        };
        return context;
    }

    [Fact]
    public async Task InvokeAsync_WithoutException_CallsNextMiddleware()
    {
        // Arrange
        var context = CreateContextWithServices();
        bool nextCalled = false;

        RequestDelegate next = (ctx) =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        };

        var middleware = new GlobalExceptionHandlingMiddleware(next);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.True(nextCalled);
    }

    [Fact]
    public async Task InvokeAsync_WithArgumentNullException_Returns400BadRequest()
    {
        // Arrange
        var context = CreateContextWithServices();
        context.Request.Path = "/api/test";

        RequestDelegate next = (ctx) =>
        {
            throw new ArgumentNullException("testParam");
        };

        var middleware = new GlobalExceptionHandlingMiddleware(next);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);
        // Content-Type includes charset when WriteAsJsonAsync is called
        Assert.StartsWith("application/json", context.Response.ContentType);
    }

    [Fact]
    public async Task InvokeAsync_WithKeyNotFoundException_Returns404NotFound()
    {
        // Arrange
        var context = CreateContextWithServices();
        context.Request.Path = "/api/test";
        context.Response.Body = new MemoryStream();

        RequestDelegate next = (ctx) =>
        {
            throw new KeyNotFoundException("Resource not found");
        };

        var middleware = new GlobalExceptionHandlingMiddleware(next);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.Equal(StatusCodes.Status404NotFound, context.Response.StatusCode);
    }

    [Fact]
    public async Task InvokeAsync_WithGenericException_Returns500InternalServerError()
    {
        // Arrange
        var context = CreateContextWithServices();
        context.Request.Path = "/api/test";
        context.Response.Body = new MemoryStream();

        RequestDelegate next = (ctx) =>
        {
            throw new Exception("Unexpected error");
        };

        var middleware = new GlobalExceptionHandlingMiddleware(next);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);
    }

    [Fact]
    public async Task InvokeAsync_ResponseContainsErrorResponse()
    {
        // Arrange
        var context = CreateContextWithServices();
        context.Request.Path = "/api/test";

        RequestDelegate next = (ctx) =>
        {
            throw new ArgumentException("Invalid argument");
        };

        var middleware = new GlobalExceptionHandlingMiddleware(next);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var reader = new StreamReader(context.Response.Body);
        var content = await reader.ReadToEndAsync();
        
        Assert.NotEmpty(content);
        // JSON is serialized with camelCase property names (default ASP.NET Core behavior)
        Assert.Contains("statusCode", content);
        Assert.Contains("message", content);
        Assert.Contains("timestamp", content);
    }
}
