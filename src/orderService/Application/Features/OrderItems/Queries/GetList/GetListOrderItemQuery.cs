using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using Core.Application.Requests;
using Core.Application.Responses;
using Core.Persistence.Paging;
using MediatR;

namespace Application.Features.OrderItems.Queries.GetList;

public class GetListOrderItemQuery : IRequest<GetListResponse<GetListOrderItemListItemDto>>
{
    public PageRequest PageRequest { get; set; }

    public class GetListOrderItemQueryHandler : IRequestHandler<GetListOrderItemQuery, GetListResponse<GetListOrderItemListItemDto>>
    {
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IMapper _mapper;

        public GetListOrderItemQueryHandler(IOrderItemRepository orderItemRepository, IMapper mapper)
        {
            _orderItemRepository = orderItemRepository;
            _mapper = mapper;
        }

        public async Task<GetListResponse<GetListOrderItemListItemDto>> Handle(GetListOrderItemQuery request, CancellationToken cancellationToken)
        {
            IPaginate<OrderItem> orderItems = await _orderItemRepository.GetListAsync(
                index: request.PageRequest.PageIndex,
                size: request.PageRequest.PageSize, 
                cancellationToken: cancellationToken
            );

            GetListResponse<GetListOrderItemListItemDto> response = _mapper.Map<GetListResponse<GetListOrderItemListItemDto>>(orderItems);
            return response;
        }
    }
}