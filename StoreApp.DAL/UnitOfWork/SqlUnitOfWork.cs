using StoreApp.DAL.Context;
using StoreApp.DAL.Infrastructure;
using StoreApp.Repository.Comman;
using StoreApp.Repository.Repositories;
using System;

namespace StoreApp.DAL.UnitOfWork;

public class SqlUnitOfWork : IUnitOfWork
{
    private readonly string _connectionString;
    private readonly StoreAppDbContext _context;

    public SqlUnitOfWork(string connectionString, StoreAppDbContext context)
    {
        _connectionString = connectionString;
        _context = context;
    }
    public Task SaveChangesAsync()
    {
        return _context.SaveChangesAsync();
    }
}
