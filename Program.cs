using AblyPOCService.Models;
using AblyPOCService.Services;
using IO.Ably;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
// builder.Services.AddSingleton<MessageRouterService>();
builder.Services.AddTransient<ChatMessageHandler>();
builder.Services.Configure<AblyKey>(builder.Configuration.GetSection("Ably"));
builder.Services.AddSingleton(new AblyRest(builder.Configuration["Ably:ApiKey"]));
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy
            .WithOrigins("http://localhost:5173", "http://localhost:5174", "http://localhost:3000", "http://localhost:4173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");

// Remove HTTPS redirection in development to allow HTTP connections
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseWebSockets();

app.MapControllers();

app.Run();
