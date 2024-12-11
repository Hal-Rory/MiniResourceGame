using System;
using UnityEngine;

namespace Common.Utility
{
    [CreateAssetMenu(fileName = "AnimationEventObject", menuName = "Utility/Animation Event Object")]
    public class AnimationEventObject : ScriptableObject
    {
        [SerializeField] private int[] _enterEvents;
        public event Action<int[]> EnterEvent;
        [SerializeField] private int[] _exitEvents;
        public event Action<int[]> ExitEvent;

        public void EnterEventStarted()
        {
            EnterEvent?.Invoke(_enterEvents);
        }

        public void ExitEventStarted()
        {
            ExitEvent?.Invoke(_exitEvents);
        }
    }
}
