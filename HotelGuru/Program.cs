using HotelGuru.DataContext.Context;
using HotelGuru.Services;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using System.Text;

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
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = true;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
       // ValidIssuer = jwtSettings["Issuer"],
        ValidateAudience = true,
        //ValidAudience = jwtSettings["Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        ValidateIssuerSigningKey = true,
        //IssuerSigningKey = new SymmetricSecurityKey(
          //  Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]))
    };
})
.AddNegotiate();
builder.Services.AddAuthorization(options =>
{
    
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
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
