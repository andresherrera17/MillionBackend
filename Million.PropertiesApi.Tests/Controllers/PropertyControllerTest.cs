using Million.PropertiesApi.Business.Interfaces;
using Million.PropertiesApi.Controllers;
using Million.PropertiesApi.Core.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Million.PropertiesApi.Tests.Controllers
{
    

    namespace Million.PropertiesApi.Tests.Controllers
    {
        [TestFixture]
        public class PropertiesControllerTests
        {
            private Mock<IPropertyService> _serviceMock;
            private PropertiesController _controller;

            [SetUp]
            public void Setup()
            {
                _serviceMock = new Mock<IPropertyService>();
                _controller = new PropertiesController(_serviceMock.Object);
            }

            [Test]
            public async Task Get_ShouldReturnOkWithData()
            {
                // Arrange
                var fakeResult = new PagedResult<PropertyWithOwnerImageDto>
                {
                    Items = new List<PropertyWithOwnerImageDto>
                {
                    new PropertyWithOwnerImageDto
                    {
                        IdProperty = "1",
                        Name = "Modern House",
                        Address = "123 Main Street",
                        Price = 200000,
                        Year = 2022,
                        OwnerName = "John Doe",
                        FirstImage = "https://cdn.example.com/img1.jpg"
                    }
                },
                    Total = 1
                };

                _serviceMock
                    .Setup(s => s.GetPropertiesAsync(It.IsAny<PropertyFilter>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(fakeResult);

                // Act
                var result = await _controller.Get("House", "Main", null, null, 1, 10);

                // Assert
                var okResult = result as OkObjectResult;
                Assert.That(okResult, Is.Not.Null);
                Assert.That(okResult!.StatusCode, Is.EqualTo(200));

                var data = okResult.Value as PagedResult<PropertyWithOwnerImageDto>;
                Assert.That(data, Is.Not.Null);
                Assert.That(data!.Items.First().Name, Is.EqualTo("Modern House"));
                Assert.That(data.Items.First().OwnerName, Is.EqualTo("John Doe"));
            }

            [Test]
            public async Task GetById_ShouldReturnOk_WhenPropertyExists()
            {
                // Arrange
                var property = new PropertyDetailsDto
                {
                    Property = new PropertyDto { Name = "Apartment 202", Address = "45 10th Avenue" },
                    Owner = new OwnerDto { Name = "Laura Smith" }
                };

                _serviceMock
                    .Setup(s => s.GetByIdAsync("123", It.IsAny<CancellationToken>()))
                    .ReturnsAsync(property);

                // Act
                var response = await _controller.GetById("123");

                // Assert
                var okResult = response as OkObjectResult;
                Assert.That(okResult, Is.Not.Null);
                Assert.That(okResult!.StatusCode, Is.EqualTo(200));

                var data = okResult.Value as PropertyDetailsDto;
                Assert.That(data, Is.Not.Null);
                Assert.That(data!.Property.Name, Is.EqualTo("Apartment 202"));
                Assert.That(data.Owner.Name, Is.EqualTo("Laura Smith"));
            }

            [Test]
            public async Task GetById_ShouldReturnNotFound_WhenPropertyDoesNotExist()
            {
                // Arrange
                _serviceMock
                    .Setup(s => s.GetByIdAsync("999", It.IsAny<CancellationToken>()))
                    .ReturnsAsync((PropertyDetailsDto?)null);

                // Act
                var response = await _controller.GetById("999");

                // Assert
                Assert.That(response, Is.TypeOf<NotFoundResult>());
            }

            [Test]
            public async Task Add_ShouldReturnOk_WhenValidData()
            {
                // Arrange
                var dto = new PropertyDetailsImageDto
                {
                    Name = "New House",
                    Address = "45 Green Street",
                    Price = 250000,
                    CodeInternal = "PROP-001",
                    Year = 2024,
                    OwnerName = "Carlos Ruiz",
                    OwnerAddress = "10 Liberty Ave",
                    OwnerBirthday = new System.DateTime(1985, 5, 10),
                    Tax = 500,
                    Files = new List<IFormFile>()
                };

                _serviceMock
                    .Setup(s => s.AddAsync(dto, It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);

                // Act
                var result = await _controller.Add(dto);

                // Assert
                Assert.That(result, Is.TypeOf<OkResult>());
                _serviceMock.Verify(s => s.AddAsync(dto, It.IsAny<CancellationToken>()), Times.Once);
            }

            [Test]
            public async Task Add_ShouldReturnBadRequest_WhenDataIsNull()
            {
                // Act
                var result = await _controller.Add(null);

                // Assert
                var badRequest = result as BadRequestObjectResult;
                Assert.That(badRequest, Is.Not.Null);
                Assert.That(badRequest!.Value, Is.EqualTo("Invalid data."));
            }
        }
    }
}