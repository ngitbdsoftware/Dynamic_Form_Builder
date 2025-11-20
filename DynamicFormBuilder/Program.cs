using DynamicFormBuilder.Data;

var builder = WebApplication.CreateBuilder(args);

// Add MVC
builder.Services.AddControllersWithViews();

// Register Repositories
var cs = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddTransient<FormRepository>(m => new FormRepository(cs));
builder.Services.AddTransient<OptionRepository>(m => new OptionRepository(cs));

var app = builder.Build();

// Configure
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
