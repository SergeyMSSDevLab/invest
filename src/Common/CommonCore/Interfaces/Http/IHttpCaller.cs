namespace MssDevLab.CommonCore.Interfaces.Http
{
    public interface IHttpCaller
    {
        Task<ICallerResult<T>> GetAsync<T>(string restApiPath, [System.Runtime.CompilerServices.CallerMemberName] string callerName = "");

        Task<ICallerResult<TReturn>> PostAsync<TReturn>(string restApiPath, IEnumerable<KeyValuePair<string, string>> formVariables, [System.Runtime.CompilerServices.CallerMemberName] string callerName = "");        Task<ICallerResult<TReturn>> PostAsync<TReturn>(string restApiPath);
        Task<ICallerResult<TReturn>> PostLongRunningAsync<TReturn>(string restApiPath);

        Task<ICallerResult<string>> PostGetStringLongRunningAsync(string restApiPath);
        Task<ICallerResult<TReturn>> PostAsync<T, TReturn>(string restApiPath, T entity, [System.Runtime.CompilerServices.CallerMemberName] string callerName = "") where T : class;

        Task<ICallerResult<T>> PutAsync<T>(string restApiPath);
        Task<ICallerResult<TReturn>> PutAsync<T, TReturn>(string restApiPath, T entity, [System.Runtime.CompilerServices.CallerMemberName] string callerName = "") where T : class;

        Task<bool> DeleteAsync(string restApiPath);
    }
}
