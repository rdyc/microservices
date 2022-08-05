using Cart.Carts;
using Cart.Carts.GettingCartById;
using Cart.WebApi.Requests;
using FluentAssertions;
using Ogooreck.API;
using Xunit;
using static Ogooreck.API.ApiSpecification;

namespace Carts.WebApi.Tests.Carts.AddingProduct;

public class AddProductFixture : ApiSpecification<Program>, IAsyncLifetime
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

public class AddProductTests : IClassFixture<AddProductFixture>
{
    private readonly AddProductFixture API;

    public AddProductTests(AddProductFixture api) => API = api;

    // [Fact]
    [Trait("Category", "Acceptance")]
    public async Task Post_Should_AddProductItem_To_Cart()
    {
        var product = new AddProductRequest(Guid.NewGuid(), 1);

        await API
            .Given(
                URI($"/carts/{API.CartId}/products"),
                BODY(product),
                HEADERS(IF_MATCH(0))
            )
            .When(POST)
            .Then(OK);

        await API
            .Given(URI($"/carts/{API.CartId}"))
            .When(GET_UNTIL(RESPONSE_ETAG_IS(1)))
            .Then(
                RESPONSE_BODY<CartDetails>(details =>
                {
                    details.Id.Should().Be(API.CartId);
                    details.Status.Should().Be(CartStatus.Pending);
                    details.Products.Should().HaveCount(1);
                    // details.Products.Single().Should()
                    //     .Be(CartProduct.From(product.ProductId, product.Quantity));
                    details.Version.Should().Be(1);
                })
            );
    }
}