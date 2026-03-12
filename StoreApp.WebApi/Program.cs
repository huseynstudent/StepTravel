using Microsoft.EntityFrameworkCore;
using StoreApp.Application;
using StoreApp.DAL.Context;
using StoreApp.DAL.UnitOfWork;
using StoreApp.Repository.Comman;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddControllers();
builder.Services.AddApplicationServices();
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

app.UseStaticFiles();
app.UseAuthorization();

app.MapControllers();

app.Run();
