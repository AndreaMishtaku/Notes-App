using AutoMapper;
using Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NotesApp.Middleware;
using NotesApp.Shared.Configuration;
using NotesApp.Utility;
using Repository;
using Shared.Configuration;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder =>
    builder.AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());
});

builder.Services.AddDbContext<RepositoryContext>(opts =>
opts.UseSqlServer(builder.Configuration.GetConnectionString("sqlConnection")).
UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));


builder.Services.AddSingleton(new MapperConfiguration(cfg =>{cfg.AddProfile(new MappingProfile());}).CreateMapper());
builder.Services.AddSingleton<IJwtUtils, JwtUtils>();
builder.Services.ConfigureRepositories();
builder.Services.ConfigureServices();


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.Configure<ApiBehaviorOptions>(options =>{ options.SuppressModelStateInvalidFilter = true; });
builder.Services.AddResponseCaching();
builder.Services.AddMemoryCache();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IClaimsUtility, ClaimsUtility>();


builder.Services.AddIdentity<User, IdentityRole<int>>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedEmail = false;
    options.Lockout.AllowedForNewUsers = false;
}).AddEntityFrameworkStores<RepositoryContext>().AddDefaultTokenProviders();


builder.Services.Configure<DataProtectionTokenProviderOptions>(opt => opt.TokenLifespan = TimeSpan.FromHours(2));

builder.Services.AddAuthentication(opt =>{opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;}).AddJwtBearer(options =>
        {
            var jwtConfiguration = new JwtConfiguration();
            builder.Configuration.Bind(jwtConfiguration.Section, jwtConfiguration);
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtConfiguration.ValidIssuer,
                ValidAudience = jwtConfiguration.ValidAudience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfiguration.SecretKey))
            };
        });



builder.Services.Configure<JwtConfiguration>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.ConfigureSwagger();

builder.Services.AddSingleton(builder.Configuration.GetSection("DefaultConfiguration"));

builder.Services.AddControllers(config =>
{
    config.RespectBrowserAcceptHeader = true;
    config.ReturnHttpNotAcceptable = true;
}).AddXmlDataContractSerializerFormatters();



var app = builder.Build();
app.UseMiddleware<JwtMiddleware>();
app.ConfigureExceptionHandler();

if (app.Environment.IsProduction())
    app.UseHsts();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseForwardedHeaders(new ForwardedHeadersOptions{ForwardedHeaders = ForwardedHeaders.All});

app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

if (builder.Configuration.GetSection("DefaultConfiguration:UseSwagger").Get<bool>() == true)
{
    app.UseSwagger();
    app.UseSwaggerUI(s => {s.SwaggerEndpoint("/swagger/v1/swagger.json", "Notes API v1");});
}

app.MapControllers();

app.Run();

NewtonsoftJsonPatchInputFormatter GetJsonPatchInputFormatter() =>
    new ServiceCollection().AddLogging().AddMvc().AddNewtonsoftJson().Services.BuildServiceProvider().
    GetRequiredService<IOptions<MvcOptions>>().Value.InputFormatters.OfType<NewtonsoftJsonPatchInputFormatter>().First();
