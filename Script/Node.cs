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

public class Node
{
    public Vector3 pos;    //点位置坐标
    public Vector3 nor;    //点推移法向
    public Node next;
    public bool judgeEdgePoint;  //逻辑值，是否为边界点
    public short judgeDisappear; //零值表示该点代表的真实点在物理上消失，不在推移或其他计算

    //若点为非边界点，则表示点本身所在的面，0-前端面，1-外侧面，2-后断面，3-内侧面
    //-1-链表行结束标识点，非真实点   -2 - 面结束点，非真实点
    public short judgeEdge;

    public double staticPressure; //静压
    public double flowVelocity;   //燃气流速
    public double density;        //燃气密度
    public double temper;          //静温
    public double burnRate;        //装药燃速
}
