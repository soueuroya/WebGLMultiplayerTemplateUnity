/****************************** Module Header ******************************\
Module Name:  <Room Row Script>
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

// This script handles the room row UI element

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomRow : MonoBehaviour
{
    //References
    [SerializeField] private GameObject roomNameObject;
    [SerializeField] private GameObject totalPlayersObject;
    [SerializeField] private GameObject joinButtonObject;

    public void UpdateRow(string name, int totalPlayers, string roomID, bool isStarted)
    {
        if (roomNameObject != null)
        {
            if (!string.IsNullOrEmpty(name))
            {
                roomNameObject.GetComponent<TextMeshProUGUI>().text = name;
            }
            else
            {
                roomNameObject.GetComponent<TextMeshProUGUI>().text = "no-name";
            }
        }
        if (totalPlayersObject != null)
        {
            totalPlayersObject.GetComponent<TextMeshProUGUI>().text = totalPlayers+"/4";
        }
        if (joinButtonObject != null)
        {
            if (totalPlayers < 4 && !isStarted)
            {
                joinButtonObject.GetComponent<Button>().onClick.AddListener(delegate { MultiplayerManager.MultiplayerManagerInstance.JoinRoom(roomID); });
                joinButtonObject.GetComponent<Button>().interactable = true;
            }
            else
            {
                joinButtonObject.GetComponent<Button>().onClick.RemoveListener(delegate { MultiplayerManager.MultiplayerManagerInstance.JoinRoom(roomID); });
                joinButtonObject.GetComponent<Button>().interactable = false;
            }
        }
    }
}
