using GoodHamburger.Core.Interfaces;
using GoodHamburger.Data.Context;
using Microsoft.EntityFrameworkCore.Storage;

namespace GoodHamburger.Data;

public class UnitOfWork(DataContext context) : IUnitOfWork
{
    private IDbContextTransaction? _transaction;

    public async Task SaveChangesAsync(CancellationToken ct = default) =>
        await context.SaveChangesAsync(ct);

    public async Task BeginTransactionAsync(CancellationToken ct = default)
    {
        _transaction = await context.Database.BeginTransactionAsync(ct);
    }

    public async Task CommitAsync(CancellationToken ct = default)
    {
        if (_transaction is not null)
            await _transaction.CommitAsync(ct);
    }

    public async Task RollbackAsync(CancellationToken ct = default)
    {
        if (_transaction is not null)
            await _transaction.RollbackAsync(ct);
    }
}
