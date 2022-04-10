using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace iffnsStuff.iffnsVRCStuff.DebugOutput
{
    public class HierarchyDisplay : UdonSharpBehaviour
    {
        [SerializeField] TMPro.TextMeshProUGUI HierarchyOutput;
        [SerializeField] Transform ParentOfHierarchyOutput;
        [SerializeField] string[] OnlyOutputIfNameStartsWith;
        [SerializeField] bool ShowLocalLocation = true;
        [SerializeField] float TimeBetweenUpdates = 1;

        //newLine = backslash n which is interpreted as a new line when showing the code in a text field
        string newLine = "\n";

        float lastUpdateTime = 0;

        void Update()
        {
            if(Time.time - lastUpdateTime > TimeBetweenUpdates)
            {
                HierarchyOutput.text = "Output at " + Time.time + newLine + GetHierarchyOutput(ParentOfHierarchyOutput, "");
                lastUpdateTime = Time.time;
            }
        }

        [RecursiveMethod]
        string GetHierarchyOutput(Transform parent, string startString)
        {
            string returnString = "";

            if (OnlyOutputIfNameStartsWith.Length == 0)
            {
                if (ShowLocalLocation) returnString += startString + parent.name + " " + parent.localPosition + " " + parent.localRotation.eulerAngles + " " + parent.localScale + newLine;
                else returnString += startString + parent.name + newLine;
            }
            else
            {
                for (int i = 0; i < OnlyOutputIfNameStartsWith.Length; i++)
                {
                    if (parent.name.StartsWith(OnlyOutputIfNameStartsWith[i]))
                    {
                        if(ShowLocalLocation) returnString += startString + parent.name + " " + parent.localPosition + " " + parent.localRotation.eulerAngles + " " + parent.localScale + newLine;
                        else returnString += startString + parent.name + newLine;
                        break;
                    }
                }
            }

            for (int i = 0; i < parent.childCount; i++)
            {
                Transform child = parent.GetChild(i);

                returnString += GetHierarchyOutput(parent: child, startString: startString + "- ");
            }

            return returnString;
        }
    }
}