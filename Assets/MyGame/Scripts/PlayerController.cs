using System.Collections;
using System.Collections.Generic;

using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun.UtilityScripts;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Runtime.CompilerServices;

public class PlayerController : MonoBehaviourPun /*IPunObservable*/
{
  public float playerSpeed = 5f;
  private Rigidbody2D rigidbody2;
  private PhotonView PV;

  private PlayerMovement playerMovement;

  private Vector2 movementVector;
  Vector2 playerRB;
  Vector3 playerPosition;
  Quaternion playerRotaion;
  float lag;

  [Header("Health")]
  public float playerHealthMax = 100f;
  float playerHealthCurrent;
  public UnityEngine.UI.Image playerHealthFill;
  public bool isDead;

  [Header("Bullet")]
  public GameObject bulletGO;
  //public GameObject bulletGOPhotonView;
  public GameObject spawnBullet;

  // Start is called before the first frame update
  void Start()
  {
    rigidbody2 = GetComponent<Rigidbody2D>();
    PV = GetComponent<PhotonView>();
    playerMovement = GetComponent<PlayerMovement>();

    HealthManager(playerHealthMax);
  }

  void Awake()
  {
    foreach (var item in PhotonNetwork.PlayerList)
    {
      if (item.IsMasterClient)
      {
        Hashtable props = new Hashtable {{
          CountdownEndGame.CountdownStartTime, (float) PhotonNetwork.Time
        }};

        PhotonNetwork.CurrentRoom.SetCustomProperties(props);

        return;
      }
    }
  }

  // Update is called once per frame
  void Update()
  {
    if (PV.IsMine)
    {
      playerMovement.PlayerMove();
      playerMovement.PlayerTurn();
      Shooting();
      Death();
    }
  }

  public void Shooting()
  {
    if (Input.GetMouseButtonDown(0))
    {
     // PV.RPC("Shoot", RpcTarget.All);
      PhotonNetwork.Instantiate(bulletGO.name, spawnBullet.transform.position, spawnBullet.transform.rotation, 0);
    }
  }

  [PunRPC]
  void Shoot()
  {
    Instantiate(bulletGO, spawnBullet.transform.position, spawnBullet.transform.rotation);
  }

  public void TakeDamage(float value, Player playerTemp)
  {
    PV.RPC("TakeDamageNetwork", RpcTarget.AllBuffered, value, playerTemp);
  }

  [PunRPC]
  public void TakeDamageNetwork(float value, Player playerTemp)
  {
    HealthManager(value);

    object playerScoreTempGet;
    playerTemp.CustomProperties.TryGetValue("Score", out playerScoreTempGet);

    Debug.Log("tentativa o Score" + playerScoreTempGet);

    int soma = (int)playerScoreTempGet;
    soma += 10;

    Hashtable playerHashtableTemp = new Hashtable();
    playerHashtableTemp.Add("Score", soma);

    playerTemp.SetCustomProperties(playerHashtableTemp);

    // Usando Score da Photon
    playerTemp.AddScore(10);

    if (playerHealthCurrent <= 0 && PV.IsMine)
    {
      Debug.Log("Aqui ele deve dar o respawn");
      PV.RPC("FakeRespawn", RpcTarget.AllViaServer, playerTemp);
      //isDead = true;
      //PV.RPC("IsGameOver", RpcTarget.MasterClient);
    }
  }

  [PunRPC]
  void FakeRespawn(Player playerTemp)
  {
    this.gameObject.SetActive(false);

    HealthManager(playerHealthMax);
  }

  [PunRPC]
  void IsGameOver()
  {
    Debug.Log("Entrei no IsGameOver");
    if (PV.Owner.IsMasterClient)
    {
      Debug.Log("Game Over");

      foreach (var item in PhotonNetwork.PlayerList)
      {
        object playerScoreTempGet;
        item.CustomProperties.TryGetValue("Score", out playerScoreTempGet);
        Debug.Log("Player Name: " + item.NickName + " | Score:" + playerScoreTempGet.ToString());
        Debug.Log("Score via Photon: " + item.GetScore());
      }
    }
  }

  void HealthManager(float value)
  {
    playerHealthCurrent += value;
    playerHealthFill.fillAmount = playerHealthCurrent / 100f;
  }

  void Death()
  {
    if (playerHealthCurrent <= 0)
    {
      Debug.Log("Morri");

      //GetComponent<BoxCollider2D>().isTrigger = true;

     // StartCoroutine(DeathEffect(2f));
    }
  }

  /* IEnumerator DeathEffect(float time)
  {
    yield return new WaitForSeconds(time);
    GetComponent<Rigidbody2D>().isKinematic = true;
    transform.Translate(new Vector3(0, -40, 0) * 2.5f * Time.deltaTime);

    StartCoroutine(WaitForRespawn(3f));
  }

  IEnumerator WaitForRespawn(float time)
  {
    yield return new WaitForSeconds(time);

    Respawn();
  }

  void Respawn()
  {
    isDead = false;
    GetComponent<BoxCollider2D>().isTrigger = false;
    GetComponent<Rigidbody2D>().isKinematic = false;

    Transform[] spawnPlayer = GameObject.Find("NetworkController")
      .GetComponent<NetworkController>().spawnPosition;
  }*/
}
