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
    public SqlSeatRepository _seatRepository;
    public ISeatRepository SeatRepository => _seatRepository ??= new SqlSeatRepository(_context, _connectionString);
    
    public SqlLocationRepository _locationRepository;
    public ILocationRepository LocationRepository => _locationRepository ??= new SqlLocationRepository(_context, _connectionString);

    public SqlBonusCardRepository _bonusCardRepository;
    public IBonusCardRepository BonusCardRepository => _bonusCardRepository ??= new SqlBonusCardRepository(_context, _connectionString);

    public SqlCountryRepository _countryRepository;
    public ICountryRepository CountryRepository => _countryRepository ??= new SqlCountryRepository(_context, _connectionString);
    
    public SqlTrainTicketRepository _trainTicketRepository;
    public ITrainTicketRepository TrainTicketRepository => _trainTicketRepository ??= new SqlTrainTicketRepository(_context, _connectionString);

    public SqlPlaneTicketRepository _planeTicketRepository;
    public IPlaneTicketRepository PlaneTicketRepository => _planeTicketRepository ??= new SqlPlaneTicketRepository(_context, _connectionString);

    public SqlVariantRepository _variantRepository;
    public IVariantRepository VariantRepository => _variantRepository ??= new SqlVariantRepository(_context, _connectionString);
    public SqlBonusProductRepository _bonusProductRepository;
    public IBonusProductRepository BonusProductRepository => _bonusProductRepository ??= new SqlBonusProductRepository(_context, _connectionString);
    public Task SaveChangesAsync()
    {
        return _context.SaveChangesAsync();
    }
}
