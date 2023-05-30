using Core.ElasticSearch.Models;

namespace Core.ElasticSearch;

public interface IElasticSearch
{
    // Yeni bir indeks oluşturur.
    Task<IElasticSearchResult> CreateNewIndexAsync(IndexModel indexModel);

}
