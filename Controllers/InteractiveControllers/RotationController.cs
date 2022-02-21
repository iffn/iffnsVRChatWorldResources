using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace iffnsStuff.iffnsVRCStuff.InteractiveControllers
{
    public class RotationController : UdonSharpBehaviour
    {

        [Header("Implementation stuff \n test")]
        //[Header("- Local position and rotation of Pickup needs to be (0, 0, 0)")]
        [SerializeField] GameObject[] OwnershipObjects;
        [SerializeField] VRC_Pickup LinkedPickupWithXYOffset;
        [SerializeField] Transform ConverterReference;
        [SerializeField] float MaxAngleDeg = 45;
        [SerializeField] bool Symetric = true;
        [SerializeField] float SnapAngleDegOffsetFromZero = 0;

        [SerializeField] TMPro.TextMeshProUGUI InfoBox;

        //newLine = backslash n which is interpreted as a new line when showing the code in a text field
        string newLine = "\n";

        bool pickupIsHeld;
        Vector3 OriginalLocalPickupPosition;
        Quaternion OriginalLocalPickupRotation;

        float outputValue = 0;
        public float GetOutputValue()
        {
            return outputValue;
        }

        void Start()
        {

        }

        void Update()
        {
            string outputText = "";

            //Set ownership on pickup
            if (!pickupIsHeld && LinkedPickupWithXYOffset.IsHeld)
            {
                if (LinkedPickupWithXYOffset.currentPlayer == Networking.LocalPlayer)
                {
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

            if (pickupIsHeld)
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
                SetOutputValueFromAngle(angleDeg: currentAngleDeg);

                //Set rotation
                transform.localRotation = Quaternion.Euler(Vector3.forward * currentAngleDeg);

                outputText += "Converter position = " + ConverterReference.transform.localPosition + newLine;
                outputText += "currentAngleDeg = " + currentAngleDeg + newLine;
                outputText += "outputValue = " + outputValue + newLine;
            }

            //Reset pickup position
            LinkedPickupWithXYOffset.transform.localPosition = Vector3.zero;
            LinkedPickupWithXYOffset.transform.localRotation = Quaternion.identity;

            if (InfoBox != null) InfoBox.text = outputText;
        }

        void SetOutputValueFromAngle(float angleDeg)
        {
            if (Symetric)
            {
                float tempValue = Mathf.InverseLerp(a: -MaxAngleDeg, MaxAngleDeg, angleDeg);
                outputValue = Mathf.Lerp(a: -1, 1, tempValue);
            }
            else
            {
                outputValue = Mathf.InverseLerp(a: 0, MaxAngleDeg, angleDeg);
            }
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
    }
}