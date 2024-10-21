using UnityEngine;
using UnityEngine.EventSystems;

namespace Common.Behaviors
{
    public class Moveable : MonoBehaviour, IDragHandler
    {
        [SerializeField] private Vector3 _movementMutliplier;
        public Vector3 ScreenPosition;
        public bool CanMove;

        public void OnDrag(PointerEventData eventData)
        {
            if (!CanMove) return;
            transform.position = Vector3.Scale(ScreenPosition, _movementMutliplier);
        }
    }
}