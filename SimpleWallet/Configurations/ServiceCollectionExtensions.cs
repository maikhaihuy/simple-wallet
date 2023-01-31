using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using SimpleWallet.Apis.Auth;
using SimpleWallet.Apis.Transaction;

namespace SimpleWallet.Configurations
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Simple wallet", Version = "v1" });
                // c.OperationFilter<SwaggerAuthorizeOperationFilter>();
                //
                // c.AddSecurityDefinition("Bearer",
                //     new OpenApiSecurityScheme
                //     {
                //         Description = "`Token only!!!` - without `Bearer_` prefix",
                //         Type = SecuritySchemeType.Http,
                //         BearerFormat = "JWT",
                //         In = ParameterLocation.Header,
                //         Scheme = "bearer"
                //     });
            });

            services.AddApiVersioning(options =>
            {
                // reporting api versions will return the headers "api-supported-versions" and "api-deprecated-versions"
                // options.ReportApiVersions = true;
                // Specify the default API Version as 1.0
                options.DefaultApiVersion = new ApiVersion(1, 0);
                // If the client hasn't specified the API version in the request, use the default API version number
                options.AssumeDefaultVersionWhenUnspecified = true;
            });

            services.AddVersionedApiExplorer(options =>
            {
                // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
                // note: the specified format code will format the version as "'v'major[.minor][-status]"
                options.GroupNameFormat = "'v'VVV";
                // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
                // can also be used to control the format of the API version in route templates
                options.SubstituteApiVersionInUrl = true;
            });

            return services;
        }
        
        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            services.AddTransient<IAuthService, AuthService>();
            services.AddTransient<ITransactionService, TransactionService>();
            
            return services;
        }
        
        public static T ConfigureAndGet<T>(
            this IConfiguration configuration, IServiceCollection services) where T: class
        {
            var appSettings = configuration.Get<T>();
            return appSettings;
        }
    }
}