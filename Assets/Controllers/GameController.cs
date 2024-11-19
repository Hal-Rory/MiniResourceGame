using System;
using System.Collections;
using System.Collections.Generic;
using Interfaces;
using JetBrains.Annotations;
using Town.TownPopulation;
using UnityEditor;
using UnityEngine;
using Utility;

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

    private Dictionary<IActionBoxHolder, ActionBox> Actions;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Actions = new Dictionary<IActionBoxHolder, ActionBox>();
            TownPopulace ??= new TownPopulaceManager();
            TownPopulace.SetUp();
            TownObject ??= new TownObjectManager();
            TownObject.SetUp();
            TownObject.OnStateChanged += SetPlacementMode;
            UI ??= new UIManager();
            UI.SetUp();
            Population ??= new PopulationFactory();
            Population.SetUp();
            Workplace ??= new WorkplaceManager();
            Workplace.SetUp();
            Income ??= new IncomeManager();
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

    private void OnEnable()
    {
        foreach (IActionBoxHolder box in Actions.Keys)
        {
            box.PickingUp();
        }
    }

    private void OnDisable()
    {
        foreach (IActionBoxHolder box in Actions.Keys)
        {
            box.PuttingDown();
        }
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

    /// <summary>
    /// Allows a holder to manually add a box
    /// </summary>
    /// <param name="holder">The holder of the box</param>
    /// <param name="box">The action box</param>
    /// <param name="start">Should the box be started now</param>
    public void PickupAction(IActionBoxHolder holder, ActionBox box)
    {
        Actions.TryAdd(holder, box);
    }
    /// <summary>
    /// Allows a holder to manually remove itself and therefore it's box.
    /// All removed holders will have their box put down.
    /// </summary>
    /// <param name="holder">The holder of the box</param>
    /// <param name="box">The action box</param>
    public void PutdownAction(IActionBoxHolder holder, ActionBox box)
    {
        Actions.Remove(holder);
        holder.PuttingDown();
    }
}