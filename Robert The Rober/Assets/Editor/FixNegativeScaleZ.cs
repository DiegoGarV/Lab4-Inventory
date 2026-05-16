using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class FixNegativeScaleZ
{
    [MenuItem("Tools/Fix Negative Scale/Scene: Z -1 to 1")]
    public static void FixSceneNegativeScaleZ()
    {
        Scene scene = SceneManager.GetActiveScene();
        if (!scene.isLoaded)
        {
            Debug.LogWarning("No hay una escena activa cargada.");
            return;
        }

        int changedCount = 0;

        GameObject[] rootObjects = scene.GetRootGameObjects();
        foreach (GameObject root in rootObjects)
        {
            Transform[] allTransforms = root.GetComponentsInChildren<Transform>(true);

            foreach (Transform t in allTransforms)
            {
                Vector3 scale = t.localScale;

                if (Mathf.Approximately(scale.z, -1f))
                {
                    Undo.RecordObject(t, "Fix Negative Scale Z");
                    scale.z = 1f;
                    t.localScale = scale;
                    EditorUtility.SetDirty(t);
                    changedCount++;
                }
            }
        }

        if (changedCount > 0)
        {
            EditorSceneManager.MarkSceneDirty(scene);
        }

        Debug.Log($"Fix Negative Scale Z completado. Objetos modificados: {changedCount}");
    }
}