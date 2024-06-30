using CommunityToolkit.Diagnostics;

using PaymentGateway.Api.Controllers;
using PaymentGateway.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var bankApiUrl = builder.Configuration["BankAPIURL"];
Guard.IsNotNullOrEmpty(bankApiUrl, nameof(bankApiUrl));
builder.Services.AddHttpClient<BankService>(httpClient =>
{
    httpClient.BaseAddress = new Uri(bankApiUrl);
});
builder.Services.AddSingleton<PaymentsRepository>();

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
