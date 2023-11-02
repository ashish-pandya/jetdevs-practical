using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace JetDevs.Common.Web.Setup
{
    /// <summary>
    /// Swagger configuration class
    /// </summary>
    public class Swagger
    {
        /// <summary>
        /// Configures Swagger Services
        /// </summary>
        /// <param name="services"></param>
        /// <param name="fileName"></param>
        public static void ConfigureServices(IServiceCollection services, string fileName)
        {
            // Register the Swagger generator, defining one or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "JetDevs API", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description =
                    "JWT Authorization header.\r\n Enter 'Bearer' [space] and then your token.\r\n\r\nExample: \"Bearer 12345abcdef\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,

                        },
                        new List<string>()
                    }
                });

                // Set the comments path for the Swagger JSON and UI.
                var basePath = AppContext.BaseDirectory;
                var xmlPath = Path.Combine(basePath, fileName);
                if (File.Exists(xmlPath))
                {
                    c.IncludeXmlComments(xmlPath);
                }
            });
        }

        /// <summary>
        /// Configure swagger endpoint
        /// </summary>
        /// <param name="app"></param>
        /// <param name="siteName"></param>
        public static void Configure(IApplicationBuilder app, string siteName)
        {
            var endpointName = siteName.ToLower();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger(o =>
            {
                o.RouteTemplate = "swagger/" + endpointName + "/{documentName}/swagger.json";
            });

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = "swagger";
                c.SwaggerEndpoint("/swagger/" + endpointName + "/v1/swagger.json", String.Format("{0} API V1", siteName));
            });
        }
    }
}
