using ASPNETCore8Filter.Filters;
using ASPNETCore8Filter.Middlewares;
using ASPNETCore8Filter.Utilities;
using ASPNETCore8SerilogDemo.Services;
using Microsoft.AspNetCore.Http;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using System.Collections.ObjectModel;
using System.Data;
using System.Text.Encodings.Web;
using System.Text.Unicode;

var builder = WebApplication.CreateBuilder(args);

#region 取得連接字串
var configuration = builder.Configuration;
var serilogConnectionString = configuration["ConnectionStrings:SerilogConnectionString"]?.ToString() ?? string.Empty;
#endregion

#region 內建 Logger 的設置
//// 不寫以下程式, 預設會輸出到 Console.
//// 但修改以下的程式, 可以設定更多的輸出目標. 
//builder.Logging.ClearProviders();
//builder.Logging.AddConsole();
//builder.Logging.AddDebug();
#endregion

#region Serilog 的設置及使用

//// 設置: 程式寫死
//Log.Logger = new LoggerConfiguration()
//    .MinimumLevel.Information()
//    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
//    .Enrich.FromLogContext()
//    .WriteTo.Console()
//    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
//    .CreateLogger();

//// ### for Seq (by appsettings.json)
//Log.Logger = new LoggerConfiguration()
//    .ReadFrom.Configuration(builder.Configuration)
//    .WriteTo.Seq("http://localhost:5341")
//    .CreateLogger();

// ### for Seq + MSSQL (by hard coding)
// 目標:
// (1) 保留 Properties 的 SourceContext 欄位
// (2) 移除 MessageTemplate 這個欄位 (Properties 還是留著好了, 以查詢一些細節)
// (3) 加入 ProblemDetails 的相關欄位  

// (1) 保留 Properties 的 SourceContext 欄位
// (3) 加入 ProblemDetails 的相關欄位
var columnOptions = new ColumnOptions
{
    AdditionalColumns = new Collection<SqlColumn>
    {
        new() { ColumnName = "SourceContext", DataType = SqlDbType.NVarChar, DataLength = 512, AllowNull = true, PropertyName = "SourceContext" },
        new() { ColumnName = "Title", DataType = SqlDbType.NVarChar, DataLength = 50, AllowNull = true, PropertyName = "unifiedOutput.Title" },
        new() { ColumnName = "Status", DataType = SqlDbType.Int, AllowNull = true, PropertyName = "unifiedOutput.Status" },
        new() { ColumnName = "Detail", DataType = SqlDbType.NVarChar, DataLength = 512, AllowNull = true, PropertyName = "unifiedOutput.Detail" },
        new() { ColumnName = "Instance", DataType = SqlDbType.NVarChar, DataLength = 512, AllowNull = true, PropertyName = "unifiedOutput.Instance" },
        new() { ColumnName = "TraceId", DataType = SqlDbType.NVarChar, DataLength = 128, AllowNull = true, PropertyName = "unifiedOutput.TraceId" },
        new() { ColumnName = "ControllerName", DataType = SqlDbType.NVarChar, DataLength = 256, AllowNull = true, PropertyName = "unifiedOutput.ControllerName" },
        new() { ColumnName = "ActionName", DataType = SqlDbType.NVarChar, DataLength = 256, AllowNull = true, PropertyName = "unifiedOutput.ActionName" },
        new() { ColumnName = "UserId", DataType = SqlDbType.NVarChar, DataLength = 64, AllowNull = true, PropertyName = "unifiedOutput.UserId" }
    }
};

// (2) 移除 MessageTemplate 這個欄位 (Properties 還是留著好了, 以查詢一些細節)
//columnOptions.Store.Remove(StandardColumn.Properties);
columnOptions.Store.Remove(StandardColumn.MessageTemplate);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .WriteTo.Seq("http://localhost:5341")
    .WriteTo.MSSqlServer(
        connectionString: serilogConnectionString,
        sinkOptions: new MSSqlServerSinkOptions
        {
            AutoCreateSqlTable = true,
            SchemaName = "dbo",
            TableName = "Logs"
        },
        columnOptions: columnOptions
    )
    .CreateLogger();

builder.Host.UseSerilog();

#endregion

// Add services to the container.
#region 註冊全域的 Filter: ModelErrorHandlerAttribute; 並加入 jsonOptions 的設定 (for System.Text.Json)
builder.Services.AddControllersWithViews(options =>
{
    // 註冊全域的 Filter
    options.Filters.Add(new ModelErrorHandlerAttribute());
})
    .AddJsonOptions(jsonOptions =>
    {
        // 預設第一字母轉小寫 (camel), 若要改成 C# 的字首大寫格式 (Pascal), 要設為 null 
        // 重要: 若設成字首小寫, 則自行在 ProblemDetails 擴增的欄位 (TraceId, ControllerName...), 並不會轉小寫!!!
        //
        // PropertyNamingPolicy = JsonNamingPolicy.CamelCase,   
        jsonOptions.JsonSerializerOptions.PropertyNamingPolicy = null;
        //允許基本拉丁英文及中日韓文字維持原字元
        jsonOptions.JsonSerializerOptions.Encoder =
            JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.CjkUnifiedIdeographs);
    });
#endregion

#region 註冊底層相關的 services
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton(typeof(ILoggingService<>), typeof(LoggingService<>));
#endregion

#region 註冊 Session 服務
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set a long timeout for testing.
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
#endregion

#region 註冊專案相關的 services
builder.Services.AddScoped<IProductService, ProductService>();
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


#region 在 Program.cs 使用 Logger
// 建立 logger
var logger = app.Services.GetRequiredService<ILogger<Program>>();
// 使用 logger
logger.LogInformation("Application starting up at {Time}, Environment is {EnvironmentName}", DateTime.Now, app.Environment.EnvironmentName);
#endregion

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

//#region 註冊 UseSerilogRequestLogging() middleware
//app.UseSerilogRequestLogging(); // <-- Add this line
//#endregion

#region 啟用 Session Middleware: 必須在使用 Session 的自定義 Middleware 之前
app.UseSession();
#endregion

#region 啟用自定義產出 TraceId 的 Middleware: TraceIdMiddleware
app.UseMiddleware<TraceIdMiddleware>();
#endregion

#region 啟用自定義例外攔截的 Middleware: ExceptionHandlingMiddleware
app.UseMiddleware<ExceptionHandlingMiddleware>();
#endregion

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
