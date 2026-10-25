using StockPilot.Application.Common.Results;
using StockPilot.Application.DTOs.External;

namespace StockPilot.Application.Interfaces.External;

public interface IZipCodeService
{
    Task<OperationResult<ZipCodeAddress>> FindAsync(
        string zipCode
    );
}