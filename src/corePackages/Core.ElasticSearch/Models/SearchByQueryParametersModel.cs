namespace Core.ElasticSearch.Models;

public class SearchByQueryParametersModel : SearchParametersModel
{
    public string QueryName { get; set; } // Sorgu adı
    public string Query { get; set; } // Sorgu
    public string[] Fields { get; set; } // Aranacak alanlar

    public SearchByQueryParametersModel(string indexName, string queryName, string query, string[] fields) : base(indexName)
    {
        QueryName = queryName;
        Query = query;
        Fields = fields;
    }
}
