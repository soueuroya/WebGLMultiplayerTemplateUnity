/****************************** Module Header ******************************\
Module Name:  <Multiplayer Manager Script>
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

//< This script handles sending and receiving msgs to/from the server>

using System.Collections.Generic;
using UnityEngine;

public class MultiplayerManager : MonoBehaviour
{
	public static MultiplayerManager MultiplayerManagerInstance;

	static private readonly char[] Delimiter = new char[] { ':' };

	[HideInInspector]
	public string local_player_id;

	public Dictionary<string, PlayerManager> networkPlayers = new Dictionary<string, PlayerManager>();
	public Dictionary<string, RoomManager> networkRooms = new Dictionary<string, RoomManager>();

	[Header("Local Player Prefab")]
	[SerializeField]
	private GameObject localPlayerPrefab;

	[Header("Network Player Prefab")]
	[SerializeField]
	private GameObject networkPlayerPrefab;

	[Header("Player Spawn Points")]
	[SerializeField]
	private Transform[] spawnPoints;

	[Header("Camera Prefab")]
	[SerializeField]
	private GameObject camPref;

	[HideInInspector]
	public GameObject camRig;

	[HideInInspector]
	public bool isGameStarted;
	private bool isGameOver;

	#region Initialization
	void Awake()
	{	
	}
	void Start()
	{
		if (MultiplayerManagerInstance == null)
		{
			DontDestroyOnLoad(this.gameObject);
			MultiplayerManagerInstance = this;
		}
		else
		{
			Destroy(this.gameObject);
		}
		//localPlayer = new PlayerManager();
		Application.ExternalEval("socket.isReady = true;");
	}
	#endregion

	#region Ping-Pong
	public void EmitPing()																													// SEND PING
	{
		Dictionary<string, string> data = new Dictionary<string, string>();
		data["msg"] = "ping!";
		Application.ExternalCall("socket.emit", "PING", new JSONObject(data));
	}

	public void OnPrintPongMsg(string data)																									// RECEIVING PONG
	{
		var pack = data.Split(Delimiter); // [0] pongMsg
		//MultiplayerCanvas.MultiplayerCanvasInstance.UpdateServerResponse("ON PONG MSG: " + pack[0]);
	}
	#endregion

	#region Multiplayer Player Management - Join multiplayer and player incoming
	public void JoinMultiplayer()																											// SEND LOCAL PLAYER CREATE REQUEST
	{
		//MultiplayerCanvas.MultiplayerCanvasInstance.UpdateServerResponse("INNITIALIZING JOIN_MULTIPLAYER");

		Dictionary<string, string> data = new Dictionary<string, string>();
		data["name"]  = MainCanvas.MainCanvasInstance.GetUsername();
		Application.ExternalCall("socket.emit", "JOIN_MULTIPLAYER", new JSONObject(data));

		//MultiplayerCanvas.MultiplayerCanvasInstance.UpdateServerResponse("JOIN_MULTIPLAYER SENT");
	}

	public void OnJoinMultiplayer(string data)																								// RECEVEING LOCAL PLAYER
	{
		//MultiplayerCanvas.MultiplayerCanvasInstance.UpdateServerResponse("ON JOIN_MULTIPLAYER_SUCCESS: " + data);

		var pack = data.Split(Delimiter); // [0] playerId - [1] playerName
		networkPlayers[pack[0]] = new PlayerManager();
		networkPlayers[pack[0]].id = pack[0];
		networkPlayers[pack[0]].playerName = pack[1];
		networkPlayers[pack[0]].isLocalPlayer = true;
		networkPlayers[pack[0]].SetRoomId("");
		local_player_id = pack[0];
		MultiplayerCanvas.MultiplayerCanvasInstance.UpdatePlayers();
		MultiplayerCanvas.MultiplayerCanvasInstance.UnlockButtons();

		//MultiplayerCanvas.MultiplayerCanvasInstance.UpdateServerResponse("MULTIPLAYER UPDATED");
	}

	public void OnPlayerIncoming(string data)																								// RECEIVING PLAYER FROM SERVER
	{
		//MultiplayerCanvas.MultiplayerCanvasInstance.UpdateServerResponse("ON PLAYER INCOMING: " + data);

		var pack = data.Split(Delimiter); // [0] playerId - [1] playerName
		networkPlayers[pack[0]] = new PlayerManager();
		networkPlayers[pack[0]].id = pack[0];
		networkPlayers[pack[0]].playerName = pack[1];
		networkPlayers[pack[0]].isLocalPlayer = false;
		MultiplayerCanvas.MultiplayerCanvasInstance.UpdatePlayers();

		//MultiplayerCanvas.MultiplayerCanvasInstance.UpdateServerResponse("INCOMING PLAYER UPDATED");
	}
	#endregion

	#region Multiplayer Room Management - Create and close room.
	public void CreateRoom()																												// SEND CREATE REQUEST TO SERVER
	{
		//MultiplayerCanvas.MultiplayerCanvasInstance.UpdateServerResponse("CREATING ROOM");

		Dictionary<string, string> data = new Dictionary<string, string>();
		data["id"] = local_player_id; // setting room id the same as player id.
		Application.ExternalCall("socket.emit", "CREATE_ROOM", new JSONObject(data));

		//MultiplayerCanvas.MultiplayerCanvasInstance.UpdateServerResponse("SENT CREATE ROOM");
	}

	public void OnCreateRoom(string data)																									// RECEIVING LOCAL CREATED ROOM
	{
		//MultiplayerCanvas.MultiplayerCanvasInstance.UpdateServerResponse("ON CREATE ROOM SUCCESS: " + data);

		var pack = data.Split(Delimiter);  // [0] roomId - [1] roomName
		networkRooms[pack[0]] = new RoomManager();
		networkRooms[pack[0]].id = pack[0];
		networkRooms[pack[0]].roomName = pack[1] + "-" + networkPlayers[local_player_id].playerName; // Creating temporary room name
		networkRooms[pack[0]].allPlayers.Add(local_player_id);
		networkRooms[pack[0]].totalPlayers = 1;
		networkPlayers[local_player_id].SetRoomId(pack[0]);
		RoomCanvas.RoomCanvasInstance.UpdatePlayers(networkRooms[pack[0]]);
		MenuManager.MenuInstance.LoadRoomMenu();

		//MultiplayerCanvas.MultiplayerCanvasInstance.UpdateServerResponse("UPDATED ROOMS");
	}

	public void OnRoomIncoming(string data)																									// RECEVEING ROOM FROM OTHER PLAYERS
	{
		//MultiplayerCanvas.MultiplayerCanvasInstance.UpdateServerResponse("ON ROOM INCOMING: " + data);

		var pack = data.Split(Delimiter); // [0] roomId - [1] ownerId - [2] roomName - [3] totalPlayers - [4] isStarted
		networkRooms[pack[0]] = new RoomManager();
		networkRooms[pack[0]].id = pack[0];
		networkRooms[pack[0]].roomName = pack[2] + "-" + networkPlayers[pack[1]].playerName;
		networkRooms[pack[0]].allPlayers.Add(pack[1]);
		networkRooms[pack[0]].totalPlayers = int.Parse(pack[3]);
		networkRooms[pack[0]].isStarted = (pack[4] == "true");
		networkPlayers[pack[0]].SetRoomId(pack[0]);
		MultiplayerCanvas.MultiplayerCanvasInstance.UpdateRooms();

		//MultiplayerCanvas.MultiplayerCanvasInstance.UpdateServerResponse("UPDATED ROOMS");
	}

	public void CloseRoom()																													// SEND CLOSE ROOM REQUEST
	{
		//MultiplayerCanvas.MultiplayerCanvasInstance.UpdateServerResponse("CLOSING ROOM");

		Dictionary<string, string> data = new Dictionary<string, string>();
		data["id"] = local_player_id;
		Application.ExternalCall("socket.emit", "CLOSE_ROOM", new JSONObject(data));

		//MultiplayerCanvas.MultiplayerCanvasInstance.UpdateServerResponse("SENT CLOSE ROOM");
	}

	public void OnCloseRoom(string data)																									// RECEIVING ROOM CLOSE FROM SERVER - BOTH LOCAL AND NETWORK
	{
		//MultiplayerCanvas.MultiplayerCanvasInstance.UpdateServerResponse("ON CLOSE ROOM: " + data);

		var pack = data.Split(Delimiter); // [0] roomId
		if (networkRooms.ContainsKey(pack[0])) // if room exists
		{
			if (networkPlayers[local_player_id].GetRoomId() == pack[0]) // if local player is in the room
			{
				networkPlayers[local_player_id].SetRoomId(null); // set room to null
				MenuManager.MenuInstance.LoadMultiplayerMenu(); // load the multiplayer canvas
				MultiplayerCanvas.MultiplayerCanvasInstance.UnlockButtons(); // unlock multiplayer buttons
				if (isGameStarted)
				{
					isGameStarted = true;
					GameManager.GameInstance.LoadMultiplayerMenu();
				}
			}
			Destroy(networkRooms[pack[0]].thisRoomObject);
			networkRooms.Remove(pack[0]);
			MultiplayerCanvas.MultiplayerCanvasInstance.UpdateRooms(); // update the rooms list
		}
		else
		{
			RoomCanvas.RoomCanvasInstance.UpdateServerResponse("Room not found!");
		}

		//MultiplayerCanvas.MultiplayerCanvasInstance.UpdateServerResponse("ON CLOSE ROOM SUCCESS");
	}
	#endregion

	#region Multiplayer Room Joining
	public void JoinRoom(string roomID)																										// SEND JOIN ROOM REQUEST TO SERVER
	{
		//MultiplayerCanvas.MultiplayerCanvasInstance.UpdateServerResponse("JOINING ROOM");

		Dictionary<string, string> data = new Dictionary<string, string>();
		data["room"] = roomID;
		data["player"] = local_player_id;
		Application.ExternalCall("socket.emit", "JOIN_ROOM", new JSONObject(data));
		MultiplayerCanvas.MultiplayerCanvasInstance.LockButtons();

		//MultiplayerCanvas.MultiplayerCanvasInstance.UpdateServerResponse("SENT JOIN ROOM");
	}

	public void OnJoinRoom(string data)																										// RECEIVING PLAYER JOIN ROOM FROM SERVER - BOTH LOCAL AND NETWORK
	{
		//MultiplayerCanvas.MultiplayerCanvasInstance.UpdateServerResponse("ON JOIN ROOM: D:" + data);

		var pack = data.Split(Delimiter); // [0] roomId - [1] playerId
		networkRooms[pack[0]].allPlayers.Add(pack[1]);
		Debug.Log("SETTING ROOM ID: " + pack[0] + " TO PLAYER: " + pack[1]);
		networkPlayers[pack[1]].SetRoomId(pack[0]);
		if (local_player_id == pack[1]) // IF LOCAL PLAYER JOINED ROOM
		{
			MenuManager.MenuInstance.LoadRoomMenu();
			RoomCanvas.RoomCanvasInstance.UpdatePlayers(networkRooms[pack[0]]);
		}
		else if (networkPlayers[local_player_id].GetRoomId() == pack[0]) // IF CURRENT PLAYER IS IN ROOM
		{
			RoomCanvas.RoomCanvasInstance.UpdatePlayers(networkRooms[pack[0]]);
		}
		else // IF CURRENT PLAYER IS OUTSIDE THE ROOM
		{
			MultiplayerCanvas.MultiplayerCanvasInstance.UpdateRooms();
		}

		//MultiplayerCanvas.MultiplayerCanvasInstance.UpdateServerResponse("JOIN ROOM SUCCESS");
	}
	#endregion

	#region Multiplayer Room Ready Unready
	public void ReadyRoom()																													// SEND READY REQUEST TO SERVER
	{
		//MultiplayerCanvas.MultiplayerCanvasInstance.UpdateServerResponse("READYING ROOM");

		Dictionary<string, string> data = new Dictionary<string, string>();
		data["room"] = networkPlayers[local_player_id].GetRoomId();
		data["player"] = local_player_id;
		Application.ExternalCall("socket.emit", "READY_ROOM", new JSONObject(data));
		MultiplayerCanvas.MultiplayerCanvasInstance.LockButtons();

		//MultiplayerCanvas.MultiplayerCanvasInstance.UpdateServerResponse("SENT READY ROOM");
	}
	public void OnReadyRoom(string data)																									// RECEIVING ROOM READY FROM SERVER - BOTH LOCAL AND NETWORK
	{
		//MultiplayerCanvas.MultiplayerCanvasInstance.UpdateServerResponse("ON READY ROOM: D:" + data);

		var pack = data.Split(Delimiter); // [0] roomId - [1] playerId
		if (!networkRooms[pack[0]].confirmedPlayers.Contains(pack[1]))
		{
			networkRooms[pack[0]].confirmedPlayers.Add(pack[1]);
		}
		if (networkPlayers[local_player_id].GetRoomId() == pack[0])
		{
			RoomCanvas.RoomCanvasInstance.UpdatePlayers(networkRooms[pack[0]]);
		}

		//MultiplayerCanvas.MultiplayerCanvasInstance.UpdateServerResponse("READY ROOM SUCCESS");
	}
	public void UnreadyRoom()																												// SEND UNREADY REQUEST TO SERVER
	{
		//MultiplayerCanvas.MultiplayerCanvasInstance.UpdateServerResponse("UNREADYING ROOM");

		Dictionary<string, string> data = new Dictionary<string, string>();
		data["room"] = networkPlayers[local_player_id].GetRoomId();
		data["player"] = local_player_id;
		Application.ExternalCall("socket.emit", "UNREADY_ROOM", new JSONObject(data));
		MultiplayerCanvas.MultiplayerCanvasInstance.LockButtons();

		//MultiplayerCanvas.MultiplayerCanvasInstance.UpdateServerResponse("SENT UNREADY ROOM");
	}
	public void OnUnreadyRoom(string data)																									// RECEIVING ROOM UNREADY FROM SERVER - BOTH LOCAL AND NETWORK
	{
		//MultiplayerCanvas.MultiplayerCanvasInstance.UpdateServerResponse("ON READY ROOM: D:" + data);

		var pack = data.Split(Delimiter); // [0] roomId - [1] playerId
		if (networkRooms[pack[0]].confirmedPlayers.Contains(pack[1]))
		{
			networkRooms[pack[0]].confirmedPlayers.Remove(pack[1]);
		}
		if (networkPlayers[local_player_id].GetRoomId() == pack[0])
		{
			RoomCanvas.RoomCanvasInstance.UpdatePlayers(networkRooms[pack[0]]);
		}

		//MultiplayerCanvas.MultiplayerCanvasInstance.UpdateServerResponse("READY ROOM SUCCESS");
	}
	#endregion

	#region Multiplayer Room Leaving - LeaveRoom and KickPlayer
	public void LeaveRoom()																													// SEND LEAVE ROOM REQUEST 
	{
		//MultiplayerCanvas.MultiplayerCanvasInstance.UpdateServerResponse("LEAVING ROOM");

		Dictionary<string, string> data = new Dictionary<string, string>();
		data["room"] = networkPlayers[local_player_id].GetRoomId();
		data["player"] = local_player_id;
		Application.ExternalCall("socket.emit", "LEAVE_ROOM", new JSONObject(data));

		//MultiplayerCanvas.MultiplayerCanvasInstance.UpdateServerResponse("SENT LEAVE ROOM");
	}
	public void OnLeaveRoom(string data)																// RECEIVING PLAYER LEAVING ROOM - BOOTH LOCAL AND NETWORK - RECEIVES ALSO KICK REQUESTS - ALSO WHEN OWNER DISCONNECTS
	{
		//MultiplayerCanvas.MultiplayerCanvasInstance.UpdateServerResponse("ON LEAVE ROOM: " + data);

		var pack = data.Split(Delimiter); // [0] roomId - [1] playerId
		if (networkRooms[pack[0]].allPlayers.Contains(pack[1])) // if room cointains player
		{
			networkRooms[pack[0]].allPlayers.Remove(pack[1]); // remove player from room
			networkRooms[pack[0]].totalPlayers--; // decrease total players
			if (networkRooms[pack[0]].confirmedPlayers.Contains(pack[1])) // if player was confirmed
			{
				networkRooms[pack[0]].confirmedPlayers.Remove(pack[1]); // remove player from confirmed list
			}
		}
		if (pack[0] == networkPlayers[local_player_id].GetRoomId()) // if local player is in the room
		{
			RoomCanvas.RoomCanvasInstance.UpdatePlayers(networkRooms[pack[0]]); // update player list
		}
		networkPlayers[pack[1]].SetRoomId(null); // set room null to the player who left
		if (pack[1] == local_player_id) // if local player left the room
		{
			MenuManager.MenuInstance.LoadMultiplayerMenu(); // load multiplayer menu
			MultiplayerCanvas.MultiplayerCanvasInstance.UnlockButtons(); // unlock multiplayer buttons
		}
		if (pack[0] == pack[1]) // if player who left is owner of the room
        { 
			
        }
		MultiplayerCanvas.MultiplayerCanvasInstance.UpdateRooms();

		//MultiplayerCanvas.MultiplayerCanvasInstance.UpdateServerResponse("LEAVE ROOM SUCCESS");
	}
	public void KickPlayer(string playerID) // SEND KICK PLAYER REQUEST TO SERVER
	{
		//MultiplayerCanvas.MultiplayerCanvasInstance.UpdateServerResponse("KICKING PLAYER: " + playerID);

		Dictionary<string, string> data = new Dictionary<string, string>();
		data["player"] = playerID;
		data["room"] = local_player_id;
		Application.ExternalCall("socket.emit", "KICK_PLAYER", new JSONObject(data));

		//MultiplayerCanvas.MultiplayerCanvasInstance.UpdateServerResponse("SENT KICK PLAYER");
	}
	#endregion

	#region Multiplayer Game Start
	public void StartRoom() // SEND START ROOM REQUEST TO SERVER
	{
		//MultiplayerCanvas.MultiplayerCanvasInstance.UpdateServerResponse("STARTING ROOM");

		Dictionary<string, string> data = new Dictionary<string, string>();
		data["id"] = local_player_id;
		Application.ExternalCall("socket.emit", "START_ROOM", new JSONObject(data));

		//MultiplayerCanvas.MultiplayerCanvasInstance.UpdateServerResponse("SENT START ROOM");
	}
	public void OnStartRoom(string data) // RECEIVING START ROOM FROM SERVER - BOTH LOCAL AND NETWORK
	{
		//MultiplayerCanvas.MultiplayerCanvasInstance.UpdateServerResponse("ON START ROOM: D:" + data);

		var pack = data.Split(Delimiter); // [0] roomId & playerId
		networkRooms[pack[0]].isStarted = true;
		networkRooms[pack[0]].spawnedPlayers = 0;
		if (networkPlayers[local_player_id].GetRoomId() == pack[0])
		{
			RoomCanvas.RoomCanvasInstance.LockButtons();
			CountdownScript.CountdownInstance.StartCountdown();
		}
		MultiplayerCanvas.MultiplayerCanvasInstance.UpdateRooms();

		//MultiplayerCanvas.MultiplayerCanvasInstance.UpdateServerResponse("START ROOM SUCCESS");
	}
	public void StartGame() // SEND START GAME REQUEST TO SERVER FOR THE CURRENT PLAYER
	{
		//MultiplayerCanvas.MultiplayerCanvasInstance.UpdateServerResponse("STARTING MULTIPLAYER GAME");

		Dictionary<string, string> data = new Dictionary<string, string>();
		data["room"] = networkPlayers[local_player_id].GetRoomId();
		data["player"] = local_player_id;
		Application.ExternalCall("socket.emit", "START_GAME", new JSONObject(data));

		//MultiplayerCanvas.MultiplayerCanvasInstance.UpdateServerResponse("SENT START GAME");
	}
	public void OnStartGame(string data) // RECEIVING GAME STARTED FROM SERVER - BOTH LOCAL AND NETWORK
	{
		//MultiplayerCanvas.MultiplayerCanvasInstance.UpdateServerResponse("ON START GAME: D:" + data);

		var pack = data.Split(Delimiter); // [0] roomId - [1] playerId
		if (networkRooms[pack[0]].allPlayers.Contains(pack[1]))
		{
			if (!networkRooms[pack[0]].startedPlayers.Contains(pack[1]))
			{
				networkRooms[pack[0]].startedPlayers.Add(pack[1]);
			}
			if (networkPlayers[pack[1]].GetRoomId() == networkPlayers[local_player_id].GetRoomId()) // if player spawning is in the same room as local player
			{
				Vector3 position = SpawnManagerScript.SpawnManagerInstance.spawnPoints[networkRooms[pack[0]].spawnedPlayers].position;
				int rotation = SpawnManagerScript.SpawnManagerInstance.rotations[networkRooms[pack[0]].spawnedPlayers];
				networkPlayers[pack[1]].SpawnPlayer(position, rotation, networkPlayers[pack[1]].playerName); // SPAWNING PLAYER
				if (pack[1] == local_player_id)
				{
					isGameStarted = true;
				}
				networkRooms[pack[0]].spawnedPlayers++;
				if (networkRooms[pack[0]].spawnedPlayers == networkRooms[pack[0]].totalPlayers)
				{
					GameManager.GameInstance.gameState = GameManager.GameState.Started; // TODO need to change the state of the game to start at the same time for all players after maybe 5 seconds
																						//GameManager.GameInstance.gameState = GameManager.GameState.Initiated; // this will be the right state
					Debug.Log("All Players have Logged!");
				}
			}
		}

		//MultiplayerCanvas.MultiplayerCanvasInstance.UpdateServerResponse("START GAME SUCCESS");
	}
	public void EndGame() // SEND START GAME REQUEST TO SERVER FOR THE CURRENT PLAYER
	{
		//MultiplayerCanvas.MultiplayerCanvasInstance.UpdateServerResponse("ENDING MULTIPLAYER GAME");

		Dictionary<string, string> data = new Dictionary<string, string>();
		data["room"] = networkPlayers[local_player_id].GetRoomId();
		data["player"] = local_player_id;
		Application.ExternalCall("socket.emit", "END_GAME", new JSONObject(data));

		//MultiplayerCanvas.MultiplayerCanvasInstance.UpdateServerResponse("SENT END GAME");
	}
	#endregion

	#region Multiplayer Update
	public void MoveAndRotate(Dictionary<string, string> data)
	{
		//MultiplayerCanvas.MultiplayerCanvasInstance.UpdateServerResponse("SENDING MOVE AND ROTATE");

		JSONObject jo = new JSONObject(data);
		Application.ExternalCall("socket.emit", "MOVE_AND_ROTATE", new JSONObject(data));

		//MultiplayerCanvas.MultiplayerCanvasInstance.UpdateServerResponse("SENT MOVE AND ROTATE");
	}
	public void OnUpdateMoveAndRotate(string data)
	{
		//MultiplayerCanvas.MultiplayerCanvasInstance.UpdateServerResponse("UPDATE MOVE AND ROTATE");
		/*
		 * data.pack[0] = id (network player id)
		 * data.pack[1] = position.x
		 * data.pack[2] = position.y
		 * data.pack[3] = position.z
		 * data.pack[4] = rotation.y
		 * data.pack[5] = velocity.x
		 * data.pack[6] = velocity.y
		 * data.pack[7] = velocity.z
		*/
		var pack = data.Split(Delimiter);
		if (networkPlayers.ContainsKey(pack[0])) // if player exists, update player with position, velocity and rotation
		{
			if (networkPlayers[local_player_id].GetRoomId() == networkPlayers[pack[0]].GetRoomId())
            {

            }
			networkPlayers[pack[0]].UpdatePosition(float.Parse(pack[1]), float.Parse(pack[2]), float.Parse(pack[3]));
			networkPlayers[pack[0]].UpdateRotation(float.Parse(pack[4]));
			networkPlayers[pack[0]].UpdateVelocity(float.Parse(pack[5]), float.Parse(pack[6]), float.Parse(pack[7]));
		}

		//MultiplayerCanvas.MultiplayerCanvasInstance.UpdateServerResponse("UPDATE MOVE AND ROTATE END");
	}
	public void OnUserDisconnected(string data)
	{
		//MultiplayerCanvas.MultiplayerCanvasInstance.UpdateServerResponse("ON USER DISCONNECTED: " + data);
		/*
		 * data.pack[0] = id (network player id)
		*/
		var pack = data.Split(Delimiter);
		if (networkPlayers.ContainsKey(pack[0]))
		{
			networkPlayers.Remove(pack[0]);
			MultiplayerCanvas.MultiplayerCanvasInstance.UpdatePlayers();
		}
		if (pack[0] == local_player_id)
		{
			OnApplicationQuit();
		}

		//MultiplayerCanvas.MultiplayerCanvasInstance.UpdateServerResponse("ON USER DISCONNECTED END");
	}
    #endregion

	void OnApplicationQuit()
	{
		Application.ExternalEval("socket.isReady = false;");
	}

}