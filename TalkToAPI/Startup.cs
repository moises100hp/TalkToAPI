using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TalkToAPI.Database;
using TalkToAPI.Helpers;
using TalkToAPI.Helpers.Constants;
using TalkToAPI.Helpers.Swagger;
using TalkToAPI.V1.Models;
using TalkToAPI.V1.Repositories;
using TalkToAPI.V1.Repositories.Contracts;

namespace TalkToAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            #region Auto Mapper - Configuração
            var config = new MapperConfiguration(configu =>
            {
                configu.AddProfile(new DTOMapperProfile());
            });
            IMapper mapper = config.CreateMapper();
            services.AddSingleton(mapper);

            #endregion


            services.Configure<ApiBehaviorOptions>(op =>
            {
                op.SuppressModelStateInvalidFilter = true;
            });
            services.AddScoped<IMensagemRepository, MensagemRepository>();
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddScoped<ITokenRepository, TokenRepository>();
            services.AddDbContext<TalkToContext>(cfg =>
            {
                cfg.UseSqlite("Data Source=Database\\TalkTo.db");
            });

            /*
             Origin: 
                        Domínio(Sub): api.site.com.br != www.site.com.br != web.site.com.br != www.empresa.com.br
                        Domínio(Protocolo): http://ww.site.com.br != https://www.site.com.br
                        Domínio(Porta): http://www.site.com.br:80 != http://www.site.com.br:367
             */

            services.AddCors(cfg =>
            {
                cfg.AddDefaultPolicy(policy =>
                {
                    policy
                    .WithOrigins("https://localhost:44371", "https://localhost:44371")
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .SetIsOriginAllowedToAllowWildcardSubdomains() //Habilitar CORS para todos os Subdomínios.
                    .WithMethods("GET")
                    .WithHeaders("Accept", "Authorization");
                });

                //Politica Específica - Habilitar todos os sites, com restrição.

                cfg.AddPolicy("AnyOrigin", policy =>
                {
                    policy.AllowAnyOrigin()
                          .WithMethods("GET")
                          .AllowAnyHeader();

                });
            });
            services.AddMvc(cfg =>
            {
                cfg.ReturnHttpNotAcceptable = true;
                cfg.InputFormatters.Add(new XmlSerializerInputFormatter(cfg));
                cfg.OutputFormatters.Add(new XmlSerializerOutputFormatter());

                var jsonOutputFormatter = cfg.OutputFormatters.OfType<JsonOutputFormatter>().FirstOrDefault();

                if (jsonOutputFormatter != null)
                {
                    jsonOutputFormatter.SupportedMediaTypes.Add(CustomMediaType.Heteoas);
                }
            })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddJsonOptions(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);


            services.AddApiVersioning(cfg =>
            {
                cfg.ReportApiVersions = true;
                cfg.AssumeDefaultVersionWhenUnspecified = true;
                cfg.DefaultApiVersion = new ApiVersion(1, 0);
            });

            services.AddSwaggerGen(cfg =>
            {
                cfg.AddSecurityDefinition("Bearer", new ApiKeyScheme()
                {
                    In = "header",
                    Type = "apiKey",
                    Description = "Adicione o JSON Web Token para autenticar",
                    Name = "Authorization"
                });

                var security = new Dictionary<string, IEnumerable<string>>()
                {
                    { "Bearer", new string[] { } }
                };
                cfg.AddSecurityRequirement(security);

                cfg.ResolveConflictingActions(apiDescription => apiDescription.First());
                cfg.SwaggerDoc("v1.0", new Swashbuckle.AspNetCore.Swagger.Info()
                {
                    Title = "TalkToAPI - V1.0",
                    Version = "v1.0"
                });

                var caminhoProjeto = PlatformServices.Default.Application.ApplicationBasePath;
                var nomeProjeto = $"{PlatformServices.Default.Application.ApplicationName}.xml";
                var caminhoArquivoXMLComentario = Path.Combine(caminhoProjeto, nomeProjeto);

                cfg.IncludeXmlComments(caminhoArquivoXMLComentario);

                cfg.DocInclusionPredicate((docName, apiDesc) =>
                {
                    var actionApiVersionModel = apiDesc.ActionDescriptor?.GetApiVersion();
                    // would mean this action is unversioned and should be included everywhere
                    if (actionApiVersionModel == null)
                    {
                        return true;
                    }
                    if (actionApiVersionModel.DeclaredApiVersions.Any())
                    {
                        return actionApiVersionModel.DeclaredApiVersions.Any(v => $"v{v.ToString()}" == docName);
                    }
                    return actionApiVersionModel.ImplementedApiVersions.Any(v => $"v{v.ToString()}" == docName);
                });

                cfg.OperationFilter<ApiVersionOperationFilter>();

            });

            //Remove restrições de senha, NÃO recomendado.
            services.AddIdentity<AplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 5;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
            })
                .AddEntityFrameworkStores<TalkToContext>()
                .AddDefaultTokenProviders();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    //Parametro validação Token

                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("chave-api-jwt-minhas-tarefas"))
                };
            });

            services.AddAuthorization(auth =>
            {
                auth.AddPolicy("Bearer", new AuthorizationPolicyBuilder()
                                             .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                                             .RequireAuthenticatedUser()
                                             .Build()
                );
            });

            services.ConfigureApplicationCookie(options =>
            {
                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = 401;
                    return Task.CompletedTask;
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseStatusCodePages();
            app.UseAuthentication();
            app.UseHttpsRedirection();
            //app.UseCors(); //Desabilitar quando for usar atributos EnableCors/DisableCors.
            app.UseMvc();

            app.UseSwagger(); // /swagger/v1/swagger.json
            app.UseSwaggerUI(cfg =>
            {
                cfg.SwaggerEndpoint("/swagger/v1.0/swagger.json", "TalkToAPI - V1.0");
                cfg.RoutePrefix = string.Empty;
            });
        }
    }
}
