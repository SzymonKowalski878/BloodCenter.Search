using Feree.ResultType.Converters;
using Feree.ResultType.Factories;
using Feree.ResultType.Results;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using BloodCenter.Search.Client.Errors;
using Feree.ResultType;
using System.Runtime.CompilerServices;
using System.Text;
using System.Net;
using BloodCenter.Search.Client.Models;
using Feree.ResultType.Errors;

namespace BloodCenter.Search.Client
{
    public abstract class CustomHttpClient
    {
        private readonly HttpClient _httpClient;
        private readonly Uri _baseUri;

        

        protected CustomHttpClient(HttpClient httpClient, Uri baseUri)
        {
            _httpClient = httpClient;
            _baseUri = baseUri;

        }

        
        public async Task<IResult<Unit>> SendPostAsync<TRequest>(string uri, TRequest request)
        {
            var resposne = await _httpClient.PostAsync($"{PrepareUri(uri)}", GetContent<TRequest>(HttpMethod.Post, request));


            return resposne.IsSuccessStatusCode
                ? await ResultFactory.CreateSuccessAsync()
                : await ResultFactory.CreateFailureAsync(new HttpResponseError(await resposne.Content.ReadAsStringAsync(), resposne.StatusCode, Array.Empty<CustomError>()));
        }

        public async Task<IResult<TResponse>> SendGetAsync<TResponse>(string uri)
        {
            var resultUri = PrepareUri(uri);
            var response = await _httpClient.GetAsync(resultUri);

            return response.IsSuccessStatusCode
                ? await Deserialize<TResponse>(await response.Content.ReadAsStringAsync())
                    .BindAsync(response => ResultFactory.CreateSuccessAsync(response))
                : await ExtractError<TResponse>(response);
        }

        public async Task<IResult<TResponse>> SendPostAsync<TRequest, TResponse>(string uri, TRequest request)
        {
            var response = await _httpClient.PostAsync($"{PrepareUri(uri)}", GetContent<TRequest>(HttpMethod.Post, request));


            return response.IsSuccessStatusCode
                ? await Deserialize<TResponse>(await response.Content.ReadAsStringAsync())
                    .BindAsync(response => ResultFactory.CreateSuccessAsync(response))
                : await ExtractError<TResponse>(response);
        }


        public async Task<IResult<T>> ExtractError<T>(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            var type = typeof(T);
            return type == typeof(CustomActionResult) ||
                type.IsGenericType && type.GetGenericTypeDefinition() == typeof(CustomActionResult<>)
                ? Deserialize<CustomActionResult>(content)
                    .Bind(deserialized => ResultFactory.CreateFailure<T>(new HttpResponseError(deserialized.Message, deserialized.StatusCode, deserialized.Errors.Select(x => new CustomError(x.Message, x.PropertyMessage)).ToArray())))
                : ResultFactory.CreateFailure<T>(new Error(content));
        }

        private HttpContent? GetContent<T>(HttpMethod method, T? value)
        {
            if (value is null) return null;

            switch (method.Method)
            {
                case "PATCH":
                    return new StringContent(JsonConvert.SerializeObject(value,new JsonSerializerSettings
                    {

                    }), Encoding.UTF8, "application/json-patch+json");
                case "POST":
                case "PUT":
                case "DELETE":
                    return new StringContent(JsonConvert.SerializeObject(value), Encoding.UTF8, "application/json");
                default:
                    return null;
            }
        }

        private static void SetHeaders(IDictionary<string, string>? headers, HttpRequestMessage request)
        {
            if (headers is null) return;
            foreach (var h in headers.Where(x => x.Key is not null))
            {
                switch (h.Key.ToLower())
                {
                    case "user-agent":
                        if (!string.IsNullOrWhiteSpace(h.Value))
                            request.Headers.TryAddWithoutValidation(h.Key, h.Value);
                        break;

                    default:
                        request.Headers.Add(h.Key, h.Value ?? string.Empty);
                        break;
                }
            }
        }

        private string PrepareUri(string requestUri)
        {
            var part1 = _baseUri.ToString().TrimEnd('/');
            var part2 = requestUri?.TrimStart('/');
            return string.IsNullOrWhiteSpace(part2) ? part1 : $"{part1}/{part2}";
        }



        private static IResult<T> Deserialize<T>(string value)
        {
            //
            bool success = true;
            List<string> errors = new List<string>();

            var settings = new JsonSerializerSettings
            {
                Error = (sender, args) =>
                {
                    success = false;
                    errors.Add(args.ErrorContext.Error.Message);
                    args.ErrorContext.Handled = true;
                },
                Converters = { new StringEnumConverter() },
                NullValueHandling = NullValueHandling.Ignore
            };

            var deserializedValue = JsonConvert.DeserializeObject<T>(value, settings);

            return success
                ? deserializedValue!.AsResult()
                : ResultFactory.CreateFailure<T>(new DeserializationError($"Failed to deserialize type {typeof(T).FullName}", errors));
        }
    }
}
