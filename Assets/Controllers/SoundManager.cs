using Placement;
using UnityEngine;

namespace Controllers
{
    public class SoundManager : MonoBehaviour
    {
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _select;
        [SerializeField] private AudioClip _cancel;
        [SerializeField] private AudioClip _place;
        [SerializeField] private AudioClip _construction;
        [SerializeField] private AudioClip _deconstruction;

        private void Start()
        {
            GameController.Instance.RegisterPlacementListener(OnLotAdded, OnLotRemoved);
            GameController.Instance.Selection.OnTownObjectSelected += OnTownObjectSelected;
        }

        private void OnDestroy()
        {
            if (GameController.Instance == null) return;
            GameController.Instance.UnregisterPlacementListener(OnLotAdded, OnLotRemoved);
            GameController.Instance.Selection.OnTownObjectSelected += OnTownObjectSelected;
        }

        private void OnTownObjectSelected(TownLot lot)
        {
            if (!lot) return;
            PlaySelect();
        }

        private void OnLotRemoved(TownLot obj)
        {
            PlayDeconstruction();
        }

        private void OnLotAdded(TownLot _)
        {
            PlayConstruction();
        }

        public void PlaySelect()
        {
            _audioSource.PlayOneShot(_select);
        }
        public void PlayCancel()
        {
            _audioSource.PlayOneShot(_cancel);
        }
        public void PlayPlace()
        {
            _audioSource.PlayOneShot(_place);
        }
        public void PlayConstruction()
        {
            _audioSource.PlayOneShot(_construction);
        }
        public void PlayDeconstruction()
        {
            _audioSource.PlayOneShot(_deconstruction);
        }
    }
}