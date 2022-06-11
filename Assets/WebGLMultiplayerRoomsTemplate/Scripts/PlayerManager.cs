/****************************** Module Header ******************************\
Module Name:  <Player Manager Script>
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

//< This script handles network players in multiplayer>

using System.Collections.Generic;
using UnityEngine;

public class PlayerManager
{
	public string id;
	public string playerName;
	public bool isLocalPlayer;
	public float verticalSpeed = 100.0f;
	public float rotateSpeed = 150f;

	private float h;
	private float v;
	private float previousMouseX;
	private float updateTimer;
	private bool forceUpdate;

	private GameObject playerObject;
	private Transform playerTransform;
	private NetworkPlayer playerNetwork;
	private Rigidbody playerRigidbody;

	private string roomId;

	public void SpawnPlayer(Vector3 _position, int _rotation, string name) // spawns the player into the scene
    {
		Debug.Log("SPAWNING PLAYER -" + name + "- AT: R" + _rotation + " - X" + _position.x + " - Y" + _position.y + " - Z" + _position.z);
		if (isLocalPlayer)
		{
			playerObject = MonoBehaviour.Instantiate(Resources.Load("Prefabs/Multiplayer/LocalPlayerPrefab", typeof(GameObject)), _position, Quaternion.Euler(0, _rotation, 0)) as GameObject; // instantiates the local player prefab
			playerNetwork = playerObject.GetComponent<NetworkPlayer>();
			playerNetwork.playerManager = this; // local player receives the reference to the manager to send updates
			updateTimer = 0;
		}
		else
        {
			playerObject = MonoBehaviour.Instantiate(Resources.Load("Prefabs/Multiplayer/NetworkPlayerPrefab", typeof(GameObject)), _position, Quaternion.Euler(0, _rotation, 0)) as GameObject; // instantiates the network player prefab
			playerNetwork = playerObject.GetComponent<NetworkPlayer>();
			playerNetwork.nameTag.text = playerName = name; // network players receive a nameTag
		}
		playerTransform = playerObject.transform; // caching transform
		playerRigidbody = playerObject.GetComponent<Rigidbody>(); // caching rigidbody
	}

	public void Update()
	{
        switch (GameManager.GameInstance.gameState)
        {
            case GameManager.GameState.Loading: WhileLoading(); break;
            case GameManager.GameState.Initiated: WhileInitiated(); break;
            case GameManager.GameState.Started: WhileStarted(); break;
            case GameManager.GameState.PlayerDead: WhilePlayerIsDead(); break;
			case GameManager.GameState.Over: WhileGameOver(); break;
            default:break;
        }
	}
	public void WhileLoading() // Any logic during the loading of the game
    {
    }

	public void WhileInitiated() // Any logic while the players are spawned into the game, but the game haven't started yet (waiting for players to load game or waiting for match to start after a countdown)
	{
	}

	public void WhileStarted() // Any logic while the game is active
    {
		if (isLocalPlayer)
		{
			Move();

			if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Tab))
            {
				PauseCanvas.PauseCanvasInstance.TogglePause();
            }
		}
	}

	public void WhileGameOver() // Any logic while the game is over (game summary...)
    {
	}

	public void WhilePlayerIsDead() // Any logic while the local player is dead (killcam, spectate...)
    {
	}

	public void SetRoomId(string _id)
    {
		roomId = _id;
    }

	public string GetRoomId()
    {
		return roomId;
    }

	void Move()
	{
		if (updateTimer > 0)
		{
			updateTimer -= Time.deltaTime;
		}
		else
        {
			updateTimer = 0.5f; // force updates the player stats every half a second (can be removed and should work as similar but player positions might flicker)
			forceUpdate = true;
        }
		// Store the input axes.
		h = Input.GetAxisRaw("Horizontal");
		v = Input.GetAxisRaw("Vertical");

		//rotate with A/D  left/right (shouldn't be used with move sideways)
		//var y = h * Time.deltaTime * rotateSpeed; 

		// rotate with mouse
		var y = (Input.mousePosition.x - previousMouseX) * Time.deltaTime * rotateSpeed;
		previousMouseX = Input.mousePosition.x;

		// move sideways with A/D  left/right (shouldn't be used with rotation on A/D)
		var x = h * Time.deltaTime * verticalSpeed;

		// move back and forth
		var z = v * Time.deltaTime * verticalSpeed; 

		//set rotation speed
		if (playerRigidbody.angularVelocity != Vector3.up * y)
        {
			forceUpdate = true;
			playerRigidbody.angularVelocity = Vector3.up * y;
		}

		//translate rotation (shouldn't be used with angularVelocity)
		//playerObject.transform.Rotate(0, y, 0);

		//set velocity
		if (playerRigidbody.velocity != playerTransform.right * x + playerTransform.forward * z + playerTransform.up * playerRigidbody.velocity.y) // simply detect any differences in speed and updates the player
		{
			forceUpdate = true;
			playerRigidbody.velocity = playerTransform.right * x + playerTransform.forward * z + playerTransform.up * playerRigidbody.velocity.y; // move player and preserve Y axis velocity
		}

		//translate movement (shouldn't be used with velocity)
		//playerObject.transform.Translate(0, 0, z); 

		if (forceUpdate) // This only updates if player moves (either by being controlled or pushed by other players)
		{
			forceUpdate = false;
			UpdateStatusToServer();
		}
	}

	void UpdateStatusToServer()
	{
		Dictionary<string, string> data = new Dictionary<string, string>();
		data["local_player_id"] = id;
		data["position"] = playerTransform.position.x + ":" + playerTransform.position.y + ":" + playerTransform.position.z;
		data["rotation"] = playerRigidbody.angularVelocity.y.ToString();
		data["velocity"] = playerRigidbody.velocity.x + ":" + playerRigidbody.velocity.y + ":" + playerRigidbody.velocity.z;
		MultiplayerManager.MultiplayerManagerInstance.MoveAndRotate(data);
	}

	public void UpdatePosition(float x, float y, float z)
	{
		playerTransform.position = Vector3.right * x + Vector3.up * y + Vector3.forward * z;
	}

	public void UpdateVelocity(float x, float y, float z)
	{
		playerRigidbody.velocity = Vector3.right * x + Vector3.up * y + Vector3.forward * z;
	}

	public void UpdateRotation(float _rotationSpeed)
	{
		playerRigidbody.angularVelocity = Vector3.up * _rotationSpeed;
	}
}