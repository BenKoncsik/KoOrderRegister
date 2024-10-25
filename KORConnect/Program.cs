var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/helloworld", () => "Hello World!");

app.Run();
