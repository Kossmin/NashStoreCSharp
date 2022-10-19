using AutoMapper;
using BusinessObjects.Models;
using NashPhaseOne.DTO.Models.Order;
using NashPhaseOne.DTO.Models.Product;

namespace NashPhaseOne.API.AutoMapperProfile
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<OrderDetail, OrderDetailDTO>();
            CreateMap<Order, CartDTO>();
            CreateMap<Product, ProductDetailDTO>();
        }
    }
}
