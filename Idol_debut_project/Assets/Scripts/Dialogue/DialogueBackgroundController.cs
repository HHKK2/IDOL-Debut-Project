using System;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using Image = UnityEngine.UI.Image;

public class DialogueBackgroundController : MonoBehaviour
{
    [System.Serializable]
    public struct BgEntry
    {
        public string key;
        public Sprite sprite;
    }

    public Image targetImage;
    public List<BgEntry> backgrounds;

    private Dictionary<string, Sprite> bgMap;

    private void Awake()
    {
        bgMap = new Dictionary<string, Sprite>();
        foreach (var e in backgrounds)
        {
            if (!string.IsNullOrEmpty(e.key) && e.sprite != null)
            {
                bgMap[e.key] = e.sprite;
            }
        }
    }

    public void SetBackground(string key)
    {
        if (bgMap == null) return;

        if (bgMap.TryGetValue(key, out var sprite))
        {
            targetImage.sprite = sprite;
            targetImage.enabled = (sprite != null);
        }
        else
        {
            Debug.LogWarning($"[BG] key '{key}' 를 찾을 수 없음 ");
        }
    }
}
