using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using PeopleHub.Web.Services;
using PeopleHub.Web.Repositories;
using PeopleHub.Web.Data;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    // Isso garante que ele pegue a raiz do projeto mesmo rodando pelo VS
    ContentRootPath = AppContext.BaseDirectory.Split(new String[] { @"\bin\" }, StringSplitOptions.None)[0],
    WebRootPath = "wwwroot"
});

DotNetEnv.Env.Load(Path.Combine(Directory.GetCurrentDirectory(), "../.env"));

DotNetEnv.Env.Load();

var rawConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");

var connectionString = rawConnectionString?
    .Replace("${DB_SERVER}", Environment.GetEnvironmentVariable("DB_SERVER") ?? "localhost,1433")
    .Replace("${DB_NAME}", Environment.GetEnvironmentVariable("DB_NAME") ?? "PeopleHubDB")
    .Replace("${DB_USER}", Environment.GetEnvironmentVariable("DB_USER") ?? "sa")
    .Replace("${DB_PASSWORD}", Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "PeopleHub@2026");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IPersonRepository, PersonRepository>();
builder.Services.AddScoped<IPersonService, PersonService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "PeopleHub API V1");
    });
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();
    context.Database.Migrate();
    DbInitializer.Seed(context);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

// Map attribute-routed API controllers (e.g. controllers decorated with [ApiController] and [Route("api/[controller]")])
app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();
