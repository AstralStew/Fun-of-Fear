// Cristian Pop - https://boxophobic.com/

using UnityEngine;
using UnityEditor;
using Boxophobic.StyledGUI;
using Boxophobic.Utils;
using System.IO;

public class HeightFogHub : EditorWindow
{
    string folderAsset = "Assets/BOXOPHOBIC/Atmospheric Height Fog";

    string[] packagePaths;
    string[] packageOptions;

    string packagesPath;
    int packageIndex;

    GUIStyle stylePopup;

    Color bannerColor;
    string bannerText;
    string helpURL;
    static HeightFogHub window;
    Vector2 scrollPosition = Vector2.zero;

    [MenuItem("Window/BOXOPHOBIC/Atmospheric Height Fog/Hub")]
    public static void ShowWindow()
    {
        window = GetWindow<HeightFogHub>(false, "Atmospheric Height Fog", true);
        window.minSize = new Vector2(389, 220);
    }

    void OnEnable()
    {
        bannerColor = new Color(0.474f, 0.709f, 0.901f);
        bannerText = "Atmospheric Height Fog";
        helpURL = "https://docs.google.com/document/d/1pIzIHIZ-cSh2ykODSZCbAPtScJ4Jpuu7lS3rNEHCLbc/edit#heading=h.hbq3w8ae720x";

        //Safer search, there might be many user folders
        string[] searchFolders;

        searchFolders = AssetDatabase.FindAssets("Atmospheric Height Fog");

        for (int i = 0; i < searchFolders.Length; i++)
        {
            if (AssetDatabase.GUIDToAssetPath(searchFolders[i]).EndsWith("Atmospheric Height Fog.pdf"))
            {
                folderAsset = AssetDatabase.GUIDToAssetPath(searchFolders[i]);
                folderAsset = folderAsset.Replace("/Atmospheric Height Fog.pdf", "");
            }
        }

        packagesPath = folderAsset + "/Core/Packages";

        GetPackages();
    }

    void OnGUI()
    {
        SetGUIStyles();

        StyledGUI.DrawWindowBanner(bannerColor, bannerText, helpURL);

        GUILayout.BeginHorizontal();
        GUILayout.Space(15);

        GUILayout.BeginVertical();

        //scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false, GUILayout.Width(this.position.width - 28), GUILayout.Height(this.position.height - 80));

        DrawInstallMessage();

        if (packageOptions[packageIndex].Contains("Universal 7.1.8"))
        {
            EditorGUILayout.HelpBox("For Universal 7.1.8+ Pipeline, Depth Texture and one of the following features need to be enabled for the depth to work properly: Opaque Texure, HDR or Post Processing!", MessageType.Info, true);
        }

        if (packageOptions[packageIndex].Contains("Universal 7.4.1"))
        {
            EditorGUILayout.HelpBox("For Universal 7.4.1+ Pipeline, Depth Texture need to be enabled on the render pipeline asset!", MessageType.Info, true);
        }

        DrawRenderPipelineSelection();
        DrawSetupButton();

        //GUILayout.EndScrollView();

        GUILayout.EndVertical();

        GUILayout.Space(13);
        GUILayout.EndHorizontal();
    }

    void SetGUIStyles()
    {
        stylePopup = new GUIStyle(EditorStyles.popup)
        {
            alignment = TextAnchor.MiddleCenter
        };
    }

    void DrawInstallMessage()
    {
        EditorGUILayout.HelpBox("Click the Install Render Pipeline to switch to another render pipeline. For Universal Render Pipeline, follow the instructions below to enable the fog rendering!", MessageType.Info, true);
    }

    void DrawRenderPipelineSelection()
    {
        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(new GUIContent("Render Pipeline", ""));
        packageIndex = EditorGUILayout.Popup(packageIndex, packageOptions, stylePopup);
        GUILayout.EndHorizontal();
    }

    void DrawSetupButton()
    {
        GUILayout.Space(10);

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Install " + packageOptions[packageIndex] + " Pipeline"))
        {
            ImportPackage();

            GUIUtility.ExitGUI();
        }

        GUILayout.EndHorizontal();
    }

    void GetPackages()
    {
        packagePaths = Directory.GetFiles(packagesPath, "*.unitypackage", SearchOption.TopDirectoryOnly);

        packageOptions = new string[packagePaths.Length];

        for (int i = 0; i < packageOptions.Length; i++)
        {
            packageOptions[i] = Path.GetFileNameWithoutExtension(packagePaths[i].Replace("Built-in Pipeline", "Standard"));
        }
    }

    void ImportPackage()
    {
        AssetDatabase.ImportPackage(packagePaths[packageIndex], false);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("[Atmospheric Height Fog] " + packageOptions[packageIndex] + " package imported into your project!");
    }
}

