#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

using UnityEngine;

[ExecuteAlways]
public class EntityID : MonoBehaviour
{
    [SerializeField] private string id;

    public string ID => id;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (Application.isPlaying) return;

        if (string.IsNullOrEmpty(id))
        {
            GenerateID();
        }
        else
        {
            EnsureUnique();
        }
    }

    private void Awake()
    {
        if (!Application.isPlaying)
        {
            if (string.IsNullOrEmpty(id))
            {
                GenerateID();
            }
            else
            {
                EnsureUnique();
            }
        }
    }

    private void GenerateID()
    {
        id = System.Guid.NewGuid().ToString();
        EditorUtility.SetDirty(this);

        if (gameObject.scene.IsValid())
        {
            EditorSceneManager.MarkSceneDirty(gameObject.scene);
        }
    }

    private void EnsureUnique()
    {
        EntityID[] objects = FindObjectsByType<EntityID>(FindObjectsSortMode.None);

        foreach (EntityID obj in objects)
        {
            if (obj != this && obj.id == this.id)
            {
                GenerateID();
                break;
            }
        }
    }
#endif
}