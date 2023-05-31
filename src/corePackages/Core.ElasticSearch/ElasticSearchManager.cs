using Core.ElasticSearch.Models;
using Elasticsearch.Net;
using Microsoft.Extensions.Configuration;
using Nest;
using Nest.JsonNetSerializer;
using Newtonsoft.Json;

namespace Core.ElasticSearch;

public class ElasticSearchManager : IElasticSearch
{
    private readonly ConnectionSettings _connectionSettings;

    public ElasticSearchManager(IConfiguration configuration)
    {
        const string configurationSectionName = "ElasticSearchConfig";
        ElasticSearchConfig elasticSearchConfig =
            configuration.GetSection(configurationSectionName).Get<ElasticSearchConfig>()
            ?? throw new InvalidOperationException($"\"{configurationSectionName}\" section cannot found in configuration");

        // ElasticSearch bağlantı bilgilerini toparliyoruz.
        SingleNodeConnectionPool connectionPool = new(new Uri(elasticSearchConfig.ConnectionString));
        _connectionSettings = new ConnectionSettings(
            connectionPool, // ElasticSearch bağlantı adresi
            sourceSerializer: (builtInSerializer, connectionSettings) => // ElasticSearch'e gönderilecek olan verinin nasıl serialize edileceğini belirtir.
                new JsonNetSerializer(
                    builtInSerializer,
                    connectionSettings,
                    jsonSerializerSettingsFactory: () => new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }
                )
        );
    }

    public async Task<IElasticSearchResult> CreateNewIndexAsync(IndexModel indexModel)
    {
        ElasticClient elasticClient = new(_connectionSettings);

        // Eğer index varsa hata döndürüyoruz.
        if ((await elasticClient.Indices.ExistsAsync(indexModel.IndexName)).Exists)
            return new ElasticSearchResult(success: false, message: $"Index that has {indexModel.IndexName} already exists.");

        // Index oluşturma isteğini gönderiyoruz.
        CreateIndexResponse response = await elasticClient.Indices.CreateAsync(
            indexModel.IndexName, // Oluşturulacak olan index'in adı
            selector: se =>
                se // Oluşturulacak olan index'in ayarlarını belirtiyoruz.
                .Settings(settings => settings.NumberOfReplicas(indexModel.NumberOfReplicas).NumberOfShards(indexModel.NumberOfShards))
                    .Aliases(settings => settings.Alias(indexModel.AliasName))
        );

        return new ElasticSearchResult(
            success: response.IsValid,
            message: response.IsValid ? $"Index that has {indexModel.IndexName} created successfully." : response.ServerError.Error.Reason
        );
    }

    public async Task<IElasticSearchResult> InsertAsync(InsertUpdateModel model)
    {
        ElasticClient elasticClient = new(_connectionSettings);

        // ElasticSearch'e veri gönderme/yeni indeksleme isteğini gönderiyoruz.
        IndexResponse response = await elasticClient.IndexAsync(
            model.Item, // ElasticSearch'e gönderilecek olan veri/belge
            selector: se =>
                se // ElasticSearch'e gönderilecek olan verinin hangi index'e gönderileceğini belirtiyoruz.
                .Index(model.IndexName) // Index adı
                    .Id(model.ElasticId) // ElasticSearch belge id'si
                    .Refresh(Refresh.True) // ElasticSearch'e gönderilen verinin anında görünmesini sağlar.
        );

        return new ElasticSearchResult(
            success: response.IsValid,
            message: response.IsValid ? $"Document that has {model.ElasticId} inserted successfully." : response.ServerError.Error.Reason
        );
    }

    public async Task<IElasticSearchResult> InsertManyAsync(InsertUpdateManyModel model)
    {
        ElasticClient elasticClient = new(_connectionSettings);

        // ElasticSearch'e toplu veri gönderme/yeni indeksleme isteğini gönderiyoruz.
        BulkResponse response = await elasticClient.BulkAsync(
            bulkDescriptor =>
                bulkDescriptor
                    .Index(model.IndexName) // Hangi index'e gönderileceğini belirtiyoruz.
                    .IndexMany(
                        model.Items,
                        (descriptor, item) =>
                            descriptor // ElasticSearch'e gönderilecek olan verinin hangi index'e gönderileceğini belirtiyoruz.
                                .Index(model.IndexName) // Index adı
                                .Id((item as InsertUpdateManyModel).ElasticId) // ElasticSearch belge id'si
                    )
        );

        return new ElasticSearchResult(
            success: response.IsValid,
            message: response.IsValid ? $"Documents inserted successfully." : response.ServerError.Error.Reason
        );
    }

    public async Task<IReadOnlyDictionary<IndexName, IndexState>> GetIndexListAsync()
    {
        ElasticClient elasticClient = new(_connectionSettings);

        GetIndexResponse response = await elasticClient.Indices.GetAsync(new GetIndexRequest(Indices.All));

        return response.Indices;
    }

    public async Task<IList<ElasticSearchGetModel<TDocument>>> GetAllSearchAsync<TDocument>(SearchParametersModel model)
        where TDocument : class
    {
        ElasticClient elasticClient = new(_connectionSettings);

        // ElasticSearch'te arama isteğini gönderiyoruz.
        ISearchResponse<TDocument> response = await elasticClient.SearchAsync<TDocument>(
            se =>
                se.Index(Indices.Index(model.IndexName)) // Hangi index'te arama yapılacağını belirtiyoruz.
                    .From(model.From) // Arama sonuçlarının kaçıncı sıradan başlayacağını belirtiyoruz.
                    .Size(model.Size) // Arama sonuçlarının kaç tanesinin döndürüleceğini belirtiyoruz.
        );

        // ElasticSearch'ten gelen sonuçları ElasticSearchGetModel'e çeviriyoruz.
        List<ElasticSearchGetModel<TDocument>> list = response.Hits // Search işleminin sonuçları
            .Select(hit => new ElasticSearchGetModel<TDocument>(elasticId: hit.Id, item: hit.Source))
            .ToList();
        return list;
    }

    public async Task<IList<ElasticSearchGetModel<TDocument>>> GetSearchByFieldAsync<TDocument>(SearchByFieldParametersModel model)
        where TDocument : class
    {
        ElasticClient elasticClient = new(_connectionSettings);

        // ElasticSearch'te arama isteğini gönderiyoruz.
        ISearchResponse<TDocument> response = await elasticClient.SearchAsync<TDocument>(
            se =>
                se.Index(model.IndexName) // Hangi index'te arama yapılacağını belirtiyoruz.
                    .From(model.From) // Arama sonuçlarının kaçıncı sıradan başlayacağını belirtiyoruz.
                    .Size(model.Size) // Arama sonuçlarının kaç tanesinin döndürüleceğini belirtiyoruz.
                    .Query(q => q.Term(t => t.Field(model.FieldName).Value(model.Value))) // Arama sorgusunu belirtiyoruz.
        );

        // ElasticSearch'ten gelen sonuçları ElasticSearchGetModel'e çeviriyoruz.
        List<ElasticSearchGetModel<TDocument>> list = response.Hits
            .Select(hit => new ElasticSearchGetModel<TDocument>(elasticId: hit.Id, item: hit.Source))
            .ToList();
        return list;
    }

    public async Task<IList<ElasticSearchGetModel<TDocument>>> GetSearchBySimpleQueryString<TDocument>(SearchByQueryParametersModel model)
        where TDocument : class
    {
        ElasticClient elasticClient = new(_connectionSettings);

        // ElasticSearch'te arama isteğini gönderiyoruz.
        ISearchResponse<TDocument> response = await elasticClient.SearchAsync<TDocument>(
            se =>
                se.Index(model.IndexName)
                    .From(model.From)
                    .Size(model.Size)
                    .MatchAll() // Tüm belgelerde eşleşme arıyoruz.
                    .Query(
                        q =>
                            q.SimpleQueryString(
                                d =>
                                    d.Name(model.QueryName) // Arama sorgusunu belirtiyoruz.
                                        .Fields(model.Fields) // Arama sorgusunun hangi alanlarda yapılacağını belirtiyoruz.
                                        .Query(model.Query) // Arama sorgusunu belirtiyoruz.
                                        .Analyzer("standard") // Kullanilacak analizleyiciyi belirtiyoruz.
                                        .DefaultOperator(Operator.Or) // Arama sorgusunda kullanılacak varsayilan operatörü belirtiyoruz.
                                        .Flags(SimpleQueryStringFlags.And | SimpleQueryStringFlags.Near)
                                        .Lenient() // Esnek sorgu modunu etkinleştiriyor.
                                        .AnalyzeWildcard(false) // Jokere izin verilip verilmeyeceğini belirtiyoruz.
                                        .MinimumShouldMatch("30%") // Arama sorgusunun en az kaç alanla eşleşmesi gerektiğini belirtiyoruz. Örneğin arama motorlarındaki Bunu mu demek istediğiniz şeklindeki önermeler gibi.
                                        .FuzzyPrefixLength(0) // Yakın eşleşme için kullanılacak ön ek uzunluğunu belirtiyoruz.
                                        .FuzzyMaxExpansions(50) // Yakın eşleşme için kullanılacak maksimum genişleme sayısını belirtiyoruz.
                                        .FuzzyTranspositions() // Yakın eşleşme için karakterlerin yer değiştirmesine izin verilip verilmeyeceğini belirtiyoruz.
                                        .AutoGenerateSynonymsPhraseQuery(false) // Eşanlamlı kelimelerin otomatik olarak oluşturulup oluşturulmayacağını belirtiyoruz.
                            )
                    )
        );

        // ElasticSearch'ten gelen sonuçları ElasticSearchGetModel'e çeviriyoruz.
        List<ElasticSearchGetModel<TDocument>> list = response.Hits
            .Select(hit => new ElasticSearchGetModel<TDocument>(elasticId: hit.Id, item: hit.Source))
            .ToList();
        return list;
    }

    public Task<IElasticSearchResult> UpdateByElasticIdAsync(InsertUpdateModel model)
    {
        throw new NotImplementedException();
    }

    public Task<IElasticSearchResult> DeleteByElasticIdAsync(ElasticSearchModel model)
    {
        throw new NotImplementedException();
    }
}
