using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
///
///</summary>
public class FollowTarget : MonoBehaviour
{
    //游戏人物的位置
    public Transform player;
    //游戏人物与相机的差
    private Vector3 offset;
    //相机的速度
    private float smoothing = 10;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        offset = transform.position - player.position;
    }

    private void LateUpdate()
    {
        //世界坐标转换为局部坐标
        Vector3 targetPosition = player.position + player.TransformDirection(offset);
        //计算相机位置和目标位置的插值
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smoothing);
        //相机的目标看向游戏人物
        transform.LookAt(player.position);
    }
}
