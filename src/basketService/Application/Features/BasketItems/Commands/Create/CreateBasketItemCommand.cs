using Application.Features.BasketItems.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.Features.BasketItems.Commands.Create;

public class CreateBasketItemCommand : IRequest<CreatedBasketItemResponse>
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string ProductId { get; set; }
    public string ProductName { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }

    public class CreateBasketItemCommandHandler : IRequestHandler<CreateBasketItemCommand, CreatedBasketItemResponse>
    {
        private readonly IMapper _mapper;
        private readonly IBasketItemRepository _basketItemRepository;
        private readonly BasketItemBusinessRules _basketItemBusinessRules;

        public CreateBasketItemCommandHandler(IMapper mapper, IBasketItemRepository basketItemRepository,
                                         BasketItemBusinessRules basketItemBusinessRules)
        {
            _mapper = mapper;
            _basketItemRepository = basketItemRepository;
            _basketItemBusinessRules = basketItemBusinessRules;
        }

        public async Task<CreatedBasketItemResponse> Handle(CreateBasketItemCommand request, CancellationToken cancellationToken)
        {
            BasketItem basketItem = _mapper.Map<BasketItem>(request);

            await _basketItemRepository.AddAsync(basketItem);

            CreatedBasketItemResponse response = _mapper.Map<CreatedBasketItemResponse>(basketItem);
            return response;
        }
    }
}