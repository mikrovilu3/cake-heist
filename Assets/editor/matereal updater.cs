using UnityEditor;
using UnityEngine;

public class MaterialUpdater : EditorWindow
{
    string shaderName = "HDRP/Lit";  // Change this to "Universal Render Pipeline/Lit" for URP

    [MenuItem("Tools/Update All Materials Shader")]
    static void ShowWindow()
    {
        GetWindow<MaterialUpdater>("Material Updater");
    }

    void OnGUI()
    {
        GUILayout.Label("Update All Materials", EditorStyles.boldLabel);

        shaderName = EditorGUILayout.TextField("Target Shader", shaderName);

        if (GUILayout.Button("Update Materials"))
        {
            UpdateMaterials(shaderName);
        }
    }

    static void UpdateMaterials(string shaderPath)
    {
        Shader targetShader = Shader.Find(shaderPath);
        if (targetShader == null)
        {
            Debug.LogError($"Shader '{shaderPath}' not found!");
            return;
        }

        string[] guids = AssetDatabase.FindAssets("t:Material");
        int updatedCount = 0;

        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(assetPath);
            if (mat != null && mat.shader != targetShader)
            {
                mat.shader = targetShader;
                EditorUtility.SetDirty(mat);
                updatedCount++;
            }
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"Updated {updatedCount} materials to shader '{shaderPath}'.");
    }
}
