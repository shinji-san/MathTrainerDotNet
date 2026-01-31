using MathTrainerDotNet.Components;
using MathTrainerDotNet.Data;
using MathTrainerDotNet.Data.Helper;
using MathTrainerDotNet.Services;
using MathTrainerDotNet.Services.Backup;
using MathTrainerDotNet.Services.Format;
using MathTrainerDotNet.Services.Id;
using MathTrainerDotNet.Services.Localization;
using MathTrainerDotNet.Services.Pdf;
using MathTrainerDotNet.Services.Repository;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ContentRootPath = AppContext.BaseDirectory,
    WebRootPath = Path.Combine(AppContext.BaseDirectory, "wwwroot")
});

//// Add services to the container.
builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents();

//// SQLite database
var rawConnectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=mathtrainer.db";
var connectionString = DatabasePathHelper.GetFullConnectionString(rawConnectionString);
builder.Services
    .AddDbContext<AppDbContext>(options => options.UseSqlite(connectionString));

//// Register services
builder.Services.AddSingleton<IDateFormatterService, DateFormatterServiceService>();
builder.Services.AddSingleton<IPublicIdService, PublicIdService>();
builder.Services.AddScoped<IExerciseRepository, ExerciseRepository>();
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<ExerciseGeneratorService>();
builder.Services.AddScoped<IPdfService, QuestPdfService>();
builder.Services.AddScoped<ILocalizationService, LocalizationService>();
builder.Services.AddScoped<SqLiteBackupService>();

// Forwarded Headers for Reverse Proxy (nginx)
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownIPNetworks.Clear();
    options.KnownProxies.Clear();
});

var app = builder.Build();

// Initialize database
using (var scope = app.Services.CreateScope())
{
    var appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await appDbContext.Database.EnsureCreatedAsync();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

await app.RunAsync();
