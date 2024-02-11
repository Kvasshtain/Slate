using SignalRApp;
using DrawingServices;

// var render = new ImageRender();
// render.TestDraw();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();

builder.Services.AddSingleton<IImageStoreService, ImageStoreService>();
builder.Services.AddSingleton<IPointStoreService, PointStoreService>();
builder.Services.AddSingleton<IRenderingService, RenderingService>();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

// app.MapGet("/Star", async () => 
// {
//     string path = "test.jpg";
//     byte[] fileContent = await File.ReadAllBytesAsync(path);  // считываем файл в массив байтов
//     string contentType = "image/jpg";       // установка mime-типа
//     string downloadName = "test.jpg";  // установка загружаемого имени
//     return Results.File(fileContent, contentType, downloadName);
// });

//app.MapHub<DrawingHub>("/drawing");
app.MapHub<ImageHub>("/imageExchanging");

app.Run();
