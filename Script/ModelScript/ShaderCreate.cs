/*
脚本名称：
脚本作者：
建立时间：
脚本功能：
版本号：
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ShaderCreate : MonoBehaviour
{
    public Material materialTemper;
    public Material materialPress;
    public Material UCLAGameLabWireframe;
    public Material materialPropellant_Transparency;
    public float max, min;
    void Start()
    {
        //max = MeshCreate.maxTemper;
        //min = MeshCreate.minTemper;
    }

    void Update()
    {

    }
    public void GetTemperShader()
    {
        GetComponent<Renderer>().material = materialTemper;
    }
    public void GetPressShader()
    {
        GetComponent<Renderer>().material = materialPress;
    }
    public void GetMeshShader()
    {
        GetComponent<Renderer>().material = UCLAGameLabWireframe;
    }
    public void GetPro_Transparency()
    {
        GetComponent<Renderer>().material = materialPropellant_Transparency;
    }
    public void SwitchScene()
    {
        SceneManager.LoadScene("Plane");
    }
}
