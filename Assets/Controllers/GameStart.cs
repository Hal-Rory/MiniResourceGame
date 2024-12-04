using System.Collections;
using Controllers;
using Town.TownObjectData;
using UnityEngine;
using UnityEngine.UI;

public class GameStart : MonoBehaviour
{
    public Button GameStartButton;
    public PolygonCollider2D _polycollider;
    public TownLotObj _lot;

    private IEnumerator Start()
    {
        for (int x = 0; x < _polycollider.bounds.size.x; x++)
        {
            for (int y = 0; y < _polycollider.bounds.size.y; y++)
            {
                if (Random.value > .95f)
                {
                    Vector3Int pos = new Vector3Int((int)_polycollider.bounds.min.x, (int)_polycollider.bounds.min.y) +
                                 new Vector3Int(x, y);
                    print(new Vector2((int)_polycollider.bounds.min.x, (int)_polycollider.bounds.min.y) + new Vector2(x, y));
                    GameController.Instance.PlaceLot(_lot, pos);
                }
            }
        }

        yield return new WaitForSeconds(1f);
        GameStartButton.interactable = true;
    }

    public void StartGame_Button()
    {
    }
}