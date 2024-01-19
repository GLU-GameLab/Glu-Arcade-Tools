using ArcadeLauncher.Models;
using System;
using System.IO;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;
using UnityEngine.UIElements;

public class ManifestEditor : EditorWindow
{
    [MenuItem("Tools/Gamelab/Manifest Editor")]
    public static void ShowExample()
    {
        ManifestEditor wnd = GetWindow<ManifestEditor>();
        wnd.titleContent = new GUIContent("Gamelab Manifest Editor");
        wnd.saveChangesMessage = "The Manifest editor has unsaved changes. Would you like to save?";
    }

    GameManifest manifest => ManifestFile.current;

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;
        
        // VisualElements objects can contain other VisualElement following a tree hierarchy
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.gamelab.gamelab-arcade-tools/Editor/ManifestEditor.uxml");
        VisualElement labelFromUXML = visualTree.Instantiate();
        root.Add(labelFromUXML);

       
        SimpleBind<string, TextField>(nameof(manifest.Name), () => manifest.Name, (x) => manifest.Name = x);
        SimpleBind<string, TextField>(nameof(manifest.Description), () => manifest.Description, (x) => manifest.Description = x);
    }

    public void Save()
    {

        ManifestFile.Save();
        hasUnsavedChanges = false;
    }


    void SimpleBind<TValue, TField>(string fieldname, Func<TValue> get, Action<TValue> set) where TField : BaseField<TValue>
    {
        var field = rootVisualElement.Q<TField>(fieldname);
        field.value = get();
        field.RegisterValueChangedCallback((x) =>
        {
            hasUnsavedChanges = true;
            set(x.newValue);
        });

    }
   
    public override void SaveChanges()
    {
        Save();
        base.SaveChanges();
    }
}
