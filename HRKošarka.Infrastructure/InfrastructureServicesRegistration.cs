using HRKošarka.Application.Contracts.Email;
using HRKošarka.Application.Contracts.Logging;
using HRKošarka.Application.Images;
using HRKošarka.Application.Models;
using HRKošarka.Application.Models.Email;
using HRKošarka.Infrastructure.EmailService;
using HRKošarka.Infrastructure.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HRKošarka.Infrastructure
{
    public static class InfrastructureServicesRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ImageSettings>(configuration.GetSection("ImageSettings"));
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
            services.Configure<ClientAppSettings>(configuration.GetSection("ClientAppSettings"));
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddScoped(typeof(IAppLogger<>), typeof(LoggerAdapter<>));
            return services;
        }
    }
}
