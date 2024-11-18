using System;
using System.Collections;
using System.Collections.Generic;
using Town.TownPopulation;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    public PopulationFactory Population;
    public InputManager Input;
    public TownObjectManager TownObject;
    public WorkplaceManager Workplace;
    public TownLotFactory TownLot;
    public GameTimeManager GameTime;
    public IncomeManager Income;
    public TownLotSelectionManager Selection;
    public UIManager UI;
    public TownPopulaceManager TownPopulace;
    [SerializeField] private GridManager _gridManager;

    public GameObject Game;
    public GameObject GameStart;
    public bool GameHasStarted;

    public bool PlacementMode { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            TownPopulace = new TownPopulaceManager();
            TownPopulace.SetUp();
            TownObject = new TownObjectManager();
            TownObject.SetUp();
            UI = new UIManager();
            UI.SetUp();
            TownObject.OnStateChanged += SetPlacementMode;
            Population = new PopulationFactory();
            Population.SetUp();
            Workplace = new WorkplaceManager();
            Workplace.SetUp();
            Income = new IncomeManager();
            Income.SetUp();
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
        GameTime.SetTimeActive(true);
    }

    public void StartGame()
    {
        GameHasStarted = true;
    }

    private void SetPlacementMode(bool active)
    {
        PlacementMode = active;
    }

    public List<T> GetLots<T>(int[] ids)
    {
        return TownLot.GetLots<T>(ids);
    }

    public void RegisterPlacementListener(Action<TownLot> creationListener = null, Action<TownLot> destructionListener = null)
    {
        if(creationListener != null) TownLot.OnLotAdded += creationListener;
        if(destructionListener != null) TownLot.OnLotRemoved += destructionListener;
    }

    public void UnregisterPlacementListener(Action<TownLot> creationListener = null, Action<TownLot> destructionListener = null)
    {
        if(creationListener != null) TownLot.OnLotAdded -= creationListener;
        if(destructionListener != null) TownLot.OnLotRemoved -= destructionListener;
    }

    public void PlaceLot(TownLotObj townLot, Vector3Int position)
    {
        if (!Income.TryPurchase(townLot.LotPrice)) return;
        _gridManager.AddLot(townLot, position);
    }

    public void RemoveLot(Vector3Int position)
    {
        _gridManager.RemoveLot(position);
    }
}