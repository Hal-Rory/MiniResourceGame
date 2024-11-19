using System;

namespace Utility
{
    public class ActionBox
    {
        private Action _onPickup;
        private Action _onPutdown;
        public bool Running;

        public ActionBox(Action pickup, Action putdown)
        {
            _onPickup = pickup;
            _onPutdown = putdown;
        }

        public void Start()
        {
            if (Running) return;
            _onPickup();
        }

        public void Stop()
        {
            if (!Running) return;
            _onPutdown();
        }
    }
}