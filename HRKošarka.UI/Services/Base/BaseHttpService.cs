using Blazored.LocalStorage;
using HRKošarka.UI.Services.Base.Common.Responses;

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

        #region Authentication
        protected async Task AddBearerToken()
        {
            if (await _localStorage.ContainKeyAsync("token"))
            {
                var token = await _localStorage.GetItemAsync<string>("token");

                if (_client.HttpClient.DefaultRequestHeaders.Authorization == null)
                {
                    _client.HttpClient.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }
            }
        }
        #endregion

        #region Exception Conversion - CommandResponse
        protected CommandResponse<T> ConvertApiExceptions<T>(ApiException<CustomProblemDetails> ex)
        {
            var validationErrors = new List<string>();
            string message = "Validation failed";

            if (ex.Result != null)
            {
                message = ex.Result.Title ?? "Validation failed";

                if (ex.Result.Errors?.Any() == true)
                {
                    foreach (var errorGroup in ex.Result.Errors)
                    {
                        foreach (var errorMessage in errorGroup.Value)
                        {
                            if (!string.IsNullOrEmpty(errorMessage))
                            {
                                validationErrors.Add(errorMessage);
                            }
                        }
                    }
                }
            }

            return new CommandResponse<T>
            {
                IsSuccess = false,
                Message = message,
                Errors = validationErrors
            };
        }

        protected CommandResponse<T> ConvertApiExceptions<T>(ApiException<ProblemDetails> ex)
        {
            var message = ex.Result?.Title ?? GetUserFriendlyMessage(ex.StatusCode);
            var errors = new List<string>();

            if (!string.IsNullOrEmpty(ex.Result?.Detail))
            {
                errors.Add(ex.Result.Detail);
            }

            return new CommandResponse<T>
            {
                IsSuccess = false,
                Message = message,
                Errors = errors
            };
        }

        protected CommandResponse<T> ConvertApiExceptions<T>(ApiException ex)
        {
            return new CommandResponse<T>
            {
                IsSuccess = false,
                Message = GetUserFriendlyMessage(ex.StatusCode),
                Errors = new List<string> { GetUserFriendlyMessage(ex.StatusCode) }
            };
        }
        #endregion

        #region Exception Conversion - QueryResponse
        protected QueryResponse<T> ConvertApiExceptionsToQuery<T>(ApiException<CustomProblemDetails> ex)
        {
            var commandResponse = ConvertApiExceptions<T>(ex);
            return new QueryResponse<T>
            {
                IsSuccess = commandResponse.IsSuccess,
                Message = commandResponse.Message,
                Errors = commandResponse.Errors
            };
        }

        protected QueryResponse<T> ConvertApiExceptionsToQuery<T>(ApiException<ProblemDetails> ex)
        {
            var commandResponse = ConvertApiExceptions<T>(ex);
            return new QueryResponse<T>
            {
                IsSuccess = commandResponse.IsSuccess,
                Message = commandResponse.Message,
                Errors = commandResponse.Errors
            };
        }

        protected QueryResponse<T> ConvertApiExceptionsToQuery<T>(ApiException ex)
        {
            return new QueryResponse<T>
            {
                IsSuccess = false,
                Message = GetUserFriendlyMessage(ex.StatusCode),
                Errors = new List<string> { GetUserFriendlyMessage(ex.StatusCode) }
            };
        }
        #endregion

        #region Exception Conversion - PaginatedResponse
        protected PaginatedResponse<T> ConvertApiExceptionsToPaginated<T>(ApiException<CustomProblemDetails> ex)
        {
            var commandResponse = ConvertApiExceptions<T>(ex);
            return new PaginatedResponse<T>
            {
                IsSuccess = commandResponse.IsSuccess,
                Message = commandResponse.Message,
                Errors = commandResponse.Errors,
                Data = new List<T>(),
                Pagination = new PaginationMetadata()
            };
        }

        protected PaginatedResponse<T> ConvertApiExceptionsToPaginated<T>(ApiException<ProblemDetails> ex)
        {
            var commandResponse = ConvertApiExceptions<T>(ex);
            return new PaginatedResponse<T>
            {
                IsSuccess = commandResponse.IsSuccess,
                Message = commandResponse.Message,
                Errors = commandResponse.Errors,
                Data = new List<T>(),
                Pagination = new PaginationMetadata()
            };
        }

        protected PaginatedResponse<T> ConvertApiExceptionsToPaginated<T>(ApiException ex)
        {
            return new PaginatedResponse<T>
            {
                IsSuccess = false,
                Message = GetUserFriendlyMessage(ex.StatusCode),
                Errors = new List<string> { GetUserFriendlyMessage(ex.StatusCode) },
                Data = new List<T>(),
                Pagination = new PaginationMetadata()
            };
        }
        #endregion

        #region Exception Conversion - SimpleResponse
        protected SimpleResponse ConvertApiExceptionsToSimple(ApiException<ProblemDetails> ex)
        {
            var message = ex.Result?.Title ?? GetUserFriendlyMessage(ex.StatusCode);
            var errors = new List<string>();

            if (!string.IsNullOrEmpty(ex.Result?.Detail))
            {
                errors.Add(ex.Result.Detail);
            }

            return new SimpleResponse
            {
                IsSuccess = false,
                Message = message,
                Errors = errors
            };
        }

        protected SimpleResponse ConvertApiExceptionsToSimple(ApiException ex)
        {
            var message = GetUserFriendlyMessage(ex.StatusCode);

            return new SimpleResponse
            {
                IsSuccess = false,
                Message = message,
                Errors = new List<string> { message }
            };
        }
        #endregion

        #region Helper Methods
        private string GetUserFriendlyMessage(int statusCode)
        {
            return statusCode switch
            {
                400 => "Bad request",
                401 => "Authentication required",
                403 => "Access denied",
                404 => "Resource not found",
                409 => "Conflict occurred",
                422 => "Validation failed",
                500 => "Server error occurred",
                502 => "Service unavailable",
                503 => "Service temporarily unavailable",
                _ => "An error occurred"
            };
        }
        #endregion
    }
}
