namespace Core.ElasticSearch.Models;

public class IndexModel
{
    public string IndexName { get; set; } // Oluşturulacak indeksin adı.
    public string AliasName { get; set; } // Oluşturulacak indeksin alias adı.
    public int NumberOfReplicas { get; set; } = 3; // Bir indekste bulunacak replika sayısı, yani bir verinin kaç farklı yerde bulunabileceğini belirtir.
    public int NumberOfShards { get; set; } = 3; // Bir indekste bulunacak shard sayısı, yani aynı anda kaç tane isteğe cevap verebileceğini belirtir.

    public IndexModel(string indexName, string aliasName)
    {
        IndexName = indexName;
        AliasName = aliasName;
    }
}
