using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] int Health = 500;

    // Private vars
    
    private void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(this.transform.position.x > GameManager.Instance.GetScreenRightBound() 
            || this.transform.position.x < GameManager.Instance.GetScreenLeftBound())
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D otherThing)
    {
        DamageDealer damageDealer = otherThing.gameObject.GetComponent<DamageDealer>();

        if (damageDealer != null)
        {
            ProcessHit(damageDealer);
        }
    }

    private void ProcessHit(DamageDealer damageDealer)
    {
        Health -= damageDealer.GetDamage();
        if (Health <= 0)
        {
            Health = 0;
            Destroy(gameObject);
        }
    }
}
