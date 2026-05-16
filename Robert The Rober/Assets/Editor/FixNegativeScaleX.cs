using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class FixNegativeScaleX
{
    [MenuItem("Tools/Fix Negative Scale/Scene: X -1 to 1")]
    public static void FixSceneNegativeScaleX()
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

                if (Mathf.Approximately(scale.x, -1f))
                {
                    Undo.RecordObject(t, "Fix Negative Scale X");
                    scale.x = 1f;
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

        Debug.Log($"Fix Negative Scale X completado. Objetos modificados: {changedCount}");
    }
}