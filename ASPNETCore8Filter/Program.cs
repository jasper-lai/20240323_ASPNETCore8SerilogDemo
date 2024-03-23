using ASPNETCore8Filter.Filters;
using ASPNETCore8Filter.Middlewares;
using System.Text.Encodings.Web;
using System.Text.Unicode;

var builder = WebApplication.CreateBuilder(args);

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

#region ���U�۩w�q���X TraceId �� Middleware: TraceIdMiddleware
app.UseMiddleware<TraceIdMiddleware>();
#endregion

#region ���U�۩w�q�ҥ~�d�I�� Middleware: ExceptionHandlingMiddleware
app.UseMiddleware<ExceptionHandlingMiddleware>();
#endregion

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
