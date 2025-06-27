using Blazored.LocalStorage;
using HRKošarka.UI.Services.Base.Common.Responses;
using System.Text.Json;

namespace HRKošarka.UI.Services.Base
{
    public class BaseHttpService
    {
        protected IClient _client;
        protected readonly ILocalStorageService _localStorage;

        public BaseHttpService(IClient client, ILocalStorageService localStorage)
        {
            _client = client;
            _localStorage = localStorage;
        }

        protected CommandResponse<T> ConvertApiExceptions<T>(ApiException ex)
        {
            var backendMessage = ExtractBackendMessage(ex);

            return new CommandResponse<T>
            {
                IsSuccess = false,
                Message = GetUserFriendlyMessage(ex.StatusCode),
                Errors = new List<string> { backendMessage }
            };
        }

        protected QueryResponse<T> ConvertApiExceptionsToQuery<T>(ApiException ex)
        {
            var backendMessage = ExtractBackendMessage(ex);

            return new QueryResponse<T>
            {
                IsSuccess = false,
                Message = GetUserFriendlyMessage(ex.StatusCode),
                Errors = new List<string> { backendMessage }
            };
        }

        protected PaginatedResponse<T> ConvertApiExceptionsToPaginated<T>(ApiException ex)
        {
            var backendMessage = ExtractBackendMessage(ex);

            return new PaginatedResponse<T>
            {
                IsSuccess = false,
                Message = GetUserFriendlyMessage(ex.StatusCode),
                Errors = new List<string> { backendMessage },
                Data = new List<T>(),
                Pagination = new PaginationMetadata()
            };
        }

        protected async Task AddBearerToken()
        {
            if (await _localStorage.ContainKeyAsync("token"))
            {
                var token = await _localStorage.GetItemAsync<string>("token");
                _client.HttpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }


        protected string ExtractBackendMessage(ApiException ex)
        {
            try
            {
                if (!string.IsNullOrEmpty(ex.Response))
                {
                    using var document = JsonDocument.Parse(ex.Response);

                    if (document.RootElement.TryGetProperty("title", out var title))
                    {
                        return title.GetString() ?? "An error occurred";
                    }


                    if (document.RootElement.TryGetProperty("message", out var message))
                    {
                        return message.GetString() ?? "An error occurred";
                    }
                }
            }
            catch { }

            return ex.Response ?? "An error occurred";
        }

        private string GetUserFriendlyMessage(int statusCode)
        {
            return statusCode switch
            {
                400 => "Invalid request",
                401 => "Authentication failed",
                403 => "Access denied",
                404 => "Resource not found",
                500 => "Server error occurred",
                _ => "An error occurred"
            };
        }
    }
}
