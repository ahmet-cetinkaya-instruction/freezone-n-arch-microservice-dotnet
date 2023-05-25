using Application.Features.BasketItems.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.Features.BasketItems.Commands.Update;

public class UpdateBasketItemCommand : IRequest<UpdatedBasketItemResponse>
{
    public string Id { get; set; }
    public int UserId { get; set; }
    public string ProductId { get; set; }
    public string ProductName { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }

    public class UpdateBasketItemCommandHandler : IRequestHandler<UpdateBasketItemCommand, UpdatedBasketItemResponse>
    {
        private readonly IMapper _mapper;
        private readonly IBasketItemRepository _basketItemRepository;
        private readonly BasketItemBusinessRules _basketItemBusinessRules;

        public UpdateBasketItemCommandHandler(IMapper mapper, IBasketItemRepository basketItemRepository,
                                         BasketItemBusinessRules basketItemBusinessRules)
        {
            _mapper = mapper;
            _basketItemRepository = basketItemRepository;
            _basketItemBusinessRules = basketItemBusinessRules;
        }

        public async Task<UpdatedBasketItemResponse> Handle(UpdateBasketItemCommand request, CancellationToken cancellationToken)
        {
            BasketItem? basketItem = await _basketItemRepository.GetAsync(predicate: bi => bi.Id == request.Id, cancellationToken: cancellationToken);
            await _basketItemBusinessRules.BasketItemShouldExistWhenSelected(basketItem);
            basketItem = _mapper.Map(request, basketItem);

            await _basketItemRepository.UpdateAsync(basketItem!);

            UpdatedBasketItemResponse response = _mapper.Map<UpdatedBasketItemResponse>(basketItem);
            return response;
        }
    }
}