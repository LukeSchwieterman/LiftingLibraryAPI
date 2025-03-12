using MySql.Data.EntityFramework;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using LiftingLibraryAPI.Context;
using LiftingLibrary.Security;

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

            builder.Services.AddSingleton<ITokenGenerator>(tk => new JwtGenerator(builder.Configuration["JwtSecret"]));
            builder.Services.AddSingleton<IPasswordHasher>(ph => new PasswordHasher());
            builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseMySql(builder.Configuration.GetConnectionString("Project"), ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("Project"))));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
