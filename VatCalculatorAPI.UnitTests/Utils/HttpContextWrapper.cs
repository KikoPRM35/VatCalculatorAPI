using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace VatCalculatorAPI.UnitTests.Utils;

/// <summary>
///  Wrapper for <see cref="HttpContext"/> to be used in unit tests.
/// </summary>
public class HttpContextWrapper : HttpContext
{
    private readonly DefaultHttpContext _context;
    private readonly MemoryStream _bodyStream;

    /// <summary>
    ///  Constructor for <see cref="HttpContextWrapper"/>.
    /// </summary>
    public HttpContextWrapper()
    {
        _bodyStream = new MemoryStream();
        _context = new DefaultHttpContext
        {
            Response =
            {
                Body = _bodyStream
            }
        };
    }
    
    /// <inheritdoc />
    public override HttpResponse Response => _context.Response;
    /// <inheritdoc />
    public override HttpRequest Request => _context.Request;
    /// <inheritdoc />
    public override IFeatureCollection Features => _context.Features;
    /// <inheritdoc />
    public override ConnectionInfo Connection => _context.Connection;
    /// <inheritdoc />
    public override WebSocketManager WebSockets => _context.WebSockets;
    /// <inheritdoc />
    public override ClaimsPrincipal User { get => _context.User; set => _context.User = value; }
    /// <inheritdoc />
    public override IDictionary<object, object?> Items { get => _context.Items; set => _context.Items = value; }
    /// <inheritdoc />
    public override IServiceProvider RequestServices { get => _context.RequestServices; set => _context.RequestServices = value; }
    /// <inheritdoc />
    public override CancellationToken RequestAborted { get => _context.RequestAborted; set => _context.RequestAborted = value; }
    /// <inheritdoc />
    public override string TraceIdentifier { get => _context.TraceIdentifier; set => _context.TraceIdentifier = value; }
    /// <inheritdoc />
    public override ISession Session { get => _context.Session; set => _context.Session = value; }

    /// <inheritdoc />
    public override void Abort() => _context.Abort();

    /// <summary>
    ///  Get the response body as string.
    /// </summary>
    /// <returns>The response body string value.</returns>
    private async Task<string> GetResponseBodyAsync()
    {
        _bodyStream.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(_bodyStream);
        return await reader.ReadToEndAsync(RequestAborted);
    }

    /// <summary>
    ///  Get the response body deserialized.
    /// </summary>
    /// <typeparam name="T">The expected response body object type.</typeparam>
    /// <returns>The response body as string.</returns>
    public async Task<T?> GetResponseBodyAsync<T>()
    {
        var json = await GetResponseBodyAsync();
        return JsonSerializer.Deserialize<T>(json);
    }
}