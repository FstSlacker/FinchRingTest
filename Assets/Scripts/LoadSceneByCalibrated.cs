using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneByCalibrated : MonoBehaviour
{
    public bool isCalibrated = false;
    private bool isLoadScene = false;
    public string sceneName = "MainScene";
    void Update()
    {
        if (!isCalibrated && !Finch.FinchCalibration.IsCalibrating)
        {
            isCalibrated = true;
            isLoadScene = true;
        }
            
        if (isLoadScene)
        {
            Debug.Log("Load scene");
            LoadScene();
            isLoadScene = false;
            
        }
    }
    private void LoadScene()
    {
        SceneManager.LoadScene(sceneName);
    }
}
