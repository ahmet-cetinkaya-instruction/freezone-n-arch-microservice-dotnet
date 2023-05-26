using Application.Features.BasketItems.Events.OrderCreated;
using Application.Features.BasketItems.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Core.EventBus.Abstraction;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.BasketItems.Commands.Checkout;

public class CheckoutCommand : IRequest<CheckoutResponse>
{
    public int UserId { get; set; } //TODO: Token'dan user id alınması gerekir.
    public string Address { get; set; }

    public class CheckoutCommandHandler : IRequestHandler<CheckoutCommand, CheckoutResponse>
    {
        private readonly IMapper _mapper;
        private readonly BasketItemBusinessRules _basketItemBusinessRules;
        private readonly IBasketItemRepository _basketItemRepository;
        private readonly IEventBus _eventBus;
        private readonly ILogger<CheckoutCommandHandler> _logger;

        public CheckoutCommandHandler(
            IMapper mapper,
            BasketItemBusinessRules basketItemBusinessRules,
            IBasketItemRepository basketItemRepository,
            IEventBus eventBus, ILogger<CheckoutCommandHandler> logger)
        {
            _mapper = mapper;
            _basketItemBusinessRules = basketItemBusinessRules;
            _basketItemRepository = basketItemRepository;
            _eventBus = eventBus;
            _logger = logger;
        }

        public async Task<CheckoutResponse> Handle(CheckoutCommand request, CancellationToken cancellationToken)
        {
            List<BasketItem> basketItems = _basketItemRepository.Query().Where(bi => bi.UserId == request.UserId).ToList(); //TODO: Repository'e yazilacak.

            OrderCreatedIntegrationEvent orderCreatedIntegrationEvent = new(userId: request.UserId, address: request.Address, basketItems);
            _eventBus.Publish(orderCreatedIntegrationEvent);
            _logger.LogInformation($"OrderCreatedIntegrationEvent published for user with id {request.UserId}.");

            CheckoutResponse response = _mapper.Map<CheckoutResponse>(null);
            return response;
        }
    }
}
