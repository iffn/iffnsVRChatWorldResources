using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace iffnsStuff.iffnsVRCStuff.InteractiveControllers
{
    public class RotationController : UdonSharpBehaviour
    {
        [Header("Note: Local position and rotation of Pickup needs to be (0, 0, 0)")]
        [SerializeField] GameObject[] OwnershipObjects;
        [SerializeField] VRC_Pickup LinkedPickupWithXYOffset;
        [SerializeField] Transform ConverterReference;
        [SerializeField] float MaxAngleDeg = 45;
        [SerializeField] bool Symetric = true;
        [SerializeField] float SnapAngleDegOffsetFromZero = 0;

        /*
        Sync behavior:
        - If noone is using the controller, the player that uses the positioning pickup declares ownership of the rotation controller and the linked ownership objects
        - The output value (between 0...1 if non symetric , between -1...1 if symetric) is shared from the owner and sets the displayed rotation angle
        */

        //newLine = backslash n which is interpreted as a new line when showing the code in a text field
        readonly string newLine = "\n";

        bool pickupIsHeld;

        [UdonSynced] float outputValue = 0;
        public float GetOutputValue()
        {
            return outputValue;
        }

        void Update()
        {
            //Detect pickup by local player and assign ownership
            if (!pickupIsHeld && LinkedPickupWithXYOffset.IsHeld)
            {
                if (LinkedPickupWithXYOffset.currentPlayer == Networking.LocalPlayer)
                {
                    if (!Networking.IsOwner(gameObject)) Networking.SetOwner(player: Networking.LocalPlayer, obj: gameObject);

                    if (OwnershipObjects.Length > 0)
                    {
                        for (int i = 0; i < OwnershipObjects.Length; i++)
                        {
                            Networking.SetOwner(player: Networking.LocalPlayer, obj: OwnershipObjects[i]);
                        }
                    }
                }
            }

            pickupIsHeld = LinkedPickupWithXYOffset.IsHeld;

            //Calculate value if owner, otherwise apply
            if (pickupIsHeld && Networking.IsOwner(gameObject))
            {
                ConverterReference.position = LinkedPickupWithXYOffset.transform.position;

                float currentAngleDeg = CalculateAnlgeDegWithY0XNeg90(ConverterReference.transform.localPosition);

                if (Mathf.Abs(currentAngleDeg) < SnapAngleDegOffsetFromZero)
                {
                    currentAngleDeg = 0;
                }
                else
                {
                    //Clamp angle
                    if (Symetric)
                    {
                        currentAngleDeg = Mathf.Clamp(value: currentAngleDeg, min: -MaxAngleDeg, max: MaxAngleDeg);
                    }
                    else
                    {
                        currentAngleDeg = Mathf.Clamp(value: currentAngleDeg, min: -0, max: MaxAngleDeg);
                    }
                }

                //Set outputValue
                outputValue =GetOutputValueFromAngle(angleDeg: currentAngleDeg);

                //Set rotation
                transform.localRotation = Quaternion.Euler(Vector3.forward * currentAngleDeg);
            }
            else
            {
                transform.localRotation = Quaternion.Euler(Vector3.forward * GetAngleDegFromOutputValue(outputValue));
            }

            //Reset pickup position
            LinkedPickupWithXYOffset.transform.localPosition = Vector3.zero;
            LinkedPickupWithXYOffset.transform.localRotation = Quaternion.identity;
        }

        float GetOutputValueFromAngle(float angleDeg)
        {
            if (Symetric)
            {
                float tempValue = Mathf.InverseLerp(a: -MaxAngleDeg, MaxAngleDeg, angleDeg);
                return Mathf.Lerp(a: -1, 1, tempValue);
            }
            else
            {
                return Mathf.InverseLerp(a: 0, MaxAngleDeg, angleDeg);
            }
        }

        string currentLerpOutput = "";

        float GetAngleDegFromOutputValue(float outputValue)
        {
            float returnValue = Mathf.LerpUnclamped(0, MaxAngleDeg, outputValue);

            return returnValue;

        }

        float CalculateAnlgeDegWithX0Y90(Vector3 targetPosLocal)
        {
            float x = targetPosLocal.x;
            float y = targetPosLocal.y;

            float angleDeg = Mathf.Atan2(y, x) * Mathf.Rad2Deg;

            //if (angleDeg > 180) angleDeg -= 360; //Automatically set to -135 and not 225

            return angleDeg;
        }

        float CalculateAnlgeDegWithY0XNeg90(Vector3 targetPosLocal)
        {
            float angleDeg = CalculateAnlgeDegWithX0Y90(targetPosLocal);

            angleDeg -= 90;

            //if (angleDeg > 180) angleDeg -= 360; //Automatically set to -135 and not 225

            return angleDeg;
        }

        public string GetCurrentDebugState()
        {
            string returnString = "";
            returnString += "Rotation controller debug of " + transform.name + ":" + newLine;

            returnString += "Is owner = " + Networking.IsOwner(gameObject) + newLine;
            returnString += "Is held = " + pickupIsHeld + newLine;
            returnString += "Converter position = " + ConverterReference.transform.localPosition + newLine;
            returnString += "currentAngleDeg = " + transform.localRotation.eulerAngles.z + newLine;
            returnString += "outputValue = " + outputValue + newLine;

            return returnString;
        }
    }
}