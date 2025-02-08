

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using WebApplication1.Data;
using WebApplication1.Repositories;
using WebApplication1.Services;
namespace WebApplication1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            // Register MongoDbContext with configuration
            builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDbSettings"));
            builder.Services.AddSingleton<MongoDbContext>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IUserService, UserService>();
            //builder.Services.AddScoped<IYoutubeRepository, YoutubeRepository>();
            //builder.Services.AddScoped<IYoutubeService, YouTubeService>();

            builder.Services.AddControllers();
            //builder.Services.AddScoped<YouTubeTranscriptService>();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
            })
            .AddCookie()
            .AddGoogle(options =>
            {
                 options.ClientId = builder.Configuration["Google:ClientId"]; // Replace with your Client ID
                 options.ClientSecret = builder.Configuration["Google:ClientSecret"]; // Replace with your Client Secret
                 options.Scope.Add("https://www.googleapis.com/auth/youtube.readonly");
                 options.SaveTokens = true; // Save tokens for later use
});

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();
            app.UseAuthentication(); // Add authentication middleware
            

            app.UseRouting();
            app.MapControllers();
            app.Run();
        }
    }
    
}
