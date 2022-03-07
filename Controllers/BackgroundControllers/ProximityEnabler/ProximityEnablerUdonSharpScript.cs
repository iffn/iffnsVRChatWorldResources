using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace iffnsStuff.iffnsVRCStuff.BackgroundControllers
{
    public class ProximityEnablerUdonSharpScript : UdonSharpBehaviour
    {
        [Header("Note: LinkedBehavior is should not link to this")]
        [SerializeField] UdonSharpBehaviour LinkedBehavior;
        [SerializeField] float ActivationDistance = 5;

        bool setupIncorrect = false;

        private void Start()
        {
            setupIncorrect = LinkedBehavior.transform == transform;
            if (setupIncorrect) Debug.LogWarning("Warning: ProximityEnablerUdonSharpScript of " + transform.name + " is linking to itself.");
        }

        private void Update()
        {
            if (setupIncorrect) return;

            float distance = (transform.position - Networking.LocalPlayer.GetPosition()).magnitude;

            LinkedBehavior.enabled = distance < ActivationDistance;
        }
    }
}