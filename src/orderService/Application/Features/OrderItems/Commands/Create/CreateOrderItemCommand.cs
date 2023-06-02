using Application.Features.OrderItems.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using Hangfire;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.OrderItems.Commands.Create;

public class CreateOrderItemCommand : IRequest<CreatedOrderItemResponse>
{
    public int UserId { get; set; }
    public string ProductId { get; set; }
    public string ProductName { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public string Address { get; set; }

    public class CreateOrderItemCommandHandler : IRequestHandler<CreateOrderItemCommand, CreatedOrderItemResponse>
    {
        private readonly IMapper _mapper;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly OrderItemBusinessRules _orderItemBusinessRules;
        private IBackgroundJobClient _backgroundJobClient;
        private ILogger<CreateOrderItemCommand> _logger;

        public CreateOrderItemCommandHandler(IMapper mapper, IOrderItemRepository orderItemRepository,
                                             OrderItemBusinessRules orderItemBusinessRules,
                                             IBackgroundJobClient backgroundJobClient, ILogger<CreateOrderItemCommand> logger)
        {
            _mapper = mapper;
            _orderItemRepository = orderItemRepository;
            _orderItemBusinessRules = orderItemBusinessRules;
            _backgroundJobClient = backgroundJobClient;
            _logger = logger;
        }

        public async Task<CreatedOrderItemResponse> Handle(CreateOrderItemCommand request, CancellationToken cancellationToken)
        {
            OrderItem orderItem = _mapper.Map<OrderItem>(request);

            await _orderItemRepository.AddAsync(orderItem);
            _backgroundJobClient.Enqueue(() => _logger.LogInformation("Email has sended when order created."));

            CreatedOrderItemResponse response = _mapper.Map<CreatedOrderItemResponse>(orderItem);
            return response;
        }
    }
}