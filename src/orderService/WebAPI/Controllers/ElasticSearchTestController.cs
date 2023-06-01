using Application.Features.OrderItems.Commands.Create;
using Application.Features.OrderItems.Queries.GetList;
using Core.ElasticSearch;
using Core.ElasticSearch.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nest;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ElasticSearchTestController : ControllerBase
{
    private readonly IElasticSearch _elasticSearch;

    public ElasticSearchTestController(IElasticSearch elasticSearch)
    {
        _elasticSearch = elasticSearch;
    }

    [HttpGet]
    public async Task<IActionResult> Test()
    {
        IndexModel indexModelToCreateNewIndex = new(indexName: "orders", aliasName: "aorders") { NumberOfReplicas = 3, NumberOfShards = 3 };
        IElasticSearchResult result = await _elasticSearch.CreateNewIndexAsync(indexModelToCreateNewIndex);
        //return Ok(result);

        InsertUpdateModel insertUpdateModel =
            new(
                indexName: "orders",
                item: new CreateOrderItemCommand()
                {
                    ProductId = "1",
                    ProductName = "Laptop",
                    UnitPrice = 1000,
                    Quantity = "1",
                    UserId = 1
                }
            );
        IElasticSearchResult result2 = await _elasticSearch.InsertAsync(insertUpdateModel);
        //return Ok(result2);

        IReadOnlyDictionary<IndexName, IndexState> result3 = await _elasticSearch.GetIndexListAsync();
        //return Ok(result3);

        SearchByFieldParametersModel searchByFieldParametersModel = new(indexName: "orders", fieldName: "userId", value: "1");
        IList<ElasticSearchGetModel<GetListOrderItemListItemDto>> result4 =
            await _elasticSearch.GetSearchByFieldAsync<GetListOrderItemListItemDto>(searchByFieldParametersModel);
        return Ok(result4);
    }
}
