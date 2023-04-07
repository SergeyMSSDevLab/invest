namespace MssDevLab.CommonCore.Interfaces.Http
{
    public interface ICallerResult<T> : IHttpCallerResult
    {
        T? Result { get; }
    }
}
