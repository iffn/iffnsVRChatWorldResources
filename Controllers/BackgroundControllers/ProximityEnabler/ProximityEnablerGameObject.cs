using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace iffnsStuff.iffnsVRCStuff.BackgroundControllers
{
    public class ProximityEnablerGameObject : UdonSharpBehaviour
    {
        [SerializeField] GameObject LinkedObject;
        [SerializeField] float ActivationDistance = 5;

        private void Update()
        {
            float distance = (transform.position - Networking.LocalPlayer.GetPosition()).magnitude;

            LinkedObject.SetActive(distance < ActivationDistance);
        }
    }
}