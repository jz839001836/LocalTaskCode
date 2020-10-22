using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

///<summary>
///滑块控制物体透明度
///</summary>
public class Transparency_Hull : MonoBehaviour
{
    public Slider sliderhull;
    Renderer rendHull;
    static float alphaHull;
    GameObject hull;
    public static float AlphaHull()
    {
        return alphaHull;
    }
    private void Start()
    {
        rendHull = GetComponent<Renderer>();
        rendHull.material.shader = Shader.Find("Unlit/Transparency_Hull");
        hull = this.gameObject;
    }
    private void Update()
    {
        HullAlphaScaleSet();
        if (alphaHull < 0.1)
            hull.SetActive(false);
    }
    public  void HullAlphaScaleSet()
    {
        alphaHull = sliderhull.value;
        rendHull.material.SetFloat("_AlphaScale", alphaHull);
    }
}
