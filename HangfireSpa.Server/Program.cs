
using Hangfire;
using HangfireSpa.Server.Data;
using HangfireSpa.Server.Hubs;
using HangfireSpa.Server.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HangfireSpa.Server;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add the db context to the services
        builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase("AppDb"));

        // - place holder for connection string
        // builder.Configuration.GetConnectionString("");

        // Add services to the container.
        builder.Services.AddAuthorization();
        builder.Services.AddIdentityApiEndpoints<IdentityUser>(opt =>
        {
            opt.Password.RequireNonAlphanumeric = false;
            opt.Password.RequireLowercase = false;
            opt.Password.RequireUppercase = false;
            opt.Password.RequiredLength = 5;
            opt.Password.RequireDigit = false;
            opt.Password.RequiredUniqueChars = 0;

            opt.User.RequireUniqueEmail = true;
            opt.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
            
            opt.SignIn.RequireConfirmedEmail = false;
            opt.SignIn.RequireConfirmedPhoneNumber = false;
            opt.SignIn.RequireConfirmedAccount = false;

        })
        .AddEntityFrameworkStores<ApplicationDbContext>();


        builder.Services.AddControllers();
        builder.Services.AddSignalR();

        // Add in solution services
        builder.Services.AddScoped<HangfireJobService>();
        builder.Services.AddSingleton<ApplicationShutdownService>();

        builder.Services.AddHangfire(config =>
        {
            config.SetDataCompatibilityLevel(CompatibilityLevel.Version_180);
            config.UseColouredConsoleLogProvider();
            config.UseInMemoryStorage();
            config.UseSimpleAssemblyNameTypeSerializer();
            config.UseRecommendedSerializerSettings();

        });
        builder.Services.AddHangfireServer(options =>
        {
            //TODO:  This is insane, dont do this in production. Recommended is ProcessorCount * 5;
            options.WorkerCount = Environment.ProcessorCount * 20;
        });

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHangfireDashboard("/hangfire");
        app.UseAuthorization();
        app.MapIdentityApi<IdentityUser>();
        app.MapControllers();
        app.MapHub<JobDataHub>("/jobdatahub");

        app.Run();
    }
}
