using System;
using System.Collections.Generic;

[Serializable]
public class QueryApiResponse
{
    public int status;
    public string message;
    public List<DataItem> data;
}

[Serializable]
public class ExploreSimilarImageAPiResponse
{
    public List<DataItem> response;
}

[Serializable]
public class ExploreNeighborImageAPiResponse
{
    public List<DataItem> response;
}
[Serializable]
public class FeedbackGroup
{
    public List<int> ids;
    public int limit;
}

[Serializable]
public class LikeGroup : FeedbackGroup
{
    public List<float> prior_scores;
}

[Serializable]
public class FeedbackRequestBody
{
    public string dataset;
    public LikeGroup like;
    public FeedbackGroup dislike;
    public string model;
}
