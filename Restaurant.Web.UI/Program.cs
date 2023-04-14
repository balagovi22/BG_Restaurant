using Restaurant.Web.UI;
using Restaurant.Web.UI.Services;
using Restaurant.Web.UI.Services.IServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
//builder.Services.AddCors();

builder.Services.AddHttpClient<IProductService, ProductService>();
SD.ProductAPIBaseURL = builder.Configuration["ServiceUrls:ProductAPI"];
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IBaseService, BaseService>();
//builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

//app.UseMvc();
//app.UseCors();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
