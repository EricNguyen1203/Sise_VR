// using System;
// using System.Collections.Generic;

// namespace EventProcessing
// {
//     public class EventAggregator : UnitySingleton<EventAggregator>
//     {
//         private readonly Dictionary<Type, List<Delegate>> _eventActionMap = new();

//         public void RaiseEvent<T>(T payload) where T : IEvent
//         {
//             if (!_eventActionMap.ContainsKey(typeof(T)))
//             {
//                 return;
//             }

//             foreach (var action in _eventActionMap[typeof(T)])
//             {
//                 action?.DynamicInvoke(payload);
//             }
//         }

//         public void AddEventListener<T>(Action<T> action) where T : IEvent
//         {
//             if (!_eventActionMap.ContainsKey(typeof(T)))
//             {
//                 _eventActionMap.Add(typeof(T), new List<Delegate>());
//             }
//             _eventActionMap[typeof(T)].Add(action);
//         }

//         public void RemoveEventListener<T>(Action<T> action) where T : IEvent
//         {
//             if (action == null) 
//                 return;

//             if (_eventActionMap.TryGetValue(typeof(T), out var actions))
//             {
//                 actions.Remove(action);

//                 if (actions.Count == 0)
//                 {
//                     _eventActionMap.Remove(typeof(T)); // Clean up empty lists
//                 }
//             }
//         }

//         public void RemoveAllEventListeners<T>() where T : IEvent
//         {
//             if (_eventActionMap.ContainsKey(typeof(T)))
//             {
//                 _eventActionMap[typeof(T)].Clear();
//             }
//         }

//         public void RemoveAll()
//         {
//             _eventActionMap.Clear();
//         }
//     }
// }
