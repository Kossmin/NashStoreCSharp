using DAO.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NashPhaseOne.API.Controllers;
using NashPhaseOne.BusinessObjects.Models;
using NashPhaseOne.DAO.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NashPhaseOne.Test
{
    public class OrderDetailsControllerAPI_Test
    {

        IQueryable<OrderDetail> DUMMY_ORDERDETAILS_DATA = new List<OrderDetail>
        {
            new OrderDetail{ Id = 1, OrderId = 1, Price = 199, ProductId = 1, Quantity = 12},
            new OrderDetail{ Id = 2, OrderId = 1, Price = 199, ProductId = 1, Quantity = 12},
            new OrderDetail{ Id = 3, OrderId = 1, Price = 199, ProductId = 1, Quantity = 12},
        }.AsQueryable();

        IQueryable<Product> DUMMY_PRODUCTS_DATA = new List<Product>
        {
            new Product{ Id = 1, Price = 199, Quantity = 12},
            new Product{ Id = 2, Price = 199, Quantity = 12},
            new Product{ Id = 3, Price = 199, Quantity = 12},
        }.AsQueryable();

        private readonly Mock<IOrderDetailRepository> _orderDetailsRepository;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly Mock<IProductRepository> _productRepository;
        private readonly Mock<IOrderRepository> _orderRepository;
        private readonly OrderDetailsController _controller;

        public OrderDetailsControllerAPI_Test()
        {
            _orderDetailsRepository = new Mock<IOrderDetailRepository>();
            _unitOfWork = new Mock<IUnitOfWork>();
            _productRepository = new Mock<IProductRepository>();
            _orderRepository = new Mock<IOrderRepository>();
            _controller = new OrderDetailsController(_orderRepository.Object, _productRepository.Object, _unitOfWork.Object, _orderDetailsRepository.Object);
        }

        [Fact]
        public async void Update_InvalidCall_OrderNull()
        {
            _orderDetailsRepository.Setup(x => x.GetByAsync(It.IsAny<Expression<Func<OrderDetail, bool>>>())).ReturnsAsync((OrderDetail)null);

            var result = await _controller.Update(null);

            Assert.Equal(new BadRequestObjectResult("Null").GetType(), result.GetType());
        }

        [Fact]
        public async void Update_InvalidCall_NotEnoughQuantity()
        {
            _orderDetailsRepository.Setup(x => x.GetByAsync(It.IsAny<Expression<Func<OrderDetail, bool>>>())).ReturnsAsync(DUMMY_ORDERDETAILS_DATA.First());
            _productRepository.Setup(x => x.GetByAsync(It.IsAny<Expression<Func<Product, bool>>>())).ReturnsAsync(DUMMY_PRODUCTS_DATA.First());

            var result = await _controller.Update(new DTO.Models.Order.OrderDetailDTO { Id = 1, Price = 199, Quantity = 16 });
            Assert.Equal(new BadRequestObjectResult("Not Enough Quanity").GetType(), result.GetType());
        }

        [Fact]
        public async void Update_ValidCall()
        {
            _orderDetailsRepository.Setup(x => x.GetByAsync(It.IsAny<Expression<Func<OrderDetail, bool>>>())).ReturnsAsync(DUMMY_ORDERDETAILS_DATA.First());
            _productRepository.Setup(x => x.GetByAsync(It.IsAny<Expression<Func<Product, bool>>>())).ReturnsAsync(DUMMY_PRODUCTS_DATA.First());

            var result = await _controller.Update(new DTO.Models.Order.OrderDetailDTO { Id = 1, Price = 199, Quantity = 11 });
            Assert.Equal(new OkResult().GetType(), result.GetType());
        }
    }
}
