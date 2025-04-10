using System;
using System.Collections.Generic;
using EnhancedScrollerDemos.GridSimulation;

[Serializable]
public class DataItem
{
    public int? id;
    public string date;
    public string time;
    public float? new_lat;
    public float? new_lng;
    public string location_displayed;
    public string img_link;
    public float? score;
    public List<Neighbor> neighbors;
}
