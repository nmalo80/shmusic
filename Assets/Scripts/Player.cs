using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region Variables
    // The player must be a singleton in order to work with multiple levels
    public static Player Instance { get; private set; }

    // Configurable fields
    [Header("Player")]
    [SerializeField] float CharacterMoveSpeed = 10f;
    public int Health = 500;
    [SerializeField] Sprite FalmencoGuitarSprite;
    [SerializeField] Sprite ElectricGuitarSprite;
    [SerializeField] GameObject TransformInstrumentExplosion = null;
    [SerializeField] float EnemyExplosionLightDuration = 1f;

    [Header("Player Firing")]
    [SerializeField] GameObject MainBulletPrefab = null;
    [SerializeField] GameObject ElectricBulletPrefab = null;
    [SerializeField] GameObject DarbukaBulletPrefab = null;
    [SerializeField] float bulletSpeed = 0f;
    [SerializeField] float FireRate = 0.2f; // when you keep fire pushed
    [SerializeField] AudioClip Fire1Clip = null;
    [SerializeField] [Range(0, 1)] float Fire1ClipVolume = 1f;    

    [Header("Player Damage")]
    [SerializeField] AudioClip BumpClip = null;
    [SerializeField] [Range(0,1)] float BumpClipVolume = 1f;
    [SerializeField] AudioClip DyingClip = null;
    [SerializeField] [Range(0, 1)] float DyingClipVolume = 1f;
    [SerializeField] GameObject ExplosionParticlesLight = null;

    // instances
    private Animator playerAnimator;
    private List<Coroutine> firingCoroutines;

    // cached values
    private float horizontalPos = 0f;
    private float verticalPos = 0f;

    // screen boundaries
    private float xMin, xMax, yMin, yMax;

    // Game stuff
    private float currentGameSpeed = 1f;
    [SerializeField] float timer = 0f;
    private int electricGuitarTimer = 15;
    private int darbukaTimer = 10;
    [SerializeField] float FlamencoGuitarGameSpeed = 1f;
    [SerializeField] float ElectricGuitarGameSpeed = 1.5f;
    [SerializeField] float DarbukaGameSpeed = 1f;
    
    private enum AnimationStates
    {
        FlamencoIdleState, FlamencoStrummingState, ElectricIdleState, ElectricStrummingState, DarbukaIdleState, DarbukaTappingState
    }
    
    public void ResetTimer() { timer = 0f; }
    

    #endregion



    #region Standard Methods
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        playerAnimator = GetComponent<Animator>();
        SetupMoveBoundaries();

        firingCoroutines = new List<Coroutine>();

        GameManager.Instance.CurrentInstrument = GameManager.Instance.GetFLAMENCO_GUITAR();
    }

    // Update is called once per frame
    void Update()
    {
        if(timer > 0)
        {
            timer -= Time.deltaTime;
            if(timer <= 0)
            {
                GameManager.Instance.CurrentInstrument = GameManager.Instance.GetFLAMENCO_GUITAR();
                TransitionToInstrument(GameManager.Instance.GetFLAMENCO_GUITAR());
            }
        }

        Move();
        ManageFire();
    }

    private void OnTriggerEnter2D(Collider2D otherThing)
    {
        // 12 is the layer corresponding to powerups. Fire all this power up effects:
        if (GameManager.Instance.GetTransformationsEnabled() &&
            GameManager.Instance.CurrentInstrument != GameManager.Instance.GetELECTRIC_GUITAR()
            && GameManager.Instance.CurrentInstrument != GameManager.Instance.GetDARBUKA()
            && otherThing.gameObject.layer == 12)
        {
            if (otherThing.name == "MetalPickPowerupPrefab(Clone)")
            {
                GameManager.Instance.CurrentInstrument = GameManager.Instance.GetELECTRIC_GUITAR();
                TransitionToInstrument(GameManager.Instance.GetELECTRIC_GUITAR());
            }
            else //if (otherThing.name == "DarbukaPowerupPrefab(Clone)")
            {
                GameManager.Instance.CurrentInstrument = GameManager.Instance.GetDARBUKA();
                TransitionToInstrument(GameManager.Instance.GetDARBUKA());
            }
        }
        else
        {
            DamageDealer damageDealer = otherThing.gameObject.GetComponent<DamageDealer>();

            if (damageDealer != null)
            {
                ProcessHit(damageDealer);
            }
        }

        // if the other thing is not a boss enemy
        if(otherThing.gameObject.layer != 13)
            Destroy(otherThing.gameObject);
    }

    #endregion

    #region Game Methods
    private void SetupMoveBoundaries()
    {
        Camera gameCamera = Camera.main;

        //this is sprite widht / 2
        float xPadding = GetComponent<SpriteRenderer>().bounds.size.x / 2;
        float yPadding = GetComponent<SpriteRenderer>().bounds.size.y / 2;

        // in the view port the left bound is 0, the right one is 1, bottom = 0, top = 1
        xMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).x + xPadding;
        xMax = gameCamera.ViewportToWorldPoint(new Vector3(1, 0, 0)).x - xPadding;
        yMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).y + yPadding;
        yMax = gameCamera.ViewportToWorldPoint(new Vector3(0, 1, 0)).y - yPadding;        
    }    

    private void Move()
    {
        // Time delta time makes my game frame rate independent
        horizontalPos = Mathf.Clamp(this.transform.position.x + Input.GetAxis("Horizontal") * Time.deltaTime * CharacterMoveSpeed, xMin, xMax);
        verticalPos = Mathf.Clamp(this.transform.position.y + Input.GetAxis("Vertical") * Time.deltaTime * CharacterMoveSpeed, yMin, yMax);

        this.transform.position = new Vector3(horizontalPos, verticalPos, 0);
    }

    private void ManageFire()
    {   
        if (Input.GetButtonDown("Fire1"))
        {
            firingCoroutines.Add(StartCoroutine(FireCoroutine()));

            // Update state
            MoveAnimationStateToStrumming();
            //characterAnimator.Play(1);
        }
        else if (Input.GetButtonUp("Fire1"))
        {
            // Many coroutines may be generated using different input mechanisms (keyboard, joypad, mouse etc.)
            // This stops all the created coroutine
            foreach (var c in firingCoroutines)
            {
                StopCoroutine(c);
            }

            MoveAnimationStateToIdle();
        }
        else if(Input.GetButtonUp("Pause"))
        {
            if (Time.timeScale == 0f)
                ResumeGame();
            else
                PauseGame();
        }

        
    }

    void PauseGame()
    {
        currentGameSpeed = Time.timeScale;
        Time.timeScale = 0;
    }

    void ResumeGame()
    {
        Time.timeScale = currentGameSpeed;
    }

    private void MoveAnimationStateToIdle()
    {
        // If I'm in a Flamenco Guitar state
        if (GameManager.Instance.CurrentInstrument == GameManager.Instance.GetFLAMENCO_GUITAR())
        {
            playerAnimator.SetInteger("AnimState", (int)AnimationStates.FlamencoIdleState);
            //print("state: " + playerAnimator.GetInteger("AnimState"));
        }
        else if (GameManager.Instance.CurrentInstrument == GameManager.Instance.GetELECTRIC_GUITAR())
        {
            playerAnimator.SetInteger("AnimState", (int)AnimationStates.ElectricIdleState);
            //print("state: " + playerAnimator.GetInteger("AnimState"));
        }
        else if (GameManager.Instance.CurrentInstrument == GameManager.Instance.GetDARBUKA())
        {
            playerAnimator.SetInteger("AnimState", (int)AnimationStates.DarbukaIdleState);
        }
    }

    private void MoveAnimationStateToStrumming()
    {
        // If I'm in a Flamenco Guitar state
        if (GameManager.Instance.CurrentInstrument == GameManager.Instance.GetFLAMENCO_GUITAR())
        {
            playerAnimator.SetInteger("AnimState", (int)AnimationStates.FlamencoStrummingState);
            //print("state: " + playerAnimator.GetInteger("AnimState"));
        }
        else if (GameManager.Instance.CurrentInstrument == GameManager.Instance.GetELECTRIC_GUITAR())
        {
            playerAnimator.SetInteger("AnimState", (int)AnimationStates.ElectricStrummingState);
            //print("state: " + playerAnimator.GetInteger("AnimState"));
        }
        else if (GameManager.Instance.CurrentInstrument == GameManager.Instance.GetDARBUKA())
        {
            playerAnimator.SetInteger("AnimState", (int)AnimationStates.DarbukaTappingState);
        }

    }

    private IEnumerator FireCoroutine()
    {
        // the while manages the continuous fire action using yield 
        while (true)
        {
            // Create bullet on screen
            GameObject bullet = null;
            if(GameManager.Instance.CurrentInstrument == GameManager.Instance.GetFLAMENCO_GUITAR())
                bullet = Instantiate(MainBulletPrefab, this.transform.position, Quaternion.identity) as GameObject;
            else if(GameManager.Instance.CurrentInstrument == GameManager.Instance.GetELECTRIC_GUITAR())
                bullet = Instantiate(ElectricBulletPrefab, this.transform.position, Quaternion.identity) as GameObject;
            else if(GameManager.Instance.CurrentInstrument == GameManager.Instance.GetDARBUKA())
                bullet = Instantiate(DarbukaBulletPrefab, this.transform.position, Quaternion.identity) as GameObject;

            // Fire the bullet
            bullet.GetComponent<Rigidbody2D>().velocity = new Vector2(bulletSpeed, 0);

            if (Fire1Clip)
                AudioSource.PlayClipAtPoint(Fire1Clip, Camera.main.transform.position, Fire1ClipVolume);

            yield return new WaitForSeconds(FireRate);
        }
    }

    private void ProcessHit(DamageDealer damageDealer)
    {
        if (!(GameManager.Instance.CurrentInstrument == GameManager.Instance.GetDARBUKA()))
        {
            Health -= damageDealer.GetDamage();
            //Debug.Log("Remaining health: " + Health);

            if (BumpClip)
                AudioSource.PlayClipAtPoint(BumpClip, Camera.main.transform.position, BumpClipVolume);

            // Destroy enemy is energy is over
            if (Health <= 0)
            {
                Health = 0;
                Die();
            }
        }

        // graphical effect
        DamageEffect();

        // Destroy bullet
        damageDealer.Hit();
    }

    private void Die()
    {
        if (DyingClip)
            AudioSource.PlayClipAtPoint(DyingClip, Camera.main.transform.position, DyingClipVolume);

        Destroy(this.gameObject);

        FindObjectOfType<Scenes>().LoadGameOver();
    }

    public void TransitionToInstrument(int Instrument)
    {
        int currentState = playerAnimator.GetInteger("AnimState");

        if (Instrument == GameManager.Instance.GetELECTRIC_GUITAR())
        {
            FireRate = 0.1f;
            timer = electricGuitarTimer;
            Time.timeScale = ElectricGuitarGameSpeed;

            MusicPlayer.Instance.PlayElectricTune();

            switch(currentState)
            {
                case ((int)AnimationStates.FlamencoIdleState):
                    playerAnimator.SetInteger("AnimState", (int)AnimationStates.ElectricIdleState);
                    break;
                case (int)AnimationStates.FlamencoStrummingState:
                    playerAnimator.SetInteger("AnimState", (int)AnimationStates.ElectricStrummingState);
                    break;
            }

            // deactivate background
            GameManager.Instance.ActivateElectricBG();
        }
        else if(Instrument == GameManager.Instance.GetFLAMENCO_GUITAR())
        {
            FireRate = 0.5f;
            Time.timeScale = FlamencoGuitarGameSpeed;

            MusicPlayer.Instance.PlayFlamencoTune();

            switch (currentState)
            {
                case ((int)AnimationStates.ElectricIdleState):
                case ((int)AnimationStates.DarbukaIdleState):
                    playerAnimator.SetInteger("AnimState", (int)AnimationStates.FlamencoIdleState);
                    break;
                case (int)AnimationStates.ElectricStrummingState:
                case (int)AnimationStates.DarbukaTappingState:
                    playerAnimator.SetInteger("AnimState", (int)AnimationStates.FlamencoStrummingState);
                    break;
            }
            // re-activate background
            GameManager.Instance.ActivateStandardBG();
        }    
        else if(Instrument == GameManager.Instance.GetDARBUKA())
        {
            FireRate = 0.5f;

            timer = darbukaTimer;
            Time.timeScale = DarbukaGameSpeed;

            MusicPlayer.Instance.PlayDarbukaTune();

            switch (currentState)
            {
                case ((int)AnimationStates.FlamencoIdleState):
                    playerAnimator.SetInteger("AnimState", (int)AnimationStates.DarbukaIdleState);
                    break;
                case (int)AnimationStates.FlamencoStrummingState:
                    playerAnimator.SetInteger("AnimState", (int)AnimationStates.DarbukaTappingState);
                    break;
            }

            GameManager.Instance.ActivateBongoBG();
        }
        GameObject explosion = Instantiate(TransformInstrumentExplosion, this.transform.position, Quaternion.identity) as GameObject;
        Destroy(explosion, EnemyExplosionLightDuration);
    }

    private void DamageEffect()
    {
        GameObject explosion = Instantiate(ExplosionParticlesLight, this.transform.position, Quaternion.identity) as GameObject;
        Destroy(explosion, EnemyExplosionLightDuration);
    }

    #endregion
}
