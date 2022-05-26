/****************************** Module Header ******************************\
Module Name:  <Multiplayer Canvas Script>
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

//< This script handles the multiplayer menu canvas

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MultiplayerCanvas : MonoBehaviour
{
    public static MultiplayerCanvas MultiplayerCanvasInstance { get; private set; }

    //References
    [SerializeField] private GameObject createRoomButton;
    [SerializeField] private GameObject serverResponseObject;
    [SerializeField] private GameObject playersContentObject;
    [SerializeField] private GameObject totalPlayersObject;
    [SerializeField] private GameObject playerRowPrefab;
    [SerializeField] private GameObject totalRoomsObject;
    [SerializeField] private GameObject roomsContentObject;
    [SerializeField] private GameObject roomRowPrefab;

    void Awake()
    {
        if (MultiplayerCanvasInstance == null)
        {
            MultiplayerCanvasInstance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LockButtons()
    {
        foreach (var btn in gameObject.GetComponentsInChildren<Button>(true))
        {
            btn.interactable = false;
        }
    }

    public void UnlockButtons()
    {
        foreach (var btn in gameObject.GetComponentsInChildren<Button>(true))
        {
            btn.interactable = true;
        }
    }

    public void UpdateRooms()
    {
        foreach (Transform child in roomsContentObject.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        foreach (var dict in MultiplayerManager.MultiplayerManagerInstance.networkRooms)
        {
            RoomRow rr = Instantiate(roomRowPrefab, roomsContentObject.transform).GetComponent<RoomRow>();
            dict.Value.thisRoomObject = rr.gameObject;
            int numberOfPlayers = dict.Value.allPlayers.Count;
            if (numberOfPlayers < dict.Value.totalPlayers)
            {
                numberOfPlayers = dict.Value.totalPlayers;
            }
            rr.UpdateRow(dict.Value.roomName, numberOfPlayers, dict.Value.id, dict.Value.isStarted);
        }
        UpdateTotalRooms();
    }

    public void UpdatePlayers()
    {
        foreach (Transform child in playersContentObject.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        foreach (var dict in MultiplayerManager.MultiplayerManagerInstance.networkPlayers)
        {
            PlayerRow pr = Instantiate(playerRowPrefab, playersContentObject.transform).GetComponent<PlayerRow>();
            pr.UpdateRow(dict.Value.playerName, false, "", false);
        }
        UpdateTotalPlayers();
    }

    public void UpdateServerResponse(string response)
    {
        serverResponseObject.GetComponent<TextMeshProUGUI>().text = response;
        Debug.Log(response);
    }

    public void UpdateTotalPlayers()
    {
        totalPlayersObject.GetComponent<TextMeshProUGUI>().text = "Total players: " + MultiplayerManager.MultiplayerManagerInstance.networkPlayers.Count;
    }

    public void UpdateTotalRooms()
    {
        totalRoomsObject.GetComponent<TextMeshProUGUI>().text = "Total rooms: " + MultiplayerManager.MultiplayerManagerInstance.networkRooms.Count;
    }
}
