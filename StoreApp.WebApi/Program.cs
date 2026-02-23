using Microsoft.EntityFrameworkCore;
using StoreApp.Application;
using StoreApp.DAL.Context;
using StoreApp.DAL.UnitOfWork;
using StoreApp.Repository.Comman;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddApplicationServices();
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddSqlServer<StoreAppDbContext>(connectionString);
builder.Services.AddScoped<IUnitOfWork, SqlUnitOfWork>((provider) =>
{
    var dbContext = provider.GetRequiredService<StoreAppDbContext>();
    return new SqlUnitOfWork(connectionString!, dbContext);
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
