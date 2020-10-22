using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

///<summary>
///
///</summary>
public class Transparency_Propellant : MonoBehaviour
{
    public Slider sliderPropellant;
    Renderer rendPropellant;
    public float alphaPropellant;
    private void Start()
    {
        rendPropellant = GetComponent<Renderer>();
        rendPropellant.material.shader = Shader.Find("Unlit/Transparency_Propellant");
    }
    private void Update()
    {
        PropellantAlphaSet();
        SetActiveHull();
    }
    public void PropellantAlphaSet()
    {
        alphaPropellant = sliderPropellant.value;
        rendPropellant.material.SetFloat("_AlphaScale", alphaPropellant);
    }
    private void SetActiveHull()
    {
        if (Transparency_Hull.AlphaHull() > 0.1)
        {
            GameObject ParentObject = GameObject.Find("GameObject");
            GameObject hull = ParentObject.transform.Find("火箭弹装配").gameObject;
            hull.SetActive(true);
        }
    }
}
