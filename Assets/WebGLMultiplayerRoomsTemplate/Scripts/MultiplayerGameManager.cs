/****************************** Module Header ******************************\
Module Name:  <Multiplayer Game Manager Script>
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

//< This script handles the multiplayer game scene start

using UnityEngine;

public class MultiplayerGameManager : MonoBehaviour
{
    void Start()
    {
        if (typeof(MultiplayerManager) != null)
        {
            if (MultiplayerManager.MultiplayerManagerInstance != null)
            {
                MultiplayerManager.MultiplayerManagerInstance.StartGame();
            }
        }
    }
}
