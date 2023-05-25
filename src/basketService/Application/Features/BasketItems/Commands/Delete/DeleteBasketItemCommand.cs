using Application.Features.BasketItems.Constants;
using Application.Features.BasketItems.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.Features.BasketItems.Commands.Delete;

public class DeleteBasketItemCommand : IRequest<DeletedBasketItemResponse>
{
    public string Id { get; set; }

    public class DeleteBasketItemCommandHandler : IRequestHandler<DeleteBasketItemCommand, DeletedBasketItemResponse>
    {
        private readonly IMapper _mapper;
        private readonly IBasketItemRepository _basketItemRepository;
        private readonly BasketItemBusinessRules _basketItemBusinessRules;

        public DeleteBasketItemCommandHandler(IMapper mapper, IBasketItemRepository basketItemRepository,
                                         BasketItemBusinessRules basketItemBusinessRules)
        {
            _mapper = mapper;
            _basketItemRepository = basketItemRepository;
            _basketItemBusinessRules = basketItemBusinessRules;
        }

        public async Task<DeletedBasketItemResponse> Handle(DeleteBasketItemCommand request, CancellationToken cancellationToken)
        {
            BasketItem? basketItem = await _basketItemRepository.GetAsync(predicate: bi => bi.Id == request.Id, cancellationToken: cancellationToken);
            await _basketItemBusinessRules.BasketItemShouldExistWhenSelected(basketItem);

            await _basketItemRepository.DeleteAsync(basketItem!);

            DeletedBasketItemResponse response = _mapper.Map<DeletedBasketItemResponse>(basketItem);
            return response;
        }
    }
}