using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Common.UI
{
    public class OnPointerOverBehavior : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public UnityEvent PointerEnter;
        public UnityEvent PointerExit;
        public void OnPointerEnter(PointerEventData eventData)
        {
            PointerEnter?.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            PointerExit?.Invoke();
        }
    }
}