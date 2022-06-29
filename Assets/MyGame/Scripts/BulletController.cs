using System.Collections;
using System.Collections.Generic;

using Photon.Pun;
using UnityEngine;

public class BulletController : MonoBehaviour
{
  private float bulletSpeed = 150f;
  private Rigidbody2D rigidBD2D;
  public float bulletTimeLife = 3f;
  private float bulletTimeCount;
  public float bulletDamage = 10f;

  private PhotonView PV;

  void Start()
  {
    rigidBD2D = GetComponent<Rigidbody2D>();
    PV = GetComponent<PhotonView>();

    rigidBD2D.AddForce(transform.up * bulletSpeed, ForceMode2D.Force);
  }

  void Update()
  {
    if (bulletTimeCount >= bulletTimeLife)
    {
      Destroy(this.gameObject);
    }

    bulletTimeCount += Time.deltaTime;
  }

  [PunRPC]
  void BulletDestroy()
  {
    Destroy(this.gameObject);
  }

  private void OnTriggerEnter2D(Collider2D other)
  {
    if (other.CompareTag("Player") && other.GetComponent<PlayerController>() && other.GetComponent<PhotonView>().IsMine)
    {
      Debug.Log(this.GetComponent<PhotonView>().ViewID);
      other.GetComponent<PlayerController>().TakeDamage(-bulletDamage, other.GetComponent<PhotonView>().Owner);
      PV.RPC("BulletDestroy", RpcTarget.AllViaServer);
    }
  }
}
