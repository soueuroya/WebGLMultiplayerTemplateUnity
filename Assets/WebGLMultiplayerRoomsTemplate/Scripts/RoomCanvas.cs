/****************************** Module Header ******************************\
Module Name:  <Room Canvas Script>
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

//< This script handles the room canvas UI>

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomCanvas : MonoBehaviour
{
    public static RoomCanvas RoomCanvasInstance { get; private set; }

    //References
    [SerializeField] private GameObject startGameButton;
    [SerializeField] private GameObject closeRoomButton;
    [SerializeField] private GameObject readyButton;
    [SerializeField] private GameObject unreadyButton;
    [SerializeField] private GameObject leaveButton;
    [SerializeField] private GameObject serverResponseObject;
    [SerializeField] private GameObject playersContentObject;
    [SerializeField] private GameObject playerRowPrefab;
    [SerializeField] private GameObject totalPlayersObject;

    private void Start()
    {
        if (RoomCanvasInstance == null)
        {
            RoomCanvasInstance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void UnlockButtons()
    {
        foreach (var btn in gameObject.GetComponentsInChildren<Button>(true))
        {
            btn.interactable = true;
        }
    }
    public void LockButtons()
    {
        foreach (var btn in gameObject.GetComponentsInChildren<Button>(true))
        {
            btn.interactable = false;
        }
    }

    //START
    public void UnlockStart(){startGameButton.GetComponent<Button>().interactable = true; }
    public void LockStart() { startGameButton.GetComponent<Button>().interactable = false; }
    public void HideStart() { startGameButton.SetActive(false); }
    public void ShowStart() { startGameButton.SetActive(true); }

    //READY
    public void UnlockReady(){readyButton.GetComponent<Button>().interactable = true;}
    public void LockReady() { readyButton.GetComponent<Button>().interactable = false;}
    public void HideReady() { readyButton.SetActive(false); }
    public void ShowReady() { readyButton.SetActive(true); }

    //UNREADY
    public void UnlockUnready(){unreadyButton.GetComponent<Button>().interactable = true;}
    public void LockUnready() { unreadyButton.GetComponent<Button>().interactable = false;}
    public void HideUnready() { unreadyButton.SetActive(false); }
    public void ShowUnready() { unreadyButton.SetActive(true); }

    //CLOSE
    public void UnlockClose(){closeRoomButton.GetComponent<Button>().interactable = true; }
    public void LockClose() { closeRoomButton.GetComponent<Button>().interactable = false; }
    public void HideClose() { closeRoomButton.SetActive(false); }
    public void ShowClose() { closeRoomButton.SetActive(true); }

    //LEAVE
    public void UnlockLeave(){leaveButton.GetComponent<Button>().interactable = true; }
    public void LockLeave() { leaveButton.GetComponent<Button>().interactable = false; }
    public void HideLeave() { leaveButton.SetActive(false); }
    public void ShowLeave() { leaveButton.SetActive(true); }

    private void ShowOwnerOptions(bool isReady)
    {
        ShowStart();
        ShowClose();
        UnlockClose();
        LockLeave();
        HideLeave();

        if (isReady) // if player is in ready state, hide ready and show unready
        {
            HideReady();
            LockReady();
            UnlockUnready();
            ShowUnready();
        }
        else
        {
            ShowReady();
            UnlockReady();
            LockUnready();
            HideUnready();
        }
    }

    private void ShowGuestOptions(bool isReady)
    {
        HideStart();
        HideClose();
        LockClose();
        ShowLeave();
        UnlockLeave();
        
        if (isReady) // if player is in ready state, hide ready and show unready
        {
            HideReady();
            LockReady();
            UnlockUnready();
            ShowUnready();
        }
        else
        {
            ShowReady();
            UnlockReady();
            LockUnready();
            HideUnready();
        }
    }

    public void UpdateServerResponse(string response)
    {
        serverResponseObject.GetComponent<TextMeshProUGUI>().text = response;
        Debug.Log(response);
    }

    public void UpdatePlayers(RoomManager rm)
    {
        foreach (Transform child in playersContentObject.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        foreach (var playerid in rm.allPlayers)
        {
            PlayerRow pr = Instantiate(playerRowPrefab, playersContentObject.transform).GetComponent<PlayerRow>();
            bool kickable = false;
            if (MultiplayerManager.MultiplayerManagerInstance.local_player_id == rm.id)                                     // if client is owner of the room
            {
                ShowOwnerOptions(rm.confirmedPlayers.Contains(MultiplayerManager.MultiplayerManagerInstance.local_player_id));
                if (playerid != MultiplayerManager.MultiplayerManagerInstance.local_player_id)                              // if this is not the owner: Add kick option
                {
                    kickable = true;
                }
                if (rm.allPlayers.Count > 1 && rm.allPlayers.Count == rm.confirmedPlayers.Count)                            // Room is ready unlock start button
                {
                    UnlockStart();
                }
                else
                {
                    LockStart();
                }
            }
            else
            {
                ShowGuestOptions(rm.confirmedPlayers.Contains(MultiplayerManager.MultiplayerManagerInstance.local_player_id));
            }
            pr.UpdateRow(MultiplayerManager.MultiplayerManagerInstance.networkPlayers[playerid].playerName, kickable, playerid, rm.confirmedPlayers.Contains(playerid));
        }
        UpdateTotalPlayers(rm.allPlayers.Count);
    }

    public void UpdateTotalPlayers(int count)
    {
        totalPlayersObject.GetComponent<TextMeshProUGUI>().text = "Total players: " + count;
    }
}
