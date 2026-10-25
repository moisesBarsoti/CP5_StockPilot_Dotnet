namespace StockPilot.Domain.Exceptions;

public class SupplierNotFoundException : Exception
{
    public SupplierNotFoundException(Guid id) : base($"Fornecedor com ID {id} não encontrado.")
    {
    }
}