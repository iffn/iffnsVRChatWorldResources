using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common;

namespace iffnsStuff.iffnsVRCStuff.SyncControllers
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class SyncControllerVector3Linear : UdonSharpBehaviour
    {
        [UdonSynced] Vector3 syncValue = Vector3.zero;
        Vector3 lastRecievedValue = Vector3.zero;
        Vector3 savedValue = Vector3.zero;
        float lastSyncTime = 0;
        float timeSinceLastSync; //Used to check serialization frequency

        [SerializeField] float SmoothSyncThreshold = 1;

        public void SetValue(Vector3 newValue)
        {
            if (!Networking.IsOwner(gameObject)) Networking.SetOwner(player: Networking.LocalPlayer, obj: gameObject);

            syncValue = newValue;

            RequestSerialization();
        }

        public void SetValueIfChanged(Vector3 newValue, float tolerance)
        {
            if ((newValue - syncValue).magnitude < tolerance) return;

            SetValue(newValue);
        }

        public Vector3 GetValue()
        {
            if (Networking.IsOwner(gameObject) || Time.time - lastSyncTime > SmoothSyncThreshold) return syncValue;

            return RemapVector3WithFloat(
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

        public Vector3 RemapVector3WithFloat(float inputMin, float inputMax, Vector3 outputMin, Vector3 outputMax, float inputValue)
        {
            float t = Mathf.InverseLerp(a: inputMin, b: inputMax, value: inputValue);
            return Vector3.Lerp(a: outputMin, b: outputMax, t: t);
        }
    }
}