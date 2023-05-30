namespace Core.ElasticSearch.Models;

public class SearchParametersModel
{
    public string IndexName { get; set; } // ElasticSearch'de bulunan indeks adı
    public int From { get; set; } = 0;
    public int Size { get; set; } = 10;

    public SearchParametersModel(string indexName)
    {
        IndexName = indexName;
    }
}
