using System.IO;
using System.IO.Pipelines;
using System.Threading.Tasks;
using System.Web;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Moq;
using Szpek.Api.Middleware;
using Szpek.Core.Interfaces;
using Szpek.Core.Models;
using Szpek.Infrastructure.SensorContext;
using Xunit;

namespace Szpek.Middleware.IntegrationTests
{
    public class SensorAuthenticationMiddlewareTests
    {
        private const string SensorCode = "SENSOR_CODE";
        private const string Secret = "9NJn6LXPkr5In6QzpGR7N9siyIUmOT+a+b6Zoa9f7So=";

        private readonly SensorAuthenticationMiddleware _middleware;
        private readonly Mock<RequestDelegate> _nextMock;
        private readonly Mock<HttpContext> _httpContextMock;
        private readonly Mock<HttpRequest> _httpRequestMock;
        private readonly Mock<HttpResponse> _httpResponseMock;
        private readonly Mock<IHeaderDictionary> _headerDictionaryMock;
        private readonly MemoryStream _requestMemoryStream;
        private readonly MemoryStream _responseMemoryStream;
        private readonly BasicSensorContext _sensorContext;
        private readonly Mock<ISensorRepository> _sensorRepository;
        private readonly Sensor _sensor;

        public SensorAuthenticationMiddlewareTests()
        {
            _nextMock = new Mock<RequestDelegate>();
            _nextMock.Setup(x => x(It.IsAny<HttpContext>())).Callback(() => { });
            _middleware = new SensorAuthenticationMiddleware(_nextMock.Object, "");

            _headerDictionaryMock = new Mock<IHeaderDictionary>();
            StringValues emptyVals;
            _headerDictionaryMock.Setup(x => x.TryGetValue(It.IsAny<string>(), out emptyVals)).Returns(false);

            _httpRequestMock = new Mock<HttpRequest>();
            _httpRequestMock.Setup(x => x.Headers).Returns(_headerDictionaryMock.Object);
            _requestMemoryStream = new MemoryStream();
            _httpRequestMock.Setup(x => x.Body).Returns(_requestMemoryStream);
            _httpRequestMock.Setup(x => x.BodyReader).Returns(PipeReader.Create(_requestMemoryStream));
            _httpResponseMock = new Mock<HttpResponse>();
            _responseMemoryStream = new MemoryStream();
            _httpResponseMock.Setup(x => x.Body).Returns(_responseMemoryStream);
            _httpResponseMock.Setup(x => x.BodyWriter).Returns(PipeWriter.Create(_responseMemoryStream));

            _httpContextMock = new Mock<HttpContext>();
            _httpContextMock.Setup(x => x.Request).Returns(_httpRequestMock.Object);
            _httpContextMock.Setup(x => x.Response).Returns(_httpResponseMock.Object);

            _sensorContext = new BasicSensorContext();

            _sensor = Sensor.Create(SensorCode, 0, Secret, false);
            _sensorRepository = new Mock<ISensorRepository>();
            _sensorRepository.Setup(x => x.Get(SensorCode)).ReturnsAsync(_sensor);
        }

        [Fact]
        public async Task AuthorizationHeaderMissing()
        {
            await InvokeMiddleware();

            _nextMock.Verify(x => x(_httpContextMock.Object), Times.Never());
            _httpResponseMock.VerifySet(x => x.StatusCode = 401);
            ReadResponseBody().Should().Be("MISSING_AUTHORIZATION_HEADER");
        }

        [Fact]
        public async Task AuthorizationHeaderInvalidType()
        {
            AddAuthorizationHeader("Basic dsadasdasdas");

            await InvokeMiddleware();

            _nextMock.Verify(x => x(_httpContextMock.Object), Times.Never());
            _httpResponseMock.VerifySet(x => x.StatusCode = 401);
            ReadResponseBody().Should().Be("SZPEK-HMAC-SHA256_INVALID_AUTHORIZATION_HEADER");
        }

        [Fact]
        public async Task AuthorizationHeaderMalformed()
        {
            AddAuthorizationHeader("SZPEK-HMAC-SHA256 szpekcodesign");

            await InvokeMiddleware();

            _nextMock.Verify(x => x(_httpContextMock.Object), Times.Never());
            _httpResponseMock.VerifySet(x => x.StatusCode = 401);
            ReadResponseBody().Should().Be("SZPEK-HMAC-SHA256_INVALID_AUTHORIZATION_HEADER");
        }

        [Fact]
        public async Task SensorNotFound()
        {
            AddAuthorizationHeader($"SZPEK-HMAC-SHA256 WrongSensor:some_signature");

            await InvokeMiddleware();

            _nextMock.Verify(x => x(_httpContextMock.Object), Times.Never());
            _httpResponseMock.VerifySet(x => x.StatusCode = 401);
            ReadResponseBody().Should().Be("SZPEK-HMAC-SHA256_SENSOR_NOT_FOUND");
        }

        [Fact]
        public async Task GETSignatureValid()
        {
            // Valid signature(base64 encoded) for "GET/api/device/path?param1=aaa&param2=bbb" == "SVpqXLh/3dk/2UOKqNs2yZCPuEzD8kxgb67y5spNK2A="

            _httpRequestMock.Setup(x => x.Method).Returns("GET");
            _httpRequestMock.Setup(x => x.PathBase).Returns("/api/device");
            _httpRequestMock.Setup(x => x.Path).Returns("/path");
            _httpRequestMock.Setup(x => x.QueryString).Returns(new QueryString("?param1=aaa&param2=bbb"));

            AddAuthorizationHeader($"SZPEK-HMAC-SHA256 {SensorCode}:SVpqXLh/3dk/2UOKqNs2yZCPuEzD8kxgb67y5spNK2A=");

            await InvokeMiddleware();

            _nextMock.Verify(x => x(_httpContextMock.Object), Times.Once());
        }

        [Fact]
        public async Task GETSignatureInvalid()
        {
            _httpRequestMock.Setup(x => x.Method).Returns("GET");
            _httpRequestMock.Setup(x => x.PathBase).Returns("/api/device");
            _httpRequestMock.Setup(x => x.Path).Returns("/path");
            _httpRequestMock.Setup(x => x.QueryString).Returns(new QueryString("?param1=aaa&param2=bbb"));

            AddAuthorizationHeader($"SZPEK-HMAC-SHA256 {SensorCode}:AAAAXLh/3dk/2UOKqNs2yZCPuEzD8kxgb67y5spNK2A=");

            await InvokeMiddleware();

            _nextMock.Verify(x => x(_httpContextMock.Object), Times.Never());
            _httpResponseMock.VerifySet(x => x.StatusCode = 401);
            ReadResponseBody().Should().Be("SZPEK-HMAC-SHA256_INVALID_SIGNATURE");
        }

        [Fact]
        public async Task POSTSignatureValid()
        {
            // Valid signature(base64 encoded) for "{ \"name\": \"value\" }" == "nSvu8BfWlyYZOXBV0unD5FtXZs9UIILf6UO9O4wp/b4="
            _httpRequestMock.Setup(x => x.Method).Returns("POST");
            WriteRequestBody("{ \"name\": \"value\" }");

            AddAuthorizationHeader($"SZPEK-HMAC-SHA256 {SensorCode}:nSvu8BfWlyYZOXBV0unD5FtXZs9UIILf6UO9O4wp/b4=");

            await InvokeMiddleware();

            _nextMock.Verify(x => x(_httpContextMock.Object), Times.Once());
        }

        [Fact]
        public async Task POSTSignatureValid_WithContentLength()
        {
            // Valid signature(base64 encoded) for "{ \"name\": \"value\" }" == "nSvu8BfWlyYZOXBV0unD5FtXZs9UIILf6UO9O4wp/b4="
            string content = "{ \"name\": \"value\" }";
            _httpRequestMock.Setup(x => x.Method).Returns("POST");
            AddHeader("Content-Length", content.Length.ToString());
            _httpRequestMock.Setup(x => x.ContentLength).Returns(content.Length);
            WriteRequestBody(content);

            AddAuthorizationHeader($"SZPEK-HMAC-SHA256 {SensorCode}:nSvu8BfWlyYZOXBV0unD5FtXZs9UIILf6UO9O4wp/b4=");

            await InvokeMiddleware();

            _nextMock.Verify(x => x(_httpContextMock.Object), Times.Once());
        }

        [Fact]
        public async Task POSTSignatureValid_BigBody()
        {
            var buffer = new byte[10000];
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = (byte)(i % 256);
            }
            _httpRequestMock.Setup(x => x.Method).Returns("POST");
            _requestMemoryStream.Write(buffer);
            _requestMemoryStream.Position = 0;

            AddAuthorizationHeader($"SZPEK-HMAC-SHA256 {SensorCode}:YFzGoTD6nVpfZ5kcudtvULBFzD+gNcDVygLQ63Ea6k4=");

            await InvokeMiddleware();

            _nextMock.Verify(x => x(_httpContextMock.Object), Times.Once());
        }

        [Fact]
        public async Task POSTSignatureValid_EmptyBody()
        {
            _httpRequestMock.Setup(x => x.Method).Returns("POST");

            AddAuthorizationHeader($"SZPEK-HMAC-SHA256 {SensorCode}:3KUyPYwA7jHD9u3iAovBF/VGDIb28eUmyZE069Z67Mk=");

            await InvokeMiddleware();

            _nextMock.Verify(x => x(_httpContextMock.Object), Times.Once());
        }

        [Fact]
        public async Task POSTSignatureInvalid()
        {
            _httpRequestMock.Setup(x => x.Method).Returns("POST");
            WriteRequestBody("{ \"name\": \"value\" }");

            AddAuthorizationHeader($"SZPEK-HMAC-SHA256 {SensorCode}:AAAA8BfWlyYZOXBV0unD5FtXZs9UIILf6UO9O4wp/b4=");

            await InvokeMiddleware();

            _nextMock.Verify(x => x(_httpContextMock.Object), Times.Never());
            _httpResponseMock.VerifySet(x => x.StatusCode = 401);
            ReadResponseBody().Should().Be("SZPEK-HMAC-SHA256_INVALID_SIGNATURE");
        }

        [Fact]
        public async Task GETSignatureSensorCodeWithSpaces()
        {
            var sensor = Sensor.Create("SENSOR CODE WITH SPACES", 0, Secret, false);
            _sensorRepository.Setup(x => x.Get("SENSOR CODE WITH SPACES")).ReturnsAsync(_sensor);
            var urlEncodedSensorCode = HttpUtility.UrlEncode("SENSOR CODE WITH SPACES");

            // Valid signature(base64 encoded) for "GET/api/device/path?param1=aaa&param2=bbb" == "SVpqXLh/3dk/2UOKqNs2yZCPuEzD8kxgb67y5spNK2A="

            _httpRequestMock.Setup(x => x.Method).Returns("GET");
            _httpRequestMock.Setup(x => x.PathBase).Returns("/api/device");
            _httpRequestMock.Setup(x => x.Path).Returns("/path");
            _httpRequestMock.Setup(x => x.QueryString).Returns(new QueryString("?param1=aaa&param2=bbb"));

            AddAuthorizationHeader($"SZPEK-HMAC-SHA256 {urlEncodedSensorCode}:SVpqXLh/3dk/2UOKqNs2yZCPuEzD8kxgb67y5spNK2A=");

            await InvokeMiddleware();

            _nextMock.Verify(x => x(_httpContextMock.Object), Times.Once());
        }

        [Fact]
        public async Task SignatureValid_Should_SetDeviceIdentity()
        {
            // Valid signature(base64 encoded) for "GET/api/device/path?param1=aaa&param2=bbb" == "SVpqXLh/3dk/2UOKqNs2yZCPuEzD8kxgb67y5spNK2A="

            _httpRequestMock.Setup(x => x.Method).Returns("GET");
            _httpRequestMock.Setup(x => x.PathBase).Returns("/api/device");
            _httpRequestMock.Setup(x => x.Path).Returns("/path");
            _httpRequestMock.Setup(x => x.QueryString).Returns(new QueryString("?param1=aaa&param2=bbb"));

            AddAuthorizationHeader($"SZPEK-HMAC-SHA256 {SensorCode}:SVpqXLh/3dk/2UOKqNs2yZCPuEzD8kxgb67y5spNK2A=");

            await InvokeMiddleware();

            _sensorContext.IsIdentitySet.Should().BeTrue();
            _sensorContext.SensorCode.Should().Be(SensorCode);
        }

        [Fact]
        public async Task SignatureInvalid_Should_NotSetDeviceIdentity()
        {
            _httpRequestMock.Setup(x => x.Method).Returns("GET");
            _httpRequestMock.Setup(x => x.PathBase).Returns("/api/device");
            _httpRequestMock.Setup(x => x.Path).Returns("/path");
            _httpRequestMock.Setup(x => x.QueryString).Returns(new QueryString("?param1=aaa&param2=bbb"));

            AddAuthorizationHeader($"SZPEK-HMAC-SHA256 {SensorCode}:AAAAXLh/3dk/2UOKqNs2yZCPuEzD8kxgb67y5spNK2A=");

            await InvokeMiddleware();

            _sensorContext.IsIdentitySet.Should().BeFalse();
        }

        private void AddHeader(string name, string value)
        {
            StringValues val = new StringValues(value);
            _headerDictionaryMock.Setup(x => x.TryGetValue(name, out val)).Returns(true);
        }

        private void AddAuthorizationHeader(string value)
        {
            AddHeader("Authorization", value);
        }

        private string ReadResponseBody()
        {
            _responseMemoryStream.Position = 0;
            using (var reader = new StreamReader(_responseMemoryStream))
            {
                return reader.ReadToEnd();
            }
        }

        private void WriteRequestBody(string value)
        {
            using (var writer = new StreamWriter(_requestMemoryStream, leaveOpen: true))
            {
                writer.Write(value);
            }
            _requestMemoryStream.Position = 0;
        }

        private async Task InvokeMiddleware()
        {
            await _middleware.InvokeAsync(_httpContextMock.Object, _sensorRepository.Object, _sensorContext);
        }
    }
}
