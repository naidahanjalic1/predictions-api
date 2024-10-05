using PredictionModels.Service;
using PreditionModels.Helpers;
using Python.Runtime;

var builder = WebApplication.CreateBuilder(args);

var pythonPath = builder.Configuration["PythonPath"];
Environment.SetEnvironmentVariable("PYTHONPATH", pythonPath);
Environment.SetEnvironmentVariable("PYTHONHOME", pythonPath);
Runtime.PythonDLL = builder.Configuration["PythonDll"];

// Initialize the Python engine
PythonEngine.Initialize();
PythonEngine.BeginAllowThreads();


// Train models
ModelTrainer.TrainXGBoostModel();
ModelTrainer.TrainTimeseriesModel();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<ForecastService>();

var app = builder.Build();

app.Lifetime.ApplicationStopping.Register(() =>
{
    PythonEngine.Shutdown();
});

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
