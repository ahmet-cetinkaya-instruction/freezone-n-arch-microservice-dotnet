using Application.Features.OrderItems.Commands.Create;
using Application.Features.OrderItems.Commands.Delete;
using Application.Features.OrderItems.Commands.Update;
using Application.Features.OrderItems.Queries.GetById;
using Application.Features.OrderItems.Queries.GetList;
using AutoMapper;
using Core.Application.Responses;
using Domain.Entities;
using Core.Persistence.Paging;

namespace Application.Features.OrderItems.Profiles;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<OrderItem, CreateOrderItemCommand>().ReverseMap();
        CreateMap<OrderItem, CreatedOrderItemResponse>().ReverseMap();
        CreateMap<OrderItem, UpdateOrderItemCommand>().ReverseMap();
        CreateMap<OrderItem, UpdatedOrderItemResponse>().ReverseMap();
        CreateMap<OrderItem, DeleteOrderItemCommand>().ReverseMap();
        CreateMap<OrderItem, DeletedOrderItemResponse>().ReverseMap();
        CreateMap<OrderItem, GetByIdOrderItemResponse>().ReverseMap();
        CreateMap<OrderItem, GetListOrderItemListItemDto>().ReverseMap();
        CreateMap<IPaginate<OrderItem>, GetListResponse<GetListOrderItemListItemDto>>().ReverseMap();
    }
}