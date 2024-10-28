using UnityEngine;
using UnityEngine.UI;

public class TimeIcon : MonoBehaviour, ITimeListener
{
    public Image Icon;

    private void Start()
    {
        GameController.Instance.GameTime.RegisterListener(this);
    }

    private void Update()
    {
        Icon.fillAmount += GameController.Instance.GameTime.GetClockSpeed() * Time.deltaTime / 100;
    }

    public void ClockUpdate(int tick)
    {
        Icon.fillAmount = 0;
    }
}