using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MailHub.Configuration
{
    public static class SwaggerExtension
    {
        public static void AddSwagger(this IServiceCollection services)
        {
              services.AddSwaggerGen(c =>
                        {
                           
                            c.SwaggerDoc("v1", new OpenApiInfo {Title = "MailHub", Version = "v1"});
                            
                            c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
                            {
                                Name = JwtBearerDefaults.AuthenticationScheme,
                                Scheme = JwtBearerDefaults.AuthenticationScheme,
                                Type = SecuritySchemeType.Http,
                                In = ParameterLocation.Header
                            });
                            
                            c.OperationFilter<BearerAuthOperationsFilter>();
                        });
        }
    }
    
    public class BearerAuthOperationsFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var authIsNotRequired = context.ApiDescription.CustomAttributes().Any(x => x.GetType() == typeof(AllowAnonymousAttribute));
            if (authIsNotRequired) return;
            
            operation.Security = new List<OpenApiSecurityRequirement>
            {
                new()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = JwtBearerDefaults.AuthenticationScheme
                            }
                        },
                        Array.Empty<string>()
                    }
                }
            };
        }
    }
}