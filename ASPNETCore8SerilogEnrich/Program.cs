using ASPNETCore8SerilogEnrich.Utilities;
using Serilog;
using Serilog.Context;
using Serilog.Events;

#region 設定環境變數 RELEASE_NUMBER, 供 WithReleaseNumber Enricher 使用.
Environment.SetEnvironmentVariable("RELEASE_NUMBER", "1.0.0");
#endregion

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

#region Serilog 初始設定
// 設置 Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()    // Adds the machine name
    .Enrich.WithThreadId()       // Adds the thread id
    .Enrich.WithEnvironmentName() // Adds the environment name (e.g., Development, Product
    // 加入自定義的屬性, 這個是全域的; 如果是用 PushProperty 的話, 則只限於某個 using 的區塊或整個 method
    .Enrich.WithProperty("Application", "SerilogEnricherDemo")
    .Enrich.WithReleaseNumber()
    .WriteTo.Console(
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:l} {Properties}{NewLine}{Exception}"
    )
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .WriteTo.Seq("http://localhost:5341")
    .CreateLogger();

// 使用 Serilog
builder.Host.UseSerilog();

//// 加入自定義的屬性至 Serilog 的 LogContext
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

Log.Information("Web Application 設置完成, 即將啟動...");

app.Run();

