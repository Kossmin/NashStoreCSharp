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
        Task<Product> GetProductByIdAsync([FromRoute] int id);

        [Post("/Products/search")]
        Task<ViewListDTO<ProductDTO>> SearchingAsync(RequestSearchProductDTO model);

        [Get("/Categories")]
        Task<List<CategoryDTO>> GetCategoriesAsync();

        [Post("/Ratings")]
        Task<ActionResult> CreateRatingAsync(RatingDTO model, [Header("Authorization")] string bearerToken);

        [Post("/Users/login")]
        Task<Token> LoginAsync(LoginModel input);

        [Post("/Orders/create")]
        Task CreateOrderAsync(OrderDTO order, [Header("Authorization")] string bearerToken);

        [Post("/Orders/cart")]
        Task<CartDTO> GetCartAsync(UserIdString userId);

        [Post("/Orders/checkout")]
        Task<ActionResult> CheckoutAsync(UserIdString userId);

        [Patch("/OrderDetails/update")]
        Task UpdateOrderDetailAsync(OrderDetailDTO orderDetail);

        [Delete("/OrderDetails")]
        Task DeleteOrderDetailAsync(int id);
    }
}
