using Microsoft.eShopOnContainers.Services.Basket.API.Services;

namespace UnitTest.Basket.Application;

using Microsoft.eShopOnContainers.Services.Basket.API.Model;

public class BasketWebApiTest
{
    private readonly Mock<IBasketRepository> _basketRepositoryMock;
    private readonly Mock<IBasketIdentityService> _identityServiceMock;
    private readonly Mock<IBasketCheckoutService> _basketCheckoutServiceMock;
    private readonly Mock<ILogger<BasketController>> _loggerMock;

    public BasketWebApiTest()
    {
        _basketRepositoryMock = new Mock<IBasketRepository>();
        _identityServiceMock = new Mock<IBasketIdentityService>();
        _basketCheckoutServiceMock = new Mock<IBasketCheckoutService>();
        _loggerMock = new Mock<ILogger<BasketController>>();
    }

    [Fact]
    public async Task Get_customer_basket_success()
    {
        //Arrange
        var fakeCustomerId = "1";
        var fakeCustomerBasket = GetCustomerBasketFake(fakeCustomerId);
    
        _basketRepositoryMock.Setup(x => x.GetBasketAsync(It.IsAny<string>()))
            .Returns(Task.FromResult(fakeCustomerBasket));
        _identityServiceMock.Setup(x => x.GetUserIdentity()).Returns(fakeCustomerId);

        //Act
        var basketController = new BasketController(
            _loggerMock.Object,
            _basketRepositoryMock.Object,
            _identityServiceMock.Object,
            _basketCheckoutServiceMock.Object);
    
        var actionResult = await basketController.GetBasketByIdAsync(fakeCustomerId);
    
        //Assert
        Assert.Equal((actionResult.Result as OkObjectResult).StatusCode, (int)System.Net.HttpStatusCode.OK);
        Assert.Equal((((ObjectResult)actionResult.Result).Value as CustomerBasket).BuyerId, fakeCustomerId);
    }

    [Fact]
    public async Task Post_customer_basket_success()
    {
        //Arrange
        var fakeCustomerId = "1";
        var fakeCustomerBasket = GetCustomerBasketFake(fakeCustomerId);
    
        _basketRepositoryMock.Setup(x => x.UpdateBasketAsync(It.IsAny<CustomerBasket>()))
            .Returns(Task.FromResult(fakeCustomerBasket));
        _identityServiceMock.Setup(x => x.GetUserIdentity()).Returns(fakeCustomerId);
        _basketCheckoutServiceMock
            .Setup(x => x.AcceptBasketCheckout(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<BasketCheckout>()))
            .Returns(Task.FromResult(true));
    
        //Act
        var basketController = new BasketController(
            _loggerMock.Object,
            _basketRepositoryMock.Object,
            _identityServiceMock.Object,
            _basketCheckoutServiceMock.Object);
    
        var actionResult = await basketController.UpdateBasketAsync(fakeCustomerBasket);
    
        //Assert
        Assert.Equal((actionResult.Result as OkObjectResult).StatusCode, (int)System.Net.HttpStatusCode.OK);
        Assert.Equal((((ObjectResult)actionResult.Result).Value as CustomerBasket).BuyerId, fakeCustomerId);
    }
    
    [Fact]
    public async Task Doing_Checkout_Without_Basket_Should_Return_Bad_Request()
    {
        var fakeCustomerId = "2";
        _basketRepositoryMock.Setup(x => x.GetBasketAsync(It.IsAny<string>()))
            .Returns(Task.FromResult((CustomerBasket)null));
        _identityServiceMock.Setup(x => x.GetUserIdentity()).Returns(fakeCustomerId);
    
        _basketCheckoutServiceMock
            .Setup(x => x.AcceptBasketCheckout(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<BasketCheckout>()))
            .Returns(Task.FromResult(false));

        //Act
        var basketController = new BasketController(
            _loggerMock.Object,
            _basketRepositoryMock.Object,
            _identityServiceMock.Object,
            _basketCheckoutServiceMock.Object);
        
        basketController.ControllerContext = new ControllerContext()
        {
            HttpContext = new DefaultHttpContext()
            {
                User = new ClaimsPrincipal(
                    new ClaimsIdentity(new Claim[] {
                        new Claim("sub", "testuser"),
                        new Claim("unique_name", "testuser"),
                        new Claim(ClaimTypes.Name, "testuser")
                    }))
            }
        };
    
        var result = await basketController.CheckoutAsync(new BasketCheckout(), Guid.NewGuid().ToString()) as BadRequestResult;
        Assert.NotNull(result);
    }

    private CustomerBasket GetCustomerBasketFake(string fakeCustomerId)
    {
        return new CustomerBasket(fakeCustomerId)
        {
            Items = new List<BasketItem>()
            {
                new BasketItem()
            }
        };
    }
}
