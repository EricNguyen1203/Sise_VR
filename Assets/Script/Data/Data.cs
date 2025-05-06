using System;
using System.Collections.Generic;
using EnhancedScrollerDemos.GridSimulation;

[Serializable]
public class DataItem
{
    public string image_id;
    public int record_id;
    public int video_id;
    public string img_link;
    public float score;
    public List<Neighbor> neighbors;
}
