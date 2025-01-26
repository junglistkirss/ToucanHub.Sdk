using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NJsonSchema;
using NJsonSchema.Generation;
using NJsonSchema.Generation.TypeMappers;
using System.Reflection.Metadata;
using Toucan.Sdk.Api.Features;
using Toucan.Sdk.Api.Middlewares;
using Toucan.Sdk.Contracts;
using Toucan.Sdk.Contracts.JsonData;

namespace Toucan.Sdk.Api.Extensions;


public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder ConfigureJsonApi(this WebApplicationBuilder builder, ApiConfiguration? config = null, Action<IMvcCoreBuilder>? mvcCoreConfigure = null)
    {
        builder.Services.ConfigureHttpJsonOptions(options =>
        {
            CommonJson.JsonSerializerOptionsConfiguration(options.SerializerOptions);
        });
        return builder;
    }
    [Obsolete("Magic method")]
    public static WebApplicationBuilder ConfigureApi(this WebApplicationBuilder builder, ApiConfiguration? config = null, Action<IMvcCoreBuilder>? mvcCoreConfigure = null)
    {
        ApiConfiguration apiConfig = config ?? ApiConfiguration.Default;

        #region Json
        // for controllers
        builder.Services.AddSingleton(CommonJson.SerializerOptionsInstance);
        // for minimal api
        builder.Services.ConfigureHttpJsonOptions(options =>
        {
            CommonJson.JsonSerializerOptionsConfiguration(options.SerializerOptions);
        });
        #endregion

        //builder.WebHost.UseHttpSys(options =>
        //{
        //    options.Authentication.Schemes = Microsoft.AspNetCore.Server.HttpSys.AuthenticationSchemes.None;
        //    options.Authentication.AllowAnonymous = true;
        //    options.EnableKernelResponseBuffering = true;
        //});

        builder.Services
            .AddExceptionHandler<GlobalExceptionHandler>();

        builder.Services
            //.AddSingleton(apiConfig)
            .AddHttpContextAccessor()
            //.AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
            .AddTransient<IAuthorizationHandler, GrantPermissionsHandler>()
            .AddTransient<IAuthorizationHandler, ScopeHandler>()
            .UseFeatureApiContextResolver();

        IMvcCoreBuilder mvcCore = builder.Services
            .AddMvcCore()
            .AddApiExplorer()
            .AddAuthorization()
            .AddCors()
            .AddDataAnnotations()
            .AddFormatterMappings();
        mvcCoreConfigure?.Invoke(mvcCore);

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerDocument(document =>
        {
            if (document.SchemaSettings is SystemTextJsonSchemaGeneratorSettings t)
                CommonJson.JsonSerializerOptionsConfiguration(t.SerializerOptions);

            document.SchemaSettings.TypeMappers.Add(
                new ObjectTypeMapper(typeof(JsonDataObject), new JsonSchema()
                {
                    Type = JsonObjectType.Object,
                    AllowAdditionalProperties = true,
                    AdditionalPropertiesSchema = new JsonSchema()
                    {
                        Type = JsonObjectType.None,
                    }
                }));
            document.SchemaSettings.TypeMappers.Add(
                new PrimitiveTypeMapper(typeof(Slug), x =>
                {
                    x.Type = JsonObjectType.String;
                    x.Example = "example";
                    x.Pattern = NamingConvention.SlugPattern;
                    x.MinLength = 1;
                    x.MaxLength = 64;
                }));
            document.SchemaSettings.TypeMappers.Add(new PrimitiveTypeMapper(typeof(DomainId), x =>
                {
                    x.Type = JsonObjectType.String;
                    x.Example = DomainId.Empty.ToString();
                    x.Pattern = "[A-Za-z0-9-~]";
                    x.MinLength = 1;
                }));
            document.SchemaSettings.TypeMappers.Add(new PrimitiveTypeMapper(typeof(Tag), x =>
                {
                    x.Type = JsonObjectType.String;
                    x.Example = "TAG_EXAMPLE";
                    x.Pattern = NamingConvention.TagPattern;
                    x.MinLength = 1;
                    x.MaxLength = 100;
                }));
            document.SchemaSettings.TypeMappers.Add(new PrimitiveTypeMapper(typeof(Color), x =>
                {
                    x.Type = JsonObjectType.String;
                    x.Example = "#FF0000";
                    x.Pattern = NamingConvention.ColorPattern;
                    x.MinLength = 4;
                    x.MaxLength = 7;
                }));
            document.SchemaSettings.TypeMappers.Add(new PrimitiveTypeMapper(typeof(JsonDataValue), x => x.Type = JsonObjectType.None));
            document.SchemaSettings.TypeMappers.Add(new PrimitiveTypeMapper(typeof(RefToken), x => x.Type = JsonObjectType.String));
            document.SchemaSettings.TypeMappers.Add(new PrimitiveTypeMapper(typeof(Tenant), x => x.Type = JsonObjectType.String));

            document.SchemaSettings.SchemaNameGenerator = new ToucanSchemaNameGenerator();
            document.SchemaSettings.GenerateExamples = true;
            apiConfig.OpenApiDocumentGenerator?.Invoke(document);

        });
        return builder;
    }
    public static WebApplicationBuilder ConfigureMvcApi(this WebApplicationBuilder builder, Action<IMvcCoreBuilder>? mvcCoreConfigure = null)
    {
        //builder.WebHost.UseHttpSys(options =>
        //{
        //    options.Authentication.Schemes = Microsoft.AspNetCore.Server.HttpSys.AuthenticationSchemes.None;
        //    options.Authentication.AllowAnonymous = true;
        //    options.EnableKernelResponseBuffering = true;
        //});

        builder.Services
            .AddHttpContextAccessor();

        builder.Services
            .AddExceptionHandler<GlobalExceptionHandler>();

        IMvcCoreBuilder mvcCore = builder.Services
            .AddMvcCore()
            .AddApiExplorer()
            .AddCors()
            .AddDataAnnotations()
            .AddFormatterMappings();
        mvcCoreConfigure?.Invoke(mvcCore);

        builder.Services.AddEndpointsApiExplorer();

        return builder;
    }
    public static void ConfigureSwaggerApi(this WebApplicationBuilder builder, ApiConfiguration? config = null)
    {
        ApiConfiguration apiConfig = config ?? ApiConfiguration.Default;
        builder.Services.AddSwaggerDocument(document =>
        {
            if (document.SchemaSettings is SystemTextJsonSchemaGeneratorSettings t)
                CommonJson.JsonSerializerOptionsConfiguration(t.SerializerOptions);

            document.SchemaSettings.TypeMappers.Add(
                new ObjectTypeMapper(typeof(JsonDataObject), new JsonSchema()
                {
                    Type = JsonObjectType.Object,
                    AllowAdditionalProperties = true,
                    AdditionalPropertiesSchema = new JsonSchema()
                    {
                        Type = JsonObjectType.None,
                    }
                }));
            document.SchemaSettings.TypeMappers.Add(new PrimitiveTypeMapper(typeof(Slug), x =>
                    {
                        x.Type = JsonObjectType.String;
                        x.Example = "example";
                        x.Pattern = NamingConvention.SlugPattern;
                        x.MinLength = 1;
                        x.MaxLength = 64;
                    }));
            document.SchemaSettings.TypeMappers.Add(new PrimitiveTypeMapper(typeof(DomainId), x =>
                    {
                        x.Type = JsonObjectType.String;
                        x.Example = DomainId.Empty.ToString();
                        x.Pattern = "[A-Za-z0-9-~]";
                        x.MinLength = 1;
                    }));
            document.SchemaSettings.TypeMappers.Add(new PrimitiveTypeMapper(typeof(Tag), x =>
                    {
                        x.Type = JsonObjectType.String;
                        x.Example = "TAG_EXAMPLE";
                        x.Pattern = NamingConvention.TagPattern;
                        x.MinLength = 1;
                        x.MaxLength = 100;
                    }));
            document.SchemaSettings.TypeMappers.Add(new PrimitiveTypeMapper(typeof(Color), x =>
                    {
                        x.Type = JsonObjectType.String;
                        x.Example = "#FF0000";
                        x.Pattern = NamingConvention.ColorPattern;
                        x.MinLength = 4;
                        x.MaxLength = 7;
                    }));
            document.SchemaSettings.TypeMappers.Add(new PrimitiveTypeMapper(typeof(JsonDataValue), x => x.Type = JsonObjectType.None));
            document.SchemaSettings.TypeMappers.Add(new PrimitiveTypeMapper(typeof(RefToken), x => x.Type = JsonObjectType.String));
            document.SchemaSettings.TypeMappers.Add(new PrimitiveTypeMapper(typeof(Tenant), x => x.Type = JsonObjectType.String));

            document.SchemaSettings.SchemaNameGenerator = new ToucanSchemaNameGenerator();
            document.SchemaSettings.GenerateExamples = true;
            apiConfig.OpenApiDocumentGenerator?.Invoke(document);

        });
    }

    public static WebApplication BuildWebApplication(this WebApplicationBuilder builder)
    {
        WebApplication app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
            app.UseDeveloperExceptionPage();
        else
        {
            app.UseHttpsRedirection();
        }
        app.UseMiddleware<AddStopWatchHeader>();


        return app;
    }

}