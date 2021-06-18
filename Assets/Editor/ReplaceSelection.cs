using UnityEngine;
using UnityEditor;

/* 
 * This wizard will replace a selection with an object or prefab.
 * Scene objects will be cloned (destroying their prefab links).
 * Original coding by 'yesfish', nabbed from Unity Forums
 * 'keep parent' added by Dave A (also removed 'rotation' option, using localRotation
 */

public class ReplaceSelection : ScriptableWizard
{
    static GameObject replacement = null;
    static bool keep = false;

    public GameObject ReplacementObject = null;
    public bool KeepOriginals = false;

    [MenuItem("GameObject/-Replace Selection...")]
    static void CreateWizard()
    {
        DisplayWizard(
            "Replace Selection", typeof(ReplaceSelection), "Replace");
    }

    public ReplaceSelection()
    {
        ReplacementObject = replacement;
        KeepOriginals = keep;
    }

    void OnWizardUpdate()
    {
        replacement = ReplacementObject;
        keep = KeepOriginals;
    }

    void OnWizardCreate()
    {
        if (replacement == null)
            return;

        Undo.RecordObjects(Selection.objects, "Replace Selection");

        Transform[] transforms = Selection.GetTransforms(
            SelectionMode.TopLevel | SelectionMode.OnlyUserModifiable);

        foreach (Transform t in transforms)
        {
            GameObject obj;
            var pref = PrefabUtility.GetPrefabAssetType(replacement);

            if (pref == PrefabAssetType.Regular || pref == PrefabAssetType.Model)
            {
                obj = (GameObject)PrefabUtility.InstantiatePrefab(replacement);
            }
            else
            {
                obj = Instantiate(replacement);
            }

            Transform gTransform = obj.transform;
            gTransform.parent = t.parent;
            obj.name = replacement.name;
            gTransform.localPosition = t.localPosition;
            gTransform.localScale = t.localScale;
            gTransform.localRotation = t.localRotation;
        }

        if (!keep)
        {
            foreach (GameObject g in Selection.gameObjects)
            {
                DestroyImmediate(g);
            }
        }
    }
}