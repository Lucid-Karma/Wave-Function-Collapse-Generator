using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float bulletSpeed;

    private void OnEnable()
    {
        Invoke("MakeUnvisible", 1.5f);
    }
    private void FixedUpdate()
    {
        transform.position += transform.forward * bulletSpeed * Time.fixedDeltaTime;
    }

    void MakeUnvisible()
    {
        gameObject.SetActive(false);
    }

    //void OnTriggerEnter(Collider other)
    //{
    //    IDamageable damageable = other.GetComponent<IDamageable>();
    //    damage = GunController.instance.damage;

    //    if (damageable != null)
    //    {
    //        damageable.Damage(damage);
    //        gameObject.SetActive(false);
    //    }

    //    if (other.gameObject.tag != "CoinDetector")
    //    {
    //        gameObject.SetActive(false);
    //    }
    //}
}
