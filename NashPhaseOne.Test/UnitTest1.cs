using AutoMapper;
using BusinessObjects.Models;
using DAO.Interfaces;
using DTO.Models;
using Microsoft.EntityFrameworkCore;
using Moq;
using NashPhaseOne.DTO.Models.Product;
using NashStoreAPI.Controllers;
using System.Linq.Expressions;

namespace NashPhaseOne.Test
{
    public class UnitTest1
    {
        IQueryable<ProductDTO> DUMMY_DATA_PRODUCTDTOS = new List<ProductDTO>
        {
            new ProductDTO{Id = 1, CategoryId = 1, Name = "ThinkPad", Description = "This laptop is greate", ImgUrls = new List<string>{ "https://cdn2.cellphones.com.vn/358x/media/catalog/product/6/_/6_130.jpg", "https://cdn2.cellphones.com.vn/358x/media/catalog/product/5/_/5_158.jpg" }, ImportedDate = DateTime.Now, IsDeleted = false, Price = 199, Quantity = 123, Version = 1 },
            new ProductDTO{Id = 2, CategoryId = 1, Name = "Asus", Description = "This laptop is greate", ImgUrls = new List<string>{ "https://cdn2.cellphones.com.vn/358x/media/catalog/product/6/_/6_130.jpg", "https://cdn2.cellphones.com.vn/358x/media/catalog/product/5/_/5_158.jpg" }, ImportedDate = DateTime.Now, IsDeleted = false, Price = 199, Quantity = 123, Version = 1 },
            new ProductDTO{Id = 3, CategoryId = 1, Name = "GigaMall", Description = "This laptop is greate", ImgUrls = new List<string>{ "https://cdn2.cellphones.com.vn/358x/media/catalog/product/6/_/6_130.jpg", "https://cdn2.cellphones.com.vn/358x/media/catalog/product/5/_/5_158.jpg" }, ImportedDate = DateTime.Now, IsDeleted = false, Price = 199, Quantity = 123, Version = 1 },
            new ProductDTO{Id = 4, CategoryId = 1, Name = "Acerius", Description = "This laptop is greate", ImgUrls = new List<string>{ "https://cdn2.cellphones.com.vn/358x/media/catalog/product/6/_/6_130.jpg", "https://cdn2.cellphones.com.vn/358x/media/catalog/product/5/_/5_158.jpg" }, ImportedDate = DateTime.Now, IsDeleted = false, Price = 199, Quantity = 123, Version = 1 },
            new ProductDTO{Id = 5, CategoryId = 1, Name = "Nitor", Description = "This laptop is greate", ImgUrls = new List<string>{ "https://cdn2.cellphones.com.vn/358x/media/catalog/product/6/_/6_130.jpg", "https://cdn2.cellphones.com.vn/358x/media/catalog/product/5/_/5_158.jpg" }, ImportedDate = DateTime.Now, IsDeleted = false, Price = 199, Quantity = 123, Version = 1 },
        }.AsQueryable();

        IQueryable<Product> DUMMY_DATA_PRODUCTS = new List<Product>
        {
            new Product{Id = 1, CategoryId = 1, Name = "ThinkPad", Description = "This laptop is greate", ImgUrls = new List<string>{ "https://cdn2.cellphones.com.vn/358x/media/catalog/product/6/_/6_130.jpg", "https://cdn2.cellphones.com.vn/358x/media/catalog/product/5/_/5_158.jpg" }, ImportedDate = DateTime.Now, IsDeleted = false, Price = 199, Quantity = 123, Version = 1 },
            new Product{Id = 2, CategoryId = 1, Name = "Asus", Description = "This laptop is greate", ImgUrls = new List<string>{ "https://cdn2.cellphones.com.vn/358x/media/catalog/product/6/_/6_130.jpg", "https://cdn2.cellphones.com.vn/358x/media/catalog/product/5/_/5_158.jpg" }, ImportedDate = DateTime.Now, IsDeleted = false, Price = 199, Quantity = 123, Version = 1 },
            new Product{Id = 3, CategoryId = 1, Name = "GigaMall", Description = "This laptop is greate", ImgUrls = new List<string>{ "https://cdn2.cellphones.com.vn/358x/media/catalog/product/6/_/6_130.jpg", "https://cdn2.cellphones.com.vn/358x/media/catalog/product/5/_/5_158.jpg" }, ImportedDate = DateTime.Now, IsDeleted = false, Price = 199, Quantity = 123, Version = 1 },
            new Product{Id = 4, CategoryId = 1, Name = "Acerius", Description = "This laptop is greate", ImgUrls = new List<string>{ "https://cdn2.cellphones.com.vn/358x/media/catalog/product/6/_/6_130.jpg", "https://cdn2.cellphones.com.vn/358x/media/catalog/product/5/_/5_158.jpg" }, ImportedDate = DateTime.Now, IsDeleted = false, Price = 199, Quantity = 123, Version = 1 },
            new Product{Id = 5, CategoryId = 1, Name = "Nitor", Description = "This laptop is greate", ImgUrls = new List<string>{ "https://cdn2.cellphones.com.vn/358x/media/catalog/product/6/_/6_130.jpg", "https://cdn2.cellphones.com.vn/358x/media/catalog/product/5/_/5_158.jpg" }, ImportedDate = DateTime.Now, IsDeleted = false, Price = 199, Quantity = 123, Version = 1 },
        }.AsQueryable();

        Category DUMMY_DATA_CATEGORY = new Category { Id = 1, Name = "Laptop" };

        ProductsController controller;

        public UnitTest1()
        {
            foreach (var item in DUMMY_DATA_PRODUCTS)
            {
                item.Category = DUMMY_DATA_CATEGORY;
            }
        }

        //[Fact]
        //public async void Test1()
        //{
        //    var productRepository = new Mock<IProductRepository>();
        //    var unitOfWork = new Mock<IUnitOfWork>();
        //    var context = new Mock<NashStoreDbContext>();
        //    var dbSet = new Mock<DbSet<Product>>();

        //    dbSet.Object.AddRange(DUMMY_DATA);

        //    context.Object.Products = dbSet.Object;

        //    var productController = new ProductsController(productRepository.Object, unitOfWork.Object);

        //    var result = await productController.GetAllProducts(1);

        //    Assert.Contains(DUMMY_DATA.First(), context.Object.Products);
        //}

        [Fact]
        public async void GetAll_ValidCall()
        {
            var productRepo = new Mock<IProductRepository>();
            productRepo.Setup(x => x.GetAll()).Returns(DUMMY_DATA_PRODUCTS);

            var listProduct = new ViewListDTO<Product>
            {
                MaxPage = 2,
                ModelDatas = DUMMY_DATA_PRODUCTS.ToList(),
                PageIndex = 1
            };
            productRepo.Setup(x => x.PagingAsync(DUMMY_DATA_PRODUCTS, 1, 4)).ReturnsAsync(listProduct);
            var unitOfWork = new Mock<IUnitOfWork>();
            var mapper = new Mock<IMapper>();
            var convertedResult = mapper.Object.Map<ViewListDTO<ProductDTO>>(listProduct);

            var controller = new ProductsController(mapper.Object, productRepo.Object, unitOfWork.Object);

            var result = await controller.GetAllProducts(1);
            Assert.Equal(result.Value.ModelDatas, convertedResult.ModelDatas);
        }

        [Fact]
        public async void Search_ValidCall()
        {
            string searchName = "e";
            var model = new RequestSearchProductDTO { CategoryId = 0 , ProductName = searchName};

            var productRepo = new Mock<IProductRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            var mapper = new Mock<IMapper>();


            Expression<Func<Product, bool>> expression = x => x.Name.ToUpper().Contains(searchName.ToUpper());
            productRepo.Setup(x => x.GetMany(expression)).Returns(DUMMY_DATA_PRODUCTS);
            productRepo.Setup(x => x.PagingAsync(DUMMY_DATA_PRODUCTS, 1, 4))
                .ReturnsAsync(new ViewListDTO<Product> { MaxPage = 1, ModelDatas = DUMMY_DATA_PRODUCTS.ToList(), PageIndex = 1 });

            controller = new ProductsController(mapper.Object, productRepo.Object, unitOfWork.Object);

            var result = await controller.GetProductByName(model);

            var expected = DUMMY_DATA_PRODUCTDTOS.Where(x => x.Name.ToLower().Contains(searchName));

            Assert.Equal(expected, result.Value.ModelDatas);
        }


    }
}