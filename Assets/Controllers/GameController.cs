using System;
using System.Collections.Generic;
using Common.Utility;
using Interfaces;
using Placement;
using Town.TownObjectData;
using Town.TownPopulation;
using UI;
using UnityEngine;
using Utility;

namespace Controllers
{
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
        public SoundManager Sound;
        public TownLotSelectionManager Selection;
        public UIManager UI;
        public TownPopulaceManager TownPopulace;
        [SerializeField] private GridManager _gridManager;

        private readonly object _townLotLock = new();

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

                GameTime.SetTimeActive(true);
            }
            else
            {
                Destroy(gameObject);
            }
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

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        private void SetPlacementMode(bool active)
        {
            PlacementMode = active;
        }

        public void GetPersonLocation(Person person)
        {
            GetPersonLocation(person, GameTime.TimeOfDay);
        }

        public void GetPersonLocation(Person person, GameTimeManager.TimesOfDay timeOfDay)
        {
            lock (_townLotLock)
            {
                if (Population.GetHousehold(person.HouseholdIndex) == null) return;
                int houseID = Population.GetHousehold(person.HouseholdIndex).HouseID;
                switch (timeOfDay)
                {
                    case GameTimeManager.TimesOfDay.Work:
                        person.SetLocation(person.JobIndex != -1
                            ? TownLot.GetLot(person.JobIndex)
                            : TownLot.GetLot(houseID));
                        break;
                    case GameTimeManager.TimesOfDay.Relax:
                        TownLot location = null;
                        if (TownLot.TryGetLots(out List<TownLot> locations))
                        {
                            location = locations.FindAll(w => w.TryGetHappiness(person.AgeGroup))
                                .GetRandomIndex();
                        }
                        person.SetLocation(location ?? TownLot.GetLot(houseID));
                        break;
                    case GameTimeManager.TimesOfDay.Prepare:
                    case GameTimeManager.TimesOfDay.Rest:
                    default:
                        person.SetLocation(TownLot.GetLot(houseID));
                        break;
                }
            }
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
            if (!_gridManager.CanPlaceObejctAt(position, townLot.LotSize) || !Income.CanPurchase(townLot.LotPrice)) return;
            if(!Income.TryPurchase(townLot.LotPrice)) return;
            _gridManager.AddLot(townLot, position);
        }

        public void RemoveLot(Vector3Int position)
        {
            lock (_townLotLock)
            {
                _gridManager.RemoveLot(position);
            }
        }

        public bool CanPurchase(TownLotObj obj)
        {
            return Income.CanPurchase(obj.LotPrice);
        }

        public void SetTimeMultiplier(float multiplier)
        {
            GameTime.SetMultiplier(multiplier);
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
}