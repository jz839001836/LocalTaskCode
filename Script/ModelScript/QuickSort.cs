using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

///<summary>
///
///</summary>
class Quick<Item> where Item : IComparable<Item>
{
    private static void RandomArr(Item[] arr)
    {
        System.Random r = new System.Random();
        for (int i = 0; i < arr.Length; i++)
        {
            int index = r.Next(arr.Length);
            Item temp = arr[i];
            arr[i] = arr[index];
            arr[index] = temp;
        }
    }
    public static void Sort(Item[] arr)
    {
        RandomArr(arr);
        Sort(arr, 0, arr.Length - 1);
    }
    private static bool Less(Item v, Item w)
    { return v.CompareTo(w) < 0; }
    private static void Exch(Item[] arr, int i, int j)
    {
        Item t = arr[i];
        arr[i] = arr[j];
        arr[j] = t;
    }
    private static int Partition(Item[] arr, int lo, int hi)
    {
        int i = lo, j = hi + 1;//扫描指针
        Item v = arr[lo];
        while (true)
        {
            while (Less(arr[++i], v)) if (i == hi) break;
            while (Less(v, arr[--j])) if (j == lo) break;
            if (i >= j) break;
            Exch(arr, i, j);
        }
        Exch(arr, lo, j);
        return j;
    }//切分算法
    private static void Sort(Item[] arr, int lo, int hi)
    {
        if (hi <= lo) return;
        int j = Partition(arr, lo, hi);
        Sort(arr, lo, j - 1);
        Sort(arr, j + 1, hi);
    }
}
