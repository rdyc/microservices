using Cart.Carts;
using Cart.Carts.GettingCartById;
using Cart.WebApi.Requests;
using FluentAssertions;
using Ogooreck.API;
using Xunit;
using static Ogooreck.API.ApiSpecification;

namespace Carts.WebApi.Tests.Carts.RemovingProduct;

public class RemoveProductFixture : ApiSpecification<Program>, IAsyncLifetime
{
    public Guid CartId { get; private set; }

    public readonly Guid ClientId = Guid.NewGuid();

    public readonly RemoveProductRequest Product = new(Guid.NewGuid(), 1, 10);

    public decimal UnitPrice;

    public async Task InitializeAsync()
    {
        var openResponse = await Send(
            new ApiRequest(POST, URI("/carts"), BODY(new OpenCartRequest(ClientId)))
        );

        await CREATED_WITH_DEFAULT_HEADERS(eTag: 0)(openResponse);

        CartId = openResponse.GetCreatedId<Guid>();

        var addResponse = await Send(
            new ApiRequest(
                DELETE,
                URI($"/carts/{CartId}/products"),
                BODY(Product),
                HEADERS(IF_MATCH(0)))
        );

        await OK(addResponse);

        var getResponse = await Send(
            new ApiRequest(
                GET_UNTIL(RESPONSE_ETAG_IS(1)),
                URI($"/carts/{CartId}")
            )
        );

        var cartDetails = await getResponse.GetResultFromJson<CartDetails>();
        UnitPrice = cartDetails.Products.Single().Price;
    }

    public Task DisposeAsync() => Task.CompletedTask;
}

public class RemoveProductTests : IClassFixture<RemoveProductFixture>
{
    private readonly RemoveProductFixture API;

    public RemoveProductTests(RemoveProductFixture api) => API = api;

    // [Fact]
    [Trait("Category", "Acceptance")]
    public async Task Delete_Should_Return_OK_And_Cancel__Cart()
    {
        await API
            .Given(
                URI(
                    $"/carts/{API.CartId}/products/{API.Product.ProductId}?quantity={RemovedCount}&unitPrice={API.UnitPrice}"),
                HEADERS(IF_MATCH(1))
            )
            .When(DELETE)
            .Then(NO_CONTENT);

        await API
            .Given(
                URI($"/carts/{API.CartId}")
            )
            .When(GET_UNTIL(RESPONSE_ETAG_IS(2)))
            .Then(
                OK,
                RESPONSE_BODY<CartDetails>(details =>
                {
                    details.Id.Should().Be(API.CartId);
                    details.Status.Should().Be(CartStatus.Pending);
                    details.Products.Should().HaveCount(1);
                    var product = details.Products.Single();
                    // product.Should().Be(
                    //     CartProduct.From(
                    //         ProductItem.From(API.Product.ProductId, API.Product.Quantity - RemovedCount),
                    //         API.UnitPrice
                    //     ));
                    details.ClientId.Should().Be(API.ClientId);
                    details.Version.Should().Be(2);
                }));
    }

    private readonly int RemovedCount = 5;
}
