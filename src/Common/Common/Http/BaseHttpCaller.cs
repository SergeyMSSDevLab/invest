using Microsoft.Extensions.Logging;
using MssDevLab.Common.Helpers;
using MssDevLab.CommonCore.Interfaces.Http;
using MssDevLab.CommonCore.Services;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http.Formatting;
using System.Net.Http.Json;

namespace MssDevLab.Common.Http
{
    public abstract class BaseHttpCaller : IHttpCaller
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private HttpClient _client;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        private const int TooLongCallThresholdMilliseconds = 2000;

        private const string IsRequestDataLoggingEnabled = "For additional information enable ApplicationSetting RequestLogging";

        private async Task EnsureHttpClientAsync(TimeSpan? timeOut)
        {
            if (_client == null || HttpClientExpired)
            {
                _client = await CreateHttpClientAsync(timeOut);
            }
        }

        protected bool RequestLogging {get;}

        protected abstract ILogger Log { get; }

        protected virtual bool HttpClientExpired => false;

        protected abstract Task<HttpClient> CreateHttpClientAsync(TimeSpan? timeOut);

        protected abstract HttpClient CreateHttpClient(TimeSpan? timeOut);

        public async Task<bool> DeleteAsync(string restApiPath)
        {
            LogInformation($"DeleteAsync called for: '{restApiPath}'");
            await EnsureHttpClientAsync(null);

            using HttpResponseMessage response = await _client.DeleteAsync(restApiPath);
            LogDebug($"DeleteAsync finished with status code: '{response.StatusCode}'");

            return response.IsSuccessStatusCode;
        }

        public async Task<ICallerResult<T>> GetAsync<T>(string restApiPath, [System.Runtime.CompilerServices.CallerMemberName] string callerName = "")
        {
            LogInformation($"GetAsync called for: '{restApiPath}'");
            await EnsureHttpClientAsync(null);

            return await GetResponseAsync<T, EmptyClass>((p) => _client.GetAsync(restApiPath), null, callerName);
        }

        public async Task<ICallerResult<string>> GetStringResponseAsync(string restApiPath, [System.Runtime.CompilerServices.CallerMemberName] string callerName = "")
        {
            LogInformation($"GetAsync called for: '{restApiPath}'");
            await EnsureHttpClientAsync(null);
            LogDebug($"{callerName} issues a get request", null);
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            ICallerResult<string> returnValue;

            using (HttpResponseMessage response = await _client.GetAsync(restApiPath))
            {
                stopwatch.Stop();
                if (response == null)
                {
                    throw new ServiceException("HttpClient returns null");
                }

                var result = default(string);
                if (response.IsSuccessStatusCode)
                {
                    result = await response.Content.ReadAsStringAsync();
                }
                returnValue = new HttpCallerResult<string>(response, result);
            }

            LogDebugPossiblePersonalData($"{callerName} result: ", returnValue);

            return returnValue;
        }

        public async Task<ICallerResult<TReturn>> PostAsync<TReturn>(string restApiPath, IEnumerable<KeyValuePair<string, string>> formVariables, [System.Runtime.CompilerServices.CallerMemberName] string callerName = "")
        {
            LogInformation($"PostAsync called for: '{restApiPath}'");
            var formContent = new FormUrlEncodedContent(formVariables);
            await EnsureHttpClientAsync(null);

            return await GetResponseAsync<TReturn, FormUrlEncodedContent>((e) => _client.PostAsync(restApiPath, e), formContent, callerName);
        }

        public Task<ICallerResult<TReturn>> PostAsync<TReturn>(string restApiPath)
        {
            return PostAsync<EmptyClass, TReturn>(restApiPath, new EmptyClass());
        }

        public async Task<ICallerResult<string>> PostGetStringLongRunningAsync(string restApiPath)
        {
            LogInformation($"PostGetStringLongRunningAsync called for: '{restApiPath}'");
            TimeSpan timeoutInMilliseconds = TimeSpan.FromMinutes(10);
            await EnsureHttpClientAsync(timeoutInMilliseconds).ConfigureAwait(false);

            return await GetStringResponseAsync(() => _client.PostAsync(restApiPath, null), nameof(PostGetStringLongRunningAsync));
        }

        public async Task<ICallerResult<string>> PostGetStringAsync<T> (string restApiPath, T request, JsonMediaTypeFormatter formatter)
        {
            LogInformation($"PostGetStringAsync called for: '{restApiPath}'");
            await EnsureHttpClientAsync(null);

            return await GetStringResponseAsync(() => _client.PostAsync(restApiPath, request, formatter), nameof(PostGetStringAsync));
        }

        protected async Task<ICallerResult<string>> PutGetStringAsync<T>(string url, T request, JsonMediaTypeFormatter formatter)
        {
            LogInformation($"PostGetStringAsync called for: '{url}'");
            await EnsureHttpClientAsync(null);

            return await GetStringResponseAsync(() => _client.PutAsync(url, request, formatter), nameof(PutGetStringAsync));
        }

        public async Task<ICallerResult<TReturn>> PostLongRunningAsync<TReturn>(string restApiPath)
        {
            LogInformation($"PostLongRunningAsync called for: '{restApiPath}'");
            TimeSpan timeoutInMilliseconds = TimeSpan.FromMinutes(10);
            await EnsureHttpClientAsync(timeoutInMilliseconds).ConfigureAwait(false);

            return await GetResponseAsync<TReturn>(() => _client.PostAsync(restApiPath, null), nameof(PostLongRunningAsync));
        }

        public async Task<ICallerResult<TReturn>> PostAsync<T, TReturn>(string restApiPath, T entity, [System.Runtime.CompilerServices.CallerMemberName] string callerName = "") where T : class
        {
            LogInformation($"PostAsync called for: '{restApiPath}'");
            await EnsureHttpClientAsync(null).ConfigureAwait(false);

            return await GetResponseAsync<TReturn, T>((e) => _client.PostAsJsonAsync(restApiPath, e), entity, callerName).ConfigureAwait(false);
        }

        private async Task<ICallerResult<TReturn>> GetResponseAsync<TReturn, TSend>(
            Func<TSend?, Task<HttpResponseMessage>> callFunc,
            TSend? entity,
            string methodName)
        {
            LogDebugPossiblePersonalData($"{methodName} sends object:", entity);
	        var stopwatch = new Stopwatch();
            stopwatch.Start();
            ICallerResult<TReturn> returnValue;
            
            using (HttpResponseMessage response = await callFunc(entity).ConfigureAwait(false))
            {
                stopwatch.Stop();

                if (response == null)
                {
                    throw new ServiceException("HttpClient returns null");
                }
                
                var result = default(TReturn);

                if (response.IsSuccessStatusCode && response.Content != null)
                {
                    try
                    {
                        result = await response.Content.ReadAsAsync<TReturn>().ConfigureAwait(false);
                    }
                    catch (JsonReaderException exception)
                    {
                        string content = await response.Content.ReadAsStringAsync();
                        LogError($"{methodName} - the response has bad json: {content}", exception);
                        throw;
                    }
                }

                returnValue = new HttpCallerResult<TReturn>(response, result);

                if (stopwatch.ElapsedMilliseconds > TooLongCallThresholdMilliseconds)
                {
	                LogError($"Too long API call: Call has taken longer than {TooLongCallThresholdMilliseconds} mlsec. Method name: {methodName}.");
	                LogErrorPossiblePersonalData($"Too long API call: Time: {stopwatch.ElapsedMilliseconds} Method name: {methodName} Request: ", entity);
	                LogErrorPossiblePersonalData($"Too long API call: Time: {stopwatch.ElapsedMilliseconds} Method name: {methodName} Response: ", returnValue);
                }

                if (!response.IsSuccessStatusCode)
                {
	                LogError($"Boomi call was unsuccessful. Statuscode: {response.StatusCode}. Reasonphrase: {response.ReasonPhrase}.");
                    LogErrorPossiblePersonalData($"{methodName} sends object:", entity);
                    LogErrorPossiblePersonalData($"{methodName} result", returnValue);
                }
            }

            LogDebugPossiblePersonalData($"{methodName} result: ", returnValue);
            LogDebug($"{methodName} took {stopwatch.ElapsedMilliseconds} ms.", null);

            return returnValue;
        }

        private async Task<ICallerResult<string>> GetStringResponseAsync(
            Func<Task<HttpResponseMessage>> callFunc,
            string methodName)
        {
            LogDebug($"{methodName} sends object: empty body", null);
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            ICallerResult<string> returnValue;

            using (HttpResponseMessage response = await callFunc().ConfigureAwait(false))
            {
                stopwatch.Stop();

                if (response == null)
                {
                    throw new ServiceException("HttpClient returns null");
                }

                string? result = null;

                if (response.IsSuccessStatusCode && response.Content!=null)
                {
                    result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                }

                returnValue = new HttpCallerResult<string>(response, result);
            }

            LogDebugPossiblePersonalData($"{methodName} result: ", returnValue);

            return returnValue;
        }

        private async Task<ICallerResult<TReturn>> GetResponseAsync<TReturn>(
           Func<Task<HttpResponseMessage>> callFunc,
           string methodName)
        {
            LogDebug($"{methodName} sends object: empty body", null);
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            ICallerResult<TReturn> returnValue;

            using (HttpResponseMessage response = await callFunc())
            {
                stopwatch.Stop();

                if (response == null)
                {
                    throw new ServiceException("HttpClient returns null");
                }

                var result = default(TReturn);

                if (response.IsSuccessStatusCode && response.Content != null)
                {
                    result = await response.Content.ReadAsAsync<TReturn>();
                }
                
                returnValue = new HttpCallerResult<TReturn>(response, result);
            }

            LogDebugPossiblePersonalData($"{methodName} result", returnValue);

            return returnValue;
        }

        public Task<ICallerResult<T>> PutAsync<T>(string restApiPath)
        {
            return PutAsync<EmptyClass, T>(restApiPath, new EmptyClass());
        }

        public async Task<ICallerResult<TReturn>> PutAsync<T, TReturn>(string restApiPath, T entity, [System.Runtime.CompilerServices.CallerMemberName] string callerName = "") where T : class
        {
            Log.LogInformation("PutAsync called for: '{restApiPath}'", restApiPath);
            await EnsureHttpClientAsync(null).ConfigureAwait(false);

            return await GetResponseAsync<TReturn, T>((e) => _client.PutAsJsonAsync(restApiPath, e), entity, callerName).ConfigureAwait(false);
        }

        private class EmptyClass
        {
        }

        private void LogInformation(string message)
        {
            Log.LogInformation("{message}", message);
        }

        private void LogDebug(string message)
        {
                Log.LogDebug("{message}", message);
        }

        private void LogDebug(string message, object? data)
        {
            string text;
            if (data != null)
            {
                text = $"{message}: {JsonConvert.SerializeObject(data)}";
            }
            else
            {
                text = $"{message}";
            }
            Log.LogDebug("{text}", text);
        }

        private void LogError(string message)
        {
            Log.LogError("{message}", message);
        }

        private void LogError(string message, object? data)
        {
            if (data != null)
            {
                Log.LogError("{message}", message + JsonHelper.GetJson(data));
            }
            else
            {
                Log.LogError("{message}", message);
            }
        }

        private void LogError(string message, Exception exception)
        {
            Log.LogError(exception, "{message}", message);
        }

        private void LogErrorPossiblePersonalData(string message, object? possiblePersonalDataObject)
        {
	        if (RequestLogging)
	        {
		        LogError(message, possiblePersonalDataObject);
		        return;
	        }

	        LogError(message + IsRequestDataLoggingEnabled);
        }

        private void LogDebugPossiblePersonalData(string message, object? possiblePersonalDataObject)
        {
	        if (RequestLogging)
	        {
		        LogDebug(message, possiblePersonalDataObject);
		        return;

	        }
            LogDebug(message + IsRequestDataLoggingEnabled);
        }
    }
}
