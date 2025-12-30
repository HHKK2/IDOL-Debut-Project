using System;
using UnityEngine;

/// <summary>
/// 강의링크: https://www.youtube.com/watch?v=A1kPszkvl44
/// </summary>
public class MouseEffect : AdolpSingleton<MouseEffect>
{
    public GameObject starEffectPrefab;
    private float spawnsTime=0f;
    public float defaultTime = 0.05f;
    
    private void Update()
    {
        if (Input.GetMouseButton(0)&&spawnsTime >= defaultTime)
        {
            StarCreate();
            spawnsTime = 0f;
        }
        spawnsTime += Time.deltaTime;
    }

    void StarCreate()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = -Camera.main.transform.position.z; 
    
        Vector3 mPosition = Camera.main.ScreenToWorldPoint(mousePos);
        Instantiate(starEffectPrefab, mPosition, Quaternion.identity);
    }
}
