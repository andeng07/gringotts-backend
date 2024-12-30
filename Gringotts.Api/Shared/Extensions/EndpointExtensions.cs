using System.Reflection;
using Gringotts.Api.Shared.Endpoints;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Gringotts.Api.Shared.Extensions
{
    public static class EndpointExtensions
    {
        /// <summary>
        /// Adds all endpoint implementations from the executing assembly to the service collection.
        /// </summary>
        /// <param name="services">The service collection to add the endpoints to.</param>
        /// <returns>The updated service collection.</returns>
        public static IServiceCollection AddEndpoints(this IServiceCollection services) =>
            services.AddEndpoints(Assembly.GetExecutingAssembly());
        

        /// <summary>
        /// Adds all endpoint implementations from the specified assembly to the service collection.
        /// </summary>
        /// <param name="services">The service collection to add the endpoints to.</param>
        /// <param name="assembly">The assembly to iterate for endpoint retrieval.</param>
        /// <returns>The updated service collection.</returns>
        public static IServiceCollection AddEndpoints(this IServiceCollection services, Assembly assembly) {
            var serviceDescriptors = assembly.DefinedTypes
                .Where(type => !type.IsAbstract && !type.IsInterface && typeof(IEndpoint).IsAssignableFrom(type))
                .Select(type => ServiceDescriptor.Transient(typeof(IEndpoint), type))
                .ToArray();

            services.TryAddEnumerable(serviceDescriptors);

            return services;
        }

        /// <summary>
        /// Execute endpoint mapping for each registered endpoint.
        /// </summary>
        /// <param name="app">The web application to map the endpoints to.</param>
        /// <param name="routeGroupBuilder">Optional route group builder for categorizing routes.</param>
        /// <returns>The updated web application.</returns>
        public static IApplicationBuilder MapEndpoints(this WebApplication app, RouteGroupBuilder? routeGroupBuilder = null) {
            var endpoints = app.Services.GetRequiredService<IEnumerable<IEndpoint>>();

            IEndpointRouteBuilder builder = routeGroupBuilder is null ? app : routeGroupBuilder;

            foreach (var endpoint in endpoints) {
                endpoint.MapEndpoint(builder);
            }

            return app;
        }

    }
}