using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using UnityEngine;

public class CanvasManager : MonoBehaviour
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

  static CanvasManager(){}

  private static CanvasManager _instance;

  public static CanvasManager GetInstance()
  {
    if (_instance == null)
    {
      _instance = new CanvasManager();
    }

    return _instance;
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

  public void EndRoom()
  {
    Debug.Log("Deve fechar a sala");
  }

  public void ShowTheWinner()
  {
    var winner = "";
    var maxScore = 0;

    foreach (var item in PhotonNetwork.PlayerList)
    {
      if (maxScore < item.GetScore())
      {
        maxScore = item.GetScore();
        winner = item.NickName;
      }
    }

    Debug.Log("O ganhador" + winner);

    foreach (var item in PhotonNetwork.PlayerList)
    {
      if (item.NickName == winner)
      {
        Debug.Log("Você é o vencedor" + item.NickName);
        GoToWin();
      }

      Debug.Log("Você perdu" + item.NickName);
      GoToLost();
    }
  }

  public void GoToWin()
  {
    SetActivePanel("");
    vitoriaGO.SetActive(true);
  }

  public void GoToLost()
  {
    SetActivePanel("");
    SetActivePanel("Derrota");
  }

  public void GoToCreditos(){
    SetActivePanel("");
    creditosGO.SetActive(true);
  }

  public void GoToSalas(){
    SetActivePanel("");
    roomListGO.SetActive(true);
  }
}
