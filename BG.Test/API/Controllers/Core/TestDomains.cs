using BG.API.Controllers.Core;
using Xunit;
using BG.Model.Core;
using Microsoft.AspNetCore.Mvc;
using FluentAssertions;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;

namespace BG.Test.API.Controllers.Core;
public class TestDomains
{
    [Fact]
    public async Task Get_AllDomains_Return200()
    {
        //* Arrange

        var mockCoreServices = new Mock<ICoreServices>();
        mockCoreServices
            .Setup(coreServices => coreServices.Domain.GetAll())
            .ReturnsAsync(new List<Domain>());

        var controller = new DomainsController(
                        mockCoreServices.Object
                        , null!,null!,null!);

        //* Act

        var actionResult = await controller.GetDomains();
        //var okResult = (OkObjectResult?)actionResult.Result;

        //* Assert

        //Assert.NotNull(okResult);
        Assert.IsType<OkObjectResult>(actionResult.Result);
        //mockCoreServices.Verify(
        //                        coreServices => coreServices.Domain.GetAll()
        //                        , Times.Once());

        //var domains = Assert.IsAssignableFrom<ActionResult<IEnumerable<Domain>>>(actionResult);
    }
    //[Fact]
    //public async Task Get_AllDomains_Return404NotFound()
    //{
    //    //* Arrange

    //    var mockCoreServices = new Mock<ICoreServices>();
    //    mockCoreServices
    //        .Setup(coreServices => coreServices.Domain.GetAll())
    //        .ReturnsAsync(default);     //* Need to make it return null

    //    var controller = new DomainsController(
    //                    mockCoreServices.Object
    //                    , null!, null!, null!);

    //    //* Act

    //    var actionResult = await controller.GetDomains();
    //    //var okResult = (OkObjectResult?)actionResult.Result;

    //    //* Assert

    //    //Assert.NotNull(okResult);
    //    Assert.IsType<NotFoundObjectResult>(actionResult.Result);
    //    //mockCoreServices.Verify(
    //    //                        coreServices => coreServices.Domain.GetAll()
    //    //                        , Times.Once());

    //    //var domains = Assert.IsAssignableFrom<ActionResult<IEnumerable<Domain>>>(actionResult);
    //}
    //[Fact]
    //public async Task Get_AllDomains_ReturnException()
    //{
    //    //* Arrange

    //    var mockCoreServices = new Mock<ICoreServices>();
    //    mockCoreServices
    //        .Setup(coreServices => coreServices.Domain.GetAll())
    //        .ReturnsAsync(new Exception()); //need to test for method throwing exception

    //    var controller = new DomainsController(
    //                    mockCoreServices.Object
    //                    , null!, null!, null!);

    //    //* Act

    //    var actionResult = await controller.GetDomains();
    //    //var okResult = (OkObjectResult?)actionResult.Result;

    //    //* Assert

    //    //Assert.NotNull(okResult);
    //    Assert.IsType<ObjectResult>(actionResult.Result);
    //    //mockCoreServices.Verify(
    //    //                        coreServices => coreServices.Domain.GetAll()
    //    //                        , Times.Once());

    //    //var domains = Assert.IsAssignableFrom<ActionResult<IEnumerable<Domain>>>(actionResult);
    //}

    [Fact]
    public async Task CreateDomain_withBadInput_ReturnBadRequest()
    {
        //* Arrange

        var controller = new DomainsController(null!, null!, null!, null!);

        //* Act

        var result = (BadRequestObjectResult)await controller.CreateDomain(null!);

        //* Assert

        result.StatusCode.Should().Be(400);
    }
}
