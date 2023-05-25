using Application.Features.BasketItems.Commands.Create;
using Application.Features.BasketItems.Commands.Delete;
using Application.Features.BasketItems.Commands.Update;
using Application.Features.BasketItems.Queries.GetById;
using Application.Features.BasketItems.Queries.GetList;
using AutoMapper;
using Core.Application.Responses;
using Domain.Entities;
using Core.Persistence.Paging;

namespace Application.Features.BasketItems.Profiles;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<BasketItem, CreateBasketItemCommand>().ReverseMap();
        CreateMap<BasketItem, CreatedBasketItemResponse>().ReverseMap();
        CreateMap<BasketItem, UpdateBasketItemCommand>().ReverseMap();
        CreateMap<BasketItem, UpdatedBasketItemResponse>().ReverseMap();
        CreateMap<BasketItem, DeleteBasketItemCommand>().ReverseMap();
        CreateMap<BasketItem, DeletedBasketItemResponse>().ReverseMap();
        CreateMap<BasketItem, GetByIdBasketItemResponse>().ReverseMap();
        CreateMap<BasketItem, GetListBasketItemListItemDto>().ReverseMap();
        CreateMap<IPaginate<BasketItem>, GetListResponse<GetListBasketItemListItemDto>>().ReverseMap();
    }
}