using UnityEngine;

public class LockpickItem : StoreItemBase
{
    [Range(0f, 1f)]
    [SerializeField] private float successChance = 0.65f;

    protected override void ApplyEffect()
    {
        float roll = Random.value;

        if (roll <= successChance)
        {
            Debug.Log("Ganzúa exitosa: la puerta se abrió.");
            // TODO: lógica para que la probabilidad varie según el tipo de casa
        }
        else
        {
            Debug.Log("La ganzúa se rompió o falló.");
        }
    }

    public override void ApplyLevelStartEffect()
    {
        Debug.Log("Ganzúa activa: intentar abrir una puerta.");
        // TODO: lógica para guardar que ya se tiene en el ProgressManager
    }

    protected override void OnConsumedCompletely()
    {
        Debug.Log("Ya no quedan ganzúas.");
    }
}