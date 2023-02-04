using UnityEngine;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif
public abstract class AudioEvent : ScriptableObject
{
    public string soundName;
    public abstract void Play(AudioSource source);

    public void SetFileNameToDefault()
    {
#if UNITY_EDITOR
        soundName = Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(GetInstanceID()));
#endif
    }
}
