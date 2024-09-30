namespace Gringotts.Api.Shared
{
    public interface IEndpoint
    {
        /// <summary>
        /// Maps the endpoint to the application's request pipeline.
        /// </summary>
        /// <param name="app">The endpoint route builder to map the endpoint to.</param>
        void MapEndpoint(IEndpointRouteBuilder app);
    }
}