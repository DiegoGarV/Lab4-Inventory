using System.Collections.Generic;
using UnityEngine;

public class PowerBoxController : MonoBehaviour
{
    [SerializeField] private List<SecurityCameraController> connectedCameras = new();
    [SerializeField] private bool powerOn = true;

    public bool PowerOn => powerOn;

    private void Start()
    {
        ApplyPowerState();
    }

    public void CutPower()
    {
        if (!powerOn)
            return;

        powerOn = false;
        ApplyPowerState();

        Debug.Log($"Caja de luz {name}: energía cortada.");
    }

    public void RestorePower()
    {
        if (powerOn)
            return;

        powerOn = true;
        ApplyPowerState();

        Debug.Log($"Caja de luz {name}: energía restaurada.");
    }

    private void ApplyPowerState()
    {
        foreach (SecurityCameraController cameraController in connectedCameras)
        {
            if (cameraController != null)
                cameraController.SetPowered(powerOn);
        }
    }
}