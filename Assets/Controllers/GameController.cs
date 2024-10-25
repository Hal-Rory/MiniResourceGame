using System;
using System.Collections;
using System.Collections.Generic;
using Town.TownPopulation;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    public PopulationFactory PopulationFactory;
    public InputManager InputManager;
    public TownObjectManager TownObjectManager;
    public WorkplaceManager WorkplaceManager;
    public ObjectFactory ObjectFactory;
    public TimeManager TimeManager;
    public MoneyManager MoneyManager;
    public TownLotSelectionManager Selection;
    public UIManager UI;

    public GameObject Game;
    public GameObject GameStart;
    public bool GameHasStarted;

    public bool PlacementMode { get; private set; }

    private int _population;

    private Dictionary<PopulationStockpiles, float> _populationStockpiles;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            TownObjectManager.OnStateChanged += SetPlacementMode;
            PopulationFactory = new PopulationFactory();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator Start()
    {
        Game.SetActive(true);
        if (GameStart.activeSelf)
        {
            yield return new WaitUntil(() => GameHasStarted);
        }
        PopulationFactory.Start();
        GameStart.SetActive(false);
        TimeManager.SetTimeActive(true);
    }

    public void StartGame()
    {
        GameHasStarted = true;
    }

    private void SetPlacementMode(bool active)
    {
        PlacementMode = active;
    }

    public TownLot GetLot(int id)
    {
        return ObjectFactory.GetLot(id);
    }

    public List<T> GetLots<T>(string id)
    {
        return ObjectFactory.GetLots<T>(id);
    }

    public List<T> GetLots<T>(int[] ids)
    {
        return ObjectFactory.GetLots<T>(ids);
    }

    public void RegisterPlacementListener(Action<TownLot> creationListener = null, Action<TownLot> destructionListener = null)
    {
        if(creationListener != null) ObjectFactory.OnLotAdded += creationListener;
        if(destructionListener != null) ObjectFactory.OnLotRemoved += destructionListener;
    }

    public void UnregisterPlacementListener(Action<TownLot> creationListener = null, Action<TownLot> destructionListener = null)
    {
        if(creationListener != null) ObjectFactory.OnLotAdded -= creationListener;
        if(destructionListener != null) ObjectFactory.OnLotRemoved -= destructionListener;
    }
}