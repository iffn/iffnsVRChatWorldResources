using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace iffnsStuff.iffnsVRCStuff.BackgroundControllers
{
    public class ProximityEnablerGameObject : UdonSharpBehaviour
    {
        [Header("Note: LinkedObject is should not link to this")]
        [SerializeField] GameObject LinkedObject;
        [SerializeField] float ActivationDistance = 5;

        bool setupIncorrect = false;

        private void Start()
        {
            setupIncorrect = LinkedObject.transform == transform;
            if (setupIncorrect) Debug.LogWarning("Warning: ProximityEnablerGameObject of " + transform.name + " is linking to itself.");
        }

        private void Update()
        {
            if (setupIncorrect) return;

            float distance = (transform.position - Networking.LocalPlayer.GetPosition()).magnitude;

            LinkedObject.SetActive(distance < ActivationDistance);
        }
    }
}