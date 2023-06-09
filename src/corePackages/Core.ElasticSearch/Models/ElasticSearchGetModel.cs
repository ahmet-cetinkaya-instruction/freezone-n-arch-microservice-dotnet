﻿using Nest;

namespace Core.ElasticSearch.Models;

public class ElasticSearchGetModel<TDocument>
{
    public Id ElasticId { get; set; }
    public TDocument Item { get; set; }

    public ElasticSearchGetModel(Id elasticId, TDocument item)
    {
        ElasticId = elasticId;
        Item = item;
    }
}
