﻿using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Projectile : MonoBehaviour, ITrigger
{
    public float _damage = 1;
    public Vector3 direction;
    public float speed;
    public float timeToExpire = 20f;

    public void Launch(Vector3 direction, float speed)
    {
        this.direction = direction;
        this.speed = speed;
        gameObject.GetComponent<Rigidbody>().velocity = direction * speed;
        AudioManager.PlaySoundAtPosition("projectileLaunch", transform.position);
        Destroy(gameObject, timeToExpire);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Player")  || other.gameObject.tag.Equals("Projectile"))
        {
            return;
        }
        Trigger(other);
    }


    public void Trigger(Collider other)
    {
        AudioManager.PlaySoundAtPosition("projectileHit", transform.position);

        var damageScript = GetComponent<Damage>();
        damageScript.SetDamage(_damage);
        damageScript.DealDamage(other);
        Destroy(gameObject);
        
    }
}