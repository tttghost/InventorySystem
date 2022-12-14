using UnityEngine;

namespace Exoa.Common
{
    /// <summary>This is the base class for all components that need to implement some kind of special logic when selected. You can do this manually without this class, but this makes it much easier.
    /// NOTE: This component will register and unregister the associated LeanSelectable in OnEnable and OnDisable.</summary>
    public abstract class TouchSelectableBehaviour : MonoBehaviour
    {
        [System.NonSerialized]
        private TouchSelectable selectable;

        /// <summary>This tells you which LeanSelectable is currently associated with this component.</summary>
        public TouchSelectable Selectable
        {
            get
            {
                if (selectable == null)
                {
                    Register();
                }

                return selectable;
            }
        }

        /// <summary>This method allows you to manually register the LeanSelectable this component is associated with. This is useful if you're manually spawning and attaching children from code.</summary>
        [ContextMenu("Register")]
        public void Register()
        {
            Register(GetComponentInParent<TouchSelectable>());
        }

        /// <summary>This method allows you to manually register the LeanSelectable this component is associated with.</summary>
        public void Register(TouchSelectable newSelectable)
        {
            if (newSelectable != selectable)
            {
                // Unregister existing
                Unregister();

                // Register a new one?
                if (newSelectable != null)
                {
                    selectable = newSelectable;

                    selectable.OnSelected.AddListener(OnSelected);
                    selectable.OnDeselected.AddListener(OnDeselected);
                }
            }
        }

        /// <summary>This method allows you to manually register the LeanSelectable this component is associated with. This is useful if you're changing the associated LeanSelectable.</summary>
        [ContextMenu("Unregister")]
        public void Unregister()
        {
            if (selectable != null)
            {
                selectable.OnSelected.RemoveListener(OnSelected);
                selectable.OnDeselected.RemoveListener(OnDeselected);

                selectable = null;
            }
        }

        protected virtual void OnEnable()
        {
            Register();
        }

        protected virtual void Start()
        {
            if (selectable == null)
            {
                Register();
            }
        }

        protected virtual void OnDisable()
        {
            Unregister();
        }

        /// <summary>Called when selection begins.</summary>
        protected virtual void OnSelected(TouchSelect select)
        {
        }

        /// <summary>Called when this is deselected.</summary>
        protected virtual void OnDeselected(TouchSelect select)
        {
        }
    }
}