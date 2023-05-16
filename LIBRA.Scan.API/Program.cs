using LIBRA.Scan.Data.EFs;
using LIBRA.Scan.Data.Repositories;
using LIBRA.Scan.Data.Repositories.Constracts;
using LIBRA.Scan.Service;
using LIBRA.Scan.Service.Constracts;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Reflection;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c=>
{
});

// DI 
builder.Services.AddDbContext<ScandbContext>(options => options.UseSqlServer("Data Source=ADMIN\\SQLDTB;Initial Catalog=SCANDB;Persist Security Info=True;User ID=sa;Password=123; TrustServerCertificate=True"));
builder.Services.AddScoped<IBatchRepo, BatchRepo>();
builder.Services.AddScoped<IBatchService, BatchService>();
builder.Services.AddAutoMapper(Assembly.GetAssembly(typeof(MappingProfile)));

builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
   .AddNegotiate();


builder.Services.AddAuthorization(options =>
{
    // By default, all incoming requests will be authorized according to the default policy.
    options.FallbackPolicy = options.DefaultPolicy;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
