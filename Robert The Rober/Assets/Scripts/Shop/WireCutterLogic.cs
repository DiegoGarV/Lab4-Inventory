using UnityEngine;

public class WireCutterLogic : StoreItemLogicBase
{
    public override void ApplyLevelStartEffect()
    {
        Debug.Log("WireCutterLogic: corta cables disponibles para usar.");
    }

    public override bool Use(RaycastHit hit)
    {
        PowerBoxController powerBox = hit.collider.GetComponentInParent<PowerBoxController>();

        if (powerBox == null)
            return false;

        if (!powerBox.PowerOn)
        {
            Debug.Log("WireCutterLogic: esa caja ya no tiene energía.");
            return true;
        }

        powerBox.CutPower();
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayWireCutterUse();
        }
        return true;
    }

    public override bool CanUseOn(RaycastHit hit)
    {
        PowerBoxController powerBox = hit.collider.GetComponentInParent<PowerBoxController>();
        return powerBox != null && powerBox.PowerOn;
    }
}