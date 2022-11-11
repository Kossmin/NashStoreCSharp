using AutoMapper;
using DAO.Interfaces;
using DTO.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NashPhaseOne.API.Controllers;
using NashPhaseOne.BusinessObjects.Models;
using NashPhaseOne.DAO.Interfaces;
using NashPhaseOne.DTO.Models.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NashPhaseOne.Test
{
    public class OrdersControllerAPI_Test
    {
        IQueryable<Order> DUMMY_ORDERS_DATA = new List<Order>
        {
            new Order{Id = 1, OrderDate = DateTime.UtcNow, Status = OrderStatus.Done, OrderDetails = new List<OrderDetail>{ new OrderDetail { Price = 12, Quantity = 13, Product = new Product { Name = "Temp"} } } },
            new Order{Id = 2, OrderDate = DateTime.UtcNow, Status = OrderStatus.Canceled, OrderDetails = new List<OrderDetail>{ new OrderDetail { Price = 12, Quantity = 13} } },
            new Order{Id = 3, OrderDate = DateTime.UtcNow, Status = OrderStatus.Delivering , OrderDetails = new List < OrderDetail >{ new OrderDetail { Price = 12, Quantity = 13} }},
            new Order{Id = 4, OrderDate = DateTime.UtcNow, Status = OrderStatus.Delivered , OrderDetails = new List < OrderDetail >{ new OrderDetail { Price = 12, Quantity = 13} }},
        }.AsQueryable();

        IQueryable<AdminGetOrderDTO> DUMMY_ORDERS_DTO_DATA = new List<AdminGetOrderDTO>
        {
            new AdminGetOrderDTO{Id = 1, OrderDate = DateTime.UtcNow, Status = OrderStatus.Done.ToString() },
            new AdminGetOrderDTO{Id = 2, OrderDate = DateTime.UtcNow, Status = OrderStatus.Canceled.ToString() },
            new AdminGetOrderDTO{Id = 3, OrderDate = DateTime.UtcNow, Status = OrderStatus.Delivering.ToString() },
            new AdminGetOrderDTO{Id = 4, OrderDate = DateTime.UtcNow, Status = OrderStatus.Delivered.ToString() },
        }.AsQueryable();

        IQueryable<ListOrderDetailsDTO> DUMMY_ORDERS_DETAILS_DTO_DATA = new List<ListOrderDetailsDTO>
        {
            new ListOrderDetailsDTO{Id = 1, OrderDate = DateTime.UtcNow, OrderDetails = new List<OrderDetailDTO>{ new OrderDetailDTO { Price = 12, Quantity = 13} } },
            new ListOrderDetailsDTO{Id = 2, OrderDate = DateTime.UtcNow, OrderDetails = new List<OrderDetailDTO>{ new OrderDetailDTO { Price = 12, Quantity = 13} } },
            new ListOrderDetailsDTO{Id = 3, OrderDate = DateTime.UtcNow, OrderDetails = new List<OrderDetailDTO>{ new OrderDetailDTO { Price = 12, Quantity = 13} } },
            new ListOrderDetailsDTO{Id = 4, OrderDate = DateTime.UtcNow, OrderDetails = new List<OrderDetailDTO>{ new OrderDetailDTO { Price = 12, Quantity = 13} } },
        }.AsQueryable();

        private Mock<IOrderRepository> _orderRepository;
        private Mock<IUnitOfWork> _unitOfWork;
        private Mock<IMapper> _mapper;
        private Mock<IProductRepository> _productRepository;
        private OrdersController _controller;

        public OrdersControllerAPI_Test()
        {
            _orderRepository = new Mock<IOrderRepository>();
            _unitOfWork = new Mock<IUnitOfWork>();
            _mapper = new Mock<IMapper>();
            _productRepository = new Mock<IProductRepository>();
            _controller = new OrdersController(_productRepository.Object, _mapper.Object, _unitOfWork.Object, _orderRepository.Object);
        }

        [Fact]
        public async void GetAll_ValidCall_PagingNull()
        {
            _orderRepository.Setup(x => x.PagingAsync(It.IsAny<IQueryable<Order>>(), 1, 4)).ReturnsAsync((ViewListDTO<Order>)null);

            var result = await _controller.GetAll(1);

            Assert.True(result.Value.MaxPage == 0);
        }

        [Fact]
        public async void GetAll_ValidCall_PagingNotNull()
        {
            var outputPaging = new ViewListDTO<Order> { MaxPage = 1, PageIndex = 1, ModelDatas = DUMMY_ORDERS_DATA.ToList() };
            _orderRepository.Setup(x => x.PagingAsync(It.IsAny<IQueryable<Order>>(), 1, 4)).ReturnsAsync(outputPaging);
            _mapper.Setup(x => x.Map<List<AdminGetOrderDTO>>(outputPaging.ModelDatas)).Returns(DUMMY_ORDERS_DTO_DATA.ToList());

            var result = await _controller.GetAll(1);

            Assert.True(result.Value.MaxPage != 0);
        }

        [Fact]
        public async void GetByStatus_ValidCall_PagingNull()
        {
            _orderRepository.Setup(x => x.PagingAsync(It.IsAny<IQueryable<Order>>(), 1, 4)).ReturnsAsync((ViewListDTO<Order>)null);

            var result = await _controller.GetByStatus(1,1);

            Assert.True(result.Value.MaxPage == 0);
        }

        [Fact]
        public async void GetByStatus_ValidCall_PagingNotNull()
        {
            var outputPaging = new ViewListDTO<Order> { MaxPage = 1, PageIndex = 1, ModelDatas = DUMMY_ORDERS_DATA.ToList() };
            _orderRepository.Setup(x => x.PagingAsync(It.IsAny<IQueryable<Order>>(), 1, 4)).ReturnsAsync(outputPaging);
            _mapper.Setup(x => x.Map<List<AdminGetOrderDTO>>(outputPaging.ModelDatas)).Returns(DUMMY_ORDERS_DTO_DATA.ToList());

            var result = await _controller.GetByStatus(1,1);

            Assert.True(result.Value.MaxPage != 0);
        }

        [Fact]
        public async void GetOrderStatistics_ValidCall()
        {
            _orderRepository.Setup(x => x.GetMany(It.IsAny<Expression<Func<Order, bool>>>())).Returns(DUMMY_ORDERS_DATA);

            var result = await _controller.GetOrderStatistics();

            Assert.NotNull(result);
        }

        [Fact]
        public async void GetOrderStatistics_ValidCall_ReturnNull()
        {
            _orderRepository.Setup(x => x.GetMany(It.IsAny<Expression<Func<Order, bool>>>())).Returns((IQueryable<Order>)null);

            var result = await _controller.GetOrderStatistics();

            Assert.Null(result);
        }

        [Fact]
        public async void GetMonthlyIncomeStatistics_ValidCall_ReturnNull()
        {
            _orderRepository.Setup(x => x.GetMany(It.IsAny<Expression<Func<Order, bool>>>())).Returns((IQueryable<Order>)null);

            var result = await _controller.GetMonthlyIncomeStatistics();

            Assert.Null(result);
        }

        [Fact]
        public async void GetMonthlyIncomeStatistics_ValidCall()
        {
            _orderRepository.Setup(x => x.GetMany(It.IsAny<Expression<Func<Order, bool>>>())).Returns(DUMMY_ORDERS_DATA);

            var result = await _controller.GetMonthlyIncomeStatistics();

            Assert.NotNull(result);
        }

        [Fact]
        public async void GetCart_ValidCall()
        {
            _orderRepository.Setup(x => x.GetByAsync(It.IsAny<Expression<Func<Order, bool>>>())).ReturnsAsync(DUMMY_ORDERS_DATA.First());

            _mapper.Setup(x => x.Map<ListOrderDetailsDTO>(DUMMY_ORDERS_DATA.First())).Returns(DUMMY_ORDERS_DETAILS_DTO_DATA.First());

            var result = await _controller.GetCart(null);

            Assert.True(result.OrderDetails.Count() != 0);
        }

        [Fact]
        public async void GetCart_ValidCall_ReturnNull()
        {
            _orderRepository.Setup(x => x.GetByAsync(It.IsAny<Expression<Func<Order, bool>>>())).ReturnsAsync((Order)null);

            //_mapper.Setup(x => x.Map<ListOrderDetailsDTO>(DUMMY_ORDERS_DATA.First())).Returns(DUMMY_ORDERS_DETAILS_DTO_DATA.First());

            var result = await _controller.GetCart(null);

            Assert.True(result.OrderDetails.Count() == 0);
        }

        [Fact]
        public async void Confirm_InvalidCall_ReturnNull()
        {
            _orderRepository.Setup(x => x.GetByAsync(It.IsAny<Expression<Func<Order, bool>>>())).ReturnsAsync((Order)null);

            var result = await _controller.Confirm(null);

            Assert.Equal(new BadRequestObjectResult("Can't find").GetType(), result.GetType());
        }

        [Fact]
        public async void Confirm_ValidCall()
        {
            _orderRepository.Setup(x => x.GetByAsync(It.IsAny<Expression<Func<Order, bool>>>())).ReturnsAsync(DUMMY_ORDERS_DATA.First());

            var result = await _controller.Confirm(null);

            Assert.Equal(new OkResult().GetType(), result.GetType());
        }

        [Fact]
        public async void GetCanceledOrder_ValidCall()
        {
            _orderRepository.Setup(x=>x.GetMany(It.IsAny<Expression<Func<Order, bool>>>())).Returns(DUMMY_ORDERS_DATA);

            _mapper.Setup(x => x.Map<List<ListOrderDetailsDTO>>(DUMMY_ORDERS_DATA)).Returns(DUMMY_ORDERS_DETAILS_DTO_DATA.ToList());

            var result = await _controller.GetCanceledOrder(null);

            Assert.True(result.Count() != 0);
        }

        [Fact]
        public async void GetCanceledOrder_ValidCall_ReturnNull()
        {
            _orderRepository.Setup(x => x.GetMany(It.IsAny<Expression<Func<Order, bool>>>())).Returns((IQueryable<Order>)null);

            //_mapper.Setup(x => x.Map<List<ListOrderDetailsDTO>>(DUMMY_ORDERS_DATA)).Returns(DUMMY_ORDERS_DETAILS_DTO_DATA.ToList());

            var result = await _controller.GetCanceledOrder(null);

            Assert.True(result.Count() == 0);
        }

        [Fact]
        public async void GetPendingOrder_ValidCall()
        {
            _orderRepository.Setup(x => x.GetMany(It.IsAny<Expression<Func<Order, bool>>>())).Returns(DUMMY_ORDERS_DATA);

            _mapper.Setup(x => x.Map<List<ListOrderDetailsDTO>>(DUMMY_ORDERS_DATA)).Returns(DUMMY_ORDERS_DETAILS_DTO_DATA.ToList());

            var result = await _controller.GetPendingOrder(null);

            Assert.True(result.Count() != 0);
        }

        [Fact]
        public async void GetPendingOrder_ValidCall_ReturnNull()
        {
            _orderRepository.Setup(x => x.GetMany(It.IsAny<Expression<Func<Order, bool>>>())).Returns((IQueryable<Order>)null);

            //_mapper.Setup(x => x.Map<List<ListOrderDetailsDTO>>(DUMMY_ORDERS_DATA)).Returns(DUMMY_ORDERS_DETAILS_DTO_DATA.ToList());

            var result = await _controller.GetPendingOrder(null);

            Assert.True(result.Count() == 0);
        }

        [Fact]
        public async void GetDoneOrder_ValidCall()
        {
            _orderRepository.Setup(x => x.GetMany(It.IsAny<Expression<Func<Order, bool>>>())).Returns(DUMMY_ORDERS_DATA);

            _mapper.Setup(x => x.Map<List<ListOrderDetailsDTO>>(DUMMY_ORDERS_DATA)).Returns(DUMMY_ORDERS_DETAILS_DTO_DATA.ToList());

            var result = await _controller.GetDoneOrder(null);

            Assert.True(result.Count() != 0);
        }

        [Fact]
        public async void GetDoneOrder_ValidCall_ReturnNull()
        {
            _orderRepository.Setup(x => x.GetMany(It.IsAny<Expression<Func<Order, bool>>>())).Returns((IQueryable<Order>)null);

            //_mapper.Setup(x => x.Map<List<ListOrderDetailsDTO>>(DUMMY_ORDERS_DATA)).Returns(DUMMY_ORDERS_DETAILS_DTO_DATA.ToList());

            var result = await _controller.GetDoneOrder(null);

            Assert.True(result.Count() == 0);
        }

        [Fact]
        public async void GetDeliveringOrder_ValidCall()
        {
            _orderRepository.Setup(x => x.GetMany(It.IsAny<Expression<Func<Order, bool>>>())).Returns(DUMMY_ORDERS_DATA);

            _mapper.Setup(x => x.Map<List<ListOrderDetailsDTO>>(DUMMY_ORDERS_DATA)).Returns(DUMMY_ORDERS_DETAILS_DTO_DATA.ToList());

            var result = await _controller.GetDeliveringOrder(null);

            Assert.True(result.Count() != 0);
        }

        [Fact]
        public async void GetDeliveringOrder_ValidCall_ReturnNull()
        {
            _orderRepository.Setup(x => x.GetMany(It.IsAny<Expression<Func<Order, bool>>>())).Returns((IQueryable<Order>)null);

            //_mapper.Setup(x => x.Map<List<ListOrderDetailsDTO>>(DUMMY_ORDERS_DATA)).Returns(DUMMY_ORDERS_DETAILS_DTO_DATA.ToList());

            var result = await _controller.GetDeliveringOrder(null);

            Assert.True(result.Count() == 0);
        }

        [Fact]
        public async void CancelOrder_ValidCall()
        {
            var product = new Product { Quantity = 1 };

            _orderRepository.Setup(x => x.GetByAsync(It.IsAny<Expression<Func<Order, bool>>>())).ReturnsAsync(DUMMY_ORDERS_DATA.First());

            _productRepository.Setup(x => x.GetByAsync(It.IsAny<Expression<Func<Product, bool>>>())).ReturnsAsync(product);

            await _controller.CancelOrder(null);
            Assert.True(product.Quantity != 1);
        }

        [Fact]
        public async void CancelOrder_InvalidCall_OrderNull()
        {
            _orderRepository.Setup(x => x.GetByAsync(It.IsAny<Expression<Func<Order, bool>>>())).ReturnsAsync((Order)null);

            var result = await _controller.CancelOrder(null);
            Assert.Equal(new BadRequestObjectResult("Empty Order").GetType(), result.GetType());
        }

        [Fact]
        public async void CancelOrder_InvalidCall_Concurrency()
        {
            var product = new Product { Quantity = 1 };

            _orderRepository.Setup(x => x.GetByAsync(It.IsAny<Expression<Func<Order, bool>>>())).ReturnsAsync(DUMMY_ORDERS_DATA.First());

            _productRepository.Setup(x => x.GetByAsync(It.IsAny<Expression<Func<Product, bool>>>())).ReturnsAsync(product);

            _productRepository.Setup(x => x.UpdateAsync(product)).Throws(new TaskCanceledException());

            var result = await _controller.CancelOrder(null);
            Assert.Equal(new BadRequestObjectResult("Concurrency").GetType(), result.GetType());
        }

        [Fact]
        public async void Checkout_ValidCall_ProductStillAvailable()
        {
            var product = new Product { Quantity = 166, IsDeleted = false };

            _orderRepository.Setup(x => x.GetByAsync(It.IsAny<Expression<Func<Order, bool>>>())).ReturnsAsync(DUMMY_ORDERS_DATA.First());

            _productRepository.Setup(x => x.GetByAsync(It.IsAny<Expression<Func<Product, bool>>>())).ReturnsAsync(product);

            var result = await _controller.Checkout(null);

            Assert.False(product.IsDeleted);
            Assert.Equal(new OkResult().GetType(), result.GetType());
        }

        [Fact]
        public async void Checkout_ValidCall_ProductChangeToUnavailable()
        {
            var product = new Product { Quantity = 13, IsDeleted = false };

            _orderRepository.Setup(x => x.GetByAsync(It.IsAny<Expression<Func<Order, bool>>>())).ReturnsAsync(DUMMY_ORDERS_DATA.First());

            _productRepository.Setup(x => x.GetByAsync(It.IsAny<Expression<Func<Product, bool>>>())).ReturnsAsync(product);

            var result = await _controller.Checkout(null);

            Assert.True(product.IsDeleted);
            Assert.Equal(new OkResult().GetType(), result.GetType());
        }

        [Fact]
        public async void Checkout_InvalidCall_OrderNull()
        {
            _orderRepository.Setup(x => x.GetByAsync(It.IsAny<Expression<Func<Order, bool>>>())).ReturnsAsync((Order)null);

            var result = await _controller.Checkout(null);

            Assert.Equal(new BadRequestObjectResult("Can't find").GetType(), result.GetType());
        }

        [Fact]
        public async void Checkout_InvalidCall_ProductNull()
        {
            _orderRepository.Setup(x => x.GetByAsync(It.IsAny<Expression<Func<Order, bool>>>())).ReturnsAsync(DUMMY_ORDERS_DATA.First());

            _productRepository.Setup(x => x.GetByAsync(It.IsAny<Expression<Func<Product, bool>>>())).ReturnsAsync((Product)null);

            var result = await _controller.Checkout(null);

            Assert.Equal(new BadRequestObjectResult("Can't find").GetType(), result.GetType());
        }

        [Fact]
        public async void Checkout_InvalidCall_QuantityIsNotEnough()
        {
            var product = new Product { Quantity = 1, IsDeleted = false };

            _orderRepository.Setup(x => x.GetByAsync(It.IsAny<Expression<Func<Order, bool>>>())).ReturnsAsync(DUMMY_ORDERS_DATA.First());

            _productRepository.Setup(x => x.GetByAsync(It.IsAny<Expression<Func<Product, bool>>>())).ReturnsAsync(product);

            var result = await _controller.Checkout(null);

            Assert.Equal(new BadRequestObjectResult("Not Enough").GetType(), result.GetType());
        }

        [Fact]
        public async void Checkout_InvalidCall_Concurrency()
        {
            var product = new Product { Quantity = 166, IsDeleted = false };

            _orderRepository.Setup(x => x.GetByAsync(It.IsAny<Expression<Func<Order, bool>>>())).ReturnsAsync(DUMMY_ORDERS_DATA.First());

            _productRepository.Setup(x => x.GetByAsync(It.IsAny<Expression<Func<Product, bool>>>())).ReturnsAsync(product);

            _productRepository.Setup(x => x.UpdateAsync(product)).Throws(new TaskCanceledException());

            var result = await _controller.Checkout(null);

            Assert.Equal(new BadRequestObjectResult("Concurrency").GetType(), result.GetType());
        }
    }
}
