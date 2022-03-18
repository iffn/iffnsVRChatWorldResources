using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace iffnsStuff.iffnsVRCStuff.BugFixing
{
    public class NanLandFixerForPlayer : UdonSharpBehaviour
    {
        float peakPlayerVelocity = 0;
        float lastPlayerVelocity = 0;
        Vector3 lastPosition = Vector3.zero;
        const float playerVelocityMax = 5.5f; //Run and jump with run speed 4 amd jump impulse 3 = 5.1 m/s
        VRCPlayerApi LocalPlayer = Networking.LocalPlayer;

        public float GetLastPlayerVelocity() { return lastPlayerVelocity; }
        public float GetPeakPlayerVelocity() { return peakPlayerVelocity; }

        void FixedUpdate()
        {
            Vector3 velocity = Networking.LocalPlayer.GetVelocity();
            float velocityMagnitude = velocity.magnitude;

            if (velocityMagnitude > playerVelocityMax)
            {
                Vector3 velocityNormalized = velocity.normalized;

                Networking.LocalPlayer.SetVelocity(velocityNormalized * playerVelocityMax);

                lastPosition += velocityNormalized * playerVelocityMax * Time.fixedDeltaTime;

                Networking.LocalPlayer.TeleportTo(lastPosition, Networking.LocalPlayer.GetRotation());
            }
            else
            {
                lastPosition = Networking.LocalPlayer.GetPosition();
            }

            lastPlayerVelocity = velocityMagnitude;
            if (peakPlayerVelocity < lastPlayerVelocity) peakPlayerVelocity = lastPlayerVelocity;
        }

        public override void OnOwnershipTransferred(VRCPlayerApi player)
        {
            if (player != Networking.LocalPlayer) return;
        }
        
    }
    
    
}