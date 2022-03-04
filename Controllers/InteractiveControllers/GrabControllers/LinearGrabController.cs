using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using iffnsStuff.iffnsVRCStuff.SyncControllers;

namespace iffnsStuff.iffnsVRCStuff.InteractiveControllers
{
    public class LinearGrabController : UdonSharpBehaviour
    {
        [Header("Note: Local position and rotation of Pickup needs to be (0, 0, 0)")]
        [SerializeField] GameObject[] OwnershipObjects;
        [SerializeField] VRC_Pickup LinkedPickupWithXYOffset;
        [SerializeField] float MaxOffset = 1;
        [SerializeField] bool Symetric = true;
        [SerializeField] float SnapOffsetFromZero = 0;

        /*
        Sync behavior:
        - If noone is using the controller, the player that uses the positioning pickup declares ownership of the rotation controller and the linked ownership objects
        - The output value (between 0...1 if non symetric , between -1...1 if symetric) is shared from the owner and sets the displayed offset
        */

        bool pickupIsHeld = false;
        Vector3 lastLocalPickupPosition;

        [SerializeField] SyncControllerFloatLinear SyncValue;
        //[UdonSynced(UdonSyncMode.Smooth)] float outputValue = 0;
        public float GetOutputValue()
        {
            return SyncValue.GetValue();
        }

        public override void OnDeserialization()
        {
            Debug.Log("Update recieved on linear controller");
        }

        public override void OnPreSerialization()
        {
            Debug.Log("Update to be sent on linear controller");
        }

        void Update()
        {
            //Detect pickup by local player and assign ownership
            if (!pickupIsHeld && LinkedPickupWithXYOffset.IsHeld)
            {
                if (LinkedPickupWithXYOffset.currentPlayer == Networking.LocalPlayer)
                {
                    if (!Networking.IsOwner(gameObject)) Networking.SetOwner(player: Networking.LocalPlayer, obj: gameObject);

                    if (OwnershipObjects != null && OwnershipObjects.Length > 0)
                    {
                        for (int i = 0; i < OwnershipObjects.Length; i++)
                        {
                            if (OwnershipObjects[i] == null) continue;
                            if (Networking.IsOwner(OwnershipObjects[i])) continue;
                            Networking.SetOwner(player: Networking.LocalPlayer, obj: OwnershipObjects[i]);
                        }
                    }
                }
            }

            pickupIsHeld = LinkedPickupWithXYOffset.IsHeld;

            //Calculate value if owner, otherwise apply
            if (pickupIsHeld && Networking.IsOwner(gameObject))
            {
                lastLocalPickupPosition = transform.parent.InverseTransformPoint(LinkedPickupWithXYOffset.transform.position);

                float currentOffset = lastLocalPickupPosition.z;

                if (Mathf.Abs(currentOffset) < SnapOffsetFromZero)
                {
                    currentOffset = 0;
                }
                else
                {
                    //Clamp offset
                    if (Symetric)
                    {
                        currentOffset = Mathf.Clamp(value: currentOffset, min: -MaxOffset, max: MaxOffset);
                    }
                    else
                    {
                        currentOffset = Mathf.Clamp(value: currentOffset, min: -0, max: MaxOffset);
                    }
                }

                //Set outputValue
                //outputValue = GetOutputValueFromOffset(offset: currentOffset);
                SyncValue.SetValue(GetOutputValueFromOffset(offset: currentOffset));

                //Set rotation
                transform.localPosition = Vector3.forward * currentOffset;
            }
            else
            {
                transform.localPosition = Vector3.forward * GetOffsetFromOutputValue(SyncValue.GetValue());
                //transform.localPosition = Vector3.forward * GetOffsetFromOutputValue(outputValue);
            }

            //Reset pickup position
            LinkedPickupWithXYOffset.transform.localPosition = Vector3.zero;
            LinkedPickupWithXYOffset.transform.localRotation = Quaternion.identity;
        }

        float GetOutputValueFromOffset(float offset)
        {
            if (Symetric)
            {
                float tempValue = Mathf.InverseLerp(a: -MaxOffset, MaxOffset, offset);
                return Mathf.Lerp(a: -1, 1, tempValue);
            }
            else
            {
                return Mathf.InverseLerp(a: 0, MaxOffset, offset);
            }
        }

        float GetOffsetFromOutputValue(float outputValue)
        {
            float returnValue = Mathf.LerpUnclamped(0, MaxOffset, outputValue);

            return returnValue;

        }
    }
}