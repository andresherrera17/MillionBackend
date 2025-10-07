using AutoMapper;
using Million.PropertiesApi.Business.Interfaces;
using Million.PropertiesApi.Core.Dtos;
using Million.PropertiesApi.Core.Models;
using Million.PropertiesApi.Infraestructure.Interfaces;
using Million.PropertiesApi.Infrastructure.Data;

namespace Million.PropertiesApi.Services
{
    public class PropertyService : IPropertyService
    {
        private readonly IPropertyRepository _propertyRepo;
        private readonly IOwnerRepository _ownerRepo;
        private readonly IPropertyImageRepository _imageRepo;
        private readonly IPropertyTraceRepository _traceRepo;
        private readonly IMapper _mapper;
        private readonly IBlobStorageService _blobStorage;
        private readonly MongoContext _context;

        public PropertyService(
            IPropertyRepository propertyRepo,
            IOwnerRepository ownerRepo,
            IPropertyImageRepository imageRepo,
            IPropertyTraceRepository traceRepo,
            IBlobStorageService blobStorage,
            MongoContext context,
            IMapper mapper)
        {
            _propertyRepo = propertyRepo;
            _ownerRepo = ownerRepo;
            _imageRepo = imageRepo;
            _traceRepo = traceRepo;
            _blobStorage = blobStorage;
            _context = context;
            _mapper = mapper;
        }

        public async Task<PagedResult<PropertyWithOwnerImageDto>> GetPropertiesAsync(PropertyFilter filter, CancellationToken ct = default)
        {
            var (items, total) = await _propertyRepo.GetAsync(filter, ct);

            return new PagedResult<PropertyWithOwnerImageDto> { Items = items, Total = total };
        }

        public async Task<PropertyDetailsDto> GetByIdAsync(string id, CancellationToken ct = default)
        {
            try
            {
                var property = await _propertyRepo.GetByIdAsync(id);
                if (property == null) return null;

                var owner = _ownerRepo.GetByIdAsync(property.IdOwner);
                var images = _imageRepo.GetByPropertyIdAsync(property.IdProperty);
                var traces = _traceRepo.GetByPropertyIdAsync(property.IdProperty);
                await Task.WhenAll(owner, images, traces);

                PropertyDetailsDto response = new PropertyDetailsDto();
                response.Property = _mapper.Map<PropertyDto>(property);
                response.Owner = _mapper.Map<OwnerDto>(owner.Result);
                response.Images = _mapper.Map<List<PropertyImageDto>>(images.Result);
                response.Traces = _mapper.Map<List<PropertyTraceDto>>(traces.Result);

                return response;
            }
            catch (Exception exc) {
                throw new Exception(exc.Message);
            }
            
        }

        public async Task AddAsync(PropertyDetailsImageDto propertyDetails, CancellationToken ct = default)
        {
            using var session = await _context.Owners.Database.Client.StartSessionAsync(cancellationToken: ct);
            session.StartTransaction();

            try
            {
                var ownerPhotoUrl = await _blobStorage.UploadFileAsync(propertyDetails.OwnerPhoto, ct);

                var owner = new Owner
                {
                    Name = propertyDetails.OwnerName,
                    Address = propertyDetails.OwnerAddress,
                    Birthday = propertyDetails.OwnerBirthday,
                    Photo = ownerPhotoUrl
                };

                await _context.Owners.InsertOneAsync(session, owner, cancellationToken: ct);

                var property = new Property
                {
                    Name = propertyDetails.Name,
                    Address = propertyDetails.Address,
                    Price = propertyDetails.Price,
                    CodeInternal = propertyDetails.CodeInternal,
                    Year = propertyDetails.Year,
                    IdOwner = owner.IdOwner
                };
                await _context.Properties.InsertOneAsync(session, property, cancellationToken: ct);

                var trace = new PropertyTrace
                {
                    IdProperty = property.IdProperty,
                    DateSale = DateTime.UtcNow,
                    Name = owner.Name,
                    Value = propertyDetails.Price,
                    Tax = propertyDetails.Tax
                };
                await _context.PropertyTraces.InsertOneAsync(session, trace, cancellationToken: ct);

                if (propertyDetails.Files != null && propertyDetails.Files.Count > 0)
                {
                    var urls = await _blobStorage.UploadFilesAsync(propertyDetails.Files, ct);
                    var images = urls.Select(url => new PropertyImage
                    {
                        IdProperty = property.IdProperty,
                        File = url,
                        Enabled = true
                    }).ToList();

                    await _context.PropertyImages.InsertManyAsync(session, images, cancellationToken: ct);
                }

                await session.CommitTransactionAsync(ct);
            }
            catch (Exception ex)
            {
                await session.AbortTransactionAsync(ct);
                Console.WriteLine($"❌ Creating propery error: {ex.Message}");
                throw new InvalidOperationException("property cannot be created", ex);
            }
        }
    }
}
