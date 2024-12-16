using System;
using Controllers;
using UnityEngine;

namespace Utility
{
    public class Tutorial : MonoBehaviour
    {

        [Serializable]
        private struct Tutorials
        {
            public string TutorialTitle;
            public int[] TutorialClips;
            public int TutorialClipIndex;

            public int NextClip()
            {
                TutorialClipIndex = (TutorialClipIndex + TutorialClips.Length + 1) % TutorialClips.Length;
                return TutorialClips[TutorialClipIndex];
            }

            public int PreviousClip()
            {
                TutorialClipIndex = (TutorialClipIndex + TutorialClips.Length - 1) % TutorialClips.Length;
                return TutorialClips[TutorialClipIndex];
            }

            public int CurrentClip()
            {
                return TutorialClips[TutorialClipIndex];
            }
        }

        private string[] _tutorialTexts;
        [SerializeField] private Tutorials[] _tutorials;
        [SerializeField] private string _switchTrigger;
        [SerializeField] private string _animationIndex;
        [SerializeField] private CardTMP _tutorialCardTMP;
        private int _currentTutorial;
        private Animator _tutorialAnimations;

        private void Start()
        {
            DefineTutorialText();
            _tutorialAnimations = _tutorialCardTMP.GetComponentInChildren<Animator>();
            StartTutorial();
        }

        private void DefineTutorialText()
        {
            _tutorialTexts = new[]
            {
                //building menu tutorial
                $"<color=#{GameController.Instance.UI.ColorPalette.DarkBrownHex}>Build Menu</color>: The build menu has several collections to choose a town lot from. Select a lot to display the various stats that are available." +
                $"\n\nSelect build to place the building on the map using the placement guide.",

                //lot types
                $"<color=#{GameController.Instance.UI.ColorPalette.DarkBrownHex}>Lot Name</color>: Each lot has a price and various stats that are displayed when selected." +
                $"\n\n<color=#{GameController.Instance.UI.ColorPalette.DarkBrownHex}>Lot Price</color>: The price of the lot" +
                $"\n\n<color=#{GameController.Instance.UI.ColorPalette.DarkBrownHex}>Lot Type</color>: The lot type" +
                "\n\tResidential lots provide housing, commercial lots provide work, and recreational lots provide happiness, a trait that some commercial lots share." +
                $"\n\n<color=#{GameController.Instance.UI.ColorPalette.DarkBrownHex}>Lot Perks</color>: What the building provides, such as income or happiness" +
                $"\n\n<color=#{GameController.Instance.UI.ColorPalette.DarkBrownHex}>Lot Inhabiting/Employee Capacity</color>: The type of people that can that will reside in this lot and the max capacity allowed" +
                $"\n\n<color=#{GameController.Instance.UI.ColorPalette.DarkBrownHex}>Lot Visitor Capacity</color>: The type of people that can visit this lot, if any, and the max capacity allowed",

                //lot display tutorial
                $"<color=#{GameController.Instance.UI.ColorPalette.DarkBrownHex}>Lot Information</color>: Once placed, lots have their own stats that are displayed, varying by the lot type." +
                "\n\nBy selecting a lot, this information can be viewed as well as additional information based on the stat." +
                "\n\nIt is also possible to demolish the lot entirely.",

                //town info tutorial
                $"<color=#{GameController.Instance.UI.ColorPalette.DarkBrownHex}>Town Information</color>: In addition to the game's date and time, the town has several stats that contribute to it's attractiveness." +
                $"\n\n<color=#{GameController.Instance.UI.ColorPalette.DarkBrownHex}>Income</color>: The funds available and the net income of the residents\n" +
                $"\n\n<color=#{GameController.Instance.UI.ColorPalette.DarkBrownHex}>Population</color>: The current population\n" +
                $"\n\n<color=#{GameController.Instance.UI.ColorPalette.DarkBrownHex}>Happiness</color>: The total happiness of the town\n" +
                "\n\nThese stats combined contribute to the likelihood of new residents moving in." +
                $"\n\nIf a lot is destroyed, these stats are updated. All resources used will be returned, but when removing a home lot, the residents will also be evicted."
            };
        }

        public void NextAnimationInTutorial()
        {
            SetAnimation(_tutorials[_currentTutorial].NextClip());
        }

        public void PreviousAnimationInTutorial()
        {
            SetAnimation(_tutorials[_currentTutorial].PreviousClip());
        }

        public void NextTutorial()
        {
            _tutorials[_currentTutorial].TutorialClipIndex = 0;
            _currentTutorial = (_currentTutorial + _tutorials.Length + 1) % _tutorials.Length;
            _tutorials[_currentTutorial].TutorialClipIndex = 0;
            StartTutorial();
        }

        public void PreviousTutorial()
        {
            _tutorials[_currentTutorial].TutorialClipIndex = 0;
            _currentTutorial = (_currentTutorial + _tutorials.Length - 1) % _tutorials.Length;
            _tutorials[_currentTutorial].TutorialClipIndex = 0;
            StartTutorial();
        }

        private void StartTutorial()
        {
            _tutorialCardTMP.SetHeader(_tutorials[_currentTutorial].TutorialTitle);
            _tutorialCardTMP.SetLabel(_tutorialTexts[_currentTutorial]);
            SetAnimation(_tutorials[_currentTutorial].CurrentClip());
        }

        private void SetAnimation(int index)
        {
            _tutorialAnimations.SetInteger(_animationIndex, index);
            _tutorialAnimations.SetTrigger(_switchTrigger);
        }
    }
}