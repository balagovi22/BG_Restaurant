using Restaurant.OrderAPI.Extentions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.RegisterServices(builder.Configuration);

var app = builder.Build();
app.RegisterPipeline();

