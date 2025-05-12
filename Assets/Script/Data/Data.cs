using System;
using System.Collections.Generic;
using EnhancedScrollerDemos.GridSimulation;

[Serializable]
public class DataItem
{
    public string image_id;
    public int record_id;
    public int video_id;
    public string local_date;
    public string local_time;
    public string location_displayed;
    public string ocr;
    public string object_tags;
    public string img_link;
    public float score;
    public List<Neighbor> neighbors;
}
