using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResumeGameButtonScript : MonoBehaviour
{
    public Canvas pauseCanvas;

    public void OnClick()
    {
        pauseCanvas.gameObject.SetActive(false);
    }

}
