using KoiCareSys.Data;
using KoiCareSys.Data.Models;
using KoiCareSys.Data.Repository;
using KoiCareSys.Data.Repository.Interface;
using KoiCareSys.Service.Mappings;
using KoiCareSys.Service.Service;
using KoiCareSys.Service.Service.Interface;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.ModelBuilder;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

var modelBuilder = new ODataConventionModelBuilder();
modelBuilder.EntitySet<Pond>("Ponds");

// Add services to the container.
builder.Services.AddScoped<IPondService, PondService>();

builder.Services.AddControllers()
.AddOData(opt => opt
        .AddRouteComponents("odata", modelBuilder.GetEdmModel())
        .Select()
        .Filter()
        .OrderBy()
        .Expand()
        .SetMaxTop(null)
        .Count());
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "KoiCareSys API", Version = "v1" });
});

// Add Unit of Work
builder.Services.AddScoped<UnitOfWork>();

// Add services to the container.
builder.Services.AddScoped<IPondRepository, PondRepository>();


//Add Configuration
//builder.Services.ConfigAddDbContext();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        policy =>
        {
            policy.WithOrigins("https://localhost:7250")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});

// AutoMapper
builder.Services.AddAutoMapper(typeof(AutoMapperProfiles));

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowSpecificOrigin");

app.UseODataBatching();

app.UseRouting();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
