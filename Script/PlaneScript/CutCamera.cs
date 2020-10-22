using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
///
///</summary>
public class CutCamera : MonoBehaviour
{
    private GameObject camera1;
    private GameObject camera2;

    private void Start()
    {
        camera1 = GameObject.Find("Camera");
        camera2 = GameObject.Find("Camera1");

        camera1.SetActive(true);
        camera2.SetActive(false);
    }

    private void Update()
    {

    }
    public void GetPanView()
    {
        camera1.SetActive(false);
        camera2.SetActive(true);
    }
    public void GetFollowView()
    {
        camera1.SetActive(true);
        camera2.SetActive(false);
    }
}
