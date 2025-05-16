using backend.Repositories;
using Microsoft.Extensions.FileProviders;
using Microsoft.EntityFrameworkCore;
using backend.Data;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Http.Features;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IBoxRepository, LocalBoxRepository>();
builder.Services.AddControllers();

string allowSpecificOrigins = "BINGO LINGO";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: allowSpecificOrigins,
    policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://127.0.0.1:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddDbContext<BoxDbContext>(opt =>
{
    if (builder.Environment.IsDevelopment())
        opt.UseSqlite("Data Source=boxes.db");

    else
        opt.UseNpgsql(builder.Configuration.GetConnectionString("Supabase_DB"));
});

long maxRequestBodySize = 5L * 1024 * 1024 * 1024; // 5 GB

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = maxRequestBodySize;
});
builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.Limits.MaxRequestBodySize = maxRequestBodySize; // for Kestrel
});
builder.Services.Configure<IISServerOptions>(options =>
{
    options.MaxRequestBodySize = maxRequestBodySize; // for IIS
});

builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy("strict", context =>
    {
        var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: ip,
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromHours(1),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 2
            }
        );
    });

    options.AddPolicy("lenient", context =>
{
    var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

    return RateLimitPartition.GetFixedWindowLimiter(
        partitionKey: ip,
        factory: _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 50,
            Window = TimeSpan.FromHours(1),
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            QueueLimit = 2
        }
    );
});
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    var boxDir = Path.Combine(builder.Environment.ContentRootPath, "Boxes");
    if (!Directory.Exists(boxDir))
        Directory.CreateDirectory(boxDir);

    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(boxDir),
        RequestPath = "/files"
    });
}

app.UseCors(allowSpecificOrigins);
app.UseHttpsRedirection();
app.UseRateLimiter();

app.MapControllers();

app.Run();
