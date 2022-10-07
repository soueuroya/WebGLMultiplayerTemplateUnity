using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResumeGameButtonScript : MonoBehaviour
{
    public Canvas pauseCanvas;

    public void OnClick()
    {
        Debug.Log("CLICKED");
        pauseCanvas.gameObject.SetActive(false);
    }

}
