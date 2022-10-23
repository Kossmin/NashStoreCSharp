using BusinessObjects.Models;
using DTO.Models;
using DTO.Models.Authen;
using NashPhaseOne.DTO.Models.Product;
using Microsoft.AspNetCore.Mvc;
using Refit;
using DTO.Models.Category;
using NashPhaseOne.DTO.Models.Rating;
using NashPhaseOne.DTO.Models.Order;
using NashPhaseOne.DTO.Models;

namespace NashStoreClient.DataAccess
{
    public interface IData
    {
        [Get("/Products/available")]
        Task<ViewListDTO<ProductDTO>> GetProductsAsync([FromQuery]int pageIndex);

        [Get("/Products/{id}")]
        Task<ProductDTO> GetProductByIdAsync([FromRoute] int id);

        [Post("/Products/search")]
        Task<ViewListDTO<ProductDTO>> SearchingAsync(RequestSearchProductDTO model);

        [Get("/Categories")]
        Task<List<CategoryDTO>> GetCategoriesAsync();

        [Get("/Ratings")]
        Task<List<RatingDTO>> GetRatingAsync(int id);

        [Post("/Ratings")]
        Task<ActionResult> CreateRatingAsync(RatingDTO model, [Header("Authorization")] string bearerToken);

        [Post("/Users/login")]
        Task<Token> LoginAsync(LoginModel input);

        [Post("/Orders/create")]
        Task CreateOrderAsync(OrderDTO order, [Header("Authorization")] string bearerToken);

        [Put("/Orders/cancel")]
        Task CancelOrderAsync(IdString idString, [Header("Authorization")] string bearerToken);

        [Post("/Orders/cart")]
        Task<ListOrderDetailsDTO> GetCartAsync(IdString userId, [Header("Authorization")] string bearerToken);

        [Post("/Orders/canceled")]
        Task<List<ListOrderDetailsDTO>> GetCanceledOrdersAsync(IdString userIdString, [Header("Authorization")] string bearerToken);

        [Post("/Orders/delivering")]
        Task<List<ListOrderDetailsDTO>> GetDeliveringOrdersAsync(IdString userIdString, [Header("Authorization")] string bearerToken);

        [Post("/Orders/paid")]
        Task<List<ListOrderDetailsDTO>> GetPaidOrdersAsync(IdString userIdString, [Header("Authorization")] string bearerToken);

        [Post("/Orders/checkout")]
        Task<ActionResult> CheckoutAsync(IdString userId);

        [Patch("/OrderDetails/update")]
        Task UpdateOrderDetailAsync(OrderDetailDTO orderDetail);

        [Delete("/OrderDetails")]
        Task DeleteOrderDetailAsync(int id);

        [Post("/Users/register")]
        Task<Response> RegisterAsync(RegisterModel registerModel);
    }
}
