using RubberSearch.Core.Services;
using RubberSearch.Core.Repositories;
using RubberSearch.Infrastructure;
using RubberSearch.Core.Utilities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var dataPath = Path.Combine(builder.Environment.ContentRootPath, "..", "data");

builder.Services.AddSingleton<IDocumentRepository>(new DocumentRepository(dataPath));
builder.Services.AddSingleton<IInvertedIndexRepository>(new InvertedIndexRepository(dataPath));
builder.Services.AddSingleton<IIndexingService, IndexingService>();
builder.Services.AddSingleton(new ApiKeyMapper(dataPath));

var app = builder.Build();

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
