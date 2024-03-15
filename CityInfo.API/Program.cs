var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(
    options =>
    {
        //return 406 error message if Accept type is not supported
        options.ReturnHttpNotAcceptable = true;
    }).AddXmlDataContractSerializerFormatters();

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
