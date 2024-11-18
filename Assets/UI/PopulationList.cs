using System.Collections.Generic;
using Common.UI;
using Town.TownPopulation;
using UnityEngine;

public class PopulationList : MonoBehaviour
{
    public bool InUse { get; private set; }
    [SerializeField] private CardListView _listView;
    private PopulationFactory _population => GameController.Instance.Population;
    private Card _cardPrefab;

    public IEnumerator<Card> CreateList(Card cardType)
    {
        _cardPrefab = cardType;
        foreach (Person person in _population.Population)
        {
            string personKey = person.ID.ToString();
            if (InUse)
            {
                yield return _listView.SpawnedCards[personKey];
            }
            else
            {
                yield return _listView.SpawnItem(personKey, person.Name, _cardPrefab);
            }
        }
        InUse = true;
    }

    public void ClearList()
    {
        _listView.ClearCards();
    }
}