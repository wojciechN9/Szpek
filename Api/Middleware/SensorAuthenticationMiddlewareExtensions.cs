using System;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Szpek.Core.Interfaces;
using Szpek.Infrastructure.SensorContext;

namespace Szpek.Api.Middleware
{
    public static class SensorAuthenticationMiddlewareExtensions
    {
        public static IApplicationBuilder UseSensorAuthentication(
            this IApplicationBuilder builder, PathString pathMatch)
        {
            return builder.UseMiddleware<SensorAuthenticationMiddleware>(pathMatch);
        }
    }

    /// <summary>
    /// Authenticates sensor
    /// Checks Authorization header in the request. Correct header example:
    /// Authorization: SZPEK-HMAC-SHA256 SENSOR_CODE:base64signature
    /// </summary>
    public class SensorAuthenticationMiddleware
    {
        private const string AuthorizationType = "SZPEK-HMAC-SHA256";

        private readonly RequestDelegate _next;
        private readonly PathString _pathMatch;

        public SensorAuthenticationMiddleware(RequestDelegate next, PathString pathMatch)
        {
            _next = next;
            _pathMatch = pathMatch;
        }

        public async Task InvokeAsync(HttpContext context, ISensorRepository sensorRepository, BasicSensorContext sensorContext)
        {
            if (!context.Request.Path.StartsWithSegments(_pathMatch))
            {
                await _next(context);
                return;
            }

            bool authorizationHeaderExists =
                context.Request.Headers.TryGetValue("Authorization", out var authorizationHeader) &&
                authorizationHeader.Count >= 1;

            if (!authorizationHeaderExists)
            {
                await Unauthorized(context, "MISSING_AUTHORIZATION_HEADER");
                return;
            }

            if (!TryParseAuthorizationHeader(authorizationHeader.First(), out var sensorCode, out var signature))
            {
                await Unauthorized(context, $"{AuthorizationType}_INVALID_AUTHORIZATION_HEADER");
                return;
            }

            var sensor = await sensorRepository.Get(sensorCode);
            if (sensor == null)
            {
                await Unauthorized(context, $"{AuthorizationType}_SENSOR_NOT_FOUND");
                return;
            }

            var data = await GetDataToVerify(context);
            if (!CheckSignature(data, signature, sensor.PublicKey))
            {
                await Unauthorized(context, $"{AuthorizationType}_INVALID_SIGNATURE");
                return;
            }

            sensorContext.SetIdentity(sensor);

            await _next(context);

        }

        private async ValueTask<byte[]> GetDataToVerify(HttpContext context)
        {
            if (context.Request.Method == "POST")
            {
                context.Request.EnableBuffering();

                byte[] result;

                if (context.Request.ContentLength.HasValue)
                {
                    result = new byte[(int)context.Request.ContentLength.Value];
                    await context.Request.Body.ReadAsync(result);
                }
                else
                {
                    result = new byte[4096];
                    int pos = 0;
                    int bytesRead;

                    do
                    {
                        int remainingBufferSize = result.Length - pos;
                        if (remainingBufferSize == 0)
                        {
                            Array.Resize(ref result, result.Length * 2);
                            remainingBufferSize = result.Length - pos;
                        }
                        bytesRead = await context.Request.Body.ReadAsync(result, pos, remainingBufferSize);
                        pos += bytesRead;
                    }
                    while (bytesRead > 0);

                    Array.Resize(ref result, pos);
                }
                
                context.Request.Body.Position = 0;

                return result;
            }
            else
            {
                var urlBody = context.Request.Method + context.Request.PathBase.Value + context.Request.Path.Value + context.Request.QueryString.Value;
                return Encoding.ASCII.GetBytes(urlBody);
            }
        }

        private bool TryParseAuthorizationHeader(string authorizationValue, out string sensorCode, out string signature)
        {
            sensorCode = null;
            signature = null;

            if (!authorizationValue.StartsWith(AuthorizationType)) return false;

            authorizationValue = authorizationValue.Remove(0, AuthorizationType.Length);
            authorizationValue = authorizationValue.Trim();

            var authSplit = authorizationValue.Split(':');
            if (authSplit.Length != 2) return false;

            sensorCode = HttpUtility.UrlDecode(authSplit[0]);
            signature = authSplit[1];


            return true;
        }

        private bool CheckSignature(byte[] data, string signatureBase64, string sensorKeyBase64)
        {
            byte[] keyBytes = System.Convert.FromBase64String(sensorKeyBase64);

            if (keyBytes.Length != 32)
            {
                throw new FormatException("Key should have length of 32 bytes!");
            }

            byte[] computedHash = ComputeHMACSHA256(data, keyBytes);
            var signatureHash = Convert.FromBase64String(signatureBase64);

            if (computedHash.Length != signatureHash.Length)
            {
                return false;
            }
            for (int i = 0; i < signatureHash.Length; i++)
            {
                if (computedHash[i] != signatureHash[i]) return false;
            }

            return true;
        }

        private byte[] ComputeHMACSHA256(byte[] data, byte[] key)
        {
            using (HMACSHA256 hmac = new HMACSHA256(key))
            {
                return hmac.ComputeHash(data);
            }
        }

        private async Task Unauthorized(HttpContext context, string message)
        {
            context.Response.ContentType = "text/plain";
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            await context.Response.WriteAsync(message);
        }
    }
}
