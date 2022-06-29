using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	private Rigidbody2D rigidbody2;

  // Start is called before the first frame update
  void Start()
  {
		rigidbody2 = GetComponent<Rigidbody2D>();
  }

  public void PlayerMove()
  {
    var x = Input.GetAxis("Horizontal");
    var y = Input.GetAxis("Vertical");

    rigidbody2.velocity = new Vector2(x, y);
  }

	public void PlayerTurn()
  {
    Vector3 mousePosition = Input.mousePosition;
    mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

    Vector2 direction = new Vector2(
        mousePosition.x - transform.position.x,
        mousePosition.y - transform.position.y
    );

    transform.right = direction;
  }
}
