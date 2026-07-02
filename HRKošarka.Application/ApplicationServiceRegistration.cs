using HRKošarka.Application.Images;
using HRKošarka.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace HRKošarka.Application
{
    public static class ApplicationServiceRegistration
    {

        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            services.AddScoped<IImageService, ImageService>();
            services.AddScoped<PlayoffAdvancementService>();
            services.AddScoped<EmailNotificationService>();

            return services;
        }
    }
}
