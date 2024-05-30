using RemoteDesktopAppService;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.Services.AddSignalR();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        corsPolicyBuilder =>
        {
            corsPolicyBuilder.WithOrigins("http://localhost:5173")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});

var app = builder.Build();

app.UseCors();

app.MapHub<ClientHub>("/clientHub");
app.MapHub<RemoteAppHub>("/remoteAppHub");
app.MapGet("/hello", () => "Hello!");

app.Run();
