using InventoryService.Data;
using InventoryService.Consumers;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configure Entity Framework and SQL Server
builder.Services.AddDbContext<InventoryDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure MassTransit for RabbitMQ
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<ProductCreatedConsumer>(); // Add consumer

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h => { });
        cfg.ReceiveEndpoint("inventory-product-created", e =>
        {
            e.ConfigureConsumer<ProductCreatedConsumer>(context);
        });
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
