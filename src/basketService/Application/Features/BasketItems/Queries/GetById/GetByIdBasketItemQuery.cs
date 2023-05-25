using Application.Features.BasketItems.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.Features.BasketItems.Queries.GetById;

public class GetByIdBasketItemQuery : IRequest<GetByIdBasketItemResponse>
{
    public string Id { get; set; }

    public class GetByIdBasketItemQueryHandler : IRequestHandler<GetByIdBasketItemQuery, GetByIdBasketItemResponse>
    {
        private readonly IMapper _mapper;
        private readonly IBasketItemRepository _basketItemRepository;
        private readonly BasketItemBusinessRules _basketItemBusinessRules;

        public GetByIdBasketItemQueryHandler(IMapper mapper, IBasketItemRepository basketItemRepository, BasketItemBusinessRules basketItemBusinessRules)
        {
            _mapper = mapper;
            _basketItemRepository = basketItemRepository;
            _basketItemBusinessRules = basketItemBusinessRules;
        }

        public async Task<GetByIdBasketItemResponse> Handle(GetByIdBasketItemQuery request, CancellationToken cancellationToken)
        {
            BasketItem? basketItem = await _basketItemRepository.GetAsync(predicate: bi => bi.Id == request.Id, cancellationToken: cancellationToken);
            await _basketItemBusinessRules.BasketItemShouldExistWhenSelected(basketItem);

            GetByIdBasketItemResponse response = _mapper.Map<GetByIdBasketItemResponse>(basketItem);
            return response;
        }
    }
}