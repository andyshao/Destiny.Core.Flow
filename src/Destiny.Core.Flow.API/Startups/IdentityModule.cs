﻿using Destiny.Core.Flow.Extensions;
using Destiny.Core.Flow.Model.Entities.Identity;
using Destiny.Core.Flow.Model.Security;
using Destiny.Core.Flow.Options;
using Destiny.Core.Flow.Security.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Destiny.Core.Flow.API.Startups
{
    public class IdentityModule : IdentityModuleBase<UserStore, RoleStore, User, UserRole, Role, Guid, Guid>
    {

        public override IServiceCollection ConfigureServices(IServiceCollection services)
        {
            var services1 = base.ConfigureServices(services);


            return services1;
        }
        public override void Configure(IApplicationBuilder app)
        {

            //app.UseAuthentication();

        }

        protected override Action<IdentityOptions> IdentityOption()
        {
            return options =>
            {

                options.SignIn.RequireConfirmedEmail = false;

                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;

                options.User.RequireUniqueEmail = false;

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);

                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;



            };
        }

        protected override void AddAuthentication(IServiceCollection services)
        {
          
            AppOptionSettings settings = services.GetAppSettings();
            var jwt = settings.Jwt;

            var keyByteArray = Encoding.UTF8.GetBytes(jwt.SecretKey);
            var signingKey = new SymmetricSecurityKey(keyByteArray);

            var tokenValidationParameters = new TokenValidationParameters
            {

                IssuerSigningKey = signingKey,
                ValidIssuer = jwt.Issuer ?? "Destiny",//发行人
                ValidAudience = jwt.Audience ?? "Destiny",//订阅人
                //ValidateLifetime = false,   ////是否验证Token有效期，使用当前时间与Token的Claims中的NotBefore和Expires对比
                ClockSkew = TimeSpan.Zero, ////允许的服务器时间偏移量
                LifetimeValidator = (nbf, exp, token, param) => exp > DateTime.UtcNow
            };

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                //x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(jwt =>
            {

                //jwt.SecurityTokenValidators.Clear();
                //jwt.SecurityTokenValidators.Add(new CmsJwtSecurityTokenHandler());
                jwt.TokenValidationParameters = tokenValidationParameters;
                jwt.Events = new JwtBearerEvents /*jwt自带事件*/
                {
                    OnAuthenticationFailed = context =>
                    {

                        // 如果过期，则把<是否过期>添加到，返回头信息中
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers.Add("Token-Expired", "true");
                        }
                        return Task.CompletedTask;
                    }

                };

            });
            services.AddScoped<IJwtBearerService, JwtBearerService>();
        }

        protected override IdentityBuilder UseIdentityBuilder(IdentityBuilder identityBuilder)
        {
            return identityBuilder.AddDefaultTokenProviders();
        }
    }
    
}
