using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using Photon.Pun;

public class RoomListItem : MonoBehaviourPunCallbacks
{
  public Text roomName;
  public Text roomPlayers;
  public string roomInfoConcat = "|";

  public void Inicialize(string ps_roomName, int pi_roomPlayers, int pi_roomPlayersMax){
    roomName.text = ps_roomName;
    roomPlayers.text = pi_roomPlayers + roomInfoConcat + pi_roomPlayersMax;
  }

  public void ButtonJoinRoom(){
    if (PhotonNetwork.InLobby)
    {
      PhotonNetwork.JoinRoom(roomName.text);
    }
  }
}
