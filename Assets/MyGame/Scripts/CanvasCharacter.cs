using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.EventSystems;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class CanvasCharacter : MonoBehaviour, IPointerClickHandler
{
  public int id = 0;
  public bool isChosen;
  public string playerOwner = "";
  public int playerOwnerId = 0;

  // Start is called before the first frame update
  void Start()
  {
    isChosen = true;
  }

  public void SetID(int value)
  {
    id = value;
  }

  public void OnPointerClick(PointerEventData eventData)
  {
    if (!isChosen)
    {
      return;
    }

    Hashtable props = new Hashtable { { "PERSONAGEM", id } };

    PhotonNetwork.LocalPlayer.SetCustomProperties(props);
  }

  public void SelectCharacter(string playerName, int playerID)
  {
    Transform allButtons = this.transform.parent;

    foreach (Transform item in allButtons)
    {
      if (item.GetComponent<CanvasCharacter>().playerOwnerId == playerID)
      {
        item.GetComponent<CanvasCharacter>().ResetCharacter();
      }
    }

    isChosen = false;
    playerOwner = playerName;
    playerOwnerId = playerID;
    GetComponentInChildren<Text>().text = playerName;
    GetComponentInChildren<Text>().color = ColorPlayer(playerID);
    GetComponent<Button>().interactable = false;
  }

  public void ResetCharacter()
  {
    isChosen = true;
    playerOwner = "";
    playerOwnerId = 0;
    GetComponentInChildren<Text>().text = "";
    GetComponentInChildren<Text>().color = ColorPlayer(0);
    GetComponent<Button>().interactable = true;
  }

  Color ColorPlayer(int playerID)
  {
    Color colorReturn;

    switch (playerID)
    {
      case 1:
        colorReturn = new Color(255, 0, 0);
        break;
      case 2:
        colorReturn = new Color(0, 0, 255);
        break;
      case 3:
        colorReturn = new Color(0, 255, 0);
        break;
      default:
        colorReturn = new Color(255, 255, 255);
        break;
    }

    return colorReturn;
  }
}
