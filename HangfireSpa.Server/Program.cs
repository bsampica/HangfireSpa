
using Hangfire;
using HangfireSpa.Server.Data;
using HangfireSpa.Server.Services;
using Microsoft.EntityFrameworkCore;

namespace HangfireSpa.Server;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add the db context to the services
        builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase("AppDb"));


        // Add services to the container.
        builder.Services.AddAuthorization();
        builder.Services.AddControllers();

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

        // builder.Services.AddIdentityApiEndpoints<IdentityUser>(opt =>
        // {
        //    opt.User.RequireUniqueEmail = true;
        //    opt.Password.RequiredLength = 5;
        //    opt.Password.RequireNonAlphanumeric = false;
        //    opt.Password.RequireUppercase = false;
        //    opt.Password.RequireLowercase = false;
        //    opt.Password.RequiredUniqueChars = 0;
        //    opt.SignIn.RequireConfirmedEmail = false;
        //    opt.SignIn.RequireConfirmedPhoneNumber = false;
        //    opt.SignIn.RequireConfirmedAccount = false;

        // });

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
        app.MapControllers();

        app.Run();
    }
}
