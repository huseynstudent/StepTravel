using Microsoft.EntityFrameworkCore;
using StoreApp.Application;
using StoreApp.DAL.Context;
using StoreApp.DAL.UnitOfWork;
using StoreApp.Repository.Comman;
using StoreApp.WebApi.Infastructure.Middlewares;
using StoreApp.WebApi.Infrastructure.Security;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy =>
        {
            // React portun 5173 və ya 5174 ola bilər, hər ikisini əlavə etmək daha təhlükəsizdir
            policy.WithOrigins("http://localhost:5173", "http://localhost:5174")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});

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
builder.Services.AddAuthenticationDependency(builder.Configuration);

builder.Services.AddSwaggerGen(c => {
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter 'Bearer {token}'"
    });
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            }, new string[] {}
        }
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();
app.UseCors("AllowReactApp");
app.UseMiddleware<ExceptionHandlerMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();