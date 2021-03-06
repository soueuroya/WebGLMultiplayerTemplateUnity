/****************************** Module Header ******************************\
Module Name:  <Game Manager Script>
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

// This script handles the game state and it is used to decide different game logics such as loading state logic, on game initialized, started and over logic..
// This script also loads necessary scenes

using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public enum GameState { Loading, Initiated, Started, PlayerDead, Over }
    public GameState gameState;
    public static GameManager GameInstance { get; private set; }

    void Start()
    {
        if (GameInstance == null)
        {
            GameInstance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadMultiplayerGame() // This should be called whenever the room countdown is finished, to load the multiplayer game for all players
    {
        gameState = GameState.Loading;
        MenuManager.MenuInstance.LoadGameMenu();
        SceneManager.LoadScene("Multiplayer"); // Can be level 1, level 2... as long as the levels are ready for multiplayer gameplay.
    }

    public void LoadRoomMenu() // This should be loaded when the game is finished and all the players are returning to the room
    {
        MenuManager.MenuInstance.LoadRoomMenu();
        SceneManager.LoadScene("Menu");
    }

    public void LoadMultiplayerMenu() // this should be called if the player is leaving the game or the owner of the room is disconnected
    {
        MenuManager.MenuInstance.LoadMultiplayerMenu();
        SceneManager.LoadScene("Menu");
    }
}
