using MassTransit;
using SBWorkflow.Seats.Domain;
using SBWorkflow.Seats.Seeder;
using SeatsInventory.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Seats related
builder.Services.AddSingleton<ISeatRepository, InMemorySeatRepository>();
builder.Services.AddTransient<SeatSeeder>();
builder.Services.AddHostedService<SeatSeederHostedService>();

builder.Services.AddMassTransit(x =>
{
    
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ConfigureEndpoints(context);
    });
    
});

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseHttpsRedirection();
app.MapControllers();
app.Run();
