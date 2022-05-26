/****************************** Module Header ******************************\
Module Name:  <Menu Manager Script>
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

// This script handles the menu UIs

using UnityEngine;

public class MenuManager : MonoBehaviour
{
    enum Menu {Main, Multiplayer, Single, Room, Game }
    private Menu currentMenu;
    public static MenuManager MenuInstance { get; private set; }

    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject multiplayerMenu;
    [SerializeField] private GameObject roomMenu;
    [SerializeField] private GameObject singleMenu;

    [SerializeField] private bool loadMainMenu;

    void Awake()
    {
        if (MenuInstance == null)
        {
            MenuInstance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (loadMainMenu)
        {
            LoadMainMenu();
        }
    }

    public void LoadMultiplayerMenu()
    {
        CloseCurrentMenu();
        currentMenu = Menu.Multiplayer;
        multiplayerMenu.SetActive(true);
    }

    public void LoadRoomMenu()
    {
        CloseCurrentMenu();
        currentMenu = Menu.Room;
        roomMenu.SetActive(true);
    }

    public void LoadGameMenu()
    {
        CloseCurrentMenu();
        currentMenu = Menu.Game;
        //gameMenu.SetActive(true);
    }

    public void LoadSingleMenu()
    {
        CloseCurrentMenu();
        currentMenu = Menu.Single;
        singleMenu.SetActive(true);
    }

    public void LoadMainMenu()
    {
        CloseCurrentMenu();
        currentMenu = Menu.Main;
        mainMenu.SetActive(true);
    }

    public void CloseCurrentMenu()
    {
        switch (currentMenu)
        {
            case Menu.Main:
                mainMenu.SetActive(false);
                break;
            case Menu.Multiplayer:
                multiplayerMenu.SetActive(false);
                break;
            case Menu.Single:
                singleMenu.SetActive(false);
                break;
            case Menu.Room:
                roomMenu.SetActive(false);
                break;
            case Menu.Game:
                //gameMenu.SetActive(false);
                break;
            default:
                break;
        }
    }
}
