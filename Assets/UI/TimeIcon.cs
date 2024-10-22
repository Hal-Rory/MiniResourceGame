using UnityEngine;
using UnityEngine.UI;

public class TimeIcon : MonoBehaviour, ITimeListener
{
    public Image Icon;

    private void Start()
    {
        GameController.Instance.TimeManager.RegisterListener(this);
    }

    private void Update()
    {
        Icon.fillAmount += GameController.Instance.TimeManager.GetClockSpeed() * Time.deltaTime / 100;
    }

    public void ClockUpdate(int tick)
    {
        Icon.fillAmount = 0;
    }
}