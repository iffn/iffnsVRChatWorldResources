using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace iffnsStuff.iffnsVRCStuff.BackgroundControllers
{
    public class ProximityEnablerUdonSharpScript : UdonSharpBehaviour
    {
        [SerializeField] UdonSharpBehaviour LinkedBehavior;
        [SerializeField] float ActivationDistance = 5;

        private void Update()
        {
            float distance = (transform.position - Networking.LocalPlayer.GetPosition()).magnitude;

            LinkedBehavior.enabled = distance < ActivationDistance;
        }
    }
}