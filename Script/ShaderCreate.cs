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

public class ShaderCreate : MonoBehaviour
{
    public Material materialTemper;
    public Material materialPress;
    void Start()
    {

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
}
