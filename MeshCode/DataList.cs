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
        last.item = node.item;
        last.temper = node.temper;
        last.next = null;
        if (IsEmpty()) first = last;
        else oldlast.next = last;
        N++;
    }
    public void Delete(Node node)
    {
        node.item = node.next.item;
        node.next = node.next.next;
        N--;
    }
}
