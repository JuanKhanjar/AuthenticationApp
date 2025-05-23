using AuthenticationApp.Application.Interfaces;
using AuthenticationApp.Infrastructure.Common;
using AuthenticationApp.Infrastructure.Persistence;
using AuthenticationApp.Infrastructure.Repositories;
using AuthenticationApp.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using AuthenticationApp.Domain.Interfaces;
using AuthenticationApp.Application.Common;
using AuthenticationApp.Application.Events.Handlers;
using AuthenticationApp.Domain.Events;
using AuthenticationApp.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace AuthenticationApp.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(config.GetConnectionString("DefaultConnection")));
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IEmailSender, EmailSender>();
            services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
            services.AddScoped<IDomainEventHandler<UserRegisteredDomainEvent>, SendEmailOnUserRegisteredHandler>();

            return services;
        }
    }
}
