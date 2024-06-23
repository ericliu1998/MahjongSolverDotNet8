using MahjongSolverApiDotNet8.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<ISolveHandService, SolveHandService>();
builder.Services.AddAWSLambdaHosting(LambdaEventSource.RestApi);

builder.Services.AddCors(options =>
{
    options.AddPolicy("MjSolver", builder =>
    {
        builder.WithOrigins("https://ericliu1998.github.io");
        builder.AllowAnyMethod();
        builder.AllowAnyHeader();
    });

    options.AddPolicy("localTesting", builder =>
    {
        builder.AllowAnyOrigin();
        builder.AllowAnyMethod();
        builder.AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseRouting();

app.UseCors("MjSolver");
//app.UseCors("localTesting");

app.UseAuthorization();

app.MapControllers();

app.Run();
