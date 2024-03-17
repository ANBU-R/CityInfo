
using Serilog;

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
builder.Services.AddSwaggerGen();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    //! for global exception handling
    //! if we want to use global exception handling we've to
    // ! manually add "builder.Services.AddProblemDetails()" services
    app.UseExceptionHandler();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.UseEndpoints(endpoints => endpoints.MapControllers());
//app.MapControllers();

app.Run();
