namespace AiTrainer.Web.Api.SignalR.Models;


public record SignalRClientEvent
{
    public bool IsSuccessful => string.IsNullOrEmpty(ExceptionMessage);
    public string? ExceptionMessage { get; init; }
}
public record SignalRClientEvent<T>: SignalRClientEvent
{
    public T? Data { get; init; }
};