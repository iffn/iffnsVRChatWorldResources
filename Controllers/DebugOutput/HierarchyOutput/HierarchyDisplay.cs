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

        //newLine = backslash n which is interpreted as a new line when showing the code in a text field
        string newLine = "\n";

        void Update()
        {
            HierarchyOutput.text = "Output at " + Time.time + newLine + GetHierarchyOutput(ParentOfHierarchyOutput, "");
        }


        [RecursiveMethod]
        string GetHierarchyOutput(Transform parent, string startString)
        {
            string returnString = "";

            if (OnlyOutputIfNameStartsWith.Length == 0)
            {
                returnString += startString + parent.name + newLine;
            }
            else
            {
                for (int i = 0; i < OnlyOutputIfNameStartsWith.Length; i++)
                {
                    if (parent.name.StartsWith(OnlyOutputIfNameStartsWith[i]))
                    {
                        returnString += startString + parent.name + newLine;
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