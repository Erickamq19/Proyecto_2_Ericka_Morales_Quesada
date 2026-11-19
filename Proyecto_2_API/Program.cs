using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Proyecto_2_API.DB;
using Proyecto_2_API.Interfaces;
using Proyecto_2_API.Logica_negocio;
using Proyecto_2_API.Modelos;
using System.Security.Claims;



namespace Proyecto_2_API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var clave = builder.Configuration["Jwt:Key"];
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options => { options.TokenValidationParameters = new TokenValidationParameters { ValidateIssuer = true, ValidateAudience = true, 
                ValidateLifetime = true, ValidateIssuerSigningKey = true, ValidIssuer = builder.Configuration["Jwt:Issuer"], ValidAudience = builder.Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(clave!)), RoleClaimType = ClaimTypes.Role}; 
           
            
            });

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddDbContext<conexion_DB>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Conexion_DB2")));
            builder.Services.AddScoped<Interface_U, Registro_U_Metodos>();
            builder.Services.AddScoped<Interface_JWT, JWT_Metodos>();
            builder.Services.AddScoped<PasswordHasher<Usuarios>>();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
            options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                Description = "Ingrese el token JWT"

            });
            options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
            {
                {
                    new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                    {
                        Reference = new Microsoft.OpenApi.Models.OpenApiReference
                        {
                            Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                            Id = "Bearer"

                        }

                    },
                    Array.Empty<string>()

                }
            });


            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
