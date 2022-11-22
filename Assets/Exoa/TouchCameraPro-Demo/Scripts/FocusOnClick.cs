using Exoa.Events;
using Exoa.Common;

namespace Exoa.Cameras.Demos
{
    public class FocusOnClick : TouchSelectableBehaviour
    {
        public bool follow;
        public bool focusOnFollow;

        public bool Focus
        {
            get => follow == false || (follow == true && focusOnFollow);
        }
        public bool Follow
        {
            get => follow || focusOnFollow;
        }
        protected override void OnSelected(TouchSelect select)
        {
            if (follow)
                CameraEvents.OnRequestObjectFollow?.Invoke(gameObject, focusOnFollow, false);
            else
                CameraEvents.OnRequestObjectFocus?.Invoke(gameObject, false);
        }

        protected override void OnDeselected(TouchSelect select)
        {
            if (follow)
                CameraEvents.OnRequestObjectFollow?.Invoke(null, focusOnFollow, false);
        }
    }
}
