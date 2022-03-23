using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace iffnsStuff.iffnsVRCStuff.BugFixing
{
    public class NanLandFixerForPlayer : UdonSharpBehaviour
    {
        float lastPlayerVelocity = 0;
        float peakPlayerVelocity = 0;
        Vector3 lastPosition2 = Vector3.zero;
        Vector3 lastPosition = Vector3.zero;
        const float playerVelocityMax = 5.5f; //Run and jump with run speed 4 amd jump impulse 3 = 5.1 m/s
        VRCPlayerApi LocalPlayer = Networking.LocalPlayer;

        public float GetLastPlayerVelocity() { return lastPlayerVelocity; }
        public float GetPeakPlayerVelocity() { return peakPlayerVelocity; }

        void FixedUpdate()
        {
            #if UNITY_EDITOR
            return;
            #endif

            Vector3 velocity = Networking.LocalPlayer.GetVelocity();
            float velocityMagnitude = velocity.magnitude;

            if (velocityMagnitude > playerVelocityMax && velocityMagnitude > lastPlayerVelocity * 1.2f)
            {
                Debug.LogWarning(Time.time + "Velocity fixed with velocity = " + velocity + ", position = " + Networking.LocalPlayer.GetPosition() + ", last position = " + lastPosition + ", last position 2 = " + lastPosition2);

                Vector3 velocityNormalized = velocity.normalized;

                //Networking.LocalPlayer.SetVelocity(velocityNormalized * playerVelocityMax);
                Networking.LocalPlayer.SetVelocity(Vector3.zero);

                //lastPosition += velocityNormalized * playerVelocityMax * Time.fixedDeltaTime;

                Networking.LocalPlayer.TeleportTo(lastPosition2, Networking.LocalPlayer.GetRotation());

            }
            else
            {
                lastPosition2 = lastPosition;
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