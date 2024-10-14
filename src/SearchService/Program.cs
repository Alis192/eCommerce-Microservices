using MassTransit;
using Microsoft.EntityFrameworkCore;
using SearchService.Consumers;
using SearchService.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// STEP 1: Configure Entity Framework and SQL Server
builder.Services.AddDbContext<SearchDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


// Configure MassTransit
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<ProductCreatedConsumer>();
    x.AddConsumer<ProductUpdatedConsumer>(); 
    x.AddConsumer<ProductDeletedConsumer>(); 

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h => { });
        cfg.ReceiveEndpoint("product-created-queue", e =>
        {
            e.ConfigureConsumer<ProductCreatedConsumer>(context);
        });

        cfg.ReceiveEndpoint("product-updated-queue", e =>
        {
            e.ConfigureConsumer<ProductUpdatedConsumer>(context);
        });

        cfg.ReceiveEndpoint("product-deleted-queue", e =>
        {
            e.ConfigureConsumer<ProductDeletedConsumer>(context);
        });
    });
});


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
