using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameStart : MonoBehaviour
{
    public Button GameStartButton;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(1f);
        GameStartButton.interactable = true;
    }

    public void StartGame_Button()
    {
        GameController.Instance.StartGame();
    }
}