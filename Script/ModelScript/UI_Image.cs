using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

///<summary>
///
///</summary>
[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter), typeof(MeshCollider))]
public class UI_Image : MonoBehaviour
{
    //public RectTransform m_recttransform;
    //public Camera camera;
    //Vector3 upperLeft, upperRight, lowerLeft, lowerRight;
    //private Mesh mesh;
    //List<Vector3> vert;
    //List<int> indices;
    //List<Color> colors;
    //MeshCollider meshCollider;
    //MeshFilter meshFilter;
    //MeshRenderer meshRenderer;

    private void Start()
    {

    }
    //private void Initialization()
    //{
    //    m_recttransform = GetComponent<RectTransform>();
    //    vert = new List<Vector3>();
    //    indices = new List<int>();
    //    colors = new List<Color>();
    //    mesh = new Mesh();
    //    meshCollider = GetComponent<MeshCollider>();
    //    meshFilter = GetComponent<MeshFilter>();
    //}
    //private void DrawGradient()
    //{
    //    mesh.SetVertices(vert);
    //    mesh.triangles = indices.ToArray();
    //    mesh.colors = colors.ToArray();
    //    mesh.RecalculateNormals();
    //    mesh.RecalculateBounds();
    //    meshFilter.mesh = mesh;
    //    meshCollider.sharedMesh = mesh;
    //}
    //private void VertAndTriangle()
    //{
    //    vert.Add(upperLeft);
    //    vert.Add(upperRight);
    //    vert.Add(lowerLeft);
    //    vert.Add(lowerRight);
    //    indices.Add(0);
    //    indices.Add(3);
    //    indices.Add(2);
    //    indices.Add(0);
    //    indices.Add(1);
    //    indices.Add(3);
    //    colors.Add(Color.red);
    //    colors.Add(Color.red);
    //    colors.Add(Color.blue);
    //    colors.Add(Color.blue);
    //}
    //private void SetPoint()
    //{
    //    int width = Screen.width;
    //    int heigh = Screen.height;
    //    upperLeft = new Vector3(0, heigh * 3 / 4, 0);
    //    upperRight = new Vector3(100, heigh * 3 / 4, 0);
    //    lowerLeft = new Vector3(0, heigh * 1 / 4, 0);
    //    lowerRight = new Vector3(100, heigh * 1 / 4, 0);
    //}
    //private Vector2 WorldToUGUIPostion(RectTransform canvasRectTransform, Camera camera, Vector3 worldPosition)
    //{
    //    Vector2 viewPos = camera.WorldToViewportPoint(worldPosition);
    //    return new Vector2(canvasRectTransform.rect.width * viewPos.x, canvasRectTransform.rect.height * viewPos.y);
    //}
}
