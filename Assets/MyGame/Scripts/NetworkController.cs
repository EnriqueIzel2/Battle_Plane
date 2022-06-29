using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class NetworkController : MonoBehaviourPunCallbacks
{
  [Header("Go")]
  public GameObject loginGO;
  public GameObject partidasGO;
  public GameObject titleGO;
  public GameObject loadingGO;
  public GameObject roomGO;
  public GameObject panelCountdown;
  public GameObject paneltimeText;
  public GameObject roomListGO;
  public GameObject creditosGO;
  public GameObject vitoriaGO;
  public GameObject derrotaGO;

  [Header("Spawn")]
  public GameObject spawnObject;
  public Transform[] spawnPoints;
  int spawnPositionUsed;

  [Header("Player")]
  public InputField playerNameInput;
  private string playerNameTemp;
  public GameObject[] myPlayer;

  [Header("Room")]
  public InputField roomName;
  public GameObject buttonStart;
  private string roomNameTemp;
  public Transform content;
  public GameObject roomListItem;
  public Text infoStatus;
  public Text infoPlayerLobby;
  public Text infoPlayerGame;
  public string infoPlayerLobbyTxt = "PlayerInLobby: ";
  public string infoPlayerGameTxt = "PlayerInGame: ";

  Hashtable gameMode = new Hashtable();
  public byte gameMaxPlayer = 2;
  string gameModeKey = "gameMode";
  Dictionary<string, RoomInfo> roomInfoList;

  // Start is called before the first frame update
  void Start()
  {
    playerNameTemp = "Guest" + Random.Range(1000, 10000);
    roomNameTemp = "GameRoom" + Random.Range(1000, 10000);
    roomInfoList = new Dictionary<string, RoomInfo>();

    titleGO.SetActive(true);
    SetActivePanel(loginGO.name);
  }

  private void FixedUpdate()
  {
    infoStatus.text = PhotonNetwork.NetworkClientState.ToString();
    infoPlayerLobby.text = infoPlayerLobbyTxt + PhotonNetwork.CountOfPlayersOnMaster.ToString();
    infoPlayerGame.text = infoPlayerGameTxt + PhotonNetwork.CountOfPlayersInRooms.ToString();
  }

  void SetActivePanel(string activePanel)
  {
    loginGO.SetActive(activePanel.Equals(loginGO.name));
    partidasGO.SetActive(activePanel.Equals(partidasGO.name));
    roomGO.SetActive(activePanel.Equals(roomGO.name));
    panelCountdown.SetActive(activePanel.Equals(panelCountdown.name));
    roomListGO.SetActive(activePanel.Equals(roomListGO.name));
    creditosGO.SetActive(activePanel.Equals(creditosGO.name));
    vitoriaGO.SetActive(activePanel.Equals(vitoriaGO.name));
    derrotaGO.SetActive(activePanel.Equals(derrotaGO.name));
  }

  public void GoToCreditos(){
    SetActivePanel("");
    creditosGO.SetActive(true);
  }

  public void GoToSalas(){
    SetActivePanel("");
    roomListGO.SetActive(true);
  }

  public void Login()
  {
    if (playerNameInput.text != "")
    {
      PhotonNetwork.NickName = playerNameInput.text;
    }
    else
    {
      PhotonNetwork.NickName = playerNameTemp;
    }

    PhotonNetwork.ConnectUsingSettings();

    SetActivePanel("");
    loadingGO.SetActive(true);
  }

  public void BotaoCriarSala()
  {
    if (!PhotonNetwork.InLobby)
    {
      Debug.Log("não tá no lobby");
      return;
    }

    if (roomName.text != "")
    {
      roomNameTemp = roomName.text;
    }

    RoomOptions roomOptions = new RoomOptions() { MaxPlayers = gameMaxPlayer };
    PhotonNetwork.CreateRoom(roomNameTemp, roomOptions);
  }

  public void LeaveRoom(){
    if (PhotonNetwork.InRoom)
    {
      PhotonNetwork.LeaveRoom();
    }

    return;
  }

  public void ButtonCancel()
  {
    SetActivePanel(loginGO.name);
    PhotonNetwork.Disconnect();
  }

  public void StartGame()
  {
    titleGO.SetActive(false);
    SetActivePanel("");

    Hashtable props = new Hashtable{{
      CountdownTimer.CountdownStartTime, (float)PhotonNetwork.Time
    }};

    PhotonNetwork.CurrentRoom.SetCustomProperties(props);
  }

  public void ConfigurarSala()
  {
    roomGO.gameObject.SetActive(true);
    if (PhotonNetwork.LocalPlayer.IsMasterClient)
    {
      buttonStart.SetActive(true);
    }
  }

  public override void OnRoomListUpdate(List<RoomInfo> roomList)
  {
    Debug.Log("Era pra atualizar a lista de salas");
    foreach (Transform item in content)
    {
      Destroy(item.gameObject);
    }

    foreach (RoomInfo info in roomList)
    {
      if (!info.IsOpen || !info.IsVisible || info.RemovedFromList)
      {
        continue;
      }

      GameObject objRoomListItem = Instantiate(roomListItem);
      objRoomListItem.transform.SetParent(content);
      objRoomListItem.transform.localScale = Vector3.one;
      objRoomListItem.GetComponent<RoomListItem>().Inicialize(
        info.Name,
        info.PlayerCount,
        info.MaxPlayers
      );
    }
  }

  /*public override void OnConnected()
  {
    Debug.Log("OnConnected");
    loadingGO.SetActive(false);
    titleGO.SetActive(false);
    SetActivePanel(roomListGO.name);
  }*/

  public override void OnConnectedToMaster()
  {
    Debug.Log("OnConnectedToMaster");
    Debug.Log("Server: " + PhotonNetwork.CloudRegion + " Ping" + PhotonNetwork.GetPing());
    PhotonNetwork.JoinLobby();
    loadingGO.SetActive(false);
    SetActivePanel(roomListGO.name);
    titleGO.SetActive(false);
  }

  public override void OnJoinedLobby()
  {
    Debug.Log("OnJoinedLobby");
  }

  public override void OnCreateRoomFailed(short returnCode, string message)
  {
    PhotonNetwork.JoinLobby();
    Debug.Log("Falhou o createRoom");
  }

  public override void OnJoinRandomFailed(short returnCode, string message)
  {
    string roomTemp = "Room" + Random.Range(1000, 10000);
    RoomOptions options = new RoomOptions();
    options.IsOpen = true;
    options.IsVisible = true;
    options.MaxPlayers = gameMaxPlayer;
    options.CustomRoomProperties = gameMode;
    options.CustomRoomPropertiesForLobby = new string[] { gameModeKey };

    PhotonNetwork.CreateRoom(roomTemp, options);
  }

  public override void OnJoinedRoom()
  {
    Debug.Log("OnJoinedRoom");
    Debug.Log("RoomName: " + PhotonNetwork.CurrentRoom.Name);
    Debug.Log("Current players in room: " + PhotonNetwork.CurrentRoom.PlayerCount);

    SetActivePanel("");

    ConfigurarSala();

    object typeGameValue;
    if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(gameModeKey, out typeGameValue))
    {
      Debug.Log("GameMode:" + typeGameValue.ToString());
    }

    foreach (var item in PhotonNetwork.PlayerList)
    {
      Debug.Log("Name: " + item.NickName);
      Debug.Log("IsMaster: " + item.IsMasterClient);

      Hashtable playerCustom = new Hashtable();
      playerCustom.Add("Lives", 3);
      playerCustom.Add("Score", 0);

      item.SetCustomProperties(playerCustom, null, null);

      // Usando Score da Photon
      item.SetScore(0);
    }
  }

  public override void OnPlayerEnteredRoom(Player newPlayer)
  {
    Debug.Log("Entrei na sala");

    /*Hashtable props = new Hashtable{{
      CountdownEndGame.CountdownStartTime, (float) PhotonNetwork.Time
    }};

    PhotonNetwork.CurrentRoom.SetCustomProperties(props);

    return;*/
  }

  public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
  {
    SetActivePanel(panelCountdown.name);
  }

  void CountDownAction()
  {
    Debug.Log("CountdownAction");

    SetActivePanel("");
    titleGO.SetActive(false);
    paneltimeText.SetActive(true);

    var charPos = (int)PhotonNetwork.LocalPlayer.CustomProperties["PERSONAGEM"] - 1;
    var localSpawn = PhotonNetwork.LocalPlayer.ActorNumber - 1;
    PhotonNetwork.Instantiate(
      myPlayer[charPos].name,
      spawnPoints[localSpawn].position,
      myPlayer[charPos].transform.rotation, 0);
      //myPlayer[charPos].transform.position,
  }

  public override void OnEnable()
  {
    base.OnEnable();
    CountdownTimer.OnCountdownTimerHasExpired += CountDownAction;
  }

  public override void OnDisable()
  {
    base.OnDisable();
    CountdownTimer.OnCountdownTimerHasExpired -= CountDownAction;
  }

  public override void OnDisconnected(DisconnectCause cause)
  {
    Debug.Log("OnDisconnected cause: " + cause);
  }

  public void QuitGame(){
    Application.Quit();
  }
}
