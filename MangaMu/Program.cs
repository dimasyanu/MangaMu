using MangaMu;
using MangaMu.Plugin;
using MangaMu.Repositories;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.RegisterDependencies();

var app = builder.Build();

//app.UseAuthorization();

app.MapControllers();

app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");

app.UseStaticFiles(new StaticFileOptions {
    FileProvider = new PhysicalFileProvider(
           Path.Combine(builder.Environment.ContentRootPath, "assets")),
    RequestPath = "/assets"
});

app.Run();
