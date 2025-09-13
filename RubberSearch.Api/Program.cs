using Microsoft.AspNetCore.Identity;
using RubberSearch.Core.Services;
using RubberSearch.Infrastructure;
using RubberSearch.Infrastructure.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var dataPath = Path.Combine(builder.Environment.ContentRootPath, "..", "data");

builder.Services.AddSingleton<IDocumentRepository>(new DocumentRepository(dataPath));
builder.Services.AddSingleton<IInvertedIndexRepository>(new InvertedIndexRepository(dataPath));
builder.Services.AddSingleton<IIndexingService, IndexingService>();

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
