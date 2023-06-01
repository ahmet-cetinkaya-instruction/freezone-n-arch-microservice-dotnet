using Nest;

namespace Core.ElasticSearch.Models;

public class InsertUpdateModel : ElasticSearchModel
{
    public object Item { get; set; } // Eklenecek ya da güncellenecek belge.

    public InsertUpdateModel(string indexName, object item)
        : base(null, indexName)
    {
        Item = item;
    }

    public InsertUpdateModel(Id elasticId, string indexName, object item)
        : base(elasticId, indexName)
    {
        Item = item;
    }
}
