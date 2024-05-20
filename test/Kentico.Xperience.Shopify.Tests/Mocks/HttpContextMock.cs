using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Text;

using Kentico.Xperience.Shopify.Tests.Constants;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace Kentico.Xperience.Shopify.Tests.Mocks
{
    internal class HttpContextMock : HttpContext
    {
        private readonly Dictionary<string, string> sessionVariables;
        public override ISession Session { get; set; }

        public HttpContextMock(string cartId)
        {
            sessionVariables = new Dictionary<string, string> { { ShopifyTestConstants.CART_SESSION_KEY, cartId } };
            Session = new MockSession()
            {
                Variables = sessionVariables
            };
        }

        public override HttpRequest Request => new HttpRequestMock(sessionVariables);

        #region Not implemented 
        public override IFeatureCollection Features => throw new NotImplementedException();

        public override HttpResponse Response => new HttpResponseMock();

        public override ConnectionInfo Connection => throw new NotImplementedException();

        public override WebSocketManager WebSockets => throw new NotImplementedException();

        public override ClaimsPrincipal User { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override IDictionary<object, object?> Items { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override IServiceProvider RequestServices { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override CancellationToken RequestAborted { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override string TraceIdentifier { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override void Abort() => throw new NotImplementedException();
        #endregion
    }

    internal class HttpResponseMock : HttpResponse
    {
        public HttpResponseMock()
        {
            Cookies = new ResponseCookies();
        }
        public override IResponseCookies Cookies { get; }

        #region Not implemented
        public override HttpContext HttpContext => throw new NotImplementedException();
        public override int StatusCode { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override IHeaderDictionary Headers => throw new NotImplementedException();
        public override Stream Body { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override long? ContentLength { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override string? ContentType { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override bool HasStarted => throw new NotImplementedException();
        public override void OnStarting(Func<object, Task> callback, object state) => throw new NotImplementedException();
        public override void OnCompleted(Func<object, Task> callback, object state) => throw new NotImplementedException();
        public override void Redirect([StringSyntax("Uri")] string location, bool permanent) => throw new NotImplementedException();
        #endregion
    }

    internal class ResponseCookies : IResponseCookies
    {
        private readonly Dictionary<string, string> cookies = [];

        public void Append(string key, string value) => AppendInternal(key, value);
        public void Append(string key, string value, CookieOptions options) => AppendInternal(key, value);
        public void Delete(string key) => DeleteInternal(key);
        public void Delete(string key, CookieOptions options) => DeleteInternal(key);

        private void AppendInternal(string key, string value)
        {
            if (!cookies.TryAdd(key, value))
            {
                cookies[key] = value;
            }
        }
        private void DeleteInternal(string key) => cookies.Remove(key);
    }

    internal class HttpRequestMock : HttpRequest
    {
        public HttpRequestMock(Dictionary<string, string> variables)
        {
            Cookies = new RequestCookiesCollectionMock(variables);
        }

        public override IRequestCookieCollection Cookies { get; set; }

        public override HttpContext HttpContext => throw new NotImplementedException();

        #region Not implemented
        public override string Method { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override string Scheme { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override bool IsHttps { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override HostString Host { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override PathString PathBase { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override PathString Path { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override QueryString QueryString { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override IQueryCollection Query { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override string Protocol { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override IHeaderDictionary Headers => throw new NotImplementedException();
        public override long? ContentLength { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override string? ContentType { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override Stream Body { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override bool HasFormContentType => throw new NotImplementedException();
        public override IFormCollection Form { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override Task<IFormCollection> ReadFormAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException();
        #endregion
    }

    internal class RequestCookiesCollectionMock : IRequestCookieCollection
    {
        private readonly Dictionary<string, string> cookies;

        public RequestCookiesCollectionMock(Dictionary<string, string> cookies)
        {
            this.cookies = cookies;
        }

        public bool TryGetValue(string key, [NotNullWhen(true)] out string? value) => cookies.TryGetValue(key, out value);
        public string? this[string key] => cookies[key];
        public int Count => cookies.Count;
        public ICollection<string> Keys => cookies.Keys;
        public bool ContainsKey(string key) => cookies.ContainsKey(key);
        public IEnumerator<KeyValuePair<string, string>> GetEnumerator() => cookies.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => cookies.GetEnumerator();
    }

    internal class MockSession : ISession
    {
        public required IDictionary<string, string> Variables { get; set; }

        public bool TryGetValue(string key, [NotNullWhen(true)] out byte[]? value)
        {
            value = Encoding.ASCII.GetBytes(GetString(key) ?? string.Empty);
            return true;
        }

        public string? GetString(string key)
        {
            Variables.TryGetValue(key, out string? value);

            return value;
        }

        public void Set(string key, byte[] value)
        {
            string valueString = Encoding.Default.GetString(value);
            if (Variables.ContainsKey(key))
            {
                Variables[key] = valueString;
            }
            else
            {
                Variables.Add(key, valueString);
            }
        }

        #region Not implemented
        public bool IsAvailable => throw new NotImplementedException();

        public string Id => throw new NotImplementedException();

        public IEnumerable<string> Keys => throw new NotImplementedException();

        public void Clear() => throw new NotImplementedException();
        public Task CommitAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task LoadAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public void Remove(string key)
        {
            if (Variables.ContainsKey(key))
            {
                Variables.Remove(key);
            }
        }
        #endregion
    }
}
