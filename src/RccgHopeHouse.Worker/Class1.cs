using Microsoft.Extensions.DependencyInjection;

namespace RccgHopeHouse.Worker
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddWorker(this IServiceCollection services)
        {
            // Register worker-specific services here
            return services;
        }
    }
}
