using UnityEngine;
using UnityEngine.UI;

namespace Common.UI
{
    public class ImageFill : MonoBehaviour
    {
        public Image FillDisplay;
        public void SetDisplay(float percent)
        {
            FillDisplay.fillAmount = percent;
        }
    }
}