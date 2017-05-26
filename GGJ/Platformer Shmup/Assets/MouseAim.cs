using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseAim : MonoBehaviour {

    public GameObject bullet;
    [Space()]
    public Vector2 positionOffset;
    public Vector2 barrelOffset;
    public Transform playerPos;

    void Update() {
        Aim();

        transform.position = Vector2.Lerp(transform.position, (Vector2)playerPos.position + Vector2.right * positionOffset.x * (playerPos.GetComponent<SpriteRenderer>().flipX ? -1 : 1) + Vector2.up * positionOffset.y, Time.deltaTime * 50);

        Fire();
    }

    void Aim() {
        GetComponent<SpriteRenderer>().flipY = playerPos.GetComponent<SpriteRenderer>().flipX;
    }

    void Fire() {
        if (Input.GetKeyDown(KeyCode.X)) {
            StopCoroutine("autoFire");
            StartCoroutine("autoFire");
        }
    }

    IEnumerator autoFire() {
        while (Input.GetKey(KeyCode.X)) {
            //GameObject projectile = Instantiate(bullet, (Vector2)transform.position + barrelOffset * (playerPos.GetComponent<SpriteRenderer>().flipX ? -1 : 1), transform.rotation);
            //projectile.GetComponent<bullet>().direction += Vector3.right * (playerPos.GetComponent<SpriteRenderer>().flipX ? -1 : 1);
            //projectile.GetComponent<SpriteRenderer>().flipY = GetComponent<SpriteRenderer>().flipX;

            //grenade Shot
            //GameObject projectile = Instantiate(bullet, (Vector2)transform.position + barrelOffset * (playerPos.GetComponent<SpriteRenderer>().flipX ? -1 : 1), transform.rotation);
            //projectile.GetComponent<bullet>().direction += Vector3.right * (playerPos.GetComponent<SpriteRenderer>().flipX ? -1 : 1) + Vector3.up / 2;
           // projectile.GetComponent<SpriteRenderer>().flipY = GetComponent<SpriteRenderer>().flipX;

           /* //Spread Shot
            GameObject projectile = Instantiate(bullet, (Vector2)transform.position + barrelOffset * (playerPos.GetComponent<SpriteRenderer>().flipX ? -1 : 1), transform.rotation);
            projectile.GetComponent<bullet>().direction += Vector3.right * (playerPos.GetComponent<SpriteRenderer>().flipX ? -1 : 1);
            projectile.GetComponent<SpriteRenderer>().flipY = GetComponent<SpriteRenderer>().flipX;

            projectile = Instantiate(bullet, (Vector2)transform.position + barrelOffset * (playerPos.GetComponent<SpriteRenderer>().flipX ? -1 : 1), transform.rotation);
            projectile.GetComponent<bullet>().direction += Vector3.right * (playerPos.GetComponent<SpriteRenderer>().flipX ? -1 : 1) + Vector3.up / 2;
            projectile.GetComponent<SpriteRenderer>().flipY = GetComponent<SpriteRenderer>().flipX;

            projectile = Instantiate(bullet, (Vector2)transform.position + barrelOffset * (playerPos.GetComponent<SpriteRenderer>().flipX ? -1 : 1), transform.rotation);
            projectile.GetComponent<bullet>().direction += Vector3.right * (playerPos.GetComponent<SpriteRenderer>().flipX ? -1 : 1) - Vector3.up / 2;
            projectile.GetComponent<bullet>().direction += Vector3.right * (playerPos.GetComponent<SpriteRenderer>().flipX ? -1 : 1) ;
            projectile.GetComponent<SpriteRenderer>().flipY = GetComponent<SpriteRenderer>().flipX; */

            transform.position -= Vector3.right * 0.15f * (playerPos.GetComponent<SpriteRenderer>().flipX ? -1 : 1);
            yield return new WaitForSeconds(0.5f);
        }
    }
}
