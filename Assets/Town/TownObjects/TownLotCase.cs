using TMPro;
using UnityEngine;

namespace Town.TownObjects
{
    /// <summary>
    /// The casing for a town lot, contains all the parts it needs to begin the setup process
    /// </summary>
    public class TownLotCase : MonoBehaviour
    {
        public SpriteRenderer Renderer;
        public BoxCollider2D Collider;
        public SpriteRenderer HoverObject;
        public SpriteRenderer PopupIcon;
        public TextMeshPro PopupText;
        public Animator PopupAnimator;
    }
}