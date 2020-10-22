using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

///<summary>
///
///</summary>
public class SwitchScene : MonoBehaviour
{
    public void SwitchTo_2()
    {
        SceneManager.LoadScene("3.14");
    }
}
