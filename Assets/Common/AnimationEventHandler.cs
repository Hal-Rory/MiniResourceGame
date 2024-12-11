using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Common.Utility
{
    public class AnimationEventHandler : MonoBehaviour
    {
        public AnimationEventObject AnimationStateEvents;
        public List<UnityEvent> Events;

        private void Awake()
        {
            if (!AnimationStateEvents) return;
            AnimationStateEvents.EnterEvent += EnterEvents;
            AnimationStateEvents.ExitEvent += ExitEvents;
        }

        private void OnDestroy()
        {
            if (!AnimationStateEvents) return;
            AnimationStateEvents.EnterEvent -= EnterEvents;
            AnimationStateEvents.ExitEvent -= ExitEvents;
        }

        private void EnterEvents(int[] indicies)
        {
            foreach (int index in indicies)
            {
                AnimateEventByIndex(index);
            }
        }
        
        private void ExitEvents(int[] indicies)
        {
            foreach (int index in indicies)
            {
                AnimateEventByIndex(index);
            }
        }

        public void AnimateEventByIndex(int index)
        {
            Events[index]?.Invoke();
        }

        public void AnimateAllEvents()
        {
            Events.ForEach(e => e?.Invoke());
        }
    }
}
