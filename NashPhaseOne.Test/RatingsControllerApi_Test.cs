using AutoMapper;
using DAO.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NashPhaseOne.BusinessObjects.Models;
using NashPhaseOne.DAO.Interfaces;
using NashStoreAPI.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NashPhaseOne.Test
{
    public class RatingsControllerApi_Test
    {
        IQueryable<Order> DUMMY_ORDERS_DATA = new List<Order>
        {
            new Order{Id = 1, OrderDate = DateTime.UtcNow, Status = OrderStatus.Done, OrderDetails = new List<OrderDetail>{ new OrderDetail { ProductId = 1, Price = 12, Quantity = 13, Product = new Product { Id = 1, Name = "Temp"} } } },
            new Order{Id = 2, OrderDate = DateTime.UtcNow, Status = OrderStatus.Canceled, OrderDetails = new List<OrderDetail>{ new OrderDetail { Price = 12, Quantity = 13} } },
            new Order{Id = 3, OrderDate = DateTime.UtcNow, Status = OrderStatus.Delivering , OrderDetails = new List < OrderDetail >{ new OrderDetail { Price = 12, Quantity = 13} }},
            new Order{Id = 4, OrderDate = DateTime.UtcNow, Status = OrderStatus.Delivered , OrderDetails = new List < OrderDetail >{ new OrderDetail { Price = 12, Quantity = 13} }},
        }.AsQueryable();

        private readonly Mock<IRatingRepository> _ratingRepository;
        private readonly Mock<IOrderRepository> _orderRepository;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly Mock<IMapper> _mapper;
        private readonly RatingsController _controller;

        public RatingsControllerApi_Test()
        {
            _ratingRepository = new Mock<IRatingRepository>();
            _orderRepository = new Mock<IOrderRepository>();
            _unitOfWork = new Mock<IUnitOfWork>();
            _mapper = new Mock<IMapper>();

            _controller = new RatingsController(_mapper.Object, _ratingRepository.Object, _orderRepository.Object, _unitOfWork.Object);
        }

        [Fact]
        public async void CreateRating_InvalidCall_OrderNull()
        {
            _orderRepository.Setup(x => x.GetMany(It.IsAny<Expression<Func<Order, bool>>>())).Returns((IQueryable<Order>)null);

            var result = await _controller.Create(new DTO.Models.Rating.RatingDTO { ProductId = 1 });
            Assert.Equal(new NotFoundResult().GetType(), result.GetType());
        }

        [Fact]
        public async void CreateRating_InvalidCall_UserDoesntBuyProduct()
        {
            _orderRepository.Setup(x => x.GetMany(It.IsAny<Expression<Func<Order, bool>>>())).Returns(DUMMY_ORDERS_DATA);

            var result = await _controller.Create(new DTO.Models.Rating.RatingDTO { ProductId = 3 });
            Assert.Equal(new BadRequestObjectResult("You hasnt buy this product").GetType(), result.GetType());
        }

        [Fact]
        public async void CreateRating_InvalidCall_RatingManyTimes()
        {
            var outputMany = new List<Order>();
            outputMany.Add(DUMMY_ORDERS_DATA.First());
            _orderRepository.Setup(x => x.GetMany(It.IsAny<Expression<Func<Order, bool>>>())).Returns(outputMany.AsQueryable());
            _unitOfWork.Setup(x => x.CommitAsync()).ThrowsAsync(new Exception());

            var result = await _controller.Create(new DTO.Models.Rating.RatingDTO { ProductId = 1 });
            Assert.Equal(new BadRequestObjectResult("Update many times").GetType(), result.GetType());
        }

        [Fact]
        public async void CreateRating_ValidCall()
        {
            var outputMany = new List<Order>();
            outputMany.Add(DUMMY_ORDERS_DATA.First());
            _orderRepository.Setup(x => x.GetMany(It.IsAny<Expression<Func<Order, bool>>>())).Returns(outputMany.AsQueryable());

            var result = await _controller.Create(new DTO.Models.Rating.RatingDTO { ProductId = 1 });

            Assert.Equal(new OkResult().GetType(), result.GetType());
        }
    }
}
