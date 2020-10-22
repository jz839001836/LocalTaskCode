using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
///
///</summary>
public class Ex : MonoBehaviour
{
    private LineRenderer line;
    Vector3[] pos = new Vector3[4];

    private float startTime;
    private float curTime;

    void Start()
    {
        // 线段上三个点的位置
        pos[0] = new Vector3(0, 0, 1);
        pos[1] = new Vector3(0, 1, 0);
        pos[2] = new Vector3(1, 0, 0);
        pos[3] = new Vector3(2, 1, 1);


        // 为当前物体添加  LineRenderer 组件。
        line = gameObject.AddComponent<LineRenderer>();
        // 设置材料的属性
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.positionCount = 4; //　设置该线段由几个点组成

        // 设置线段的起点颜色和终点颜色
        line.startColor = Color.blue;
        line.endColor = Color.red;
        // 设置线段起点宽度和终点宽度
        line.startWidth = 0.009f;
        line.endWidth = 0.009f;

        line.SetPosition(0, pos[0]);
        line.SetPosition(1, pos[1]);
        line.SetPosition(2, pos[2]);
        line.SetPosition(3, pos[3]);

        startTime = Time.time;
    }

    private void Update()
    {
        // 设置所画的线按指定的时间间隔闪烁
        curTime = Time.time;
        if (curTime - startTime > 0.1)
        {
            line.enabled = false;
            startTime = curTime;
        }
        else
        {
            line.enabled = true;
        }
    }
}
