using System;
using Amazon.SimpleEmailV2;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Aeta.Extensions.Aws.Ses
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAmazonSimpleEmailService(this IServiceCollection services,
            ServiceLifetime lifetime = ServiceLifetime.Scoped,
            Action<AmazonSimpleEmailServiceV2Config> configModifications = null)
        {
            var sesClient = new ServiceDescriptor(typeof(IAmazonSimpleEmailServiceV2), serviceProvider =>
            {
                var config = serviceProvider.GetAmazonSesClientConfig();
                configModifications?.Invoke(config);

                return new AmazonSimpleEmailServiceV2Client(config);
            }, lifetime);

            services.Add(sesClient);
            return services;
        }

        private static AmazonSimpleEmailServiceV2Config GetAmazonSesClientConfig(this IServiceProvider serviceProvider)
        {
            var configuration = serviceProvider.GetService<IConfiguration>()?
                .GetSection("amazon")?
                .GetSection("ses");

            var amazonConfig = new AmazonSimpleEmailServiceV2Config();
            if (configuration is null) return amazonConfig;

            configuration.Bind(amazonConfig);
            return amazonConfig;
        }
    }
}