namespace StockPilot.Application.Common.Results;

public class OperationResult<T>
{
    public bool IsSuccess { get; }
    public T? Data { get; }
    public string? ErrorCode { get; }
    public string? ErrorMessage { get; }

    private OperationResult(
        bool isSuccess,
        T? data,
        string? errorCode,
        string? errorMessage)
    {
        IsSuccess = isSuccess;
        Data = data;
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
    }

    public static OperationResult<T> Success(T data)
    {
        return new OperationResult<T>(
            true, data, null, null);
    }

    public static OperationResult<T> Failure(
        string errorCode,
        string errorMessage)
    {
        return new OperationResult<T>(
            false, default, errorCode, errorMessage);
    }
}