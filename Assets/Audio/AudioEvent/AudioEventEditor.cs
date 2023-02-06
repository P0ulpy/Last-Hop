#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AudioEvent), true)]
public class AudioEventEditor : Editor
{
    [SerializeField] private AudioSource _previewerAudioSource;

    public void OnEnable()
    {
        _previewerAudioSource = EditorUtility.CreateGameObjectWithHideFlags(
            "Audio Preview",
            HideFlags.
            HideAndDontSave,typeof(AudioSource)).GetComponent<AudioSource>();
    }

    public void OnDisable()
    {
        DestroyImmediate(_previewerAudioSource.gameObject);
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUI.BeginDisabledGroup(serializedObject.isEditingMultipleObjects);

        if (GUILayout.Button("Preview"))
        {
            ((AudioEvent)target).Play(_previewerAudioSource);
        }

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        if (GUILayout.Button("Set name to file name"))
        {
            ((AudioEvent)target).SetFileNameToDefault();
        }

        EditorGUI.EndDisabledGroup();
    }
}
#endif