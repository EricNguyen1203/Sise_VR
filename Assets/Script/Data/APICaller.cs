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


