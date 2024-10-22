using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    public PopulationManager PopulationManager;
    public InputManager InputManager;
    public TownObjectManager TownObjectManager;
    public WorkplaceManager WorkplaceManager;
    public ObjectPlacer ObjectPlacer;
    public TimeManager TimeManager;
    public MoneyManager MoneyManager;
    public TownLotSelectionManager Selection;
    public UIManager UI;

    public GameObject Game;
    public GameObject GameStart;
    public bool GameHasStarted;

    public bool PlacementMode { get; private set; }

    private int _population;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            TownObjectManager.OnStateChanged += SetPlacementMode;
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
        return ObjectPlacer.GetLot(id);
    }

    public List<T> GetLots<T>(string id)
    {
        return ObjectPlacer.GetLots<T>(id);
    }

    public List<T> GetLots<T>(int[] ids)
    {
        return ObjectPlacer.GetLots<T>(ids);
    }

    public void RegisterPlacementListener(Action<TownLot> creationListener = null, Action<TownLot> destructionListener = null)
    {
        if(creationListener != null) ObjectPlacer.OnLotAdded += creationListener;
        if(destructionListener != null) ObjectPlacer.OnLotRemoved += destructionListener;
    }

    public void UnregisterPlacementListener(Action<TownLot> creationListener = null, Action<TownLot> destructionListener = null)
    {
        if(creationListener != null) ObjectPlacer.OnLotAdded -= creationListener;
        if(destructionListener != null) ObjectPlacer.OnLotRemoved -= destructionListener;
    }
}