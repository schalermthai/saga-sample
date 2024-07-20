using BookingAPI.Activities.Seats;
using MassTransit;
using SBWorkflow.Booking.Domain;
using SBWorkflow.Booking.StateMachine;
using SBWorkflow.Payments.Activities;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
    x.AddActivity<CommitSeatsActivity, CommitSeatsArguments, CommitSeatsLog>();
    x.AddActivity<CreatePaymentActivity, CreatePaymentArguments, CreatePaymentLog>();
});

builder.Services.AddHttpClient("PaymentService", c =>
{
    c.BaseAddress = new Uri("http://localhost:5255/");
});

builder.Services.AddHttpClient("SeatService", c =>
{
    c.BaseAddress = new Uri("http://localhost:5179/");
});

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSession();
builder.Services.AddControllersWithViews();
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