using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
public class AuthorizeCheckOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var hasAuth = context.MethodInfo.DeclaringType
                        .GetCustomAttributes(true)
                        .OfType<AuthorizeAttribute>()
                      .Any()
                      || context.MethodInfo
                        .GetCustomAttributes(true)
                        .OfType<AuthorizeAttribute>()
                        .Any();

        if (!hasAuth) return;

        operation.Responses.TryAdd("401",
          new OpenApiResponse { Description = "Unauthorized" });

        var bearerScheme = new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            }
        };

        operation.Security = new List<OpenApiSecurityRequirement>
    {
      new OpenApiSecurityRequirement
      {
        [bearerScheme] = Array.Empty<string>()
      }
    };
    }
}
