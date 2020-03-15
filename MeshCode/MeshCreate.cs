/*
脚本名称：
脚本作者：
建立时间：
脚本功能：
版本号：
*/
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MeshCreate : MonoBehaviour
{
    MeshFilter meshFilter;
    MeshRenderer meshRenderer;
    MeshCollider meshCollider;


    //存放顶点数据
    string path;
    public List<Vector3> verts;//顶点位置
    List<int> indices;  //顶点顺序
    DataList<Vector3> rowsOfData;         //某一行的点数据
    List<DataList<Vector3>> overAllData;  //外侧面所有的点数据,每个元素存放每行的点数据链表
    List<DataList<Vector3>> overAllData1; //下端面所有点数据
    List<DataList<Vector3>> overAllData2; //上端面所有点数据
    List<DataList<Vector3>> overAllData3; //内侧面所有点数据
    
    //MergePoint所用
    DataList<Vector3> insteadLine = new DataList<Vector3>();
    DataList<Vector3> insteadLine1 = new DataList<Vector3>();
    Node<Vector3> upPoint = new Node<Vector3>();      //上一行的某点
    Node<Vector3> downPoint = new Node<Vector3>();    //下一行的某点
    Node<Vector3> insteadPoint = new Node<Vector3>(); //用于添加替换的替换点

    //顶点顺序
    int downSide = 0;  //下行的顺序
    int upSide;    //上行的顺序
    void Start()
    {
        verts = new List<Vector3>();
        indices = new List<int>();
        rowsOfData = new DataList<Vector3>();
        overAllData = new List<DataList<Vector3>>();
        overAllData1 = new List<DataList<Vector3>>();
        overAllData2 = new List<DataList<Vector3>>();
        overAllData3 = new List<DataList<Vector3>>();
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        meshCollider = GetComponent<MeshCollider>();
        Generate();
    }
    private void Update()
    {
        
    }
    private void Generate()
    {
        //读取数据
        ReadData();
        Vector3 center = CenterOfCircle(overAllData2);
        //填写数据
        AddMeshData(overAllData);        //外侧面的网格划分
        AddMeshData1(overAllData3);      //内侧面的网格划分
        NewMergePoint(overAllData1, 1);  //下端面的网格划分
        NewMergePoint(overAllData2, 2);  //上端面的网格划分
        Mesh mesh = new Mesh();
        mesh.vertices = verts.ToArray();
        mesh.triangles = indices.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        meshFilter.mesh = mesh;
        //碰撞体专用mesh，只负责物体的碰撞外形
        meshCollider.sharedMesh = mesh;
    }
    private void AddMeshData(List<DataList<Vector3>> overData)
    {//外侧面的网格划分
        MergePoint(overData);
        upSide += overData[1].Size;
        int index0,index1,index2,index3;
        for (int serialNum = 0; serialNum < overData.Count-1; serialNum++)
        {
            for (int i = 0; i < overData[serialNum].Size-1; i++)
            {
                index0 = downSide++;      //左下
                index1 = upSide++;        //左上
                index2 = upSide;          //右上
                index3 = downSide;        //右下

                indices.Add(index0); indices.Add(index1); indices.Add(index2);
                indices.Add(index0); indices.Add(index2); indices.Add(index3);
            }
            //最后的端点和首部的端点也需要网格划分
            index0 = downSide;
            index1 = upSide;
            index2 = upSide - overData[serialNum + 1].Size + 1;
            index3 = downSide - overData[serialNum].Size + 1;
            indices.Add(index0); indices.Add(index1); indices.Add(index2);
            indices.Add(index0); indices.Add(index2); indices.Add(index3);
            upSide++;
            downSide++;
        }
        downSide = upSide;
    }
    private void AddMeshData1(List<DataList<Vector3>> overData)
    {//内端面的网格划分
        MergePoint(overData);
        upSide += overData[1].Size;
        int index0, index1, index2, index3;
        for (int serialNum = 0; serialNum < overData.Count - 1; serialNum++)
        {
            for (int i = 0; i < overData[serialNum].Size - 1; i++)
            {
                index0 = downSide++;      //左下
                index1 = upSide++;        //左上
                index2 = upSide;          //右上
                index3 = downSide;        //右下

                indices.Add(index0); indices.Add(index2); indices.Add(index1);
                indices.Add(index0); indices.Add(index3); indices.Add(index2);
            }
            //最后的端点和首部的端点也需要网格划分
            index0 = downSide;
            index1 = upSide;
            index2 = upSide - overData[serialNum + 1].Size + 1;
            index3 = downSide - overData[serialNum].Size + 1;
            indices.Add(index0); indices.Add(index2); indices.Add(index1);
            indices.Add(index0); indices.Add(index3); indices.Add(index2);
            upSide++;
            downSide++;
        }
        downSide = upSide;
    }
    private void ReadData()
    {
        path = @"e:\Point3.txt";
        FileStream stream = new FileStream(path, FileMode.Open);
        StreamReader reader = new StreamReader(stream);
        while(!reader.EndOfStream)
        {
            string read = reader.ReadLine();
            if (read == "0")
            {//外侧面
                overAllData.Add(rowsOfData);
                rowsOfData = new DataList<Vector3>();
                continue;
            }
            else if (read == "1")
            {//下端面的点数据
                overAllData1.Add(rowsOfData);
                rowsOfData = new DataList<Vector3>();
                continue;
            }
            else if (read == "2")
            {//上端面的点数据
                overAllData2.Add(rowsOfData);
                rowsOfData = new DataList<Vector3>();
                continue;
            }
            else if (read == "3")
            {//内侧面的点数据
                overAllData3.Add(rowsOfData);
                rowsOfData = new DataList<Vector3>();
                continue;
            }
            else
            {
                string[] arrTemp = read.Split(',');
                Vector3 point = new Vector3(float.Parse(arrTemp[0]), float.Parse(arrTemp[1]), float.Parse(arrTemp[2]));
                rowsOfData.Enqueue(point);
            }
        }
    }
    //向verts中添加点，若上下两层点数不同，则进行合并操作
    private void MergePoint(List<DataList<Vector3>> overData)
    {//用于计算侧面
        int min = overData[0].Size;                    //所有顶点行中点数量最小的数量
        int minLine = 0;                                  //顶点行中点数量最小的一行
        for (int i=1;i<overData.Count;i++)
        {//找出所有点集中数量最少的一行
            if (min > overData[i].Size)
            {
                min = overData[i].Size;
                minLine = i;
            }
        }
        //向下合并
        for (int downCount=minLine-1;downCount>=0;downCount--)
        {//以minLine为准向下进行点合并,downCount为当前进行合并的行数
            int insteadMin = min;
            if (overData[downCount].Size == min)
                continue;  //如果点数据数量与最小量相同，则不需要进行合并
            insteadLine = new DataList<Vector3>();
            insteadLine.Enqueue(overData[downCount].first.item);//先向insteadLine插入第一个点
            upPoint = overData[downCount + 1].first.next; //上一行点击的第二个点开始
            downPoint = overData[downCount].first.next;   //下一行点集的第二个点开始
            int downNum = overData[downCount].Size - 1;  //当前进行合并行的剩余点数量
            while (upPoint != null)
            {
                if (downPoint.next == null)
                {//当downPoint为最后一个点时，直接插入insteadLine;
                    upPoint = upPoint.next;
                    insteadLine.Enqueue(downPoint.item);
                }
                if (Distance2(downPoint.item, upPoint.item) >=
                   Distance2(downPoint.next.item, upPoint.item))
                {//点集的合并，选择最优点，删除不必要的点
                    downPoint = downPoint.next;
                    downNum--;
                }
                else
                {
                    upPoint = upPoint.next;
                    insteadMin--;
                    insteadLine.Enqueue(downPoint.item);
                    downPoint = downPoint.next;
                    downNum--;
                }
                if (downNum == insteadMin-1)
                {
                    while (downPoint != null)
                    {
                        insteadLine.Enqueue(downPoint.item);
                        downPoint = downPoint.next;
                        upPoint = upPoint.next;
                    }
                }
            }
            overData[downCount] = insteadLine;
        }
        //向上合并...
        for (int upCount = minLine + 1; upCount < overData.Count; upCount++)
        {//以minLine为准向上进行点合并,upCount为当前进行合并的行数
            int insteadMin = min;
            if (overData[upCount].Size == min)
                continue;
            insteadLine = new DataList<Vector3>();
            insteadLine.Enqueue(overData[upCount].first.item);
            upPoint = overData[upCount].first.next;        //上一行点集的第二个点开始
            downPoint = overData[upCount - 1].first.next;  //下一行点集的第二个点开始
            int upNum = overData[upCount].Size - 1;
            while (downPoint != null)
            {
                if (upPoint.next == null)
                {//当upPoint为最后一个点时，直接插入insteadLine;
                    downPoint = downPoint.next;
                    insteadLine.Enqueue(upPoint.item);
                }
                if(Distance2(upPoint.item,downPoint.item)>=
                   Distance2(upPoint.next.item,downPoint.item))
                {
                    upPoint = upPoint.next;
                    upNum--;
                }
                else
                {
                    downPoint = downPoint.next;
                    insteadMin--;
                    insteadLine.Enqueue(upPoint.item);
                    upPoint = upPoint.next;
                    upNum--;
                }
                if(upNum==insteadMin-1)
                {
                    while (upPoint != null)
                    {
                        insteadLine.Enqueue(upPoint.item);
                        upPoint = upPoint.next;
                        downPoint = downPoint.next;
                    }
                }
            }
            overData[upCount] = insteadLine;
        }
        //将点数据输入到verst中
        for (int i=0;i<overData.Count;i++)
        {
            insteadPoint = overData[i].first;
            while (insteadPoint != null)
            {
                verts.Add(insteadPoint.item);
                insteadPoint = insteadPoint.next;
            }
        }
    }

    private void NewMergePoint(List<DataList<Vector3>> overData,int mark)
    {//用于计算端面，mark为1时，下端面；2时，上端面
        int upNum;
        int downNum;
        Vector3 center = CenterOfCircle(overData);
        for (int i = 0; i < overData.Count; i++)
        {
            insteadLine = new DataList<Vector3>();
            insteadLine1 = new DataList<Vector3>();
            if (i == overData.Count - 1)
            {//当i为最后一行时，最后一行要与第一行划分三角
                insteadLine.Enqueue(overData[i].first.item);
                insteadLine1.Enqueue(overData[0].first.item);
                upPoint = overData[0].first.next;
                downPoint = overData[i].first.next;
                upNum = overData[0].Size - 1;
                downNum = overData[i].Size - 1;
            }
            else
            {
                insteadLine.Enqueue(overData[i].first.item);      //下行的点
                insteadLine1.Enqueue(overData[i + 1].first.item); //上行的点
                upPoint = overData[i + 1].first.next;
                downPoint = overData[i].first.next;
                upNum = overData[i + 1].Size - 1;    //待合并的点数量
                downNum = overData[i].Size - 1;      //待合并的点数量
            }
            if (downNum > upNum)
            {//如果下层比上层点数量多，下层合并
                while (upPoint != null)
                {
                    if (downPoint.next == null)
                    {
                        upPoint = upPoint.next;
                        insteadLine.Enqueue(downPoint.item);
                    }
                    if (P2PDistance(downPoint.item, upPoint.item,center) >
                       P2PDistance(downPoint.next.item, upPoint.item,center))
                    {//点合并的依据
                        downPoint = downPoint.next;
                        downNum--;
                    }
                    else
                    {
                        insteadLine1.Enqueue(upPoint.item);
                        upPoint = upPoint.next;
                        upNum--;
                        insteadLine.Enqueue(downPoint.item);
                        downPoint = downPoint.next;
                        downNum--;
                    }
                    if (downNum == upNum)
                    {
                        while (downPoint != null)
                        {
                            insteadLine.Enqueue(downPoint.item);
                            insteadLine1.Enqueue(upPoint.item);
                            downPoint = downPoint.next;
                            upPoint = upPoint.next;
                        }
                    }
                }
            }
            else if (downNum < upNum)
            {
                while (downPoint != null)
                {
                    if (upPoint.next == null)
                    {
                        downPoint = downPoint.next;
                        insteadLine1.Enqueue(upPoint.item);
                    }
                    if (P2PDistance(upPoint.item, downPoint.item,center)
                      > P2PDistance(upPoint.next.item, downPoint.item,center))
                    {
                        upPoint = upPoint.next;
                        upNum--;
                    }
                    else
                    {
                        insteadLine.Enqueue(downPoint.item);
                        insteadLine1.Enqueue(upPoint.item);
                        upPoint = upPoint.next;
                        downPoint = downPoint.next;
                        upNum--;
                        downNum--;
                    }
                    if (downNum == upNum)
                    {
                        while (downPoint != null)
                        {
                            insteadLine.Enqueue(downPoint.item);
                            insteadLine1.Enqueue(upPoint.item);
                            downPoint = downPoint.next;
                            upPoint = upPoint.next;
                        }
                    }
                }
            }
            else
            {
                while (downPoint != null)
                {
                    insteadLine.Enqueue(downPoint.item);
                    insteadLine1.Enqueue(upPoint.item);
                    downPoint = downPoint.next;
                    upPoint = upPoint.next;
                }
            }
            AddMerge(insteadLine, insteadLine1, mark);
            downSide = upSide;//面完成后，downSide指向verts的最后位置的后一位
        }
        
    }
    private void AddMerge(DataList<Vector3> lns0,DataList<Vector3> lns1,int mark)
    {
        //点插入verts中,lns0为下行点，lns1为上行点
        upSide += lns0.Size;
        insteadPoint = lns0.first;
        List<int> breakpoint = new List<int>();   //断点，其与下一个点之间的网格不需要画出来
        while(insteadPoint!=null)
        {
            //if(Vector3.Distance(insteadPoint.item,insteadPoint.next.item)>某个值)
            //    breakpoint.Add(verts.Count);
            verts.Add(insteadPoint.item);
            insteadPoint = insteadPoint.next;
        }
        insteadPoint = lns1.first;
        while (insteadPoint != null)
        {
            verts.Add(insteadPoint.item);
            insteadPoint = insteadPoint.next;
        }
        int index0, index1, index2, index3;
        if (mark == 2)//上端面
        {
            for (int i = 0; i < lns0.Size - 1; i++)
            {
                if (breakpoint.Count != 0)
                {
                    for (int j = 0; j < breakpoint.Count; j++)
                    {
                        if (downSide == breakpoint[j])
                        {
                            downSide++;
                            upSide++;
                        }
                    }
                }
                index0 = downSide++;      //左下
                index1 = upSide++;        //左上
                index2 = upSide;          //右上
                index3 = downSide;        //右下

                indices.Add(index0); indices.Add(index1); indices.Add(index2);
                indices.Add(index0); indices.Add(index2); indices.Add(index3);
            }
        }
        else if (mark == 1)//下端面
        {
            for (int i = 0; i < lns0.Size - 1; i++)
            {
                if (breakpoint.Count != 0)
                {
                    for (int j = 0; j < breakpoint.Count; j++)
                    {
                        if (downSide == breakpoint[j])
                        {
                            downSide++;
                            upSide++;
                        }
                    }
                }
                index0 = downSide++;      //左下
                index1 = upSide++;        //左上
                index2 = upSide;          //右上
                index3 = downSide;        //右下

                indices.Add(index0); indices.Add(index2); indices.Add(index1);
                indices.Add(index0); indices.Add(index3); indices.Add(index2);
            }
        }
        downSide++;
        upSide++;
    }
    private float Distance2(Vector3 p,Vector3 q)
    {//用于外侧面和内侧面的点合并
        return (p.x - q.x) * (p.x - q.x)
             + (p.y - q.y) * (p.y - q.y)
             + (p.z - q.z) * (p.z - q.z);
    }
    private float P2PDistance(Vector3 point1,Vector3 point2,Vector3 center)
    {//point1到圆心的距离 - point2到圆心距离
        float ins = Vector3.Distance(point1, center) - Vector3.Distance(point2, center);
        if (ins >= 0)
            return ins;
        else
            return -ins;
    }
    private Vector3 CenterOfCircle(List<DataList<Vector3>> overData)
    {
        int N = overData.Count / 3;
        Vector3 point1 = overData[0].last.item;
        Vector3 point2 = overData[N].last.item;
        Vector3 point3 = overData[2 * N].last.item;
        Vector3 centerPoint = new Vector3();
        float a1, b1, c1, d1;
        float a2, b2, c2, d2;
        float a3, b3, c3, d3;
        float x1 = point1.x, y1 = point1.y, z1 = point1.z;
        float x2 = point2.x, y2 = point2.y, z2 = point2.z;
        float x3 = point3.x, y3 = point3.y, z3 = point3.z;

        a1 = (y1 * z2 - y2 * z1 - y1 * z3 + y3 * z1 + y2 * z3 - y3 * z2);
        b1 = -(x1 * z2 - x2 * z1 - x1 * z3 + x3 * z1 + x2 * z3 - x3 * z2);
        c1 = (x1 * y2 - x2 * y1 - x1 * y3 + x3 * y1 + x2 * y3 - x3 * y2);
        d1 = -(x1 * y2 * z3 - x1 * y3 * z2 - x2 * y1 * z3 + x2 * y3 * z1 + x3 * y1 * z2 - x3 * y2 * z1);

        a2 = 2 * (x2 - x1);
        b2 = 2 * (y2 - y1);
        c2 = 2 * (z2 - z1);
        d2 = x1 * x1 + y1 * y1 + z1 * z1 - x2 * x2 - y2 * y2 - z2 * z2;

        a3 = 2 * (x3 - x1);
        b3 = 2 * (y3 - y1);
        c3 = 2 * (z3 - z1);
        d3 = x1 * x1 + y1 * y1 + z1 * z1 - x3 * x3 - y3 * y3 - z3 * z3;

        centerPoint.x = -(b1 * c2 * d3 - b1 * c3 * d2 - b2 * c1 * d3 + b2 * c3 * d1 + b3 * c1 * d2 - b3 * c2 * d1)
        / (a1 * b2 * c3 - a1 * b3 * c2 - a2 * b1 * c3 + a2 * b3 * c1 + a3 * b1 * c2 - a3 * b2 * c1);
        centerPoint.y = (a1 * c2 * d3 - a1 * c3 * d2 - a2 * c1 * d3 + a2 * c3 * d1 + a3 * c1 * d2 - a3 * c2 * d1)
            / (a1 * b2 * c3 - a1 * b3 * c2 - a2 * b1 * c3 + a2 * b3 * c1 + a3 * b1 * c2 - a3 * b2 * c1);
        centerPoint.z = -(a1 * b2 * d3 - a1 * b3 * d2 - a2 * b1 * d3 + a2 * b3 * d1 + a3 * b1 * d2 - a3 * b2 * d1)
            / (a1 * b2 * c3 - a1 * b3 * c2 - a2 * b1 * c3 + a2 * b3 * c1 + a3 * b1 * c2 - a3 * b2 * c1);

        return centerPoint;
    }
    private Vector3 CenterOfCircle(Vector3 point1,Vector3 point2,Vector3 point3)
    {
        Vector3 centerPoint = new Vector3();
        float a1, b1, c1, d1;
        float a2, b2, c2, d2;
        float a3, b3, c3, d3;
        float x1 = point1.x, y1 = point1.y, z1 = point1.z;
        float x2 = point2.x, y2 = point2.y, z2 = point2.z;
        float x3 = point3.x, y3 = point3.y, z3 = point3.z;

        a1 = (y1 * z2 - y2 * z1 - y1 * z3 + y3 * z1 + y2 * z3 - y3 * z2);
        b1 = -(x1 * z2 - x2 * z1 - x1 * z3 + x3 * z1 + x2 * z3 - x3 * z2);
        c1 = (x1 * y2 - x2 * y1 - x1 * y3 + x3 * y1 + x2 * y3 - x3 * y2);
        d1 = -(x1 * y2 * z3 - x1 * y3 * z2 - x2 * y1 * z3 + x2 * y3 * z1 + x3 * y1 * z2 - x3 * y2 * z1);

        a2 = 2 * (x2 - x1);
        b2 = 2 * (y2 - y1);
        c2 = 2 * (z2 - z1);
        d2 = x1 * x1 + y1 * y1 + z1 * z1 - x2 * x2 - y2 * y2 - z2 * z2;

        a3 = 2 * (x3 - x1);
        b3 = 2 * (y3 - y1);
        c3 = 2 * (z3 - z1);
        d3 = x1 * x1 + y1 * y1 + z1 * z1 - x3 * x3 - y3 * y3 - z3 * z3;

        centerPoint.x = -(b1 * c2 * d3 - b1 * c3 * d2 - b2 * c1 * d3 + b2 * c3 * d1 + b3 * c1 * d2 - b3 * c2 * d1)
        / (a1 * b2 * c3 - a1 * b3 * c2 - a2 * b1 * c3 + a2 * b3 * c1 + a3 * b1 * c2 - a3 * b2 * c1);
        centerPoint.y = (a1 * c2 * d3 - a1 * c3 * d2 - a2 * c1 * d3 + a2 * c3 * d1 + a3 * c1 * d2 - a3 * c2 * d1)
            / (a1 * b2 * c3 - a1 * b3 * c2 - a2 * b1 * c3 + a2 * b3 * c1 + a3 * b1 * c2 - a3 * b2 * c1);
        centerPoint.z = -(a1 * b2 * d3 - a1 * b3 * d2 - a2 * b1 * d3 + a2 * b3 * d1 + a3 * b1 * d2 - a3 * b2 * d1)
            / (a1 * b2 * c3 - a1 * b3 * c2 - a2 * b1 * c3 + a2 * b3 * c1 + a3 * b1 * c2 - a3 * b2 * c1);

        return centerPoint;
    }

}
