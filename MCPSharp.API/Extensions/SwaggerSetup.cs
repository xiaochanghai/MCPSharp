using Microsoft.OpenApi.Models;

namespace MCPSharp.API.Extensions;

/// <summary>
/// Swagger 启动服务
/// </summary>
public static class SwaggerSetup
{
    /// <summary>
    /// AddSwaggerSetup
    /// </summary>
    /// <param name="services"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public static void AddSwaggerSetup(this IServiceCollection services)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));

        var basePath = AppContext.BaseDirectory;

        services.AddSwaggerGen(c =>
        {
            c.UseInlineDefinitionsForEnums();

            // API注释所需XML文件
            try
            {
                c.IncludeXmlComments(Path.Combine(basePath, "MCPSharp.API.xml"), true);
            }
            catch (Exception)
            {
            }

            //c.MapType<QueryFilter>(() => new OpenApiSchema { Type = "string", Format = "string" });


            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Scheme = "Bearer",
                Description = "JWT授权(数据将在请求头中进行传输) 直接在下框中输入Bearer {token}（注意两者之间是一个空格）\"",

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
                        }
                    },
                    new List<string>()
                }
            });

        });
    }
}