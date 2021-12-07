using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using CustomTemplate.Exceptions;
using static System.Net.Mime.MediaTypeNames;

namespace CustomTemplate.Installers.Models
{
    public class MvcInstaller : IServiceInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers(options =>
                options.Filters.Add(new HttpResponseExceptionFilter()))
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.SuppressConsumesConstraintForFormFileParameters = true;
                    options.SuppressInferBindingSourcesForParameters = true;
                    options.SuppressModelStateInvalidFilter = true;
                    options.SuppressMapClientErrors = true;
                    options.ClientErrorMapping[StatusCodes.Status404NotFound].Link =
                        "https://httpstatuses.com/404";

                    options.InvalidModelStateResponseFactory = context =>
                    {
                        int k = 0;
                        var keys = context.ModelState.Keys.ToArray();
                        var errors = new Dictionary<object, List<string>>();

                        foreach (var value in context.ModelState.Values) 
                        {
                            var errorMessages = new List<string>();
                            for (int i = 1; i <= value.Errors.Count(); i++)
                            {
                                var err = value.Errors[i - 1] as Microsoft.AspNetCore.Mvc.ModelBinding.ModelError;
                                errorMessages.Add(err.ErrorMessage);
                            }

                            errors[keys[k]] = errorMessages;
                            k++;
                        }

                        var result = new BadRequestObjectResult(new
                        {
                            Message = "Fill all required fields.",
                            StatusCode = 400,
                            Details = errors,
                            TimeStamp = DateTime.Now
                        });

                        result.ContentTypes.Add(Application.Json);
                        result.ContentTypes.Add(Application.Xml);

                        return result;
                    };
                });

            services.AddApiVersioning(config =>
            {
                config.DefaultApiVersion = new ApiVersion(1, 0);
                config.AssumeDefaultVersionWhenUnspecified = true;
                config.ReportApiVersions = true;
            });
                
            services.AddEndpointsApiExplorer();

#if (EnableSwaggerSupport)

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "CustomTemplate", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Authorization using JWT.",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "Bearer",
                            Name = "Bearer",
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                    }
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
            });
#endif

        }
    }
}