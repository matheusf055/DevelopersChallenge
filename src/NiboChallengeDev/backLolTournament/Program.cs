using LolTournament.Application.Services;
using LolTournament.Data;
using LolTournament.Models.Interfaces;
using Microsoft.EntityFrameworkCore;
using LolTournament.Data.Repositories;
using Microsoft.OpenApi.Models;

// Create the web application builder
var builder = WebApplication.CreateBuilder(args);

// Configure CORS policy to allow any origin, header, and method
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});

// Register services for controllers with views
builder.Services.AddControllersWithViews();

// Configure Entity Framework with MySQL
builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"), 
        new MySqlServerVersion(new Version(8, 0, 3))
    )
);

// Add AutoMapper for object mapping
builder.Services.AddAutoMapper(typeof(Program));

// Register application services and repositories
builder.Services.AddScoped<ITournamentRepository, TournamentRepository>();
builder.Services.AddScoped<TournamentService, TournamentService>();
builder.Services.AddScoped<IMatchRepository, MatchRepository>();
builder.Services.AddScoped<MatchService, MatchService>();
builder.Services.AddScoped<ITeamRepository, TeamRepository>();
builder.Services.AddScoped<TeamService, TeamService>();

// Add support for API endpoint exploration
builder.Services.AddEndpointsApiExplorer();

// Configure Swagger for API documentation
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "LolTournament API", Version = "v1" });

    // Add XML comments for documentation
    var xmlFile = "LolTournament.xml"; 
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);  
});

// Configure CORS policy to allow specific origin
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder =>
        {
            builder.WithOrigins("http://127.0.0.1:5500")
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});

// Build the web application
var app = builder.Build();

app.UseCors("AllowSpecificOrigin");

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error"); 
    app.UseHsts(); 
}

app.UseRouting();
app.UseAuthorization();

// Configure Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // Enable Swagger
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "LolTournament API V1"); // Set Swagger endpoint
        c.RoutePrefix = string.Empty; // Set Swagger UI 
    });
}

// Set up default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();
