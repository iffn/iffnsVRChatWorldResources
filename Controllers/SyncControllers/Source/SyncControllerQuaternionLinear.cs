using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common;

namespace iffnsStuff.iffnsVRCStuff.SyncControllers
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class SyncControllerQuaternionLinear : UdonSharpBehaviour
    {
        [UdonSynced] Quaternion syncValue = Quaternion.identity;
        Quaternion lastRecievedValue = Quaternion.identity;
        Quaternion savedValue = Quaternion.identity;
        float lastSyncTime = 0;
        float timeSinceLastSync; //Used to check serialization frequency

        [SerializeField] float SmoothSyncThreshold = 1;

        public void SetValue(Quaternion newValue)
        {
            if (!Networking.IsOwner(gameObject)) Networking.SetOwner(player: Networking.LocalPlayer, obj: gameObject);

            syncValue = newValue;

            RequestSerialization();
        }

        public void SetValueIfChanged(Quaternion newValue, float toleranceAngleDeg)
        {
            if (Quaternion.Angle(newValue, syncValue) < toleranceAngleDeg) return;

            SetValue(newValue);
        }

        public Quaternion GetValue()
        {
            if (Networking.IsOwner(gameObject) || Time.time - lastSyncTime > SmoothSyncThreshold) return syncValue;

            return RemapQuaternionWithFloat(
                inputMin: lastSyncTime,
                inputMax: lastSyncTime + timeSinceLastSync,
                outputMin: lastRecievedValue,
                outputMax: syncValue,
                inputValue: Time.time);
        }

        public float GetTimeSinceLastSync()
        {
            return Time.time - lastSyncTime;
        }

        public override void OnDeserialization()
        {
            lastRecievedValue = savedValue;
            savedValue = syncValue;

            timeSinceLastSync = Time.time - lastSyncTime;
            lastSyncTime = Time.time;
        }

        public override void OnPostSerialization(SerializationResult result)
        {
            timeSinceLastSync = Time.time - lastSyncTime;
            lastSyncTime = Time.time;
        }

        public Quaternion RemapQuaternionWithFloat(float inputMin, float inputMax, Quaternion outputMin, Quaternion outputMax, float inputValue)
        {
            float t = Mathf.InverseLerp(a: inputMin, b: inputMax, value: inputValue);
            return Quaternion.Lerp(a: outputMin, b: outputMax, t: t);
        }

    }
}