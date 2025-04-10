using System;
using System.Collections.Generic;

[Serializable]
public class QueryApiResponse
{
    public int status;
    public string message;
    public List<DataItem> data;
}
