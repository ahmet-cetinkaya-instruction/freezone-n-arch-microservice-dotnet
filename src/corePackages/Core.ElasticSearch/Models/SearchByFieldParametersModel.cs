namespace Core.ElasticSearch.Models;

public class SearchByFieldParametersModel : SearchParametersModel
{
    public string FieldName { get; set; } // Arama yapılacak alan/özellik adı
    public string Value { get; set; } // Arama yapılacak olan alan ait değer

    public SearchByFieldParametersModel(string indexName, string fieldName, string value)
        : base(indexName)
    {
        FieldName = fieldName;
        Value = value;
    }
}
