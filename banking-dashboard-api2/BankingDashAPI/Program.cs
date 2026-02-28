using BankingDashAPI.Data;
using BankingDashAPI.Services;
using BankingDashAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// DATABASE
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// SERVICES
builder.Services.AddScoped<IDashboardService, DashboardService>();

// CONTROLLERS
builder.Services.AddControllers();

// SWAGGER
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS - CORRECTED VERSION
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000")  // Your React app URL
                  .AllowAnyHeader()
                  .AllowAnyMethod();
            // Removed .AllowAnyOrigin() as it conflicts with WithOrigins
        });
});

var app = builder.Build();

// PIPELINE
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Disable HTTPS redirect for development
// app.UseHttpsRedirection();

// IMPORTANT: CORS must be called before Authorization
app.UseCors("AllowReact");

app.UseAuthorization();

app.MapControllers();

app.Run();