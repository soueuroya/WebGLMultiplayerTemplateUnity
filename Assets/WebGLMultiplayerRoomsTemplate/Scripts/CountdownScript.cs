/****************************** Module Header ******************************\
Module Name:  <Countdown Script>
Project:      <WebGL Socket Multiplayer Template>
Copyright (c) 2022 Daniel Corbi Boldrin

This source is subject to Daniel Corbi Boldrin.
See https://danielboldrin.azurewebsites.net/.
All other rights reserved.

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THIS CODE AND INFORMATION IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
\***************************************************************************/

// This script handles the countdown in the room after the room is started

using TMPro;
using UnityEngine;

public class CountdownScript : MonoBehaviour
{
    public static CountdownScript CountdownInstance;
    private int timer;
    private int initialTime;
    private bool isStarted;
    private Transform cachedTransform;
    private TextMeshProUGUI text;
    void Start()
    {
        if (CountdownInstance == null)
        {
            DontDestroyOnLoad(this.gameObject);
            CountdownInstance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        timer = initialTime = 4;
        cachedTransform = gameObject.transform;
        text = GetComponent<TextMeshProUGUI>();
        isStarted = false;
        gameObject.SetActive(false);
    }

    public void StartCountdown()
    { 
        gameObject.SetActive(true);
        isStarted=true;
        InvokeRepeating("TimeCount", 0, 1);
    }

    private void TimeCount()
    {
        if (isStarted)
        {
            if (timer > 0)
            {
                timer--;
                cachedTransform.localScale = Vector3.one * ((initialTime+1) - timer) / 2;
                text.text = timer.ToString();
            }
            else
            {
                GameManager.GameInstance.LoadMultiplayerGame();
                ResetCountdown();
                gameObject.SetActive(false);
            }
        }
        else
        {
            CancelInvoke("TimeCount");
        }
    }

    public void StopCountdown()
    {
        isStarted = false;
    }

    public void ResetCountdown()
    {
        StopCountdown();
        timer = initialTime;
        text.text = timer.ToString();
    }
}
