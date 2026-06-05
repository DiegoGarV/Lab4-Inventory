using UnityEngine;
using UnityEngine.AI;

public class MeatLogic : StoreItemLogicBase
{
    [Header("Throw Settings")]
    [SerializeField] private GameObject thrownMeatPrefab;
    [SerializeField] private float throwDistance = 2f;
    [SerializeField] private float spawnHeight = 0.5f;
    [SerializeField] private float groundCheckDistance = 5f;
    [SerializeField] private LayerMask groundMask;

    public override void ApplyLevelStartEffect()
    {
        Debug.Log("MeatLogic: carne disponible para usar.");
    }

    public override bool Use(RaycastHit hit)
    {
        DogController dog = hit.collider.GetComponentInParent<DogController>();

        if (dog == null)
            return false;

        if (PlayerProgressManager.Instance == null)
            return true;

        int meatCount = PlayerProgressManager.Instance.GetItemQuantity(ItemId);

        if (meatCount <= 0)
        {
            Debug.Log("No tienes carne.");
            return true;
        }

        bool consumed = PlayerProgressManager.Instance.ConsumeItem(ItemId);

        if (!consumed)
        {
            Debug.Log("No se pudo consumir la carne.");
            return true;
        }

        Transform cameraTransform = Camera.main != null ? Camera.main.transform : null;

        if (cameraTransform == null)
        {
            Debug.LogWarning("No se encontró Camera.main para lanzar la carne.");
            return true;
        }

        Vector3 spawnPosition = cameraTransform.position + cameraTransform.forward * throwDistance;
        spawnPosition.y += spawnHeight;

        if (Physics.Raycast(spawnPosition, Vector3.down, out RaycastHit groundHit, groundCheckDistance, groundMask))
        {
            spawnPosition = groundHit.point;
        }
        else
        {
            NavMeshHit navHit;
            if (NavMesh.SamplePosition(spawnPosition, out navHit, 2f, NavMesh.AllAreas))
            {
                spawnPosition = navHit.position;
            }
        }

        GameObject meatInstance = null;

        if (thrownMeatPrefab != null)
        {
            meatInstance = Instantiate(thrownMeatPrefab, spawnPosition, Quaternion.identity);
        }
        else
        {
            meatInstance = new GameObject("ThrownMeat");
            meatInstance.transform.position = spawnPosition;
            meatInstance.AddComponent<ThrownMeat>();
        }

        dog.DistractWithMeat(meatInstance.transform);

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayThrowMeat();
        }

        return true;
    }

    public override bool CanUseOn(RaycastHit hit)
    {
        DogController dog = hit.collider.GetComponentInParent<DogController>();
        return dog != null;
    }
}