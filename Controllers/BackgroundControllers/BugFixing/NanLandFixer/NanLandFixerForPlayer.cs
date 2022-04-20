using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace iffnsStuff.iffnsVRCStuff.BugFixing
{
    public class NanLandFixerForPlayer : UdonSharpBehaviour
    {
        //Settings
        const float playerVelocityMax = 5.5f; //Run and jump with run speed 4 amd jump impulse 3 = 5.1 m/s
        const float maxVelocityIncreaseFactorPerFixedFrame = 1.2f;

        //Runtime variables
        float lastPlayerVelocity = 0;
        float peakPlayerVelocity = 0;
        Vector3 lastPosition2 = Vector3.zero;
        Vector3 lastPosition = Vector3.zero;
        VRCPlayerApi LocalPlayer;

        public float GetLastPlayerVelocity() { return lastPlayerVelocity; }
        public float GetPeakPlayerVelocity() { return peakPlayerVelocity; }

        private void Start()
        {
            LocalPlayer = Networking.LocalPlayer;
        }

        void FixedUpdate()
        {
            #if UNITY_EDITOR
            return;
            #endif

            if (LocalPlayer == null)
            {
                Debug.LogWarning("Error with NanLandFixer: LocalPlayer not assigned. Likely that FixedUpdate was run before Start of this object -> Ignore if once");
                return;
            }

            Vector3 velocity = LocalPlayer.GetVelocity();
            float velocityMagnitude = velocity.magnitude;

            if (velocityMagnitude > playerVelocityMax && velocityMagnitude > lastPlayerVelocity * maxVelocityIncreaseFactorPerFixedFrame)
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
                lastPosition = LocalPlayer.GetPosition();
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