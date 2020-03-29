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
/// <summary>
/// 存放点数据的链表
/// </summary>
public class DataList
{
    public Node first; //头节点
    public Node last;  //尾节点
    public int N;      //元素个数

    public Node Last
    {
        get { return last; }
    }
    public int Size
    {
        get { return N; }
    }
    public bool IsEmpty()
    { return N == 0; }
    /// <summary>
    /// 元素插入
    /// </summary>
    /// <param name="item"></param>
    public void Enqueue(Node node)
    {
        Node oldlast = last;
        last = new Node();
        last.pos = node.pos;
        last.temper = node.temper;
        last.burnRate = node.burnRate;
        last.density = node.density;
        last.judgeEdgePoint = node.judgeEdgePoint;
        last.flowVelocity = node.flowVelocity;
        last.judgeDisappear = node.judgeDisappear;
        last.judgeEdge = node.judgeEdge;
        last.staticPressure = node.staticPressure;
        last.next = null;
        if (IsEmpty()) first = last;
        else oldlast.next = last;
        N++;
    }
    public void Delete(Node node)
    {
        node.pos = node.next.pos;
        node.temper = node.next.temper;
        node.burnRate = node.next.burnRate;
        node.density = node.next.density;
        node.judgeEdgePoint = node.next.judgeEdgePoint;
        node.flowVelocity = node.next.flowVelocity;
        node.judgeDisappear = node.next.judgeDisappear;
        node.judgeEdge = node.next.judgeEdge;
        node.staticPressure = node.next.staticPressure;
        node.next = node.next.next;
        N--;
    }
}
