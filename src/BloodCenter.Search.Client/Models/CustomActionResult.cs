using Newtonsoft.Json.Converters;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Http;

namespace BloodCenter.Search.Client.Models
{
    public class CustomActionResult : IActionResult
    {
        public string Message { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public CustomActionResultError[] Errors { get; set; }

        public CustomActionResult(HttpStatusCode statusCode, string? message = null, CustomActionResultError[]? errors = null)
        {
            Message = message ?? string.Empty;
            StatusCode = statusCode;
            Errors = errors ?? Array.Empty<CustomActionResultError>();
        }

        public Task ExecuteResultAsync(ActionContext context)
        {
            context.HttpContext.Response.StatusCode = (int)StatusCode;
            context.HttpContext.Response.ContentType = "application/json";
            return context.HttpContext.Response.WriteAsync(JsonConvert.SerializeObject(this, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.None,
                Converters = { new StringEnumConverter() },
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            }));
        }
    }

    public class CustomActionResult<T> : IActionResult
    {
        public T? Value { get; set; }
        public string Message { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public CustomActionResultError[] Errors { get; set; }

        public CustomActionResult(T? value,  HttpStatusCode statusCode, string? message = null, CustomActionResultError[]? errors = null)
        {
            Value = value;
            Message = message ?? string.Empty;
            StatusCode = statusCode;
            Errors = errors ?? Array.Empty<CustomActionResultError>();
        }

        public Task ExecuteResultAsync(ActionContext context)
        {
            context.HttpContext.Response.StatusCode = (int)StatusCode;
            context.HttpContext.Response.ContentType = "application/json";
            return context.HttpContext.Response.WriteAsync(JsonConvert.SerializeObject(this, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.None,
                Converters = { new StringEnumConverter() },
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            }));
        }
    }
}
