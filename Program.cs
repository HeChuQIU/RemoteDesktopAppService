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

app.MapHub<ChatHub>("/chatHub");
app.MapHub<RemoteAppHub>("/remoteAppHub");

app.Run();
