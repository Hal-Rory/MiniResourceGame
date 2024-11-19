using UnityEngine;
using UnityEngine.UI;

public class TimeIcon : MonoBehaviour
{
    public Image Icon;

    private void Start()
    {
        GameController.Instance.GameTime.RegisterListener(earlyClockUpdate: ClockUpdate);
    }

    private void Update()
    {
        Icon.fillAmount = GameController.Instance.GameTime.GetTimePercentage();
    }

    public void ClockUpdate(int tick)
    {
        Icon.fillAmount = 0;
    }
}