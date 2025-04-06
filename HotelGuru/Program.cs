using HotelGuru.DataContext.Context;
using HotelGuru.Services;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "HotelGuru API", Version = "v1" });
});

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer("Server=(local);Database=HotelGuruDB;Trusted_Connection=True;TrustServerCertificate=True;");
});

builder.Services.AddScoped<IHotelServices, HotelServices>();
builder.Services.AddScoped<IFoglalasService, FoglalasService>();
builder.Services.AddScoped<ISzobaService, SzobaService>();
builder.Services.AddScoped<IVendegService, VendegService>();
builder.Services.AddScoped<IRecepciosService, RecepciosService>();
builder.Services.AddScoped<IPluszSzolgaltatasService, PluszSzolgaltatasService>();
builder.Services.AddScoped<IAdminisztratorService, AdminisztratorService>();

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
