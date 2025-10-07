using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Million.PropertiesApi.Business.Interfaces;
using Million.PropertiesApi.Core.Dtos;
using Million.PropertiesApi.Core.Models;
using Million.PropertiesApi.Infraestructure.Interfaces;
using Million.PropertiesApi.Infrastructure.Interfaces.Data;
using Million.PropertiesApi.Services;
using MongoDB.Driver;
using Moq;

namespace Million.PropertiesApi.Tests.Services
{
    [TestFixture]
    public class PropertyServiceTests
    {
        private Mock<IPropertyRepository> _propertyRepoMock = null!;
        private Mock<IOwnerRepository> _ownerRepoMock = null!;
        private Mock<IPropertyImageRepository> _imageRepoMock = null!;
        private Mock<IPropertyTraceRepository> _traceRepoMock = null!;
        private Mock<IMapper> _mapperMock = null!;
        private Mock<IBlobStorageService> _blobStorageMock = null!;
        private Mock<IMongoContext> _mongoContextMock = null!;

        private PropertyService _service = null!;

        [SetUp]
        public void Setup()
        {
            _propertyRepoMock = new Mock<IPropertyRepository>();
            _ownerRepoMock = new Mock<IOwnerRepository>();
            _imageRepoMock = new Mock<IPropertyImageRepository>();
            _traceRepoMock = new Mock<IPropertyTraceRepository>();
            _blobStorageMock = new Mock<IBlobStorageService>();
            _mapperMock = new Mock<IMapper>();
            _mongoContextMock = new Mock<IMongoContext>();

            _service = new PropertyService(
                _propertyRepoMock.Object,
                _ownerRepoMock.Object,
                _imageRepoMock.Object,
                _traceRepoMock.Object,
                _blobStorageMock.Object,
                _mongoContextMock.Object,
                _mapperMock.Object
            );
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnMappedDto_WhenPropertyExists()
        {
            // Arrange
            var propertyId = "prop-123";
            var ownerId = "owner-456";

            var property = new Property { IdProperty = propertyId, IdOwner = ownerId, Name = "House" };
            var owner = new Owner { IdOwner = ownerId, Name = "Juan Perez" };
            var images = new List<PropertyImage> { new PropertyImage { File = "https://image.png" } };
            var traces = new List<PropertyTrace> { new PropertyTrace { Name = "Seld 1" } };

            _propertyRepoMock.Setup(r => r.GetByIdAsync(propertyId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(property);

            _ownerRepoMock.Setup(r => r.GetByIdAsync(ownerId))
                .ReturnsAsync(owner);

            _imageRepoMock.Setup(r => r.GetByPropertyIdAsync(propertyId))
                .ReturnsAsync(images);

            _traceRepoMock.Setup(r => r.GetByPropertyIdAsync(propertyId))
                .ReturnsAsync(traces);

            _mapperMock.Setup(m => m.Map<PropertyDto>(property)).Returns(new PropertyDto { Name = property.Name });
            _mapperMock.Setup(m => m.Map<OwnerDto>(owner)).Returns(new OwnerDto { Name = owner.Name });
            _mapperMock.Setup(m => m.Map<List<PropertyImageDto>>(images))
                .Returns(new List<PropertyImageDto> { new PropertyImageDto { File = "https://image.png" } });
            _mapperMock.Setup(m => m.Map<List<PropertyTraceDto>>(traces))
                .Returns(new List<PropertyTraceDto> { new PropertyTraceDto { Name = "Seld 1" } });

            // Act
            var result = await _service.GetByIdAsync(propertyId);

            // Assert
            result.Should().NotBeNull();
            result.Property.Should().NotBeNull();
            result.Property.Name.Should().Be("House");
            result.Owner.Should().NotBeNull();
            result.Owner.Name.Should().Be("Juan Perez");
            result.Images.Should().HaveCount(1);
            result.Traces.Should().HaveCount(1);
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnNull_WhenPropertyDoesNotExist()
        {
            // Arrange
            _propertyRepoMock.Setup(r => r.GetByIdAsync("no-exist", It.IsAny<CancellationToken>()))
                .ReturnsAsync((Property)null!);

            // Act
            var result = await _service.GetByIdAsync("no-exist");

            // Assert
            result.Should().BeNull();
        }

        [Test]
        public async Task AddAsync_ShouldInsertAllDocuments_WhenDataIsValid()
        {
            // Arrange
            var propertyDetails = new PropertyDetailsImageDto
            {
                OwnerName = "Juan Perez",
                OwnerAddress = "Cll 123",
                OwnerBirthday = new DateTime(1990, 5, 1),
                OwnerPhoto = new Mock<IFormFile>().Object,
                Name = "house",
                Address = "Av. Central 456",
                Price = 120000,
                Year = 2020,
                CodeInternal = "PROP001",
                Tax = 1000,
                Files = new List<IFormFile> { new Mock<IFormFile>().Object }
            };

            // Mock del session y del context
            var sessionMock = new Mock<IClientSessionHandle>();
            var owners = new Mock<IMongoCollection<Owner>>();
            var properties = new Mock<IMongoCollection<Property>>();
            var traces = new Mock<IMongoCollection<PropertyTrace>>();
            var images = new Mock<IMongoCollection<PropertyImage>>();

            var contextMock = new Mock<IMongoContext>();
            contextMock.Setup(c => c.Owners).Returns(owners.Object);
            contextMock.Setup(c => c.Properties).Returns(properties.Object);
            contextMock.Setup(c => c.PropertyTraces).Returns(traces.Object);
            contextMock.Setup(c => c.PropertyImages).Returns(images.Object);
            contextMock
                .Setup(c => c.Owners.Database.Client.StartSessionAsync(
                    It.IsAny<ClientSessionOptions>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(sessionMock.Object);

            // BlobStorage simulado
            var blobMock = new Mock<IBlobStorageService>();
            blobMock.Setup(b => b.UploadFileAsync(propertyDetails.OwnerPhoto, It.IsAny<CancellationToken>()))
                .ReturnsAsync("https://blobstorage/owner-photo.jpg");
            blobMock.Setup(b => b.UploadFilesAsync(propertyDetails.Files, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<string> { "https://blobstorage/img1.jpg" });

            var service = new PropertyService(
                Mock.Of<IPropertyRepository>(),
                Mock.Of<IOwnerRepository>(),
                Mock.Of<IPropertyImageRepository>(),
                Mock.Of<IPropertyTraceRepository>(),
                blobMock.Object,
                contextMock.Object,
                Mock.Of<IMapper>()
            );

            // Act
            await service.AddAsync(propertyDetails);

            // Assert
            blobMock.Verify(b => b.UploadFileAsync(It.IsAny<IFormFile>(), It.IsAny<CancellationToken>()), Times.Once);
            blobMock.Verify(b => b.UploadFilesAsync(It.IsAny<IEnumerable<IFormFile>>(), It.IsAny<CancellationToken>()), Times.Once);
            owners.Verify(o => o.InsertOneAsync(sessionMock.Object, It.IsAny<Owner>(), null, It.IsAny<CancellationToken>()), Times.Once);
            properties.Verify(p => p.InsertOneAsync(sessionMock.Object, It.IsAny<Property>(), null, It.IsAny<CancellationToken>()), Times.Once);
            traces.Verify(t => t.InsertOneAsync(sessionMock.Object, It.IsAny<PropertyTrace>(), null, It.IsAny<CancellationToken>()), Times.Once);
            images.Verify(i => i.InsertManyAsync(sessionMock.Object, It.IsAny<IEnumerable<PropertyImage>>(), null, It.IsAny<CancellationToken>()), Times.Once);
            sessionMock.Verify(s => s.CommitTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task AddAsync_ShouldAbortTransaction_WhenExceptionOccurs()
        {
            // Arrange
            var propertyDetails = new PropertyDetailsImageDto
            {
                OwnerName = "Juan Perez",
                OwnerAddress = "kr 123",
                OwnerBirthday = new DateTime(1990, 5, 1),
                Name = "house",
                Address = "Av. Central 456",
                Price = 120000,
                Year = 2020,
                CodeInternal = "PROP001",
                Tax = 1000
            };

            var sessionMock = new Mock<IClientSessionHandle>();
            var contextMock = new Mock<IMongoContext>();
            contextMock
                .Setup(c => c.Owners.Database.Client.StartSessionAsync(
                    It.IsAny<ClientSessionOptions>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(sessionMock.Object);

            var blobMock = new Mock<IBlobStorageService>();
            blobMock.Setup(b => b.UploadFileAsync(It.IsAny<IFormFile>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Error to updload file"));

            var service = new PropertyService(
                Mock.Of<IPropertyRepository>(),
                Mock.Of<IOwnerRepository>(),
                Mock.Of<IPropertyImageRepository>(),
                Mock.Of<IPropertyTraceRepository>(),
                blobMock.Object,
                contextMock.Object,
                Mock.Of<IMapper>()
            );

            // Act & Assert
            await FluentActions.Invoking(() => service.AddAsync(propertyDetails))
                .Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("property cannot be created*");

            sessionMock.Verify(s => s.AbortTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
