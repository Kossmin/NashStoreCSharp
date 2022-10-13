using BusinessObjects.Models;
using DTO.Models;
using Microsoft.AspNetCore.Mvc;
using Refit;

namespace NashStoreClient.DataAccess
{
    public interface IProductData
    {
        [Get("/Products")]
        Task<List<Product>> GetProducts([FromQuery]int pageIndex);

        [Get("/Products/{id}")]
        Task<Product> GetProductById([FromRoute] int id);

        [Post("/Products/search")]
        Task<List<Product>> Searching(RequestGetProductModel model);
    }
}
