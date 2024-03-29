
using Asp.Versioning;
using CityInfo.API;
using CityInfo.API.DbContexts;
using CityInfo.API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;

//configure serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("log/cityInfo.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();


var builder = WebApplication.CreateBuilder(args);


////! clear all the providers configured by default -- console provider configured by default
//builder.Logging.ClearProviders();
//// ! add console logging -- manually add console logging provider 
//// removing and adding just for learning
//builder.Logging.AddConsole();


//change logger to use serilog 
builder.Host.UseSerilog();


// Add services to the container.

builder.Services.AddControllers(
    options =>
    {
        //return 406 error message if Accept type is not supported
        options.ReturnHttpNotAcceptable = true;
    }).AddNewtonsoftJson()  // changed default json formatter   support "RFC 6902" json patch
    .AddXmlDataContractSerializerFormatters();

builder.Services.AddProblemDetails();
//service to manipulate custom error details
//builder.Services.AddProblemDetails((options) =>
//{
//    options.CustomizeProblemDetails = context =>
//    {
//        context.ProblemDetails.Extensions.Add("Additional dummy info", "example info");
//        context.ProblemDetails.Extensions.Add("server", Environment.MachineName);
//    };
//});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// Add Swagger generation services to the service container.
builder.Services.AddSwaggerGen(setupAction =>
{
    // Get the full path of the XML documentation file for the API project.
    var xmlCommentsFullPath = Path.Combine(AppContext.BaseDirectory, "CityInfo.API.xml");

    // Include XML comments from the specified file in Swagger documentation.
    setupAction.IncludeXmlComments(xmlCommentsFullPath);
}); builder.Services.AddSwaggerGen(setupAction =>
{
    var xmlCommentsFullPath = Path.Combine(AppContext.BaseDirectory, "CityInfo.API.xml");
    setupAction.IncludeXmlComments(xmlCommentsFullPath);
});

//by compiler directory we're using different mail serivices for different builds
#if DEBUG
builder.Services.AddTransient<IMailService, LocalMailService>();
#else 
builder.Services.AddTransient<IMailService, CloudMailService>();
#endif

builder.Services.AddSingleton<CitiesDataStore>();
builder.Services.AddDbContext<CityInfoContext>(
   (dbContextOptions) => dbContextOptions.UseSqlite(builder.Configuration["ConnectionStrings:CityInfoDBConnectionString"]));
builder.Services.AddScoped<ICityInfoRepository, CityInfoRepository>();

// Configures AutoMapper to scan the profile configurations for mappings in the current application domain
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());



// Adding authentication services with JWT bearer authentication
builder.Services.AddAuthentication("Bearer").AddJwtBearer(options =>
{
    // Configuring token validation parameters
    options.TokenValidationParameters = new()
    {
        // Validating the issuer of the token
        ValidateIssuer = true,
        // Validating the audience of the token
        ValidateAudience = true,
        // Validating the signing key used to sign the token
        ValidateIssuerSigningKey = true,
        // Setting the valid issuer from configuration
        ValidIssuer = builder.Configuration["Authentication:Issuer"],
        // Setting the valid audience from configuration
        ValidAudience = builder.Configuration["Authentication:Audience"],
        // Setting the issuer signing key from configuration
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Authentication:SecretForKey"]))
    };
});

// Configure API versioning 
builder.Services.AddApiVersioning(setupAction =>
{
    // Enable reporting of API versions in responses
    setupAction.ReportApiVersions = true;

    // Set the default API version to 1.0
    setupAction.DefaultApiVersion = new ApiVersion(1, 0);

    // Assume the default API version (1.0) when no version is specified in the request
    setupAction.AssumeDefaultVersionWhenUnspecified = true;
}).AddMvc();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    //! for global exception handling
    //! if we want to use global exception handling we've to
    //! manually add "builder.Services.AddProblemDetails()" services
    app.UseExceptionHandler();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseEndpoints(endpoints => endpoints.MapControllers());
//app.MapControllers();

app.Run();
