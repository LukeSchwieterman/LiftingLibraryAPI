using Microsoft.EntityFrameworkCore;
using LiftingLibraryAPI.Context;
using LiftingLibrary.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace LiftingLibraryAPI
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.
			builder.Services.AddControllers();
			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();

			// Register JwtGenerator as a Singleton, using the secret key from the configuration for JWT token generation.
			builder.Services.AddSingleton<ITokenGenerator>(tk => new JwtGenerator(builder.Configuration["JwtSecret"]));
			// Register PasswordHasher as a Singleton, ensuring that the same instance is used for hashing passwords throughout the application.
			builder.Services.AddSingleton<IPasswordHasher>(ph => new PasswordHasher());

			// Configures the application to use Entity Framework with MySQL for database access, using the connection string defined in appsettings.json
			builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseMySql(builder.Configuration.GetConnectionString("Project"), ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("Project"))));

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigins",
                    builder =>
                    {
                        builder.WithOrigins("http://localhost:3000") // Replace with your frontend URL
                               .AllowAnyHeader()
                               .AllowAnyMethod();
                    });
            });

            // Add JWT Authentication
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.ASCII.GetBytes(builder.Configuration["JwtSecret"]))
                };
            });

            var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}
            else
            {
                // Add global exception handler for production
                app.UseExceptionHandler("/error");
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCors("AllowSpecificOrigins");

            app.MapControllers();

			app.Run();
		}
	}
}
