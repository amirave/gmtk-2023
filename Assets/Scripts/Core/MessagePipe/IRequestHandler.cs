namespace Core
{
    public interface IRequestHandler<in TRequest, out TResponse>
    {
        TResponse Invoke(TRequest request);
    }
}