using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class CharactersList : MonoBehaviourPunCallbacks
{
  // Start is called before the first frame update
  void Start()
  {
    SetCharacterID();
  }

  void SetCharacterID()
  {
    int i = 1;

    foreach (Transform item in transform)
    {
      item.GetComponent<CanvasCharacter>().SetID(i);
      i++;
    }
  }

  public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
  {
    object characterID_obj;

    if (changedProps.TryGetValue("PERSONAGEM", out characterID_obj))
    {
      int characterID = (int)characterID_obj;

      if (characterID > 0)
      {
        foreach (Transform item in transform)
        {
          if (item.GetComponent<CanvasCharacter>().id == characterID)
          {
            item.GetComponent<CanvasCharacter>().SelectCharacter(targetPlayer.NickName, targetPlayer.ActorNumber);
          }
        }
      }
    }
  }
}
