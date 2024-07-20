using MassTransit;
using SBWorkflow.Booking.Domain;
using SBWorkflow.Booking.StateMachine;
using SBWorkflow.Payments.Activities;
using SBWorkflow.Seats.Activities;
using SBWorkflow.Seats.Domain;
using SBWorkflow.Seats.Repository;
using SBWorkflow.Seats.Seeder;

var builder = WebApplication.CreateBuilder(args);

// Seats related
builder.Services.AddSingleton<ISeatRepository, InMemorySeatRepository>();
builder.Services.AddTransient<SeatSeeder>();
builder.Services.AddHostedService<SeatSeederHostedService>();

builder.Services.AddHttpClient("SeatService", c =>
{
    c.BaseAddress = new Uri("https://localhost:5229/");
});
builder.Services.AddHttpClient("PaymentService", c =>
{
    c.BaseAddress = new Uri("https://localhost:5229/");
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();

builder.Services.AddMassTransit(x =>
{
    
    x.AddSagaStateMachine<BookingStateMachine, BookingState>()
        .InMemoryRepository();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ConfigureEndpoints(context);
    });
    
    x.AddActivity<ReserveSeatsActivity, ReserveSeatsArguments, ReserveSeatsLog>();
    x.AddActivity<CreatePaymentActivity, CreatePaymentArguments, CreatePaymentLog>();
    x.AddActivity<CommitSeatsActivity, CommitSeatsArguments, CommitSeatsLog>();
});

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSession();
builder.Services.AddControllersWithViews();

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
