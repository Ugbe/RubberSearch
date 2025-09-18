using RubberSearch.Core.Services;
using RubberSearch.Core.Repositories;
using RubberSearch.Infrastructure;
using RubberSearch.Core.Utilities;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

// Recent addition
builder.Services.AddAuthentication("ApiKeyScheme")
    .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>("ApiKeyScheme", options => { });

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "RubbersearchAPI", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme. <br/>
                      Enter 'Bearer' [space] and then your token in the text input below.
                      <br/>Example: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.OperationFilter<AuthorizeCheckOperationFilter>();
});

var dataPath = Path.Combine(builder.Environment.ContentRootPath, "..", "data");

builder.Services.AddSingleton<IDocumentRepository>(new DocumentRepository(dataPath));
builder.Services.AddSingleton<IInvertedIndexRepository>(new InvertedIndexRepository(dataPath));
builder.Services.AddSingleton<IIndexingService, IndexingService>();
builder.Services.AddSingleton(new ApiKeyMapper(dataPath));
builder.Services.AddSingleton<ISearchService, SearchService>();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();                      
    app.UseSwaggerUI();                     
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();