using ASPNETCore8SerilogEnrich.Utilities;
using Serilog;
using Serilog.Context;
using Serilog.Events;

#region �]�w�����ܼ� RELEASE_NUMBER, �� WithReleaseNumber Enricher �ϥ�.
Environment.SetEnvironmentVariable("RELEASE_NUMBER", "1.0.0");
#endregion

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

#region Serilog ��l�]�w
// �]�m Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()    // Adds the machine name
    .Enrich.WithThreadId()       // Adds the thread id
    .Enrich.WithEnvironmentName() // Adds the environment name (e.g., Development, Product
    // �[�J�۩w�q���ݩ�, �o�ӬO���쪺; �p�G�O�� PushProperty ����, �h�u����Y�� using ���϶��ξ�� method
    .Enrich.WithProperty("Application", "SerilogEnricherDemo")
    .Enrich.WithReleaseNumber()
    .WriteTo.Console(
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:l} {Properties}{NewLine}{Exception}"
    )
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .WriteTo.Seq("http://localhost:5341")
    .CreateLogger();

// �ϥ� Serilog
builder.Host.UseSerilog();

//// �[�J�۩w�q���ݩʦ� Serilog �� LogContext
//LogContext.PushProperty("WebAppName", "ASPNETCore8SerilogEnrich");

#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

Log.Information("Web Application �]�m����, �Y�N�Ұ�...");

app.Run();

