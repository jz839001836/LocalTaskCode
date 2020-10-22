using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
///
///</summary>
public class SetPosition : MonoBehaviour
{
    Read_F data;
    public int m;       //质量
    public int angle;   //起飞角度
    float time = 0;
    bool down = false; 
    Vector3 position;

    private void Start()
    {
        Initialized();
    }
    private void Update()
    {
        if (!Equal(position.y, 30))
            Set();
    }

    private void Initialized()
    {
        string path;
#if UNITY_EDITOR
        path = UnityEditor.EditorUtility.OpenFilePanel("打开文件", @"E:\data", "txt");
#endif
        data = new Read_F(path);
        data.Read();
    }
    private void Set()
    {
        if (time > 0.5)
        {
            position = this.GetComponent<Transform>().position;
            if (position.y > 180)
                down = true;
            if (down)
                position.y -= 0.5f;
            else
                position.y += 0.5f;
            position.z -= 1;
            this.GetComponent<Transform>().position = position;
        }
        time += Time.deltaTime;
    }
    private bool Equal(float x, float y)
    {
        if (x - y < 1 && x - y > -1)
            return true;
        return false;
    }
}
