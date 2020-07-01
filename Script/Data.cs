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
using System.Text;
using UnityEngine;

public class Data
{
    string fileName = UnityEditor.EditorUtility.OpenFilePanel("打开文件", "", "pbsi");
    BinaryReader read;
    public int step;            //推移的步数
    public double stepLength;   //推移单位时间步长
    public double judgeSpace;   //推移过程中用来判断的间距
    public double thick;        //装药头部燃去的肉厚
    public int N_sacestep;      //每隔N_savestep-1个推移步保存一次点数据
    private int chargeNameByte; //装药名称字节数
    public string chargeName;   //装药名称
    private int typeNameByte;   //装药类型名称字节数
    public string typeName;     //装药类型名称

    //如果此时读取到的装药类型名称为“自定义”
    //则存储文件中会多出下面大括号内的存储内容，这才是其命名的自定义装药的装药类型名称
    //{整型值（代表装药类型名称的字节数）
    //字符串（装药类型名称）}

    private int propellantTypeNameByte;   //推进剂种类名称字节数
    public string propellantTypeName;     //装药的推进剂种类名称

    //【如果此时读取到的字符串为“其他”
    //则存储文件中会再次多出下面大括号内的存储内容，这是选择其他推进剂后自定义的推进剂种类名称】
    //{整型值（代表装药的推进剂种类名称的字节数）
    //字符串（装药的推进剂种类名称）}

    public double characteristicVelocity; //推进剂特征速度C*
    public double R;                      //气体常数R
    public double density;                //推进剂密度
    public double ratioHeats;             //燃气比热比
    public int burningLaw;                //燃速定律
    public double firstCoefficient;       //燃速公式中的第一个系数
    public double secondCoefficient;      //燃速公式中的第二个系数
    public double Kth;                    //速度侵蚀公式中的侵蚀界限流速Kth
    public double Ky;                     //速度侵蚀公式中的系数Ky
    public int numberOfRoot;              //装药根数
    public double D;                      //燃烧室内直径
    public double length;                 //燃烧室长度
    public double DSpray;                 //喷喉直径
    public double DOutlet;                //喷管出口直径
    public int n_face;                    //装药面数
    //n_face个逻辑值，记录面是否为燃面
    public bool first;                    //前端面
    public bool second;                   //外侧面
    public bool third;                    //后端面
    public bool fourth;                   //内侧面

    Node point;
    DataList datalist;
    public List<DataList>[] faceData;
    byte[] buffer;
    public void Read()
    {
        int faceNum = 0;
        datalist = new DataList();
        faceData = new List<DataList>[4];
        if (File.Exists(fileName))
        {
            using (read = new BinaryReader(new FileStream(fileName, FileMode.Open)))
            {
                step = read.ReadInt32();
                stepLength = read.ReadDouble();
                judgeSpace = read.ReadDouble();
                thick = read.ReadDouble();
                N_sacestep = read.ReadInt32();

                chargeNameByte = read.ReadInt32();
                buffer = new byte[chargeNameByte];
                read.Read(buffer, 0, buffer.Length);
                chargeName = Encoding.Default.GetString(buffer);
            
                typeNameByte = read.ReadInt32();
                buffer = new byte[typeNameByte];
                read.Read(buffer, 0, buffer.Length);
                typeName = Encoding.Default.GetString(buffer);

                propellantTypeNameByte = read.ReadInt32();
                buffer = new byte[propellantTypeNameByte];
                read.Read(buffer, 0, buffer.Length);
                propellantTypeName = Encoding.Default.GetString(buffer);

                characteristicVelocity = read.ReadDouble();
                R = read.ReadDouble();
                density = read.ReadDouble();
                ratioHeats = read.ReadDouble();
                burningLaw = read.ReadInt32();
                firstCoefficient = read.ReadDouble();
                secondCoefficient = read.ReadDouble();
                Kth = read.ReadDouble();
                Ky = read.ReadDouble();
                numberOfRoot = read.ReadInt32();
                D = read.ReadDouble();
                length = read.ReadDouble();
                DSpray = read.ReadDouble();
                DOutlet = read.ReadDouble();
                n_face = read.ReadInt32();
                first = read.ReadBoolean();
                second = read.ReadBoolean();
                third = read.ReadBoolean();
                if (n_face == 4)
                    fourth = read.ReadBoolean();
                faceData[faceNum] = new List<DataList>();
                while (read.BaseStream.Position < read.BaseStream.Length )
                {
                    point = new Node();

                    float a = (float)read.ReadDouble();
                    float b = (float)read.ReadDouble();
                    float c = (float)read.ReadDouble();
                    point.pos = new Vector3(a, c, b);

                    a = (float)read.ReadDouble();
                    b = (float)read.ReadDouble();
                    c = (float)read.ReadDouble();
                    point.nor = new Vector3(a, c, b);

                    point.judgeEdgePoint = read.ReadBoolean();
                    point.judgeDisappear = read.ReadInt16();
                    point.judgeEdge = read.ReadInt16();
                    point.staticPressure = read.ReadDouble();
                    point.flowVelocity = read.ReadDouble();
                    point.density = read.ReadDouble();
                    point.temper = read.ReadDouble();
                    point.burnRate = read.ReadDouble();
                    if (point.judgeEdge == -1)
                    {
                        faceData[faceNum].Add(datalist);
                        datalist = new DataList();
                    }
                    else if (point.judgeEdge == -2)
                    {
                        faceNum++;
                        datalist = new DataList();
                        if (faceNum > n_face - 1)
                            break;
                        faceData[faceNum] = new List<DataList>();
                    }
                    else
                    {
                        datalist.Enqueue(point);
                    }
                }
            }
        }

    }
}
