using Application.Features.OrderItems.Commands.Create;
using Application.Features.OrderItems.Commands.Delete;
using Application.Features.OrderItems.Commands.Update;
using Application.Features.OrderItems.Queries.GetById;
using Application.Features.OrderItems.Queries.GetList;
using Core.Application.Requests;
using Core.Application.Responses;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrderItemsController : BaseController
{
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] CreateOrderItemCommand createOrderItemCommand)
    {
        CreatedOrderItemResponse response = await Mediator.Send(createOrderItemCommand);

        return Created(uri: "", response);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateOrderItemCommand updateOrderItemCommand)
    {
        UpdatedOrderItemResponse response = await Mediator.Send(updateOrderItemCommand);

        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        DeletedOrderItemResponse response = await Mediator.Send(new DeleteOrderItemCommand { Id = id });

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        GetByIdOrderItemResponse response = await Mediator.Send(new GetByIdOrderItemQuery { Id = id });
        return Ok(response);
    }

    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] PageRequest pageRequest)
    {
        GetListOrderItemQuery getListOrderItemQuery = new() { PageRequest = pageRequest };
        GetListResponse<GetListOrderItemListItemDto> response = await Mediator.Send(getListOrderItemQuery);
        return Ok(response);
    }
}