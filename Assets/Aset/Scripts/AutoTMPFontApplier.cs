using UnityEngine;
using TMPro;

[ExecuteAlways]
public class AutoTMPFontApplier : MonoBehaviour
{
    [Header("Font Target")]
    public TMP_FontAsset targetFont;

    [Header("Apply Scope")]
    public bool includeInactive = true;

    void Awake()
    {
        ApplyFont();
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        ApplyFont();
    }
#endif

    void ApplyFont()
    {
        if (targetFont == null) return;

        TMP_Text[] texts = GetComponentsInChildren<TMP_Text>(includeInactive);

        foreach (var text in texts)
        {
            if (text.font != targetFont)
            {
                text.font = targetFont;
#if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(text);
#endif
            }
        }
    }
}
