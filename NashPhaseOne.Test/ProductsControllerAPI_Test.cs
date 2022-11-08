using AutoMapper;
using NashPhaseOne.BusinessObjects.Models;
using DAO.Interfaces;
using DTO.Models;
using Microsoft.EntityFrameworkCore;
using Moq;
using NashPhaseOne.API.BlobHelper;
using NashPhaseOne.DTO.Models.Product;
using NashStoreAPI.Controllers;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using NashPhaseOne.DTO.Models;

namespace NashPhaseOne.Test
{
    public class ProductsControllerAPI_Test
    {
        IQueryable<ProductDTO> DUMMY_DATA_PRODUCTDTOS = new List<ProductDTO>
        {
            new ProductDTO{Id = 1, CategoryId = 1, Name = "ThinkPad", Description = "This laptop is greate", ImgUrls = new List<string>{ "https://cdn2.cellphones.com.vn/358x/media/catalog/product/6/_/6_130.jpg", "https://cdn2.cellphones.com.vn/358x/media/catalog/product/5/_/5_158.jpg" }, ImportedDate = DateTime.Now, IsDeleted = false, Price = 199, Quantity = 123, Version = 1 },
            new ProductDTO{Id = 2, CategoryId = 1, Name = "Asus", Description = "This laptop is greate", ImgUrls = new List<string>{ "https://cdn2.cellphones.com.vn/358x/media/catalog/product/6/_/6_130.jpg", "https://cdn2.cellphones.com.vn/358x/media/catalog/product/5/_/5_158.jpg" }, ImportedDate = DateTime.Now, IsDeleted = false, Price = 199, Quantity = 123, Version = 1 },
            new ProductDTO{Id = 3, CategoryId = 1, Name = "GigaMall", Description = "This laptop is greate", ImgUrls = new List<string>{ "https://cdn2.cellphones.com.vn/358x/media/catalog/product/6/_/6_130.jpg", "https://cdn2.cellphones.com.vn/358x/media/catalog/product/5/_/5_158.jpg" }, ImportedDate = DateTime.Now, IsDeleted = false, Price = 199, Quantity = 123, Version = 1 },
            new ProductDTO{Id = 4, CategoryId = 1, Name = "Acerius", Description = "This laptop is greate", ImgUrls = new List<string>{ "https://cdn2.cellphones.com.vn/358x/media/catalog/product/6/_/6_130.jpg", "https://cdn2.cellphones.com.vn/358x/media/catalog/product/5/_/5_158.jpg" }, ImportedDate = DateTime.Now, IsDeleted = true, Price = 199, Quantity = 123, Version = 1 },
            new ProductDTO{Id = 5, CategoryId = 1, Name = "Nitor", Description = "This laptop is greate", ImgUrls = new List<string>{ "https://cdn2.cellphones.com.vn/358x/media/catalog/product/6/_/6_130.jpg", "https://cdn2.cellphones.com.vn/358x/media/catalog/product/5/_/5_158.jpg" }, ImportedDate = DateTime.Now, IsDeleted = true, Price = 199, Quantity = 123, Version = 1 },
        }.AsQueryable();

        IQueryable<Product> DUMMY_DATA_PRODUCTS = new List<Product>
        {
            new Product{Id = 1, CategoryId = 1, Name = "ThinkPad", Description = "This laptop is greate", ImgUrls = new List<string>{ "https://cdn2.cellphones.com.vn/358x/media/catalog/product/6/_/6_130.jpg", "https://cdn2.cellphones.com.vn/358x/media/catalog/product/5/_/5_158.jpg" }, ImportedDate = DateTime.Now, IsDeleted = false, Price = 199, Quantity = 123, Version = 1 },
            new Product{Id = 2, CategoryId = 1, Name = "Asus", Description = "This laptop is greate", ImgUrls = new List<string>{ "https://cdn2.cellphones.com.vn/358x/media/catalog/product/6/_/6_130.jpg", "https://cdn2.cellphones.com.vn/358x/media/catalog/product/5/_/5_158.jpg" }, ImportedDate = DateTime.Now, IsDeleted = false, Price = 199, Quantity = 123, Version = 1 },
            new Product{Id = 3, CategoryId = 1, Name = "GigaMall", Description = "This laptop is greate", ImgUrls = new List<string>{ "https://cdn2.cellphones.com.vn/358x/media/catalog/product/6/_/6_130.jpg", "https://cdn2.cellphones.com.vn/358x/media/catalog/product/5/_/5_158.jpg" }, ImportedDate = DateTime.Now, IsDeleted = false, Price = 199, Quantity = 123, Version = 1 },
            new Product{Id = 4, CategoryId = 1, Name = "Acerius", Description = "This laptop is greate", ImgUrls = new List<string>{ "https://cdn2.cellphones.com.vn/358x/media/catalog/product/6/_/6_130.jpg", "https://cdn2.cellphones.com.vn/358x/media/catalog/product/5/_/5_158.jpg" }, ImportedDate = DateTime.Now, IsDeleted = true, Price = 199, Quantity = 123, Version = 1 },
            new Product{Id = 5, CategoryId = 1, Name = "Nitor", Description = "This laptop is greate", ImgUrls = new List<string>{ "https://cdn2.cellphones.com.vn/358x/media/catalog/product/6/_/6_130.jpg", "https://cdn2.cellphones.com.vn/358x/media/catalog/product/5/_/5_158.jpg" }, ImportedDate = DateTime.Now, IsDeleted = true, Price = 199, Quantity = 123, Version = 1 },
        }.AsQueryable();

        IQueryable<AdminAddProductDTO> DUMMY_DATA_ADMIN_ADD_PRODUCTS = new List<AdminAddProductDTO>
        {
            new AdminAddProductDTO{ CategoryId = 1,Imgs= new List<IFormFile>(), Name = "ThinkPad", Description = "This laptop is greate", ImportedDate = DateTime.Now, IsDeleted = false, Price = 199, Quantity = 123 },
            new AdminAddProductDTO{ CategoryId = 1,Imgs= new List<IFormFile>(), Description = "This laptop is greate", ImportedDate = DateTime.Now, IsDeleted = false, Price = 199, Quantity = 123 },
            new AdminAddProductDTO{ CategoryId = 1,Imgs= new List<IFormFile>(), Name = "GigaMall", Description = "This laptop is greate", ImportedDate = DateTime.Now, IsDeleted = false, Price = 199, Quantity = 123 },
            new AdminAddProductDTO{ CategoryId = 1,Imgs= new List<IFormFile>(), Name = "Acerius", Description = "This laptop is greate",  ImportedDate = DateTime.Now, IsDeleted = true, Price = 199, Quantity = 123 },
            new AdminAddProductDTO{ CategoryId = 1,Imgs= new List<IFormFile>(), Name = "Nitor", Description = "This laptop is greate", ImportedDate = DateTime.Now, IsDeleted = true, Price = 199, Quantity = 123},
        }.AsQueryable();

        IQueryable<AdminUpdateProductDTO> DUMMY_DATA_ADMIN_UPDATE_PRODUCTS = new List<AdminUpdateProductDTO>
        {
            new AdminUpdateProductDTO{Id = 1, Version = 1, CategoryId = 1,Imgs= new List<IFormFile>(), Name = "ThinkPad", Description = "This laptop is greate", ImportedDate = DateTime.Now, IsDeleted = false, Price = 199, Quantity = 123 },
            new AdminUpdateProductDTO{Id = 2, Version = 1, CategoryId = 1,Imgs= new List<IFormFile>(), Description = "This laptop is greate", ImportedDate = DateTime.Now, IsDeleted = false, Price = 199, Quantity = 123 },
            new AdminUpdateProductDTO{Id = 3, Version = 1, CategoryId = 1,Imgs= new List<IFormFile>(), Name = "GigaMall", Description = "This laptop is greate", ImportedDate = DateTime.Now, IsDeleted = false, Price = 199, Quantity = 123 },
            new AdminUpdateProductDTO{Id = 4, Version = 1, CategoryId = 1,Imgs= new List<IFormFile>(), Name = "Acerius", Description = "This laptop is greate",  ImportedDate = DateTime.Now, IsDeleted = true, Price = 199, Quantity = 123 },
            new AdminUpdateProductDTO{Id = 5, Version = 1, CategoryId = 1,Imgs= new List<IFormFile>(), Name = "Nitor", Description = "This laptop is greate", ImportedDate = DateTime.Now, IsDeleted = true, Price = 199, Quantity = 123},
        }.AsQueryable();

        Category DUMMY_DATA_CATEGORY = new Category { Id = 1, Name = "Laptop" };

        private readonly ProductsController _controller;
        private readonly Mock<IProductRepository> _productRepository;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly Mock<IBlobService> _blobContainer;
        private readonly Mock<IMapper> _mapper;

        public ProductsControllerAPI_Test()
        {
            foreach (var item in DUMMY_DATA_PRODUCTS)
            {
                item.Category = DUMMY_DATA_CATEGORY;
            }
            _productRepository = new Mock<IProductRepository>();
            _unitOfWork = new Mock<IUnitOfWork>();
            _blobContainer = new Mock<IBlobService>();
            _mapper = new Mock<IMapper>();
            _controller = new ProductsController(_mapper.Object, _productRepository.Object, _unitOfWork.Object, _blobContainer.Object);
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
        public async void GetAllProducts_ValidCall()
        {
            var pagingOutput = new ViewListDTO<Product> { MaxPage = 2, PageIndex = 1, ModelDatas = DUMMY_DATA_PRODUCTS.ToList() };
            _productRepository.Setup(x => x.GetAll()).Returns(DUMMY_DATA_PRODUCTS);
            _productRepository.Setup(x => x.PagingAsync(It.IsAny<IQueryable<Product>>(), 1, 4)).ReturnsAsync(pagingOutput);
            _mapper.Setup(x => x.Map<List<ProductDTO>>(pagingOutput.ModelDatas));

            var result = await _controller.GetAllProductsAsync(1);

            Assert.Equal(new OkObjectResult(pagingOutput).GetType(), result.Result.GetType());
        }

        //[Fact]
        //public async void GetAllProductsCustomer_ValidCall()
        //{
        //    var productRepo = new Mock<IProductRepository>();
        //    productRepo.Setup(x => x.GetAll()).Returns(DUMMY_DATA_PRODUCTS);

        //    var listProduct = new ViewListDTO<Product>
        //    {
        //        MaxPage = 2,
        //        ModelDatas = DUMMY_DATA_PRODUCTS.ToList(),
        //        PageIndex = 1
        //    };
        //    productRepo.Setup(x => x.PagingAsync(DUMMY_DATA_PRODUCTS, 1, 4)).ReturnsAsync(listProduct);
        //    var unitOfWork = new Mock<IUnitOfWork>();
        //    var mapper = new Mock<IMapper>();
        //    var blobContainer = new Mock<IBlobService>();
        //    var convertedResult = mapper.Object.Map<ViewListDTO<ProductDTO>>(listProduct);

        //    var controller = new ProductsController(mapper.Object, productRepo.Object, unitOfWork.Object, blobContainer.Object);

        //    var result = await controller.GetAllProducts(1);
        //    Assert.Equal(result.Value.ModelDatas, convertedResult.ModelDatas);
        //}

        [Fact]
        public async void SearchProduct_ValidCall()
        {
            var pagingOutput = new ViewListDTO<Product> { MaxPage = 1, ModelDatas = DUMMY_DATA_PRODUCTS.ToList(), PageIndex = 1 };
            var model = new RequestSearchProductDTO { CategoryId = 2 , ProductName = null, PageIndex = 1};

            _productRepository.Setup(x => x.GetMany(It.IsAny<Expression<Func<Product, bool>>>())).Returns(DUMMY_DATA_PRODUCTS);
            _productRepository.Setup(x => x.PagingAsync(DUMMY_DATA_PRODUCTS, 1, 4))
                .ReturnsAsync(pagingOutput);
            _mapper.Setup(x => x.Map<List<ProductDTO>>(pagingOutput.ModelDatas)).Returns(DUMMY_DATA_PRODUCTDTOS.ToList());

            var result = await _controller.GetProductByNameAsync(model);

            Assert.Equal(new OkObjectResult(pagingOutput).GetType(), result.Result.GetType());
        }

        [Fact]
        public async void SearchProduct_InvalidCall_EmptySearchDTO()
        {
            var pagingOutput = new ViewListDTO<Product> { MaxPage = 1, ModelDatas = DUMMY_DATA_PRODUCTS.ToList(), PageIndex = 1 };
            var model = new RequestSearchProductDTO { CategoryId = 0, ProductName = null, PageIndex = 1 };

            var result = await _controller.GetProductByNameAsync(model);

            Assert.Equal(new NotFoundResult().GetType(), result.Result.GetType());
        }

        [Fact]
        public async void SearchProduct_InvalidCall_EmptyReturnList()
        {
            var pagingOutput = new ViewListDTO<Product> { MaxPage = 1, ModelDatas = DUMMY_DATA_PRODUCTS.ToList(), PageIndex = 1 };
            var model = new RequestSearchProductDTO { CategoryId = 0, ProductName = null, PageIndex = 1 };

            _productRepository.Setup(x => x.GetMany(It.IsAny<Expression<Func<Product, bool>>>())).Returns(DUMMY_DATA_PRODUCTS);
            _productRepository.Setup(x => x.PagingAsync(DUMMY_DATA_PRODUCTS, 1, 4))
                .ReturnsAsync((ViewListDTO<Product>)null);
            _mapper.Setup(x => x.Map<List<ProductDTO>>(pagingOutput.ModelDatas)).Returns(DUMMY_DATA_PRODUCTDTOS.ToList());

            var result = await _controller.GetProductByNameAsync(model);

            Assert.Equal(new NotFoundResult().GetType(), result.Result.GetType());
        }

        [Fact]
        public async void Detail_ValidCall()
        {
            var productRepo = new Mock<IProductRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            var blobContainer = new Mock<IBlobService>();
            var mapper = new Mock<IMapper>();
            int id = 1;

            mapper.Setup(x => x.Map<ProductDTO>(DUMMY_DATA_PRODUCTS.FirstOrDefault(x => x.Id == id))).Returns(DUMMY_DATA_PRODUCTDTOS.FirstOrDefault(x => x.Id == id));
            productRepo.Setup(x => x.GetByAsync(It.IsAny<Expression<Func<Product, bool>>>())).ReturnsAsync(DUMMY_DATA_PRODUCTS.FirstOrDefault(x => x.Id == id));

            var expected = DUMMY_DATA_PRODUCTDTOS.FirstOrDefault(x => x.Id == id);

            var result = await _controller.GetProductAsync(id);

            Assert.Equal(expected, result.Value);
        }

        [Fact]
        public async void Detail_InValidCall()
        {
            int id = -1;

            _mapper.Setup(x => x.Map<ProductDTO>(DUMMY_DATA_PRODUCTS.FirstOrDefault(x => x.Id == id))).Returns(DUMMY_DATA_PRODUCTDTOS.FirstOrDefault(x => x.Id == id));
            _productRepository.Setup(x => x.GetByAsync(It.IsAny<Expression<Func<Product, bool>>>())).ReturnsAsync(DUMMY_DATA_PRODUCTS.FirstOrDefault(x => x.Id == id));

            var expected = new NotFoundResult();

            var result = await _controller.GetProductAsync(id);

            Assert.Equal(expected.GetType(), result.Result.GetType());
        }

        [Fact]
        public async void Add_InValidCall()
        {
            var userModel = DUMMY_DATA_PRODUCTS.First();
            var userAdminModel = DUMMY_DATA_ADMIN_ADD_PRODUCTS.Skip(1).First();
            _mapper.Setup(x=>x.Map<Product>(userAdminModel)).Returns(userModel);

            var result = async () => await _controller.AddAsync(userAdminModel);

            var expected = new OkResult();
            Assert.ThrowsAnyAsync<Exception>(result);
        }

        [Fact]
        public async void Add_ValidCall()
        {
            var userModel = DUMMY_DATA_PRODUCTS.First();
            var userAdminModel = DUMMY_DATA_ADMIN_ADD_PRODUCTS.First();

            _mapper.Setup(x => x.Map<Product>(userAdminModel)).Returns(userModel);

            var result = await _controller.AddAsync(userAdminModel);

            var expected = new OkResult();
            Assert.Equal(expected.GetType(), result.GetType());
        }

        [Fact]
        public async void Update_ImgIsNull()
        {
            var userModel = DUMMY_DATA_PRODUCTS.First();
            var userAdminModel = DUMMY_DATA_ADMIN_UPDATE_PRODUCTS.First();

            userAdminModel.Imgs = null;

            _mapper.Setup(x => x.Map<Product>(userAdminModel)).Returns(userModel);

            await _controller.UpdateAsync(userAdminModel);

            Assert.True(userModel.ImgUrls.Count() != 0);
        }

        [Fact]
        public async void Update_ImgIsNotNull()
        {
            var userModel = DUMMY_DATA_PRODUCTS.First();
            var userAdminModel = DUMMY_DATA_ADMIN_UPDATE_PRODUCTS.First();

            using (var stream = File.OpenRead("../../../Assets/Windows 10_ (5).jpg"))
            {
                var file = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(stream.Name));
                userAdminModel.Imgs = new List<IFormFile> { file };
            }

            _mapper.Setup(x => x.Map<Product>(userAdminModel)).Returns(userModel);

            await _controller.UpdateAsync(userAdminModel);

            Assert.True(userModel.ImgUrls.Count()  == 1);
        }

        [Fact]
        public async void ToggleStatus_ValidCall()
        {
            var userModel = DUMMY_DATA_PRODUCTS.First();

            _productRepository.Setup(x => x.GetByAsync(It.IsAny<Expression<Func<Product, bool>>>())).ReturnsAsync(userModel);

            var expected = !userModel.IsDeleted;

            await _controller.ToggleProductStatusAsync(new IdString { Id = userModel.Id.ToString() });

            var result = userModel.IsDeleted;

            Assert.True(expected == result);
        }

        [Fact]
        public async void ToggleStatus_InValidCall()
        {
            var userModel = DUMMY_DATA_PRODUCTS.First();

            _productRepository.Setup(x => x.GetByAsync(It.IsAny<Expression<Func<Product, bool>>>())).ReturnsAsync((Product)null);

            var expected = new NotFoundObjectResult(null);

            var result = await _controller.ToggleProductStatusAsync(new IdString { Id = 6.ToString() });

            Assert.Equal(expected.GetType(), result.GetType());
        }
    }
}