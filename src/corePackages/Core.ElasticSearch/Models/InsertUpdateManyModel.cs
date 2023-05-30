using Nest;

namespace Core.ElasticSearch.Models;

public class InsertUpdateManyModel : ElasticSearchModel
{
    public object[] Items { get; set; } // Eklenecek ya da güncellenecek belgeler.

    public InsertUpdateManyModel(Id elasticId, string indexName, object[] items)
        : base(elasticId, indexName)
    {
        Items = items;
    }
}
