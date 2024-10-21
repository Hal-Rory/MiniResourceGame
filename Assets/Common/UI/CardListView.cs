using System.Collections.Generic;
using System.Linq;
using Common.UI;
using UnityEngine;
using UnityEngine.UI;

public class CardListView : MonoBehaviour
{
    [SerializeField] private ScrollRect _listView;
    public Dictionary<string, Card> SpawnedCards { get; private set; }

    private void Awake()
    {
        SpawnedCards = new Dictionary<string, Card>();
    }

    public Card SpawnItem(string id, string nameValue, Card _cardPrefab)
    {
        Card card = Instantiate(_cardPrefab, _listView.content);
        card.Set(id, nameValue);
        SpawnedCards.Add(id, card);
        return card;
    }

    public Card GetCard(string ID)
    {
        return SpawnedCards[ID];
    }

    public void ClearCards()
    {
        if (SpawnedCards == null) return;
        int count = SpawnedCards.Count;
        if (count == 0) return;
        Card[] cards = SpawnedCards.Values.ToArray();
        for (int i = 0; i < count; i++)
        {
            Destroy(cards[i].gameObject);
        }

        SpawnedCards.Clear();
    }

    public void UpdateLayout()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(_listView.content.transform as RectTransform);
    }
}