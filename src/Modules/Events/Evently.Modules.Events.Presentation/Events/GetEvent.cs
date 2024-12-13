using Evently.Modules.Events.Application.Events.GetEvent;
using MediatR;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;

namespace Evently.Modules.Events.Presentation.Events;

internal static class GetEvent
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("events/{id:guid}", async (Guid id, ISender sender) =>
            {
                var query = new GetEventQuery(id);

                EventResponse @event = await sender.Send(query);


                return @event is null ? Results.NotFound() : Results.Ok(@event);
            })
            .WithTags(Tags.Events);
    }
}
