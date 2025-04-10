using UnityEngine;

namespace EventProcessing
{
    public interface IEvent 
    {

    }

    public class InitialLoadingFinish : IEvent
    {
        
    }

    public class OnChangeParagraphEvent : IEvent
    {
        public bool IsNext { get; set; } = true;
    }
    
}