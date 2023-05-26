using Application.Features.OrderItems.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.Features.OrderItems.Commands.Create;

public class CreateOrderItemCommand : IRequest<CreatedOrderItemResponse>
{
    public int UserId { get; set; }
    public string ProductId { get; set; }
    public string ProductName { get; set; }
    public decimal UnitPrice { get; set; }
    public string Quantity { get; set; }
    public string Address { get; set; }

    public class CreateOrderItemCommandHandler : IRequestHandler<CreateOrderItemCommand, CreatedOrderItemResponse>
    {
        private readonly IMapper _mapper;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly OrderItemBusinessRules _orderItemBusinessRules;

        public CreateOrderItemCommandHandler(IMapper mapper, IOrderItemRepository orderItemRepository,
                                         OrderItemBusinessRules orderItemBusinessRules)
        {
            _mapper = mapper;
            _orderItemRepository = orderItemRepository;
            _orderItemBusinessRules = orderItemBusinessRules;
        }

        public async Task<CreatedOrderItemResponse> Handle(CreateOrderItemCommand request, CancellationToken cancellationToken)
        {
            OrderItem orderItem = _mapper.Map<OrderItem>(request);

            await _orderItemRepository.AddAsync(orderItem);

            CreatedOrderItemResponse response = _mapper.Map<CreatedOrderItemResponse>(orderItem);
            return response;
        }
    }
}