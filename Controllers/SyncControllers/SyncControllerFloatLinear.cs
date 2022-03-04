using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common;

namespace iffnsStuff.iffnsVRCStuff.SyncControllers
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class SyncControllerFloatLinear : UdonSharpBehaviour
    {
        [UdonSynced] float syncValue = 0;
        float lastRecievedValue = 0;
        float savedValue = 0;
        float lastSyncTime = 0;
        float timeSinceLastSync;

        [SerializeField] float SmoothSyncThreshold = 1;

        public void SetValue(float newValue)
        {
            if (!Networking.IsOwner(gameObject)) Networking.SetOwner(player: Networking.LocalPlayer, obj: gameObject);

            syncValue = newValue;

            RequestSerialization();
        }

        public void SetValueIfChanged(float newValue, float tolerance)
        {
            if (Mathf.Abs(newValue - syncValue) < tolerance) return;

            SetValue(newValue);
        }

        public float GetValue()
        {
            if (Time.time - lastSyncTime > SmoothSyncThreshold) return syncValue;

            return RemapFloat(
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

        public float RemapFloat(float inputMin, float inputMax, float outputMin, float outputMax, float inputValue)
        {
            float t = Mathf.InverseLerp(a: inputMin, b: inputMax, value: inputValue);
            return Mathf.Lerp(a: outputMin, b: outputMax, t: t);
        }
    }
}