using Microsoft.EntityFrameworkCore;
using ProductService.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// STEP 1: Configure Entity Framework and SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// STEP 2: Add Controllers and Swagger for API Documentation
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// STEP 3: Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
