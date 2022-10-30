using AutoMapper;
using BusinessObjects.Models;
using DTO.Models.Category;
using NashPhaseOne.DTO.Models.Order;
using NashPhaseOne.DTO.Models.Product;
using NashPhaseOne.DTO.Models.Rating;

namespace NashPhaseOne.API.AutoMapperProfile
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<OrderDetail, OrderDetailDTO>();
            CreateMap<Order, ListOrderDetailsDTO>();
            CreateMap<Product, ProductDetailDTO>();
            CreateMap<Rating, RatingDTO>();
            CreateMap<Product, ProductDTO>()
                .ForMember(dest => dest.CategoryName, opt=> opt.MapFrom(src=>src.Category.Name))
                .ForMember(dest => dest.ImgUrls, opt => opt.MapFrom(src => src.ImgUrls));
            CreateMap<Category, CategoryDTO>();
            CreateMap<AdminProductDTO, Product>();
        }
    }
}
