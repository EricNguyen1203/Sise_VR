using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

namespace Oculus.Interaction
{
    public class SelectorFeedback : MonoBehaviour
    {

        [SerializeField, Interface(typeof(ISelector))]
        private UnityEngine.Object _selector;

        [SerializeField]
        private ToggleDeselect toggleDeselect;

        private ISelector Selector;
        private bool _isHighlighted;
        protected bool _started = false;

        protected virtual void Awake()
        {
            Selector = _selector as ISelector;
        }

        protected virtual void Start()
        {
            this.BeginStart(ref _started);
            this.AssertField(Selector, nameof(Selector));
            this.AssertField(toggleDeselect, nameof(toggleDeselect));
            // _isHighlighted = toggleDeselect.IS
            this.EndStart(ref _started);
        }


        // Update is called once per frame
        void Update()
        {
            
        }
    }
}