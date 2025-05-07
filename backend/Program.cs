using backend.Repositories;
using Microsoft.Extensions.FileProviders;
using Microsoft.EntityFrameworkCore;
using backend.Data;

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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "Boxes")),
        RequestPath = "/files"
    });
}

app.UseCors(allowSpecificOrigins);
app.UseHttpsRedirection();

app.MapControllers();

app.Run();
