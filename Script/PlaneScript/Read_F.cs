using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

///<summary>
///
///</summary>
public class Read_F
{
    string fileName;
    public List<int> step;
    public List<float> time;
    public List<float> F;
    string str;
    StreamReader read;
    string[] stringArray;

    public Read_F(string file)
    {
        fileName = file;
        step = new List<int>();
        time = new List<float>();
        F = new List<float>();
    }
    public void Read()
    {
        if (File.Exists(fileName))
        {
            using(read = new StreamReader(new FileStream(fileName, FileMode.Open)))
            {
                str = read.ReadLine();
                str = read.ReadLine();
                while (str != null)
                {
                    stringArray = str.Split('\t');
                    step.Add(int.Parse(stringArray[0]));
                    time.Add(float.Parse(stringArray[1]));
                    F.Add(float.Parse(stringArray[2]));
                    str = read.ReadLine();
                }
            }
        }
    }
}
