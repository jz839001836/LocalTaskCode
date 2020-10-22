/*
脚本名称：
脚本作者：
建立时间：
脚本功能：
版本号：
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(MeshRenderer),typeof(MeshFilter),typeof(MeshCollider))]
public class MeshCreate : MonoBehaviour
{
    MeshFilter meshFilter;
    MeshRenderer meshRenderer;
    MeshCollider meshCollider;
    public Mesh mesh;
    public Material materialTemper;
    public Material materialPress;

    Data data;

    //存放顶点数据
    string path;
    public List<Vector3> verts;//顶点位置
    List<int> indices;  //顶点顺序
    List<Color> Presscolors; //顶点颜色
    List<Color> temperColors;



    List<string> fileNames;
    int fileIndex = 0;

    DataList rowsOfData;         //某一行的点数据
    List<DataList> overAllData;  //外侧面所有的点数据,每个元素存放每行的点数据链表
    List<DataList> overAllData1; //下端面所有点数据
    List<DataList> overAllData2; //上端面所有点数据
    List<DataList> overAllData3; //内侧面所有点数据

    public GameObject text1;
    public GameObject text2;
    public GameObject text3;
    public GameObject text4;
    public GameObject text5;
    public GameObject text6;
    public Slider speedSlider;
    float speed = 1;

    //MergePoint所用
    DataList insteadLine = new DataList();
    DataList insteadLine1 = new DataList();
    Node upPoint = new Node();      //上一行的某点
    Node downPoint = new Node();    //下一行的某点
    Node insteadPoint = new Node(); //用于添加替换的替换点

    //顶点顺序
    int downSide = 0;  //下行的顺序
    int upSide;    //上行的顺序

    int choose = 0; //选择压强还是温度渲染；0-温度， 1-压强
    public static float maxTemper = 2317.24f;    //某个值最大的点
    public static float minTemper = 2314.041f;    //某个值最小的点
    public static float maxPress = 3.885e7f;
    public static float minPress = 3.835e7f;

    //剖分网格
    List<List<Node>> outerHeadAndTailNodes;
    List<List<Node>> insideHeadAndTailNodes;

    //绘制线
    private LineRenderer line;

    int divide = 0; //0:代表原物体，1：代表剖分物体
    private void Awake()
    {

    }
    float i;  //用于帧数计数
    private void Start()
    {
        Initialize();
        //GenerateSubdivision(fileNames[fileIndex]);
        //Generate(fileNames[fileIndex]);
        text6.GetComponent<Text>().text = "单位：℃";
        SetMaxAndMin(maxTemper, minTemper);
        //DontDestroyOnLoad(this.gameObject);
    }
    bool pause = true;
    private void Update()
    {
        if (pause)
        {
            ShowCombustion();
        }
    }
    public void PauseControl()
    {
        if (pause)
            pause = false;
        else
            pause = true;
    }
    private void ShowCombustion()
    {
        if (i >= speed && divide == 0)
        {
            if (fileIndex != fileNames.Count)
            {
                Generate(fileNames[fileIndex]);
                fileIndex++;
            }
            i = 0;
        }
        else if (i >= speed && divide == 1)
        {
            if (fileIndex != fileNames.Count)
            {
                GenerateSubdivision(fileNames[fileIndex]);
                fileIndex++;
            }
            i = 0;
        }
        i += Time.deltaTime;
    }
    public void Divide()
    {
        if (divide == 0)
        {
            if (fileIndex == fileNames.Count)
            {
                GenerateSubdivision(fileNames[fileNames.Count - 1]);
                divide = 1;
            }
            else
            {
                divide = 1;
                GenerateSubdivision(fileNames[fileIndex - 1]);
            }
        }
        else if (divide == 1)
        {
            if (fileIndex == fileNames.Count)
            {
                Generate(fileNames[fileNames.Count - 1]);
                divide = 0;
            }
            else
            {
                divide = 0;
                Generate(fileNames[fileIndex - 1]);
            }
        }
    }
    //数据初始化
    private void Initialize()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        meshCollider = GetComponent<MeshCollider>();
        fileNames = new List<string>();
#if UNITY_EDITOR
        string path = UnityEditor.EditorUtility.OpenFolderPanel("打开文件", @"E:\data", "");
#endif
        string[] files = Directory.GetFiles(path);
        foreach (string file in files)
        {
            if (file.EndsWith(".pbsi"))
                fileNames.Add(file);
        }
        mesh = new Mesh();
        line = gameObject.AddComponent<LineRenderer>();
        line.material = new Material(Shader.Find("Sprites/Default"));
    }
    private void Generate(string filename)
    {
        mesh.Clear();
        downSide = 0;
        upSide = 0;
        //读取数据
        data = new Data(filename);
        data.Read();
        //CalMaxAndMin();

        verts = new List<Vector3>();
        indices = new List<int>();

        Presscolors = new List<Color>();
        temperColors = new List<Color>();

        rowsOfData = new DataList();
        overAllData = new List<DataList>();
        overAllData1 = new List<DataList>();
        overAllData2 = new List<DataList>();
        overAllData3 = new List<DataList>();

        //填写数据
        AddMeshData(data.faceData[1]);        //外侧面的网格划分
        if (data.faceData[3] != null)
            AddMeshData1(data.faceData[3]);      //内侧面的网格划分
        NewMergePoint(data.faceData[0], 1, false);  //下端面的网格划分
        NewMergePoint(data.faceData[2], 2, false);  //上端面的网格划分


        mesh.SetVertices(verts);
        mesh.triangles = indices.ToArray();
        if (choose == 0)
            mesh.colors = temperColors.ToArray();
        else if (choose == 1)
            mesh.colors = Presscolors.ToArray();


        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        meshFilter.mesh = mesh;
        //碰撞体专用mesh，只负责物体的碰撞外形
        meshCollider.sharedMesh = mesh;
    }
    private void GenerateSubdivision(string filename)
    {
        mesh.Clear();
        downSide = 0;
        upSide = 0;
        //读取数据
        data = new Data(filename);
        data.Read();
        //CalMaxAndMin();

        verts = new List<Vector3>();
        indices = new List<int>();

        Presscolors = new List<Color>();
        temperColors = new List<Color>();

        rowsOfData = new DataList();
        overAllData = new List<DataList>();
        overAllData1 = new List<DataList>();
        overAllData2 = new List<DataList>();
        overAllData3 = new List<DataList>();

        DivisionOuter(data.faceData[1]);
        DivisionInside(data.faceData[3]);
        InsideAndOuterConnect();

        NewMergePoint(data.faceData[0], 1, true);  //下端面的网格划分
        NewMergePoint(data.faceData[2], 2, true);  //上端面的网格划分

        mesh.SetVertices(verts);
        mesh.triangles = indices.ToArray();

        if (choose == 0)
            mesh.colors = temperColors.ToArray();
        else if (choose == 1)
            mesh.colors = Presscolors.ToArray();

        //DrawLine(outerHeadAndTailNodes[0]);

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        meshFilter.mesh = mesh;
        //碰撞体专用mesh，只负责物体的碰撞外形
        meshCollider.sharedMesh = mesh;
    }
    //外侧面的网格划分
    private void AddMeshData(List<DataList> overData)
    {
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
    //内端面的网格划分
    private void AddMeshData1(List<DataList> overData)
    {
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
    //剖分面的切分网格节点
    private List<List<Node>> DivideMergePoint(List<DataList> overData)
    {//用于计算侧面
        int min = overData[0].Size;                    //所有顶点行中点数量最小的数量
        int minLine = 0;                                  //顶点行中点数量最小的一行

        List<Node> first = new List<Node>();
        List<Node> last = new List<Node>();

        for (int i = 1; i < overData.Count; i++)
        {//找出所有点集中数量最少的一行
            if (min > overData[i].Size)
            {
                min = overData[i].Size;
                minLine = i;
            }
        }
        //向下合并
        for (int downCount = minLine - 1; downCount >= 0; downCount--)
        {//以minLine为准向下进行点合并,downCount为当前进行合并的行数
            int insteadMin = min;
            if (overData[downCount].Size == min)
                continue;  //如果点数据数量与最小量相同，则不需要进行合并
            insteadLine = new DataList();
            insteadLine.Enqueue(overData[downCount].first);//先向insteadLine插入第一个点
            upPoint = overData[downCount + 1].first.next; //上一行点击的第二个点开始
            downPoint = overData[downCount].first.next;   //下一行点集的第二个点开始
            int downNum = overData[downCount].Size - 1;  //当前进行合并行的剩余点数量
            while (upPoint != null)
            {
                if (downPoint.next == null)
                {//当downPoint为最后一个点时，直接插入insteadLine;
                    upPoint = upPoint.next;
                    insteadLine.Enqueue(downPoint);
                }
                if (Distance2(downPoint.pos, upPoint.pos) >=
                   Distance2(downPoint.next.pos, upPoint.pos))
                {//点集的合并，选择最优点，删除不必要的点
                    downPoint = downPoint.next;
                    downNum--;
                }
                else
                {
                    upPoint = upPoint.next;
                    insteadMin--;
                    insteadLine.Enqueue(downPoint);
                    downPoint = downPoint.next;
                    downNum--;
                }
                if (downNum == insteadMin - 1)
                {
                    while (downPoint != null)
                    {
                        insteadLine.Enqueue(downPoint);
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
            insteadLine = new DataList();
            insteadLine.Enqueue(overData[upCount].first);
            upPoint = overData[upCount].first.next;        //上一行点集的第二个点开始
            downPoint = overData[upCount - 1].first.next;  //下一行点集的第二个点开始
            int upNum = overData[upCount].Size - 1;
            while (downPoint != null)
            {
                if (upPoint.next == null)
                {//当upPoint为最后一个点时，直接插入insteadLine;
                    downPoint = downPoint.next;
                    insteadLine.Enqueue(upPoint);
                }
                if (Distance2(upPoint.pos, downPoint.pos) >=
                   Distance2(upPoint.next.pos, downPoint.pos))
                {
                    upPoint = upPoint.next;
                    upNum--;
                }
                else
                {
                    downPoint = downPoint.next;
                    insteadMin--;
                    insteadLine.Enqueue(upPoint);
                    upPoint = upPoint.next;
                    upNum--;
                }
                if (upNum == insteadMin - 1)
                {
                    while (upPoint != null)
                    {
                        insteadLine.Enqueue(upPoint);
                        upPoint = upPoint.next;
                        downPoint = downPoint.next;
                    }
                }
            }
            overData[upCount] = insteadLine;
        }
        //将点数据输入到verts中
        for (int i = 0; i < overData.Count; i++)
        {
            int j;
            insteadPoint = overData[i].first;
            first.Add(insteadPoint);
            for (j = 0; j < overData[i].Size / 2; j++)
            {
                if (j == overData[i].Size / 2 - 1)
                    last.Add(insteadPoint);
                verts.Add(insteadPoint.pos);
                Presscolors.Add(Color.Lerp(Color.blue, Color.red, CalInterpolate(maxPress, minPress, (float)insteadPoint.staticPressure)));
                temperColors.Add(Color.Lerp(Color.blue, Color.red, CalInterpolate(maxTemper, minTemper, (float)insteadPoint.temper)));
                insteadPoint = insteadPoint.next;
            }
        }
        List<List<Node>> ret = new List<List<Node>>();
        ret.Add(first);
        ret.Add(last);

        return ret;
    }
    //剖分的外侧面切分网格
    private void DivisionOuter(List<DataList> overData)
    {
        outerHeadAndTailNodes = DivideMergePoint(overData);
        upSide += overData[1].Size / 2;
        int index0, index1, index2, index3;
        for (int serialNum = 0; serialNum < overData.Count - 1; serialNum++)
        {
            for (int i = 0; i < overData[serialNum].Size / 2 - 1; i++)
            {
                index0 = downSide++;      //左下
                index1 = upSide++;        //左上
                index2 = upSide;          //右上
                index3 = downSide;        //右下

                indices.Add(index0); indices.Add(index1); indices.Add(index2);
                indices.Add(index0); indices.Add(index2); indices.Add(index3);
            }
            upSide++;
            downSide++;
        }
        downSide = upSide;
    }
    //剖分的内侧面切分网格
    private void DivisionInside(List<DataList> overData)
    {
        insideHeadAndTailNodes = DivideMergePoint(overData);
        upSide += overData[1].Size / 2;
        int index0, index1, index2, index3;
        for (int serialNum = 0; serialNum < overData.Count - 1; serialNum++)
        {
            for (int i = 0; i < overData[serialNum].Size / 2 - 1; i++)
            {
                index0 = downSide++;      //左下
                index1 = upSide++;        //左上
                index2 = upSide;          //右上
                index3 = downSide;        //右下

                indices.Add(index0); indices.Add(index2); indices.Add(index1);
                indices.Add(index0); indices.Add(index3); indices.Add(index2);
            }
            upSide++;
            downSide++;
        }
        downSide = upSide;
    }
    //内外侧面的连接网格
    private void InsideAndOuterConnect()
    {
        AddVertex(outerHeadAndTailNodes[0], insideHeadAndTailNodes[0]);
        int index0, index1, index2, index3;
        upSide += outerHeadAndTailNodes[0].Count;
        for (int i = 0; i < outerHeadAndTailNodes[0].Count - 1; i++)
        {
            index0 = downSide++;
            index1 = downSide;
            index2 = upSide++;
            index3 = upSide;

            indices.Add(index0); indices.Add(index3); indices.Add(index1);
            indices.Add(index0); indices.Add(index2); indices.Add(index3);
        }
        upSide++;
        downSide = upSide;

        AddVertex(outerHeadAndTailNodes[1], insideHeadAndTailNodes[1]);
        upSide += insideHeadAndTailNodes[1].Count;

        for (int i = 0; i < outerHeadAndTailNodes[1].Count - 1; i++)
        {
            index0 = downSide++;
            index1 = downSide;
            index2 = upSide++;
            index3 = upSide;

            indices.Add(index0); indices.Add(index3); indices.Add(index2);
            indices.Add(index0); indices.Add(index1); indices.Add(index3);
        }
        upSide++;
        downSide = upSide;
    }
    private void AddVertex(List<Node> outer, List<Node> inside)
    {
        for (int i = 0; i < outer.Count; i++)
        {
            verts.Add(outer[i].pos);
            Presscolors.Add(Color.Lerp(Color.blue, Color.red, CalInterpolate(maxPress, minPress, (float)outer[i].staticPressure)));
            temperColors.Add(Color.Lerp(Color.blue, Color.red, CalInterpolate(maxTemper, minTemper, (float)outer[i].temper)));
        }

        for (int i = 0; i < inside.Count; i++)
        {
            verts.Add(inside[i].pos);
            Presscolors.Add(Color.Lerp(Color.blue, Color.red, CalInterpolate(maxPress, minPress, (float)inside[i].staticPressure)));
            temperColors.Add(Color.Lerp(Color.blue, Color.red, CalInterpolate(maxTemper, minTemper, (float)inside[i].temper)));
        }
    }
    //向verts中添加点，若上下两层点数不同，则进行合并操作
    private void MergePoint(List<DataList> overData)
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
            insteadLine = new DataList();
            insteadLine.Enqueue(overData[downCount].first);//先向insteadLine插入第一个点
            upPoint = overData[downCount + 1].first.next; //上一行点击的第二个点开始
            downPoint = overData[downCount].first.next;   //下一行点集的第二个点开始
            int downNum = overData[downCount].Size - 1;  //当前进行合并行的剩余点数量
            while (upPoint != null)
            {
                if (downPoint.next == null)
                {//当downPoint为最后一个点时，直接插入insteadLine;
                    upPoint = upPoint.next;
                    insteadLine.Enqueue(downPoint);
                }
                if (Distance2(downPoint.pos, upPoint.pos) >=
                   Distance2(downPoint.next.pos, upPoint.pos))
                {//点集的合并，选择最优点，删除不必要的点
                    downPoint = downPoint.next;
                    downNum--;
                }
                else
                {
                    upPoint = upPoint.next;
                    insteadMin--;
                    insteadLine.Enqueue(downPoint);
                    downPoint = downPoint.next;
                    downNum--;
                }
                if (downNum == insteadMin-1)
                {
                    while (downPoint != null)
                    {
                        insteadLine.Enqueue(downPoint);
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
            insteadLine = new DataList();
            insteadLine.Enqueue(overData[upCount].first);
            upPoint = overData[upCount].first.next;        //上一行点集的第二个点开始
            downPoint = overData[upCount - 1].first.next;  //下一行点集的第二个点开始
            int upNum = overData[upCount].Size - 1;
            while (downPoint != null)
            {
                if (upPoint.next == null)
                {//当upPoint为最后一个点时，直接插入insteadLine;
                    downPoint = downPoint.next;
                    insteadLine.Enqueue(upPoint);
                }
                if(Distance2(upPoint.pos,downPoint.pos)>=
                   Distance2(upPoint.next.pos,downPoint.pos))
                {
                    upPoint = upPoint.next;
                    upNum--;
                }
                else
                {
                    downPoint = downPoint.next;
                    insteadMin--;
                    insteadLine.Enqueue(upPoint);
                    upPoint = upPoint.next;
                    upNum--;
                }
                if(upNum==insteadMin-1)
                {
                    while (upPoint != null)
                    {
                        insteadLine.Enqueue(upPoint);
                        upPoint = upPoint.next;
                        downPoint = downPoint.next;
                    }
                }
            }
            overData[upCount] = insteadLine;
        }
        //将点数据输入到verts中
        for (int i=0;i<overData.Count;i++)
        {
            insteadPoint = overData[i].first;
            while (insteadPoint != null)
            {
                verts.Add(insteadPoint.pos);
                Presscolors.Add(Color.Lerp(Color.blue, Color.red, CalInterpolate(maxPress, minPress, (float)insteadPoint.staticPressure)));
                temperColors.Add(Color.Lerp(Color.blue, Color.red, CalInterpolate(maxTemper, minTemper, (float)insteadPoint.temper)));
                insteadPoint = insteadPoint.next;
            }
        }
    }

    private void NewMergePoint(List<DataList> overData,int mark, bool isDivide)
    {//用于计算端面，mark为1时，下端面；2时，上端面
        int upNum;
        int downNum;
        Vector3 center = overData[0].last.pos;
        Vector3 centerPoint = new Vector3(center.x, 0, 0);
        int count = overData.Count;
        if (isDivide)
            count = overData.Count / 2;
        for (int i = 0; i < count; i++)
        {
            insteadLine = new DataList();
            insteadLine1 = new DataList();
            if (i == overData.Count - 1)
            {//当i为最后一行时，最后一行要与第一行划分三角
                insteadLine.Enqueue(overData[i].first);
                insteadLine1.Enqueue(overData[0].first);
                upPoint = overData[0].first.next;
                downPoint = overData[i].first.next;
                upNum = overData[0].Size - 1;
                downNum = overData[i].Size - 1;
            }
            else
            {
                insteadLine.Enqueue(overData[i].first);      //下行的点
                insteadLine1.Enqueue(overData[i + 1].first); //上行的点
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
                        insteadLine.Enqueue(downPoint);
                    }
                    if (P2PDistance(downPoint.pos, upPoint.pos,centerPoint) >
                       P2PDistance(downPoint.next.pos, upPoint.pos,centerPoint))
                    {//点合并的依据
                        downPoint = downPoint.next;
                        downNum--;
                    }
                    else
                    {
                        insteadLine1.Enqueue(upPoint);
                        upPoint = upPoint.next;
                        upNum--;
                        insteadLine.Enqueue(downPoint);
                        downPoint = downPoint.next;
                        downNum--;
                    }
                    if (downNum == upNum)
                    {
                        while (downPoint != null)
                        {
                            insteadLine.Enqueue(downPoint);
                            insteadLine1.Enqueue(upPoint);
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
                        insteadLine1.Enqueue(upPoint);
                    }
                    if (P2PDistance(upPoint.pos, downPoint.pos,centerPoint)
                      > P2PDistance(upPoint.next.pos, downPoint.pos,centerPoint))
                    {
                        upPoint = upPoint.next;
                        upNum--;
                    }
                    else
                    {
                        insteadLine.Enqueue(downPoint);
                        insteadLine1.Enqueue(upPoint);
                        upPoint = upPoint.next;
                        downPoint = downPoint.next;
                        upNum--;
                        downNum--;
                    }
                    if (downNum == upNum)
                    {
                        while (downPoint != null)
                        {
                            insteadLine.Enqueue(downPoint);
                            insteadLine1.Enqueue(upPoint);
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
                    insteadLine.Enqueue(downPoint);
                    insteadLine1.Enqueue(upPoint);
                    downPoint = downPoint.next;
                    upPoint = upPoint.next;
                }
            }
            AddMerge(insteadLine, insteadLine1, mark);
            downSide = upSide;//面完成后，downSide指向verts的最后位置的后一位
        }
        
    }
    private void AddMerge(DataList lns0,DataList lns1,int mark)
    {
        //点插入verts中,lns0为下行点，lns1为上行点
        upSide += lns0.Size;
        insteadPoint = lns0.first;
        float distance = Vector3.Distance(lns0.last.pos, lns0.oldlast.pos);
        List<int> breakpoint = new List<int>();   //断点，其与下一个点之间的网格不需要画出来
        while(insteadPoint!=null)
        {
            //if(insteadPoint.next!=null && Vector3.Distance(insteadPoint.pos,insteadPoint.next.pos) > distance)
            //  breakpoint.Add(verts.Count);
            verts.Add(insteadPoint.pos);
            Presscolors.Add(Color.Lerp(Color.blue, Color.red, CalInterpolate(maxPress, minPress, (float)insteadPoint.staticPressure)));
            temperColors.Add(Color.Lerp(Color.blue, Color.red, CalInterpolate(maxTemper, minTemper, (float)insteadPoint.temper)));
            insteadPoint = insteadPoint.next;
        }
        insteadPoint = lns1.first;
        while (insteadPoint != null)
        {
            verts.Add(insteadPoint.pos);
            Presscolors.Add(Color.Lerp(Color.blue, Color.red, CalInterpolate(maxPress, minPress, (float)insteadPoint.staticPressure)));
            temperColors.Add(Color.Lerp(Color.blue, Color.red, CalInterpolate(maxTemper, minTemper, (float)insteadPoint.temper)));
            insteadPoint = insteadPoint.next;
        }
        int index0, index1, index2, index3;
        if (mark == 2)//上端面
        {
            for (int i = 0; i < lns0.Size - 1; i++)
            {
                if (breakpoint.Count != 0)
                {//判断该点是不是断点
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
    {//用于外侧面和内侧面的点合并的距离计算
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

    private void CalMaxAndMin()
    {
        List<DataList> datalist = null;
        for (int i = 0; i < data.faceData.Length; i++)
        {
            if (data.faceData[i] == null)
                break;
            datalist = data.faceData[i];
            maxPress = (float)datalist[0].first.staticPressure;
            minPress = (float)datalist[0].first.staticPressure;
            maxTemper = (float)datalist[0].first.temper;
            minTemper = (float)datalist[0].first.temper;
            for (int j = 0; j < datalist.Count; j++)
            {
                Node nowNode = datalist[j].first;
                while (nowNode != null)
                {
                    if (nowNode.staticPressure > maxPress)
                        maxPress = (float)nowNode.staticPressure;
                    if (nowNode.staticPressure < minPress)
                        minPress = (float)nowNode.staticPressure;
                    if (nowNode.temper > maxTemper)
                        maxTemper = (float)nowNode.temper;
                    if (nowNode.temper < minTemper)
                        minTemper = (float)nowNode.temper;
                    nowNode = nowNode.next;
                }
            }
        }
    }
    private float CalInterpolate(float maxPoint, float minPoint, float x)
    {
        float i = maxPoint - minPoint;
        float j = x - minPoint;
        float interpolate = j / i;
        return interpolate;
    }

    public void ChooseTemper()
    {
        choose = 0;
        GetComponent<Renderer>().material = materialTemper;
        SetMaxAndMin(maxTemper, minTemper);
        text6.GetComponent<Text>().text = "单位：℃";
        ChooseAndGenerate();
    }
    public void ChoosePress()
    {
        choose = 1;
        GetComponent<Renderer>().material = materialPress;
        SetMaxAndMin(maxPress, minPress);
        text6.GetComponent<Text>().text = "单位：Pa";
        ChooseAndGenerate();
    }
    private void ChooseAndGenerate()
    {
        if (divide == 1)
        {
            if (fileIndex == fileNames.Count)
            {
                GenerateSubdivision(fileNames[fileNames.Count - 1]);
            }
            else
            {
                GenerateSubdivision(fileNames[fileIndex - 1]);
            }
        }
        else if (divide == 0)
        {
            if (fileIndex == fileNames.Count)
            {
                Generate(fileNames[fileNames.Count - 1]);
            }
            else
            {
                Generate(fileNames[fileIndex - 1]);
            }
        }
    }

    private void SetMaxAndMin(float max, float min)
    {
        float dif = (max - min) / 4;
        text1.GetComponent<Text>().text = max.ToString();
        text2.GetComponent<Text>().text = (max - dif).ToString();
        text3.GetComponent<Text>().text = (max - 2 * dif).ToString();
        text4.GetComponent<Text>().text = (max - 3 * dif).ToString();
        text5.GetComponent<Text>().text = min.ToString();
    }
    public void SpeedControl()
    {
        speed = 1 / speedSlider.value;
    }

    private void DrawLine(List<Node> point)
    {
        line.positionCount = point.Count;
        line.startColor = Color.black;
        line.endColor = Color.black;
        line.startWidth = 10f;
        line.endWidth = 10f;
        for (int i = 0; i < point.Count; i++)
        {
            line.SetPosition(i, point[i].pos);
        }
    }
}
