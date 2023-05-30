using Core.ElasticSearch.Models;
using Nest;

namespace Core.ElasticSearch;

public interface IElasticSearch
{
    // Yeni bir indeks oluşturur. (sql tablosu gibi düşünülebilir)
    Task<IElasticSearchResult> CreateNewIndexAsync(IndexModel indexModel);

    // Bir belge eklemek için kullanılır. (sql tablosuna yeni bir kayıt eklemek gibi düşünülebilir)
    Task<IElasticSearchResult> InsertAsync(InsertUpdateModel model);

    // Birden çok belgeyi eklemek için kullanılır.
    Task<IElasticSearchResult> InsertManyAsync(InsertUpdateManyModel model);

    // ElasticSearch'de bulunan indekleri bize getirir.
    Task<IReadOnlyDictionary<IndexName, IndexState>> GetIndexListAsync();
}
