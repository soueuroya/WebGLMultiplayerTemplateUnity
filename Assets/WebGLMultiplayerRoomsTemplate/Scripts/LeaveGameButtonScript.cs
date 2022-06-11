using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LeaveGameButtonScript : MonoBehaviour
{
    public void OnClick()
    {
        MultiplayerManager.MultiplayerManagerInstance.LeaveRoom(); // Just emits leave room. The multiplayer manager will update the situation to the other players.
        SceneManager.LoadScene("Menu"); // Loads the menu scene.
    }
}
