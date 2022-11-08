using AutoMapper;
using DAO.Interfaces;
using DTO.Models.Category;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NashPhaseOne.BusinessObjects.Models;
using NashStoreAPI.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NashPhaseOne.Test
{
    public class CategoriesControllerAPI_Test
    {
        private readonly Mock<ICategoryRepository> _categoryRepository;
        private readonly Mock<IProductRepository> _productRepository;
        private readonly Mock<IMapper> _mapper;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly CategoriesController _controller;

        IQueryable<Category> DUMMY_CATEGORIES_DATA = new List<Category>
        {
            new Category{Id =1 , IsDeleted= false, Name="Laptop", Products = new List<Product>() },
            new Category{Id =2 , IsDeleted= true, Name="Console", Products = new List<Product>() },
            new Category{Id =3 , IsDeleted= false, Name="PC", Products = new List<Product>() },
        }.AsQueryable();

        IQueryable<CategoryDTO> DUMMY_CATEGORIES_DTOS_DATA = new List<CategoryDTO>
        {
            new CategoryDTO{Id =1 , IsDeleted= false, Name="Laptop" },
            new CategoryDTO{Id =2 , IsDeleted= true, Name="Console" },
            new CategoryDTO{Id =3 , IsDeleted= false, Name="PC" },
        }.AsQueryable();

        public CategoriesControllerAPI_Test()
        {
            _categoryRepository = new Mock<ICategoryRepository>();
            _productRepository = new Mock<IProductRepository>();
            _mapper = new Mock<IMapper>();
            _unitOfWork = new Mock<IUnitOfWork>();
            _controller = new CategoriesController(_categoryRepository.Object, _mapper.Object, _unitOfWork.Object, _productRepository.Object);
        }


        [Fact]
        public async void GetAll_ValidCall()
        {
            var output = DUMMY_CATEGORIES_DATA.Where(x => !x.IsDeleted);

            _categoryRepository.Setup(x => x.GetAll()).Returns(output);

            _mapper.Setup(x => x.Map<List<CategoryDTO>>(output)).Returns(DUMMY_CATEGORIES_DTOS_DATA.Where(x => !x.IsDeleted).ToList());

            var result = DUMMY_CATEGORIES_DTOS_DATA.Where(x => !x.IsDeleted).ToList();

            var expected = await _controller.GetCategories();

            Assert.Equal(result.Count(), expected.Value.Count());
        }

        [Fact]
        public async void ToggleStatus_InvalidCall_WrongId()
        {
            _categoryRepository.Setup(x => x.GetByAsync(It.IsAny<Expression<Func<Category, bool>>>())).ReturnsAsync((Category)null);

            var expected = new BadRequestObjectResult("Can't find");

            var result = await _controller.ToggleStatus(null);

            Assert.Equal(expected.GetType(), result.GetType());
        }

        [Fact]
        public async void ToggleStatus_ValidCall()
        {
            _categoryRepository.Setup(x => x.GetByAsync(It.IsAny<Expression<Func<Category, bool>>>())).ReturnsAsync(DUMMY_CATEGORIES_DATA.First());

            var expected = !DUMMY_CATEGORIES_DATA.First().IsDeleted;

            await _controller.ToggleStatus(null);
            var result = DUMMY_CATEGORIES_DATA.First().IsDeleted;

            Assert.Equal(expected, result);
        }

        [Fact]
        public async void Add_ValidCall()
        {
            var input = DUMMY_CATEGORIES_DTOS_DATA.First();
            
            var result = await _controller.Add(input);
            var expected = new OkResult();
            Assert.Equal(expected.GetType(), result.GetType());
        }

        [Fact]
        public async void Add_InvalidCall_ThrowException()
        {
            var input = DUMMY_CATEGORIES_DTOS_DATA.First();
            _mapper.Setup(x => x.Map<Category>(input)).Throws(new Exception("Exception"));

            var result = await _controller.Add(input);
            var expected = new BadRequestObjectResult("Exception");

            Assert.Equal(expected.GetType(), result.GetType());
        }

        [Fact]
        public async void Update_ValidCall()
        {
            var input = DUMMY_CATEGORIES_DTOS_DATA.First();

            var result = await _controller.Update(input);
            var expected = new OkResult();
            Assert.Equal(expected.GetType(), result.GetType());
        }

        [Fact]
        public async void Update_InvalidCall_ThrowException()
        {
            var input = DUMMY_CATEGORIES_DATA.First();
            _mapper.Setup(x => x.Map<Category>(DUMMY_CATEGORIES_DTOS_DATA.First())).Returns(input);
            _categoryRepository.Setup(x => x.UpdateAsync(input)).ThrowsAsync(new Exception("Exception"));

            var result = await _controller.Update(DUMMY_CATEGORIES_DTOS_DATA.First());
            var expected = new BadRequestObjectResult("Exception");

            Assert.Equal(expected.GetType(), result.GetType());
        }
    }
}
