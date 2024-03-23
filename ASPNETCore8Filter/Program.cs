using ASPNETCore8Filter.Filters;
using ASPNETCore8Filter.Middlewares;
using System.Text.Encodings.Web;
using System.Text.Unicode;

var builder = WebApplication.CreateBuilder(args);

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

#region 註冊自定義產出 TraceId 的 Middleware: TraceIdMiddleware
app.UseMiddleware<TraceIdMiddleware>();
#endregion

#region 註冊自定義例外攔截的 Middleware: ExceptionHandlingMiddleware
app.UseMiddleware<ExceptionHandlingMiddleware>();
#endregion

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
