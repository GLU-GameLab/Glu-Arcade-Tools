using ArcadeLauncher.Models;
using System;
using UnityEditor;
using UnityEditor.UIElements;
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



    private GameManifest manifest => ManifestFile.current;

    private const string AutosaveKey = "GamelabManifestEditorAutoSave";

    private bool autoSave
    {
        get
        {
            return EditorPrefs.GetBool(AutosaveKey, false);
        }
        set
        {
            EditorPrefs.SetBool(AutosaveKey, value);
        }
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // VisualElements objects can contain other VisualElement following a tree hierarchy
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.gamelab.gamelab-arcade-tools/Editor/Manifest/ManifestEditor.uxml");
        VisualElement labelFromUXML = visualTree.Instantiate();
        root.Add(labelFromUXML);

        // save button
        var savebutton = root.Q<ToolbarButton>("SaveButton");
        savebutton.clicked += Save;



        // autosave button
        var AutoSaveToggle = root.Q<ToolbarToggle>("AutoSaveToggle");
        AutoSaveToggle.value = autoSave;
        AutoSaveToggle.RegisterValueChangedCallback((x) =>
        {
            // disable the save button when autosave is enabled
            savebutton.style.display = (x.newValue) ? DisplayStyle.None : DisplayStyle.Flex;
            autoSave = x.newValue;
            if (x.newValue)
            {
                Save();
            }
        });

        try
        {
            StartBinding();
        }
        catch
        {
            ManifestFile.CreateNewManifest();
            StartBinding();
        }

    }

    void StartBinding()
    {
        // binding of string
        SimpleBind<string, TextField>(nameof(manifest.Name), () => manifest.Name, (x) => manifest.Name = x.newValue);
        SimpleBind<string, TextField>(nameof(manifest.Description), () => manifest.Description, (x) => manifest.Description = x.newValue);

        // binding of the background color
        // the color needs to be converted to comply with the manifest standard
        SimpleBind<Color, ColorField>(nameof(manifest.BackgroundColor), () => { ColorUtility.TryParseHtmlString(manifest.BackgroundColor, out var color); return color; }, (x) => manifest.BackgroundColor = ColorUtility.ToHtmlStringRGB(x.newValue));

        // binding of the players 
        // the index needs to be offset by one when written or read to comply with the manifest standard
        SimpleBind<string, DropdownField>(nameof(manifest.PlayersNeeded), (x) => x.choices[manifest.PlayersNeeded - 1], (x) => manifest.PlayersNeeded = (x.currentTarget as DropdownField).index + 1);

        // binding of the icon that will be provided in the build path
        SimpleBind<UnityEngine.Object, ObjectField>(nameof(manifest.IconPath),
            () => // getter
            {

                if (!string.IsNullOrEmpty(manifest.IconPath))
                    return AssetDatabase.LoadAssetAtPath<Texture2D>(manifest.IconPath);
                return AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/com.gamelab.gamelab-arcade-tools/Editor/Default_icon.png");

            },
            (x) => // setter
            {
                if (x.newValue)
                    manifest.IconPath = AssetDatabase.GetAssetPath(x.newValue);
                else
                    manifest.IconPath = string.Empty;
            });

        // set up the author list
        var list = rootVisualElement.Q<ListView>();

        Func<VisualElement> makeItem = () => new TextField();

        // As the user scrolls through the list, the ListView object
        // recycles elements created by the "makeItem" function,
        // and invoke the "bindItem" callback to associate
        // the element with the matching data item (specified as an index in the list).
        Action<VisualElement, int> bindItem = (e, i) =>
        {

            var textfield = e as TextField;
            textfield.value = manifest.Authors[i];
            SetCallback<string, TextField>((x) => manifest.Authors[i] = x.newValue, textfield);
        };

        // set the source and actions
        list.itemsSource = manifest.Authors;
        list.makeItem = makeItem;
        list.bindItem = bindItem;
    }

    public void Save()
    {

        ManifestFile.Save();
        hasUnsavedChanges = false;
    }

    /// <summary>
    /// binds a basefield to a parameter using delegades
    /// </summary>
    void SimpleBind<TValue, TField>(string fieldname, Func<TValue> get, Action<ChangeEvent<TValue>> set) where TField : BaseField<TValue>
    {
        var field = rootVisualElement.Q<TField>(fieldname);
        field.value = get();
        SetCallback(set, field);
    }
    /// <summary>
    /// binds a basefield to a parameter using delegades
    /// </summary>
    void SimpleBind<TValue, TField>(string fieldname, Func<TField, TValue> get, Action<ChangeEvent<TValue>> set) where TField : BaseField<TValue>
    {
        var field = rootVisualElement.Q<TField>(fieldname);
        field.value = get(field);
        SetCallback(set, field);
    }
    /// <summary>
    /// creates a update callback to save the changes made to the manifest
    /// </summary>
    private void SetCallback<TValue, TField>(Action<ChangeEvent<TValue>> set, TField field) where TField : BaseField<TValue>
    {
        field.RegisterValueChangedCallback((x) =>
        {
            set(x);
            hasUnsavedChanges = true;
            if(autoSave)
            Save();
        });
    }

    /// <summary>
    /// save when the editor is closed while unsavedchanges == true
    /// </summary>
    public override void SaveChanges()
    {
        Save();
        base.SaveChanges();
    }
}
