using Microsoft.AspNetCore.Diagnostics;
using StockPilot.Domain.Exceptions;

namespace StockPilot.API.Exceptions;

public class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext context,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not SupplierNotFoundException)
            return false;

        context.Response.StatusCode = StatusCodes.Status404NotFound;

        await context.Response.WriteAsJsonAsync(
            new
            {
                code = "SUPPLIER_NOT_FOUND",
                message = exception.Message
            },
            cancellationToken
        );

        return true;
    }
}