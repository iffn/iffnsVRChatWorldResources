using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class LocalEnableDisableButton : UdonSharpBehaviour
{
    [SerializeField] GameObject[] EnableObjects;
    [SerializeField] GameObject[] DisableObjects;
    [SerializeField] bool DisableWhenClickingAgain = false;

    public override void Interact()
    {
        if (EnableObjects.Length != 0)
        {
            if (DisableWhenClickingAgain)
            {
                SetObjects(elements: EnableObjects, newState: !EnableObjects[0].activeSelf);
            }
            else
            {
                SetObjects(elements: EnableObjects, newState: true);
            }
        }

        SetObjects(elements: DisableObjects, newState: false);
    }

    void SetObjects(GameObject[] elements, bool newState)
    {
        foreach (GameObject element in elements)
        {
            element.SetActive(newState);
        }
    }
}