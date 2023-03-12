using EthioTelQuizBot;
using EthioTelQuizBotBusinessLogic;
using EthioTelQuizBotBusinessLogic.BusinessLogic;
using EthioTelQuizBotBusinessLogic.Infrastructure;
using EthioTelQuizBotBusinessLogic.Interface;
using EthioTelQuizBotBusinessLogic.Models.Entity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Configuration;
using System.Text;
using Telegram.Bot;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddNewtonsoftJson();

var key = Encoding.ASCII.GetBytes(builder.Configuration["JwtConfig:Secret"]);
builder.Services.AddHttpClient("telegram_bot_client")
                .AddTypedClient<ITelegramBotClient>((httpClient, sp) =>
                {
                    TelegramBotClientOptions options = new(builder.Configuration.GetConnectionString("Bot_Token"));
                    return new TelegramBotClient(options, httpClient);
                });


builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly("EthioTelQuizBotBusinessLogic")));
builder.Services.AddScoped<AuthDbContext>();
builder.Services.AddScoped<UserManager<AppUser>>();
builder.Services.AddScoped<RoleManager<IdentityRole>>();
builder.Services.AddScoped<IAuthManager, AuthManager>();
builder.Services.AddScoped<IQuizManager, QuizManager>();
builder.Services.AddScoped<ISubscriberManager, SubscriberManager>();
builder.Services.AddCors(p => p.AddPolicy("corsapp", builder =>
{
    builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));
builder.Services.AddIdentity<AppUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<AuthDbContext>();
builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("JwtConfig"));

// setup JWT authentication policy
builder.Services.AddScoped<IUpdateHandler, UpdateHandler>();

var tokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(key),
    ValidateIssuer = false,
    ValidateAudience = false,
    ValidateLifetime = true,
    RequireExpirationTime = false,
    ClockSkew = TimeSpan.Zero,
};
builder.Services.AddSingleton(tokenValidationParameters);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
            .AddJwtBearer(jwt =>
            {
                jwt.SaveToken = true;
                jwt.TokenValidationParameters = tokenValidationParameters;
            });
builder.Services.AddAuthorization(async options =>
{

    var usersRead = await File.ReadAllTextAsync("Infrastructure/Config/Seed" + Path.DirectorySeparatorChar + "policy.json");
    var policyList = JsonConvert.DeserializeObject<List<AuthPolicy>>(usersRead);

    foreach (var policyModel in policyList)
    {
        options.AddPolicy(policyModel.Name, policy =>
        {
            policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);

            policy.RequireAuthenticatedUser();

            policy.RequireClaim(policyModel.Claim, policyModel.Value);
        });
    }

});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseFileServer(new FileServerOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "StaticFiles")),
    RequestPath = "/StaticFiles",
    EnableDefaultFiles = true
});
app.UseCors("corsapp");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
