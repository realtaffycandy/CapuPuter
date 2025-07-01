using UnityEngine;
using MelonLoader;
using System;

[RegisterTypeInIl2Cpp(true)]
public class MenuButtonKey : MonoBehaviour
{
    public MenuButtonKey(IntPtr ptr) : base(ptr) {}
    
    public static float cooldown;
    public int buttonType;

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("FingerTip") && Time.time > cooldown)
        {
            cooldown = Time.time + .1f;
            MenuHandler.instance.HandleButton((ButtonKeyType)buttonType);
            UnityEngine.XR.InputDevices.GetDeviceAtXRNode(UnityEngine.XR.XRNode.RightHand).SendHapticImpulse(0u, .1f, .05f);
        }
    }
}
