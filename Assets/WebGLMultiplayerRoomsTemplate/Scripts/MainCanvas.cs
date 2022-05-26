/****************************** Module Header ******************************\
Module Name:  <Main Canvas Script>
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

// This script handles the main canvas UI

using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainCanvas : MonoBehaviour
{
    public static MainCanvas MainCanvasInstance { get; private set; }

    //References
    [SerializeField] private GameObject usernameInput;

    void Awake()
    {
    }

    void Start()
    {
        if (MainCanvasInstance == null)
        {
            MainCanvasInstance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        TMP_InputField input = usernameInput.GetComponent<TMP_InputField>();
        EventSystem.current.SetSelectedGameObject(usernameInput.gameObject, null);
        input.OnPointerClick(new PointerEventData(EventSystem.current));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            MenuManager.MenuInstance.LoadMultiplayerMenu();
            MultiplayerManager.MultiplayerManagerInstance.JoinMultiplayer();
        }
    }

    public string GetUsername()
    {
        if (usernameInput == null)
        {
            return "TestUsername";
        }
        if (string.IsNullOrEmpty(usernameInput.GetComponent<TMP_InputField>().text))
        {
            return "TestUsername";
        }
        return usernameInput.GetComponent<TMP_InputField>().text;
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
}
