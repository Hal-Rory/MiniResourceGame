using UnityEngine;
using UnityEngine.Events;

namespace Utility.Behaviors
{
    public class ColliderButton : MonoBehaviour
    {
        public UnityEvent OnClick;

        private void OnMouseDown()
        {
            OnClick?.Invoke();
        }
    }
}
