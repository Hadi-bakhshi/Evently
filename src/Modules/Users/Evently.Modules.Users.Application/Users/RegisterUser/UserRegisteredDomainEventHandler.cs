using Evently.Common.Application.Exceptions;
using Evently.Common.Application.Messaging;
using Evently.Common.Domain;
using Evently.Modules.Ticketing.PublicApi;
using Evently.Modules.Users.Application.Users.GetUser;
using Evently.Modules.Users.Domain.Users;
using MediatR;

namespace Evently.Modules.Users.Application.Users.RegisterUser;
internal sealed class UserRegisteredDomainEventHandler(ISender sender, ITicketingApi ticketingApi)
    : IDomainEventHandler<UserRegisteredDomainEvent>
{
    public async Task Handle(UserRegisteredDomainEvent notification, CancellationToken cancellationToken)
    {

        Result<UserResponse> userResult = await sender.Send(new GetUserQuery(notification.UserId), cancellationToken);

        if (userResult.IsFailure)
        {
            throw new EventlyException(nameof(GetUserQuery), userResult.Error);
        }


        await ticketingApi.CreateCustomerAsync(
            userResult.Value.Id,
            userResult.Value.Email,
            userResult.Value.FirstName,
            userResult.Value.LastName,
            cancellationToken);

    }
}
