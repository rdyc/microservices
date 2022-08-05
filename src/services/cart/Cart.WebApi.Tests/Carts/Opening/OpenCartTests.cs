using Cart.Carts;
using Cart.WebApi.Requests;
using FluentAssertions;
using FW.Core.Testing;
using Ogooreck.API;
using Xunit;
using static Ogooreck.API.ApiSpecification;

namespace Carts.WebApi.Tests.Carts.Opening;

public class OpenCartTests : IClassFixture<TestWebApplicationFactory<Program>>
{
    private readonly ApiSpecification<Program> API;
    public readonly Guid ClientId = Guid.NewGuid();

    public OpenCartTests(TestWebApplicationFactory<Program> fixture) =>
        API = ApiSpecification<Program>.Setup(fixture);

    [Fact]
    public Task Post_ShouldReturn_CreatedStatus_With_CartId() =>
        API.Scenario(
            API.Given(
                    URI("/carts"),
                    BODY(new OpenCartRequest(ClientId))
                )
                .When(POST)
                // .Then(CREATED_WITH_DEFAULT_HEADERS(eTag: 0)),
                .Then(CREATED),

            response =>
                API.Given(
                        URI($"/carts/{response.GetCreatedId()}")
                    )
                    // .When(GET_UNTIL(RESPONSE_ETAG_IS(0)))
                    .When(GET_UNTIL(RESPONSE_SUCCEEDED()))
                    .Then(
                        OK,
                        RESPONSE_BODY<Cart.Carts.Cart>(cart =>
                        {
                            cart.Id.Should().Be(response.GetCreatedId<Guid>());
                            cart.Status.Should().Be(CartStatus.Pending);
                            cart.Products.Should().BeEmpty();
                            cart.ClientId.Should().Be(ClientId);
                            cart.Version.Should().Be(0);
                        }))
        );
}