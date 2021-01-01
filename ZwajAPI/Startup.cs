using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Stripe;
using ZwajAPI.Data;
using ZwajAPI.Helpers;
using ZwajAPI.Models;

namespace ZwajAPI
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
            services.AddDbContext<DataContext>(x=> x.UseSqlite(Configuration.GetConnectionString("DefaultConnection"))) ; 
            IdentityBuilder builder = services.AddIdentityCore<User>(opt=>
            {
                opt.Password.RequireDigit=false ;
                opt.Password.RequiredLength = 4 ;
                opt.Password.RequireNonAlphanumeric = false ;
                opt.Password.RequireUppercase = false ;  



            }) ;
            builder = new IdentityBuilder(builder.UserType,typeof(Role),builder.Services);
            builder.AddEntityFrameworkStores<DataContext>() ;
            builder.AddRoleValidator<RoleValidator<Role>>() ;
            builder.AddRoleManager<RoleManager<Role>>() ; 
            builder.AddSignInManager<SignInManager<User>>()  ; 
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(Options=>
            {
                Options.TokenValidationParameters = new TokenValidationParameters{
                    ValidateIssuerSigningKey = true , 
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration.GetSection("AppSettings:Token").Value)) , 
                    ValidateIssuer = false , 
                    ValidateAudience = false 
                    //development ( ValidateIssuer , ValidateAudience  = false )
                

                };
            }) ; 
            services.AddAuthorization(options=>
            {
                options.AddPolicy("RequireAdminRole",policy=>policy.RequireRole("Admin"));
                options.AddPolicy("RequirePhotoRole",policy=>policy.RequireRole("Admin","Moderator"));
                options.AddPolicy("VIPOnly",policy=>policy.RequireRole("VIP"));



            });
            services.AddMvc(options=>
            {
                var policy =  new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
                options.Filters.Add(new AuthorizeFilter(policy)) ; 

                
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
            .AddJsonOptions(option => 
            {
                option.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore ;

            });

            services.AddSingleton(typeof(IConverter),new SynchronizedConverter(new PdfTools())) ;
             services.AddCors();
             services.AddSignalR() ; 

            services.AddAutoMapper();
            //Mapper.Reset() ;
            services.Configure<CloudinarySettings>(Configuration.GetSection("CloudinarySettings"));
            services.Configure<StripeSettings>(Configuration.GetSection("Stripe:SecretKey"));


            //للكلاسات الخفيفة 
            services.AddTransient<TrialData>() ; 
            //services.AddScoped<IAuthRepository,AuthRepository>() ;
            services.AddScoped<IZwajRepository,ZwajRepository>() ;
            services.AddScoped<LogUserActivity>();

           

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env , TrialData trialData)
        {
            StripeConfiguration.SetApiKey(Configuration.GetSection("Stripe:SecretKey").Value);
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {   // Production mode 
                app.UseExceptionHandler(BuilderExtensions =>
                 {

                     BuilderExtensions.Run(async context =>
                     {
                         context.Response.StatusCode = (int)HttpStatusCode.InternalServerError ; 
                         var error = context.Features.Get<IExceptionHandlerFeature>() ; 
                         if (error !=null)
                         {
                             //نص الرسالة في response header 
                             context.Response.AddApplicationError(error.Error.Message) ; 
                             await context.Response.WriteAsync(error.Error.Message); 
                         }

                     });


                
                 });


            }
            trialData.TrialUsers() ; 
            app.UseCors(x=> x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().AllowCredentials());
            app.UseSignalR(route=>
            {
                route.MapHub<ChatHub>("/chat") ;
            
            });

            app.UseAuthentication() ; 
            app.UseMvc();

        }
    }
}
