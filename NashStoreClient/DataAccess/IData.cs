using BusinessObjects.Models;
using DTO.Models;
using DTO.Models.Authen;
using NashPhaseOne.DTO.Models.Product;
using Microsoft.AspNetCore.Mvc;
using Refit;
using DTO.Models.Category;
using NashPhaseOne.DTO.Models.Rating;
using NashPhaseOne.DTO.Models.Order;

namespace NashStoreClient.DataAccess
{
    public interface IData
    {
        [Get("/Products/available")]
        Task<ViewListDTO<ProductDTO>> GetProducts([FromQuery]int pageIndex);

        [Get("/Products/{id}")]
        Task<Product> GetProductById([FromRoute] int id);

        [Post("/Products/search")]
        Task<ViewListDTO<ProductDTO>> Searching(RequestSearchProductDTO model);

        [Get("/Categories")]
        Task<List<CategoryDTO>> GetCategories();

        [Post("/Ratings")]
        Task<ActionResult> CreateRating(RatingDTO model, [Header("Authorization")] string bearerToken);

        [Post("/Users/login")]
        Task<Token> Login(LoginModel input);

        [Post("/Orders")]
        Task CreateOrder(OrderDTO order);
    }
}
