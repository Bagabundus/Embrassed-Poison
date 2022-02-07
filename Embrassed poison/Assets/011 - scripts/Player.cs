using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class Player : MonoBehaviour, IPausable
{

    #region variables public
    [Space(10)]
    public mState CurrentPlayerState;
    [Space(10)]
    public wState CurrentWeaponState;

    [Space(10)]
    public int health = 10;

    [Space(10)]
    public float XSpeed = 5;

    [Space(10)]

    public float JumpStrength = 1;
    public float fallMultiplyer = 2.5f;
    public float lowJumpMultipliyer = 2.0f;

    [Space(10)]

    public float MaxGameSpeed = 10;

    [Space(10)]
    public Weapon[] inventory = new Weapon[9];

    [Space(10)]
    public KeyCode[] controls;



    public enum wState
    {
        NoWeapon,
        locked,
        Waiting,
        Reloading,
        NoBullet,
        //LastBullets,
        NeedReload,
        HaveBullets,
        MagazineFull
    }
    public enum mState
    {
        In_Game,
        In_Pause,
        In_Cinematic,
        Dead
    };
    #endregion

    #region variable private
    private Animator anim;
    private CharacterController charaCTRL;
    private TextMeshProUGUI GameSpeedTMP;
    private RotateLookTarget neck;
    private RotateLookTarget L_Elbow;
    private RotateLookTarget R_Elbow;
    private AudioSource SFX;


    [HideInInspector]
    public int MaxHealth;
    [HideInInspector]
    public float GameSpeed = 1;
    [HideInInspector]
    public bool hittable;
    [HideInInspector]
    public int indexCurentWeapon = 0;
   [HideInInspector]
    public TextMeshPro TMPFeedback;
   [HideInInspector]
    public GameObject SevenArms;

    public float bonusSpeed = 1;
    private float VerticalDirection = 0;
    private float gravity = 9.8f;
    private Camera mCamera;
    private BarrelExit barrelExit;
    private ClonesParticleParent psParent;
    private HashSet<wState> shootCondition = new HashSet<wState> { wState.HaveBullets, wState.MagazineFull };
    private HashSet<wState> reloadCondition = new HashSet<wState> { wState.NeedReload, wState.HaveBullets };
    private SpriteRenderer R_Arm;
    private SpriteRenderer L_Arm;
    #endregion

    #region start
    private void Start()
    {
    SetHealth();
    Initiate();
    }
    public void SetHealth()
    {
        MaxHealth = health;
    }
    public void Initiate()
    {
        anim = GetComponent<Animator>();
        charaCTRL = GetComponent<CharacterController>();
        SFX = GetComponent<AudioSource>();

        GameSpeedTMP = FindObjectOfType<TMPUI_GameSpeed>().GetComponent<TextMeshProUGUI>();
        barrelExit = FindObjectOfType<BarrelExit>();
        psParent = FindObjectOfType<ClonesParticleParent>();
        mCamera = Camera.main;

        RotateLookTarget[] rotatingSights;
        rotatingSights = FindObjectsOfType<RotateLookTarget>();
        neck = rotatingSights[0];
        L_Elbow = rotatingSights[1];
        R_Elbow = rotatingSights[2];
        R_Arm = R_Elbow.transform.GetChild(0).GetComponent<SpriteRenderer>();
        L_Arm = L_Elbow.transform.GetChild(0).GetComponent<SpriteRenderer>();

    }
    #endregion

    #region update


    private void Update()
    {
        if (CurrentPlayerState == mState.In_Game)
        {

            Mover();
            Aim();
            Trigger(2);
            QuickChangeWeapon(3);
            Jump(4);
            //Reload();

            UseTime();
        }
        if (CurrentPlayerState == mState.In_Pause)
        {
            QuickChangeWeapon(3);
        }
   
    }
    public void Mover()
    {
        charaCTRL.Move(Direction(0, 1));

        transform.rotation = Quaternion.Euler(Orientation(0, 1));

        if (Input.GetKey(controls[0]) || Input.GetKey(controls[1]) && OnGround() == true)
        {
            anim.Play("Displacement.Walk");
        }

        else if (Input.GetKey(controls[0]) || Input.GetKey(controls[1]) == false && OnGround() == true)
        {
            anim.Play("Displacement.Idle");
        }

}
    public void Aim()
    {
    Vector3 target = new Vector3(GetMousePosition().x, GetMousePosition().y, transform.position.z);

        neck.transform.LookAt(target);
        L_Elbow.transform.LookAt(target);
        R_Elbow.transform.LookAt(target);
    }
    public void Trigger(int triggerInput)
    {
        if (Input.GetKeyDown(controls[triggerInput]))  // si input
        {
            if (shootCondition.Contains(CurrentWeaponState)) // si peut tirer
            {
                StartCoroutine(Shoot());

                if (inventory[indexCurentWeapon].shootingMode == Weapon.mode.semiAutoWithRecoil)
                {
                    StartCoroutine(ApplyRecoil(1));
                    StartCoroutine(InvincibilityOnRecoil());
                }
            }
            else if (CurrentWeaponState == wState.NeedReload)  // si ! munition
            {
                StartCoroutine(Reload());
            }
        }
    }
    public void QuickChangeWeapon(int QuiChangeWeaponInput)
    {
        if (Input.GetKeyDown(controls[QuiChangeWeaponInput]))
        {
            indexCurentWeapon = -indexCurentWeapon + 1;
            Feedback(inventory[indexCurentWeapon].weaponCategory,new Color(1, 0.45f, 0, 1));
            R_Arm.sprite = inventory[indexCurentWeapon].R_Arm;
            L_Arm.sprite = inventory[indexCurentWeapon].L_Arm;
            CheckWeaponState();
        }
    } 
    public void Jump(int JumpInput)
    {
        if (Input.GetKeyDown(controls[JumpInput]) && OnGround() == true)
        {
            VerticalDirection = JumpStrength;
        }
    }
    public void UseTime()
    {

        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (GameSpeed <= 1)
            {
                GameSpeed -= 0.1f;
                GameSpeed = Mathf.Clamp(GameSpeed, 0, MaxGameSpeed);
            }
            else
            {
                GameSpeed -= 0.5f;
                GameSpeed = Mathf.Clamp(GameSpeed, 0, MaxGameSpeed);
            }
        }

        else if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if (GameSpeed < 1)
            {
                GameSpeed += 0.1f;
                GameSpeed = Mathf.Clamp(GameSpeed, 0, MaxGameSpeed);
            }
            else
            {
                GameSpeed += 0.5f;
                GameSpeed = Mathf.Clamp(GameSpeed, 0, MaxGameSpeed);
            }
        }

        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {

            GameSpeedTMP.text = "SPEED X " + GameSpeed;

            if (GameSpeed <= 1)
            {
                Time.timeScale = GameSpeed;
                bonusSpeed = 1 / GameSpeed;
                anim.SetFloat("SpeedAnimation", 1 / GameSpeed);
            }
            else
            {
                Time.timeScale = 1;
                bonusSpeed = GameSpeed;
            }
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
        }
    }  // à revoir reloading sprite ect..

    #endregion

    public void OnTriggerEnter(Collider col)
    {
        if(col.CompareTag("PickupHealth") == true)
        {
            health += 5;
            Destroy(col.gameObject);
        }
        if (col.CompareTag("PickupAmmo") == true)
        {
            inventory[indexCurentWeapon].CurrentAmmoOnPlayer += 100;
            Destroy(col.gameObject);
            CheckWeaponState();
        }
        if (col.CompareTag("PickupOctofinity") == true)
        {
            OCTOFINITY();
            Destroy(col.gameObject);
            CheckWeaponState();
        }
        if (col.CompareTag("PickupPistol") == true)
        {
            inventory[0].PickupWeapon();
            Destroy(col.gameObject);
            CheckWeaponState();
        }
        if (col.CompareTag("PickupShotgun") == true)
        {
            inventory[1].PickupWeapon();
            Destroy(col.gameObject);
            CheckWeaponState();
        }
        if (col.CompareTag("PickupSMG") == true)
        {
            inventory[2].PickupWeapon();
            Destroy(col.gameObject);
            CheckWeaponState();
        }

        if (col.CompareTag("Exit") == true)
        {
            col.transform.parent.GetChild(2).GetComponent<Camera>().enabled = false;
            mCamera.enabled = true;
            transform.position = col.transform.parent.position;
            transform.localScale = col.transform.parent.localScale;

        }
    }
    public void OnTriggerStay(Collider col)
    {
        if (col.CompareTag("Entrance") == true)
        {
            if(Input.GetKey(controls[8]))
            {

            StartCoroutine(ChangeArea(col.transform.GetChild(0), col.transform.GetChild(0).GetComponent<SphereCollider>(),col.transform.GetChild(2).GetComponent<Camera>()));

            }
        }
        
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(new Vector3(transform.position.x, transform.position.y - 0.05f, transform.position.z), new Vector3(0.3f, 0.1f, 0.3f));
    }

    #region fonctions
    public void CheckWeaponState()
    {
        if (inventory[indexCurentWeapon].locked == true)
        {
            CurrentWeaponState = wState.locked;
        }                                                                             // mon arme est bloqué
        else if (inventory[indexCurentWeapon].CurrentBulletInMagazine == inventory[indexCurentWeapon].MaxBulletInMagazine)
        {
            CurrentWeaponState = wState.MagazineFull;
        }           // chargeur remplis
        else if(inventory[indexCurentWeapon].CurrentBulletInMagazine > 0)
        {
            CurrentWeaponState = wState.HaveBullets;
        }                                                            // j'ai des balles  
        else if (inventory[indexCurentWeapon].CurrentAmmoOnPlayer > 0) 
        {
            CurrentWeaponState = wState.NeedReload;
        }                                                               // plus de balles dans le chargeur mais j'ai d'autres balles
        else 
        {
            CurrentWeaponState = wState.NoBullet;
        }                                                                                                                         // plus de balles du tout
    }      
    public void ExchangeWeapon(int w)
{
    Weapon var = inventory[indexCurentWeapon];

    for (int i = 0; i < inventory.Length; i++)
    {
        if (inventory[i].ID == w)
        {
            inventory[indexCurentWeapon] = inventory[i];
            inventory[i] = var;
            break;
        }
    }
}  // à revoir
    public void OnHit(int damage)
    {
    if (hittable == true)
    {
        health -= damage;
        health = Mathf.Clamp(health, 0, MaxHealth);
        anim.SetTrigger("Hit");

            if (health == 0)
            {
                Death();
            }
        }
    }  // à revoir
    public void Death()
    {
        Feedback("OH NO!", Color.red);
        anim.SetTrigger("Death");
        CurrentPlayerState = mState.Dead;
    }  // à faire
    public void Feedback(string text,Color c)
    {
        TextMeshPro var = Instantiate(TMPFeedback, transform.position + new Vector3(0,0.5f,0), Quaternion.identity, psParent.transform);
        var.text = text;
        var.GetComponent<FeedbackEffect>().c = c;

    }
    public void OCTOFINITY()
    {
        Instantiate(SevenArms,R_Elbow.transform.position,R_Arm.transform.rotation,R_Elbow.transform);
    }
    #endregion

    #region Values

    public bool OnGround()
    {
        return Physics.CheckBox(new Vector3(transform.position.x, transform.position.y - 0.05f, transform.position.z), new Vector3(0.3f, 0.1f, 0.3f) / 2);
    }
    public float BetterJumpValue()
    {
        float var = 0;

        if (charaCTRL.velocity.y < 0)
        {
            var = gravity * (fallMultiplyer - 1);
        }
        else if (charaCTRL.velocity.y > 0 && Input.GetKey(controls[4]) == false)
        {
            var = gravity * (lowJumpMultipliyer - 1);
        }
        return var;
    } //?
    public Vector3 Direction(int leftInput, int rightInput)
    {
        float var = 0;

        if (Input.GetKey(controls[leftInput]))
        {
            var = 1;
        }
        else if (Input.GetKey(controls[rightInput]))
        {
            var = -1;
        }

        float speedMultiply;
        if (OnGround() == true)
        {
            speedMultiply = 1;
        }
        else
        {
            speedMultiply = 0.5f;
        }

        VerticalDirection += (-gravity - BetterJumpValue()) * Time.deltaTime;

        VerticalDirection = Mathf.Clamp(VerticalDirection, -100, 100);

        Vector3 direction = new Vector3(var * XSpeed * bonusSpeed * speedMultiply * Time.deltaTime * transform.localScale.x, VerticalDirection * Time.deltaTime, 0);

        return direction;

    }
    public Vector3 Orientation(int leftInput, int rightInput)
    {
        float var = 0;

        if (Input.GetKey(controls[leftInput]))
        {
            var = 0;
        }
        else if (Input.GetKey(controls[rightInput]))
        {
            var = 180;
        }

        return new Vector3(0, var, 0);
    }
    public Vector3 GetMousePosition()
    {
        Vector3 position;
        position = mCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -mCamera.transform.position.z));
        return position;
    }
    #endregion

    #region call from an another script
    public void ToggleStateToPause()
    {
        if (CurrentPlayerState == mState.In_Game)
        {
            CurrentPlayerState = mState.In_Pause;
        }
        else if (CurrentPlayerState == mState.In_Pause)
        {
            CurrentPlayerState = mState.In_Game;
        }
    }
    #endregion

    #region coroutines
    public IEnumerator Shoot()
    {
        ParticleSystem ps = Instantiate(inventory[indexCurentWeapon].FXBullet, barrelExit.transform.position, barrelExit.transform.rotation, psParent.transform);
        ps.Play();
        Destroy(ps.gameObject, 2);
        inventory[indexCurentWeapon].CurrentBulletInMagazine--;

        SFX.clip = inventory[indexCurentWeapon].SFXFire;
        SFX.Play();

        CurrentWeaponState = wState.Waiting;
        yield return new WaitForSeconds((1 / inventory[indexCurentWeapon].FireRate) / bonusSpeed);
        CheckWeaponState();
    }
    public IEnumerator Reload()
    {
        if (reloadCondition.Contains(CurrentWeaponState))
        {
            Feedback("Reload !", Color.green);
            CurrentWeaponState = wState.Reloading;
            SFX.clip = inventory[indexCurentWeapon].SFXReload;
            SFX.Play();
            yield return new WaitForSeconds(inventory[indexCurentWeapon].ReloadingCooldown / bonusSpeed);
            inventory[indexCurentWeapon].Reload();
            CheckWeaponState();

        }
    }
    public IEnumerator ApplyRecoil(float Timer)
    {
        
        float x = transform.position.x;
        float y = transform.position.y;
        float xs = GetMousePosition().x;
        float ys = GetMousePosition().y;

        Vector2 a = new Vector2(x - xs, y - ys);

        a.x = a.x / Mathf.Abs(a.x);
        a.y = a.y / Mathf.Abs(a.y);

        Vector3 var = new Vector3(a.x, a.y, 0);

        var *= Timer / 200;

        charaCTRL.Move(var);

        Timer -= Time.deltaTime;
        yield return Time.deltaTime;
        Timer = Mathf.Clamp(Timer, 0, 1);

        if (Timer >= 0)
        {
           StartCoroutine(ApplyRecoil(Timer));
        }
    }
    public IEnumerator InvincibilityOnRecoil()
    {
        hittable = !hittable;
        yield return 3 * Time.deltaTime;
        hittable = !hittable;
    }
    public IEnumerator ChangeArea(Transform t,SphereCollider s,Camera c)
    {
        mCamera.enabled = false;
        c.enabled = true;
        transform.position = t.transform.position;
        transform.localScale = t.transform.localScale;
        gravity = 9.8f * transform.localScale.x;
        s.enabled = false;
        yield return new WaitForSeconds(0.5f);
        s.enabled = true;
    } 
    #endregion
}
