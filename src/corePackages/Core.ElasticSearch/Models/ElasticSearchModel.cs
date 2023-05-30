using Nest;

namespace Core.ElasticSearch.Models;

public class ElasticSearchModel
{
    public Id ElasticId { get; set; } // ElasticSearch tarafindaki belge kimliği
    public string IndexName { get; set; } // ElasticSearch tarafindaki indeks adi

    public ElasticSearchModel(Id elasticId, string indexName)
    {
        ElasticId = elasticId;
        IndexName = indexName;
    }
}
