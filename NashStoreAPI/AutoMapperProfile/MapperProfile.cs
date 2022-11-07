using AutoMapper;
using NashPhaseOne.BusinessObjects.Models;
using DTO.Models.Category;
using NashPhaseOne.DTO.Models.Order;
using NashPhaseOne.DTO.Models.Product;
using NashPhaseOne.DTO.Models.Rating;
using DTO.Models;
using NashPhaseOne.DTO.Models.Authen;

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
            CreateMap<Category, CategoryDTO>().ReverseMap();
            CreateMap<AdminAddProductDTO, Product>();
            CreateMap<AdminUpdateProductDTO, Product>();
            CreateMap<AdminGetOrderDTO, Order>().ReverseMap()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src=> src.Status.ToString()));
            CreateMap<User, UserInfo>().ReverseMap();
        }
    }
}
