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
    void Start()
    {
        
    }

    void Update()
    {
        if(Input.GetAxis("Mouse ScrollWheel") < 0 )
        {
            if (Camera.main.fieldOfView <= 100)
                Camera.main.fieldOfView += 2;
            else if (Camera.main.fieldOfView <= 20)
                Camera.main.fieldOfView += 0.5f;
        }
        if(Input.GetAxis("Mouse ScrollWheel")>0)
        {
            if (Camera.main.fieldOfView > 2)
                Camera.main.fieldOfView -= 2;
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
