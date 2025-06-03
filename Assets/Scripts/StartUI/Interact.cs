using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Interact : MonoBehaviour
{
    public void SceneChange()
    {
        SceneManager.LoadScene(1);
    }
}
