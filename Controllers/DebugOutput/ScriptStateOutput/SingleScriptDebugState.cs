using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace iffnsStuff.iffnsVRCStuff.DebugOutput
{
    public class SingleScriptDebugState  : UdonSharpBehaviour
    {
        public bool EnableOutput = true;
        public float TimeBetweenOutputs = 0.5f;
        public bool CheckActivationDistance = true;
        public float ActivationDistance = 5;

        public const string newLine = "\n";

        float nextOutputTime = 0;

        public bool IsReadyForOutput()
        {
            if (!EnableOutput) return false;

            if (Time.time < nextOutputTime) return false;

            if (CheckActivationDistance)
            {
                if ((Networking.LocalPlayer.GetPosition() - transform.position).magnitude > ActivationDistance) return false; //2 Errors happen when you leave the world: Ignore
            }

            return true;
        }

        string displayName = "";
        string currentState = "";
        public void SetCurrentState(string displayName, string currentState)
        {
            this.displayName = displayName;
            nextStateAvailable = true;
            this.currentState = "Current state of " + displayName + " at " + Time.time + ":" + newLine + currentState;

            nextOutputTime = Time.time + TimeBetweenOutputs;
        }

        bool nextStateAvailable;

        public bool IsNextStateAvailable()
        {
            return nextStateAvailable;
        }

        public string GetCurrentState()
        {
            nextStateAvailable = false;
            return currentState;
        }

        public string GetCurrentName()
        {
            return displayName;
        }
    }
}