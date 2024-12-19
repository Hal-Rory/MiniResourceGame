using System;
using Controllers;
using UnityEngine;
using UnityEngine.UI;

namespace Utility
{
    public class Tutorial : MonoBehaviour
    {
        [Serializable]
        private class Tutorials
        {
            public string TutorialTitle;
            public int TutorialClipIndex;
        }

        private string[] _tutorialTexts;
        [SerializeField] private Tutorials[] _tutorials;

        [SerializeField] private ScrollRect _scrollPanel;

        [SerializeField] private GameObject _tutorialPanel;

        [SerializeField] private string _switchTrigger;
        [SerializeField] private string _animationIndex;

        [SerializeField] private CardTMP _tutorialCardTMP;
        private bool _active;

        private int _currentTutorial;
        private Animator _tutorialAnimations;

        private void Start()
        {
            DefineTutorialText();
            _tutorialAnimations = _tutorialCardTMP.GetComponentInChildren<Animator>();
            StartTutorial();
            _active = _tutorialPanel.activeSelf;
        }

        private void OnEnable()
        {
            GameController.Instance.SetKeyMenuPause(true);
        }

        private void DefineTutorialText()
        {
            _tutorialTexts = new[]
            {
                //building menu tutorial
                "Open the build menu and look through the categories to find a lot. Select that lot to look at the specific perks and information. " +
                "When you're ready, click build and place it wherever you want (and wherever the placement guide allows).",

                //destroy lot
                "Simply click on your building and click the big destroy button. Easy! " +
                "Be careful though: you do get resources back, but this could still affect overall town needs and perks.",

                //lot types
                "Before you commit to building, you can see the information for that lot," +
                " such as name, the price, and the category it's from. You can also click on any already built lots to get this information as well." +
                $"\n\n<color=#{ColorPaletteUtilities.DarkBrownHex}>Categories/Perks</color>:" +
                "\t\nResidential lots provide housing and increases population. " +
                "\t\nCommercial lots provide work and money. " +
                "\t\nRecreational category lots provide happiness. Sometimes, a lot can even provide multiple perks!" +
                $"\n\n<color=#{ColorPaletteUtilities.DarkBrownHex}>Capacity</color>: Basically, how many people you can fit in one lot. " +
                "Depending on the type of lot, this can mean how many people can live there, how many people can work there, or even max capacity if it's a recreational building (our lots are up to code after all).",

                //town info tutorial
                "In addition to the game's date and time, you'll see that everyone who lives on the town helps contribute to the overall stats as well as their own stats. " +
                //"This is important, because some lots or people aren't available until the town perks have raised enough overall." This is V2
                $"\n\n<color=#{ColorPaletteUtilities.DarkBrownHex}>Perk Specifics</color>:" +
                $"\t\n<color=#{ColorPaletteUtilities.DarkBrownHex}>Income</color>: The funds you have to buy new lots and the net income of the residents that have jobs." +
                $"\t\n<color=#{ColorPaletteUtilities.DarkBrownHex}>Population</color>: How many people are living there right now. " +
                "(It's important to have enough housing to fit new residents.)" +
                $"\t\n<color=#{ColorPaletteUtilities.DarkBrownHex}>Happiness</color>: The total happiness of the town. This goes up and down depending on each resident." +
                "\n\nAll of these help attract new people to your town, so keep those numbers up!"
            };
        }

        public void ToggleTutorial()
        {
            _active = !_active;
            if (!_tutorialPanel.gameObject.activeSelf)
            {
                _currentTutorial = 0;
                StartTutorial();
                GameController.Instance.Sound.PlaySelect();
            }
            else
            {
                GameController.Instance.Sound.PlayCancel();
            }
            _tutorialPanel.gameObject.SetActive(_active);
            GameController.Instance.SetKeyMenuPause(_active);
        }

        public void CloseTutorial_UI()
        {
            _active = false;
            _tutorialPanel.gameObject.SetActive(_active);
            GameController.Instance.SetKeyMenuPause(_active);
        }

        public void NextTutorial()
        {
            _currentTutorial = (_currentTutorial + _tutorials.Length + 1) % _tutorials.Length;
            StartTutorial();
            GameController.Instance.Sound.PlaySelect();
        }

        public void PreviousTutorial()
        {
            _currentTutorial = (_currentTutorial + _tutorials.Length - 1) % _tutorials.Length;
            StartTutorial();
            GameController.Instance.Sound.PlaySelect();
        }

        private void StartTutorial()
        {
            _tutorialCardTMP.SetHeader(_tutorials[_currentTutorial].TutorialTitle);
            _tutorialCardTMP.SetLabel(_tutorialTexts[_currentTutorial]);
            _scrollPanel.verticalNormalizedPosition = 1;
            LayoutRebuilder.ForceRebuildLayoutImmediate(_scrollPanel.content);
            SetAnimation(_tutorials[_currentTutorial].TutorialClipIndex);
        }

        private void SetAnimation(int index)
        {
            _tutorialAnimations.SetInteger(_animationIndex, index);
            _tutorialAnimations.SetTrigger(_switchTrigger);
        }
    }
}