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
            bulkDescriptor => bulkDescriptor.Index(model.IndexName) // Hangi index'e gönderileceğini belirtiyoruz.
                                            .IndexMany(model.Items) // Yapacağımız index işlemini belirtiyoruz.
        );

        return new ElasticSearchResult(
            success: response.IsValid,
            message: response.IsValid ? $"Documents inserted successfully." : response.ServerError.Error.Reason
        );
    }

    public Task<IReadOnlyDictionary<IndexName, IndexState>> GetIndexListAsync()
    {
        throw new NotImplementedException();
    }

    public Task<IList<ElasticSearchGetModel<TDocument>>> GetAllSearchAsync<TDocument>(SearchParametersModel model)
        where TDocument : class
    {
        throw new NotImplementedException();
    }

    public Task<IList<ElasticSearchGetModel<TDocument>>> GetSearchByFieldAsync<TDocument>(SearchByFieldParametersModel model)
        where TDocument : class
    {
        throw new NotImplementedException();
    }

    public Task<IList<ElasticSearchGetModel<TDocument>>> GetSearchBySimpleQueryString<TDocument>(
        SearchByQueryParametersModel parametersModel
    )
        where TDocument : class
    {
        throw new NotImplementedException();
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
