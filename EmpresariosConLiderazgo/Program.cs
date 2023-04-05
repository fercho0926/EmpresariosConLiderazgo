using DinkToPdf;
using DinkToPdf.Contracts;
using EmpresariosConLiderazgo.Data;
using EmpresariosConLiderazgo.Services;
using EmpresariosConLiderazgo.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using Amazon;
using Amazon.CDK.AWS.SSM;
using Amazon.SecretsManager;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using static Org.BouncyCastle.Math.EC.ECCurve;


var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddSecretsManager(
    configurator: options =>
    {
        options.ConfigureSecretsManagerConfig(new AmazonSecretsManagerConfig());
        options.PollingInterval = TimeSpan.FromSeconds(10);
        options.SecretFilter = entry => entry.Name.Contains("Database");
    });

//var connectionString = builder.Configuration.GetValue<string>("Dev_DatabaseConection");
var connectionString = builder.Configuration.GetValue<string>("Prod_DatabaseConection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();


builder.Services.AddControllersWithViews();
builder.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));

// Services dependencies
builder.Services.AddTransient<IDocumentService, DocumentService>();
builder.Services.AddTransient<IMailService, MailService>();
builder.Services.AddTransient<ICloudwatchLogs, CloudwatchLogs>();


builder.Services.AddSwaggerGen();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints => { endpoints.MapDefaultControllerRoute().RequireAuthorization(); });


app.MapControllerRoute(
    name: "default",
    pattern: "{area:identity}/{controller=account}/{action=login}");
app.MapRazorPages();

app.Run();