using Microsoft.AspNetCore.Http;

namespace FW.Core.WebApi;

public static class WithCancellation
{
    public static async Task<IResult> TryExecute(
        Task task,
        Func<IResult> onCompleted,
        Action<OperationCanceledException> onCancelled,
        CancellationToken cancellationToken)
    {
        try
        {
            await task.WaitAsync(cancellationToken);

            return onCompleted.Invoke();
        }
        catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
        {
            onCancelled.Invoke(ex);
        }

        return Results.NoContent();
    }

    public static async Task<IResult> TryExecute<T>(
        Task<T> task,
        Func<T, IResult> onCompleted,
        Action<OperationCanceledException> onCancelled,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await task.WaitAsync(cancellationToken);

            return onCompleted.Invoke(result);
        }
        catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
        {
            onCancelled.Invoke(ex);
        }

        return Results.NoContent();
    }
}