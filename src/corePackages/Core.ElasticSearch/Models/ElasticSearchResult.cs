namespace Core.ElasticSearch.Models;

public class ElasticSearchResult : IElasticSearchResult
{
    public bool Success { get; set; } // İşlem başarılı mı?
    public string Message { get; set; } = string.Empty; // İşlem sonucunda dönen mesaj.

    public ElasticSearchResult(bool success, string message) : this(success)
    {
        Message = message;
    }

    public ElasticSearchResult(bool success)
    {
        Success = success;
    }
}
