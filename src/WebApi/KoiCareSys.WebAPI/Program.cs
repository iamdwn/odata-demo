using KoiCareSys.Data;
using KoiCareSys.Data.Models;
using KoiCareSys.Data.Repository;
using KoiCareSys.Data.Repository.Interface;
using KoiCareSys.Service.Mappings;
using KoiCareSys.Service.Service;
using KoiCareSys.Service.Service.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OData.ModelBuilder;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var modelBuilder = new ODataConventionModelBuilder();
modelBuilder.EntitySet<Pond>("Ponds");

// Add services to the container.
builder.Services.AddScoped<IPondService, PondService>();
builder.Services.AddScoped<ITokenService>(provider =>
        new TokenService(
            "8mQoT5QrHRjElgHsmcKtVQQb6u3nMhVxGQ6rKc9B2gY",
            int.Parse("14400000"),
            provider.GetRequiredService<UnitOfWork>()
        )
    );

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

// Add Unit of Work
builder.Services.AddScoped<UnitOfWork>();

// Add services to the container.
builder.Services.AddScoped<IPondRepository, PondRepository>();


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("8mQoT5QrHRjElgHsmcKtVQQb6u3nMhVxGQ6rKc9B2gY"))
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API Title", Version = "v1" });

    // Add JWT Bearer Authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter 'Bearer' [space] and then your token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new string[] {}
            }
        });
});

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

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
