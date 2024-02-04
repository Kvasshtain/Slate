using SignalRApp;
using Render;
using DrawingServices;

// var render = new ImageRender();
// render.TestDraw();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();

builder.Services.AddSingleton<IPointService, PointService>();

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

app.MapHub<ChatHub>("/drawing");

app.Run();
