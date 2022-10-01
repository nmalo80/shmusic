using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    #region Variables
    [Header("Prefabs")]
    [SerializeField] GameObject[] BulletPrefab = null;
    [SerializeField] GameObject ExplosionParticles = null;
    [SerializeField] GameObject ExplosionParticlesLight = null;

    [Header("Params")]
    [SerializeField] float bulletSpeed = 0f;
    [SerializeField] float shotCounter;
    [SerializeField] float minTimeBetweenShots = 1f;
    [SerializeField] float maxTimeBetweenShots = 3f;
    [SerializeField] float EnemyExplosionDuration = 1f;
    [SerializeField] float EnemyExplosionLightDuration = 1f;

    [Header("Attributes")]
    [SerializeField] int Health = 300;
    [SerializeField] int EnemyScoreValue = 0;
    [SerializeField] bool IsFinalBoos = false;

    [Header("Sounds")]
    [SerializeField] AudioClip DyingClip = null;
    [SerializeField] [Range(0, 1)]  float DyingVolume = 1f;

    [Header("PowerUps")]
    [SerializeField] int MetalPickPowerupChances = 0;
    [SerializeField] int DarbukaPowerupChances = 0; // this must be > MetalPickPowerupChances because of the way I set up the random generation

    bool isDead = false;

    #endregion

    #region Accessors
    // Accessory methods
    public float GetBulletSpeed() { return bulletSpeed; }
    public void SetBulletSpeed(float speed) { this.bulletSpeed = speed; }
    #endregion

    #region Standard Methods
    // Start is called before the first frame update
    void Start()
    {
        isDead = false;
        shotCounter = Random.Range(minTimeBetweenShots, maxTimeBetweenShots);
    }

    // Update is called once per frame
    void Update()
    {
        CountDownAndShoot();
    }

    private void OnTriggerEnter2D(Collider2D otherThing)
    {
        DamageDealer damageDealer = otherThing.gameObject.GetComponent<DamageDealer>();

        if (damageDealer != null)
        {
            ProcessHit(damageDealer);
        }
    }
    #endregion

    #region Game Methods
    private void CountDownAndShoot()
    {
        shotCounter -= Time.deltaTime;
        if(shotCounter <= 0)
        {
            Fire();
            shotCounter = Random.Range(minTimeBetweenShots, maxTimeBetweenShots);
        }
    }

    private void Fire()
    {        
        // Create bullet on screen
        GameObject bullet = Instantiate(BulletPrefab[Random.Range(0, BulletPrefab.Length)], this.transform.position, Quaternion.identity) as GameObject;

        // Fire the bullet
        bullet.GetComponent<Rigidbody2D>().velocity = new Vector2(Random.Range(-5.0f,-2.0f), Random.Range(-0.5f,0.5f));
    }

    private void ProcessHit(DamageDealer damageDealer)
    {
        Health -= damageDealer.GetDamage();
        //Debug.Log("Remaining health: " + Health);
        
        // Destroy enemy is energy is over
        if (!isDead && Health <= 0)
        {
            Die();
        }
        else
        {
            DamageEffect();
        }

        // Destroy bullet
        damageDealer.Hit();
    }

    private void DamageEffect()
    {
        GameObject explosion = Instantiate(ExplosionParticlesLight, this.transform.position, Quaternion.identity) as GameObject;
        Destroy(explosion, EnemyExplosionLightDuration);
    }

    private void Die()
    {
        isDead = true;

        // Explosion effect
        GameObject explosion = Instantiate(ExplosionParticles, this.transform.position, Quaternion.identity) as GameObject;
        
        // Play clip
        if(DyingClip)
            AudioSource.PlayClipAtPoint(DyingClip, Camera.main.transform.position, DyingVolume);

        // Add points to game manager
        GameManager.Instance.IncreaseScore(EnemyScoreValue);

        Destroy(explosion, EnemyExplosionDuration);
        Destroy(this.gameObject);

        ReleasePowerUp();

        // if you killed the final boss => show game over screen
        if(IsFinalBoos)
        {
            FindObjectOfType<Scenes>().LoadSuccess();
        }
    }

    private void ReleasePowerUp()
    {
        if (GameManager.Instance.CurrentInstrument == GameManager.Instance.GetFLAMENCO_GUITAR())
        {
            int randomValue = Random.Range(0, 1000);
            // release powerups randomly
            if (randomValue < MetalPickPowerupChances)
            {
                var powerup = Instantiate(GameManager.Instance.GetMetalPickPrefab(), this.transform.position, Quaternion.identity);

                powerup.GetComponent<Rigidbody2D>().velocity = new Vector2(-4, 0);
            }
            else if (randomValue < DarbukaPowerupChances)
            {
                var powerup = Instantiate(GameManager.Instance.GetDarbukaPowerupPrefab(), this.transform.position, Quaternion.identity);
                powerup.GetComponent<Rigidbody2D>().velocity = new Vector2(-2, 0);
            }
        }
    }
    #endregion
}
