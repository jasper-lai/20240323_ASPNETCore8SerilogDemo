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

#region ���o�s���r��
var configuration = builder.Configuration;
var serilogConnectionString = configuration["ConnectionStrings:SerilogConnectionString"]?.ToString() ?? string.Empty;
#endregion

#region ���� Logger ���]�m
//// ���g�H�U�{��, �w�]�|��X�� Console.
//// ���ק�H�U���{��, �i�H�]�w��h����X�ؼ�. 
//builder.Logging.ClearProviders();
//builder.Logging.AddConsole();
//builder.Logging.AddDebug();
#endregion

#region Serilog ���]�m�Ψϥ�

//// �]�m: �{���g��
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
// �ؼ�:
// (1) �O�d Properties �� SourceContext ���
// (2) ���� MessageTemplate �o����� (Properties �٬O�d�ۦn�F, �H�d�ߤ@�ǲӸ`)
// (3) �[�J ProblemDetails ���������  

// (1) �O�d Properties �� SourceContext ���
// (3) �[�J ProblemDetails ���������
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

// (2) ���� MessageTemplate �o����� (Properties �٬O�d�ۦn�F, �H�d�ߤ@�ǲӸ`)
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
#region ���U���쪺 Filter: ModelErrorHandlerAttribute; �å[�J jsonOptions ���]�w (for System.Text.Json)
builder.Services.AddControllersWithViews(options =>
{
    // ���U���쪺 Filter
    options.Filters.Add(new ModelErrorHandlerAttribute());
})
    .AddJsonOptions(jsonOptions =>
    {
        // �w�]�Ĥ@�r����p�g (camel), �Y�n�令 C# ���r���j�g�榡 (Pascal), �n�]�� null 
        // ���n: �Y�]���r���p�g, �h�ۦ�b ProblemDetails �X�W����� (TraceId, ControllerName...), �ä��|��p�g!!!
        //
        // PropertyNamingPolicy = JsonNamingPolicy.CamelCase,   
        jsonOptions.JsonSerializerOptions.PropertyNamingPolicy = null;
        //���\�򥻩ԤB�^��Τ�������r������r��
        jsonOptions.JsonSerializerOptions.Encoder =
            JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.CjkUnifiedIdeographs);
    });
#endregion

#region ���U���h������ services
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton(typeof(ILoggingService<>), typeof(LoggingService<>));
#endregion

#region ���U Session �A��
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set a long timeout for testing.
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
#endregion

#region ���U�M�׬����� services
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


#region �b Program.cs �ϥ� Logger
// �إ� logger
var logger = app.Services.GetRequiredService<ILogger<Program>>();
// �ϥ� logger
logger.LogInformation("Application starting up at {Time}, Environment is {EnvironmentName}", DateTime.Now, app.Environment.EnvironmentName);
#endregion

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

//#region ���U UseSerilogRequestLogging() middleware
//app.UseSerilogRequestLogging(); // <-- Add this line
//#endregion

#region �ҥ� Session Middleware: �����b�ϥ� Session ���۩w�q Middleware ���e
app.UseSession();
#endregion

#region �ҥΦ۩w�q���X TraceId �� Middleware: TraceIdMiddleware
app.UseMiddleware<TraceIdMiddleware>();
#endregion

#region �ҥΦ۩w�q�ҥ~�d�I�� Middleware: ExceptionHandlingMiddleware
app.UseMiddleware<ExceptionHandlingMiddleware>();
#endregion

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
