using System;
using System.Collections;
using System.Collections.Generic;
using Common.Utility;
using Placement;
using Town.TownManagement;
using Town.TownObjectData;
using Town.TownObjectManagement;
using Town.TownObjects;
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
        public TownLotFactory LotFactory;
        public GameTimeManager GameTime;
        public IncomeManager Income;
        public SoundManager Sound;
        public TownLotSelectionManager Selection;
        public UIManager UI;
        public TownPopulaceManager TownPopulace;

        public bool KeyMenuPause { get; private set; }

        [SerializeField] private GridManager _gridManager;
        [SerializeField] private GameObject _game;

        private readonly object _townLotLock = new();

        public bool PlacementMode { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
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
                Income.Pay(300);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(1);
            _game.SetActive(true);
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        /// <summary>
        /// Set the state of the game to or from placement mode where several features need to be disabled
        /// </summary>
        /// <param name="active"></param>
        private void SetPlacementMode(bool active)
        {
            PlacementMode = active;
        }

        #region Person Relocation Management
        public void GetPersonLocation(Person person)
        {
            GetPersonLocation(person, GameTime.TimeOfDay);
        }

        public void GetPersonLocation(Person person, GameTimeManager.TimesOfDay timeOfDay)
        {
            lock (_townLotLock)
            {
                if (Population.GetHousehold(person.HouseholdIndex) == null) return;
                TownLot location = LotFactory.GetLot(person.HouseID);
                if(person.CurrentLocationID != person.HouseID)
                {
                    RemovePersonLocation(person);
                }
                if (person.AgeGroup == PersonAgeGroup.Deceased)
                {
                    location = FindRecreationalLot(person);
                }
                else if (timeOfDay == GameTimeManager.TimesOfDay.Work)
                {
                    if (person.JobIndex != -1)
                    {
                        location = FindJobLot(person);
                    }
                }
                else if(timeOfDay == GameTimeManager.TimesOfDay.Relax)
                {
                    location = FindRecreationalLot(person);
                }

                if (location == null || !location.ValidLot)
                {
                    person.SetLocation();
                    return;
                }
                person.SetLocation(location);
            }
        }

        private TownLot FindJobLot(Person person)
        {
            if (person.JobIndex == -1) return LotFactory.GetLot(person.HouseID);
            TownLot jobLocation = LotFactory.GetLot(person.JobIndex);
            return !jobLocation.ValidLot ? LotFactory.GetLot(person.HouseID) : jobLocation;
        }

        private TownLot FindRecreationalLot(Person person)
        {
            if (!LotFactory.TryGetLots(out List<TownLot> locations) || locations.Count <= 0)
                return LotFactory.GetLot(person.HouseID);
            List<TownLot> viableLocations = locations.FindAll(lot => lot.CheckHappinessGroup(person.AgeGroup) && lot.ValidLot && lot.GetVisitorCount() < lot.GetMaxVisitorCapacity());
            if (viableLocations.Count <= 0) return LotFactory.GetLot(person.HouseID);
            TownLot location = viableLocations.GetRandomIndex();
            location.AddVisitors(person);
            return location;
        }

        public void RemovePersonLocation(Person person)
        {
            if(person.CurrentLocationID != -1)
            {
                TownLot previousLocation = LotFactory.GetLot(person.CurrentLocationID);
                if(previousLocation) previousLocation.RemoveVisitor(person);
            }
            person.SetLocation();
        }
        #endregion

        /// <summary>
        /// For fast subscription to the lotfactory's placement process
        /// </summary>
        /// <param name="creationListener"></param>
        /// <param name="destructionListener"></param>
        public void RegisterPlacementListener(Action<TownLot> creationListener = null, Action<TownLot> destructionListener = null)
        {
            if(creationListener != null) LotFactory.OnLotAdded += creationListener;
            if(destructionListener != null) LotFactory.OnLotRemoved += destructionListener;
        }

        /// <summary>
        /// For fast subscription removal from lotfactory's placement process
        /// </summary>
        /// <param name="creationListener"></param>
        /// <param name="destructionListener"></param>
        public void UnregisterPlacementListener(Action<TownLot> creationListener = null, Action<TownLot> destructionListener = null)
        {
            if(creationListener != null) LotFactory.OnLotAdded -= creationListener;
            if(destructionListener != null) LotFactory.OnLotRemoved -= destructionListener;
        }

        #region Lot Controls
        public void PlaceLot(TownLotObj townLot, Vector3Int position)
        {
            if (!_gridManager.CanPlaceObjectAt(position, townLot.LotSize) || !Income.CanPurchase(townLot.LotPrice)) return;
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
        #endregion

        #region Income Management
        public bool CanPurchase(TownLotObj obj)
        {
            return Income.CanPurchase(obj.LotPrice);
        }
        #endregion

        #region Game State Management
        public void SetTimeMultiplier(int index)
        {
            if (index == GameTime.TimeMultiplier) return;
            GameTime.SetMultiplier(index);
        }

        public void SetKeyMenuPause(bool paused)
        {
            KeyMenuPause = paused;
            if(paused) GameTime.StopTime();
            else GameTime.SetMultiplier(GameTime.TimeMultiplier);
        }
        #endregion
    }
}