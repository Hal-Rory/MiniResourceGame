using System.Collections.Generic;
using UnityEngine;

namespace Common.Utility
{
    public class AnimationStateEventHandler : StateMachineBehaviour
    {
        public List<AnimationEventObject> EventObjects;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            foreach (AnimationEventObject events in EventObjects)
            {
                events.EnterEventStarted();
            }
        }

        //OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            foreach (AnimationEventObject events in EventObjects)
            {
                events.ExitEventStarted();
            }
        }
    }
}
