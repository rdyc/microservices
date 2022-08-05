using Cart.Carts;
using Cart.Carts.GettingCartById;
using Cart.WebApi.Requests;
using FluentAssertions;
using Ogooreck.API;
using Xunit;
using static Ogooreck.API.ApiSpecification;

namespace Carts.WebApi.Tests.Carts.Canceling;

public class CancelCartFixture : ApiSpecification<Program>, IAsyncLifetime
{
    public Guid CartId { get; private set; }

    public readonly Guid ClientId = Guid.NewGuid();

    public async Task InitializeAsync()
    {
        var openResponse = await Send(
            new ApiRequest(POST, URI("/api/Carts"), BODY(new OpenCartRequest(ClientId)))
        );

        await CREATED_WITH_DEFAULT_HEADERS(eTag: 0)(openResponse);

        CartId = openResponse.GetCreatedId<Guid>();
    }

    public Task DisposeAsync() => Task.CompletedTask;
}

public class CancelCartTests : IClassFixture<CancelCartFixture>
{
    private readonly CancelCartFixture API;

    public CancelCartTests(CancelCartFixture api) => API = api;

    // [Fact]
    [Trait("Category", "Acceptance")]
    public async Task Delete_Should_Return_OK_And_Cancel__Cart()
    {
        await API
            .Given(
                URI($"/api/Carts/{API.CartId}"),
                HEADERS(IF_MATCH(0))
            )
            .When(DELETE)
            .Then(OK);

        await API
            .Given(
                URI($"/api/Carts/{API.CartId}")
            )
            .When(GET_UNTIL(RESPONSE_ETAG_IS(1)))
            .Then(
                OK,
                RESPONSE_BODY<CartDetails>(details =>
                {
                    details.Id.Should().Be(API.CartId);
                    details.Status.Should().Be(CartStatus.Canceled);
                    details.Products.Should().BeEmpty();
                    details.ClientId.Should().Be(API.ClientId);
                    details.Version.Should().Be(1);
                }));

        // API.PublishedExternalEventsOfType<CartFinalized>();
    }
}
