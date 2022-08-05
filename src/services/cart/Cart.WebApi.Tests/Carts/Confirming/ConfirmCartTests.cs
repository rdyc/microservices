using Cart.Carts;
using Cart.Carts.GettingCartById;
using Cart.WebApi.Requests;
using FluentAssertions;
using Ogooreck.API;
using Xunit;
using static Ogooreck.API.ApiSpecification;

namespace Carts.WebApi.Tests.Carts.Confirming;

public class ConfirmCartFixture : ApiSpecification<Program>, IAsyncLifetime
{
    public Guid CartId { get; private set; }

    public readonly Guid ClientId = Guid.NewGuid();

    public async Task InitializeAsync()
    {
        var openResponse = await Send(
            new ApiRequest(POST, URI("/carts"), BODY(new OpenCartRequest(ClientId)))
        );

        await CREATED_WITH_DEFAULT_HEADERS(eTag: 0)(openResponse);

        CartId = openResponse.GetCreatedId<Guid>();
    }

    public Task DisposeAsync() => Task.CompletedTask;
}

public class ConfirmCartTests : IClassFixture<ConfirmCartFixture>
{

    private readonly ConfirmCartFixture API;

    public ConfirmCartTests(ConfirmCartFixture api) => API = api;

    // [Fact]
    [Trait("Category", "Acceptance")]
    public async Task Put_Should_Return_OK_And_Cancel__Cart()
    {
        await API
            .Given(
                URI($"/carts/{API.CartId}/confirm"),
                HEADERS(IF_MATCH(0))
            )
            .When(PUT)
            .Then(OK);

        await API
            .Given(
                URI($"/carts/{API.CartId}")
            )
            .When(GET_UNTIL(RESPONSE_ETAG_IS(1)))
            .Then(
                OK,
                RESPONSE_BODY<CartDetails>(details =>
                {
                    details.Id.Should().Be(API.CartId);
                    details.Status.Should().Be(CartStatus.Confirmed);
                    details.Products.Should().BeEmpty();
                    details.ClientId.Should().Be(API.ClientId);
                    details.Version.Should().Be(1);
                }));

        // API.PublishedExternalEventsOfType<CartFinalized>();
    }
}
