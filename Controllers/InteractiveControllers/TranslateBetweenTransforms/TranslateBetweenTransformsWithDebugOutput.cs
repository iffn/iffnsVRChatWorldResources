using iffnsStuff.iffnsVRCStuff.DebugOutput;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace iffnsStuff.iffnsVRCStuff.InteractiveControllers
{
    public class TranslateBetweenTransformsWithDebugOutput : UdonSharpBehaviour
    {
        [SerializeField] Transform MovingTransform;
        [SerializeField] Transform TargetTransform;
        [SerializeField] float transitionTimeInSeconds = 1;
        [SerializeField] SingleScriptDebugState LinkedStateOutput;

        /*
        enum transitionStates
        {
            atOriginPosition,       // 0
            movingToTargetPosition, // 1
            atTargetPosition,       // 2
            movingToOriginPosition  // 3
        }
        */

        [UdonSynced] bool atStart = true;
        int localTransitionState = 0;

        //Runtime parameters
        float startTime;

        //Preset parameters
        Vector3 originalLocalPosition;
        Quaternion originalLocalRotation;
        Vector3 originalLocalScale;
        bool move;
        bool rotate;
        bool scale;

        string newLine = "\n";

        void Start()
        {
            originalLocalPosition = MovingTransform.localPosition;
            originalLocalRotation = MovingTransform.localRotation;
            originalLocalScale = MovingTransform.localScale;

            move = (TargetTransform.localPosition - originalLocalPosition).magnitude > 0.001f;
            rotate = Quaternion.Angle(TargetTransform.localRotation, originalLocalRotation) > 0.01f;
            scale = (TargetTransform.localPosition - originalLocalPosition).magnitude > 0.001f;

            //Sync -> Not working for late joiners since atStart is not synced when Start is run?
            if (atStart) localTransitionState = 0;
            else localTransitionState = 2;

            if (localTransitionState == 2)
            {
                MovingTransform.localPosition = TargetTransform.localPosition;
                MovingTransform.localRotation = TargetTransform.localRotation;
                MovingTransform.localScale = TargetTransform.localScale;
            }
            else
            {
                MovingTransform.localPosition = originalLocalPosition;
                MovingTransform.localRotation = originalLocalRotation;
                MovingTransform.localScale = originalLocalScale;
            }
        }

        private void Update()
        {
            float betweenState = 0;

            switch (localTransitionState)
            {
                case 0: // atOriginPosition
                    if (!atStart)
                    {
                        startTime = Time.time;
                        localTransitionState = 1;
                    }
                    break;

                case 1: // movingToTargetPosition
                    if (Time.time - startTime > transitionTimeInSeconds)
                    {
                        //Move complete
                        localTransitionState = 2;

                        MovingTransform.localPosition = TargetTransform.localPosition;
                        MovingTransform.localRotation = TargetTransform.localRotation;
                        MovingTransform.localScale = TargetTransform.localScale;
                    }
                    else
                    {
                        //Move
                        betweenState = (Time.time - startTime) / transitionTimeInSeconds;

                        if (move) MovingTransform.localPosition = Vector3.Lerp(a: originalLocalPosition, b: TargetTransform.localPosition, t: betweenState);
                        if (rotate) MovingTransform.localRotation = Quaternion.Lerp(a: originalLocalRotation, b: TargetTransform.localRotation, t: betweenState);
                        if (scale) MovingTransform.localScale = Vector3.Lerp(a: originalLocalScale, b: TargetTransform.localScale, t: betweenState);
                    }
                    break;

                case 2: // atTargetPosition
                    if (atStart)
                    {
                        startTime = Time.time;
                        localTransitionState = 3;
                    }
                    break;

                case 3: // movingToOriginPosition
                    if (Time.time - startTime > transitionTimeInSeconds)
                    {
                        //Move complete
                        localTransitionState = 0;

                        MovingTransform.localPosition = originalLocalPosition;
                        MovingTransform.localRotation = originalLocalRotation;
                        MovingTransform.localScale = originalLocalScale;
                    }
                    else
                    {
                        //Move
                        betweenState = (Time.time - startTime) / transitionTimeInSeconds;
                        if (move) MovingTransform.localPosition = Vector3.Lerp(a: TargetTransform.localPosition, b: originalLocalPosition, t: betweenState);
                        if (rotate) MovingTransform.localRotation = Quaternion.Lerp(a: TargetTransform.localRotation, b: originalLocalRotation, t: betweenState);
                        if (scale) MovingTransform.localScale = Vector3.Lerp(a: TargetTransform.localScale, b: originalLocalScale, t: betweenState);
                    }
                    break;
            }

            PrepareDebugState();
        }

        public override void Interact()
        {
            //base.Interact();

            if (localTransitionState == 0 || localTransitionState == 2)
            {
                Networking.SetOwner(Networking.LocalPlayer, gameObject);
                atStart = !atStart;
                RequestSerialization();
            }
        }

        public void PrepareDebugState()
        {
            if (LinkedStateOutput == null) return;

            if (!LinkedStateOutput.IsReadyForOutput()) return;

            string name = "TranslateBetweenTransforms";

            string currentState = "";

            currentState += "Settings:" + newLine;
            currentState += "Move: " + move + newLine;
            currentState += "Rotate: " + rotate + newLine;
            currentState += "Scale: " + scale + newLine;

            currentState += newLine;

            currentState += "Runtime varibles:" + newLine;
            currentState += "Local transition state = " + localTransitionState + newLine;
            currentState += "Synced atStart state = " + atStart + newLine;
            currentState += "Start time = " + startTime + newLine;

            LinkedStateOutput.SetCurrentState(displayName: name, currentState: currentState);
        }
    }
}