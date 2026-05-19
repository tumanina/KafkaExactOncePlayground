using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Users.Database;
using UsersApi.Interfaces;
using UsersApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var connectionString = "Host=localhost;Port=5432;Database=usersdb;Username=postgres;Password=playgroundpass";
builder.Services
    .AddDbContext<UsersContext>(options => options.UseNpgsql(connectionString));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
