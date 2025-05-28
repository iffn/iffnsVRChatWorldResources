
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Teleporter : UdonSharpBehaviour
{
    [SerializeField] Transform TargetTransform;
    [SerializeField] bool RotatePlayer;

    public override void Interact()
    {
        if (RotatePlayer)
        {
            Networking.LocalPlayer.TeleportTo(
                teleportPos: TargetTransform.position,
                TargetTransform.rotation);
        }
        else
        {
            Networking.LocalPlayer.TeleportTo(
                teleportPos: TargetTransform.position,
                Networking.LocalPlayer.GetRotation());
        }
    }
}
