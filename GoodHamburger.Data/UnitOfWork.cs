using GoodHamburger.Core.Interfaces;
using GoodHamburger.Data.Context;
using Microsoft.EntityFrameworkCore.Storage;

namespace GoodHamburger.Data;

public class UnitOfWork(DataContext context) : IUnitOfWork
{
    private IDbContextTransaction? _transaction;

    public async Task SaveChangesAsync() => await context.SaveChangesAsync();

    public async Task BeginTransactionAsync()
    {
        _transaction = await context.Database.BeginTransactionAsync();
    }

    public async Task CommitAsync()
    {
        if (_transaction is not null)
            await _transaction.CommitAsync();
    }

    public async Task RollbackAsync()
    {
        if (_transaction is not null)
            await _transaction.RollbackAsync();
    }
}
