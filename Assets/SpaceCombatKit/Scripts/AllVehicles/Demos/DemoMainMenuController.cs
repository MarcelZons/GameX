using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class DemoMainMenuController : MonoBehaviour
{

	public void StartScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }
}
