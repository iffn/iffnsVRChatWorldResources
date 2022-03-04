using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace iffnsStuff.iffnsVRCStuff.DebugOutput
{
    public class ScriptDebugDisplay : UdonSharpBehaviour
    {
        [SerializeField] SingleScriptDebugState SingleScriptState;
        [SerializeField] TMPro.TextMeshProUGUI TitleText;
        [SerializeField] TMPro.TextMeshProUGUI OutputTextField;


        private void Update()
        {
            if (SingleScriptState == null) return;

            if (!SingleScriptState.IsNextStateAvailable()) return;

            TitleText.text = "Output of " + SingleScriptState.GetCurrentName() + ":";
            OutputTextField.text = SingleScriptState.GetCurrentState();
        }
    }
}