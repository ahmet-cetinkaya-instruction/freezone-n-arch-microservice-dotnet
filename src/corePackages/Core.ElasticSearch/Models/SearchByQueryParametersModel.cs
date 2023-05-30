namespace Core.ElasticSearch.Models;

public class SearchByQueryParametersModel : SearchParametersModel
{
    public string QueryName { get; set; } // Sorgu adı
    public string Query { get; set; } // Sorgu

    public SearchByQueryParametersModel(string indexName, string queryName, string query) : base(indexName)
    {
        QueryName = queryName;
        Query = query;
    }
}
