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
builder.Services.AddTransient<CursorMessageHandler>();
builder.Services.AddSingleton(new AblyRest(builder.Configuration["Ably:ApiKey"]));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseWebSockets();

app.MapControllers();

app.Run();
