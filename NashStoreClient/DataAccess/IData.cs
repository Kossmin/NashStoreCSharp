using BusinessObjects.Models;
using DTO.Models;
using Microsoft.AspNetCore.Mvc;
using Refit;

namespace NashStoreClient.DataAccess
{
    public interface IData
    {
        [Get("/Products/available")]
        Task<ViewListModel<ViewProductModel>> GetProducts([FromQuery]int pageIndex);

        [Get("/Products/{id}")]
        Task<Product> GetProductById([FromRoute] int id);

        [Post("/Products/search")]
        Task<ViewListModel<ViewProductModel>> Searching(RequestSearchProductModel model);

        [Get("/Categories")]
        Task<List<ViewCategoryModel>> GetCategories();
    }
}
