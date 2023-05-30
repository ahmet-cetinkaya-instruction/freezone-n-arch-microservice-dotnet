namespace Core.ElasticSearch.Models;

public interface IElasticSearchResult
{
    public bool Success { get; set; } // İşlem başarılı mı?
    public string Message { get; set; } // İşlem sonucunda dönen mesaj.
}
