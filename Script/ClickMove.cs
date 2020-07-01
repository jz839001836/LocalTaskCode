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

public class ClickMove : MonoBehaviour
{
    private float offsetX = 0;
    private float offsetY = 0;
    public float speed = 6f;
    public float sensitivityMouse = 2f;
    public float sensitivetyKeyBoard = 1f;
    public float sensitivetyMouseWheel = 10f;

    void Update()
    {
        if(Input.GetAxis("Mouse ScrollWheel") < 0 )
        {
            if (Camera.main.fieldOfView <= 100)
                Camera.main.fieldOfView += 5;
            else if (Camera.main.fieldOfView <= 20)
                Camera.main.fieldOfView += 0.5f;
        }
        if(Input.GetAxis("Mouse ScrollWheel")>0)
        {
            if (Camera.main.fieldOfView > 2)
                Camera.main.fieldOfView -= 5;
            else if (Camera.main.fieldOfView >= 1)
                Camera.main.fieldOfView -= 0.5f;
        }
        if(Input.GetMouseButton(0))
        {
            offsetX = Input.GetAxis("Mouse X");
            offsetY = Input.GetAxis("Mouse Y");
            transform.Rotate(new Vector3(offsetY, -offsetX, 0) * speed, Space.World);
        }
    }
    void MouseMove()
    {

        //键盘按钮←/a和→/d实现视角水平移动，键盘按钮↑/w和↓/s实现视角水平旋转
        if (Input.GetAxis("Horizontal") != 0)
        {
            transform.Translate(Input.GetAxis("Horizontal") * sensitivetyKeyBoard, 0, 0);
        }
        if (Input.GetAxis("Vertical") != 0)
        {
            transform.Translate(0, Input.GetAxis("Vertical") * sensitivetyKeyBoard, 0);
        }
    }
    //private IEnumerator OnMouseDown()
    //{
    //    Vector3 screenSpace = Camera.main.WorldToScreenPoint(transform.position);
    //    Vector3 offset = transform.position - Camera.main.ScreenToViewportPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenSpace.z));
    //    while(Input.GetMouseButton(0))
    //    {
    //        Vector3 curScreenSpace = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenSpace.z);
    //        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenSpace) + offset;
    //        transform.position = curPosition;
    //        yield return new WaitForFixedUpdate();
    //    }
    //}
}
