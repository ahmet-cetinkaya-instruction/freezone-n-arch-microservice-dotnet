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

    // ElasticSearch'de Get All işlemi yapar.
    Task<IList<ElasticSearchGetModel<TDocument>>> GetAllSearchAsync<TDocument>(SearchParametersModel model)
        where TDocument : class; // TDocument: class => TDocument tipi referans tip olmalıdır.

    // ElasticSearch'de belgelerde bulunan bir alana ve değerine göre arama yapar.
    Task<IList<ElasticSearchGetModel<TDocument>>> GetSearchByFieldAsync<TDocument>(SearchByFieldParametersModel model)
        where TDocument : class;

    // ElasticSearch'ın kullandığı basit bir sorgu diline göre arama yapar.
    Task<IList<ElasticSearchGetModel<TDocument>>> GetSearchBySimpleQueryString<TDocument>(SearchByQueryParametersModel parametersModel)
        where TDocument : class;

    // ElasticSearch'de bulunan bir belgeyi, ElasticId'sine göre güncelleme işlemini gerçekleştirir.
    Task<IElasticSearchResult> UpdateByElasticIdAsync(InsertUpdateModel model);

    // ElasticSearch'de bulunan bir belgeyi, ElasticId'sine göre silme işlemini gerçekleştirir.
    Task<IElasticSearchResult> DeleteByElasticIdAsync(ElasticSearchModel model);
}
