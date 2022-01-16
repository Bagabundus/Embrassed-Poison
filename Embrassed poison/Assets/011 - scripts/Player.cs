using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class Player : MonoBehaviour, IPausable
{
    public enum pState {In_Game,In_Pause,In_Cinematic,Dead };
    [Space(10)]
    public pState CurrentPlayerState;

    //bool arme canShoot BulletMagazine Ammo
    public enum wState
    {
        NoWeapon ,
        Waiting ,
        NoBullet ,
        LastBullets ,
        NeedReload ,
        FullBullets
    }

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
    public KeyCode[] controls = new KeyCode[7] { KeyCode.D , KeyCode.Q, KeyCode.Mouse0, KeyCode.Mouse1, KeyCode.Space, KeyCode.LeftShift, KeyCode.R};

    [Space(10)]
    public AnimationCurve recoilEffect;





    private Animator anim;
    private CharacterController charaCTRL;
    private TextMeshProUGUI GameSpeedTMP;
    private RotateLookTarget neck;
    private RotateLookTarget L_Elbow;
    private RotateLookTarget R_Elbow;
    private BarrelExit barrelExit;
    private ClonesParticleParent psParent;

    [HideInInspector]
    public int MaxHealth;
    [HideInInspector]
    public float GameSpeed = 1;

    private bool hittable;
    private float bonusSpeed = 1;
    private float VerticalDirection = 0;
    private bool canShoot = true;
    private float gravity = 9.8f;
    private float RecoilTime = 0;
    private Camera mCamera;

    private HashSet<wState> shootSimpleCondition = new HashSet<wState> {wState.LastBullets, wState.FullBullets};
    private HashSet<wState> reloadCondition = new HashSet<wState> {wState.NeedReload, wState.FullBullets};


    //_________________________________________________________________________________________________________________________________________________________________________________________

    private void Start()
    {
        SetHealth();
        Initiate();
        CheckWeaponState();
    } 
    public void SetHealth()
    {
        MaxHealth = health;
    } 
    public void Initiate()
    {
        anim = GetComponent<Animator>();
        charaCTRL = GetComponent<CharacterController>();
        GameSpeedTMP = FindObjectOfType<TMPUI_GameSpeed>().GetComponent<TextMeshProUGUI>();
        barrelExit = FindObjectOfType<BarrelExit>();
        mCamera = Camera.main;


        RotateLookTarget[] rotatingSights;
        rotatingSights = FindObjectsOfType<RotateLookTarget>();
        neck = rotatingSights[0];
        L_Elbow = rotatingSights[1];
        R_Elbow = rotatingSights[2];

        barrelExit = FindObjectOfType<BarrelExit>();
        psParent = FindObjectOfType<ClonesParticleParent>();

        canShoot = true;
    } 

    //_________________________________________________________________________________________________________________________________________________________________________________________
    
    private void Update()
    {
        if (CurrentPlayerState == pState.In_Game)
        {

            Mover();
            Aim();
            Trigger(2);
            QuickChangeWeapon(3);
            Jump(4);
            Reload(6,false);

            UseTime();
        }
        if (CurrentPlayerState == pState.In_Pause)
        {
            QuickChangeWeapon(3);
        }

    } 
    public void Mover()
    {
        charaCTRL.Move(Direction(0, 1) + RecoilValue());

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
        if (Input.GetKeyDown(controls[triggerInput]))  
        {   
            if (shootSimpleCondition.Contains(CurrentWeaponState)) 
            {
                Shoot();
                if (inventory[0].shootingMode == Weapon.mode.semiAutoWithRecoil)
                {
                    Debug.Log("WAY!");
                    ApplyRecoil();
                    InvincibilityOnRecoil();
                }

                CheckWeaponState();
            }
            else if (CurrentWeaponState == wState.NeedReload) 
            {
                Reload(6, true);
                CheckWeaponState();
            }
        }  
    } 
    public void Shoot()
    {
        ParticleSystem ps = Instantiate(inventory[0].FXBullet, barrelExit.transform.position, barrelExit.transform.rotation, psParent.transform);// spawn tir
        ps.Play();
        Destroy(ps.gameObject, 2);
        inventory[0].CurrentBulletInMagazine--; // --ammo
        inventory[0].CurrentBulletInMagazine = Mathf.Clamp(inventory[0].CurrentBulletInMagazine, 0, inventory[0].MaxBulletInMagazine);

        StartCoroutine(CantShoot(1 / inventory[0].FireRate));
    }
    public void QuickChangeWeapon(int QuickChangeWeaponInput)
    {
        if (Input.GetKey(controls[QuickChangeWeaponInput]))
        {
            if (CurrentWeaponState != wState.NoWeapon)
            {

                inventory[8] = inventory[0];
                inventory[0] = inventory[1];
                inventory[1] = inventory[8];
                inventory[8] = null;
                CheckWeaponState();
            }
        }
    }  
    public void Jump(int JumpInput)
    {
        if (Input.GetKeyDown(controls[JumpInput]) && OnGround() == true)
        {
            VerticalDirection = JumpStrength;
        }
    } 
    public void Reload(int ReloadInput, bool trigger)
    {
        if((Input.GetKeyDown(controls[ReloadInput]) || trigger == true))
        {
           if (reloadCondition.Contains(CurrentWeaponState))
            {
                StartCoroutine(CantShoot(inventory[0].ReloadingCooldown));

                int a = inventory[0].MaxBulletInMagazine - inventory[0].CurrentBulletInMagazine - Mathf.Clamp((inventory[0].MaxBulletInMagazine - inventory[0].CurrentAmmoOnPlayer - inventory[0].CurrentBulletInMagazine), 0, 999);
                inventory[0].CurrentBulletInMagazine += a;
                inventory[0].CurrentAmmoOnPlayer -= a;

            }

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
            }
            else
            {
                Time.timeScale = 1;
                bonusSpeed = GameSpeed;
            }
                Time.fixedDeltaTime = 0.02f * Time.timeScale;
        }
    }

    //________________________________________________________________________________________________________________________________________________________________________________________
    
    public void OCTOFINITY()
    {
        StartCoroutine(Octofinity(10));
    }
    public void InvincibilityOnRecoil()
    {
        StartCoroutine(InvincibilityOnRecoil(3 * Time.deltaTime));
    }
    public void ExchangeWeapon(int w)
    {
        Weapon var = inventory[0];

        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i].ID == w)
            {
                inventory[0] = inventory[i];
                inventory[i] = var;
                break;
            }
        }         
    }
    public void OnHit(int damage)
    {
        if (hittable == true)
        {
            health -=damage;
            health = Mathf.Clamp(health, 0, MaxHealth);
            anim.SetTrigger("Hit");

            if (health == 0)
            {
                Death();
            }
        }
    } 
    public void ToggleHittable()
    {
        if (hittable == true)
        {
            hittable = false;
        }
        else if (hittable == false)
        {
            hittable = true;
        }
    } 
    public void Death()
    {
        anim.SetTrigger("Death");
        CurrentPlayerState = pState.Dead;
    }
    public void CheckWeaponState()
    {
        
        if (inventory[0] != null || inventory[0] != null)
        {
            //si j'ai une arme
            CurrentWeaponState = wState.Waiting;

            if (canShoot == true)
            {
                //si je peux tirer
                CurrentWeaponState = wState.NoBullet;

                if (inventory[0].CurrentBulletInMagazine != 0)
                {
                    //si chargeur ! vide
                    CurrentWeaponState = wState.LastBullets;

                    if (inventory[0].CurrentAmmoOnPlayer != 0)
                    {
                        //si j'ai des munitions
                        CurrentWeaponState = wState.FullBullets;
                    }
                }
                else if  (inventory[0].CurrentBulletInMagazine == 0)
                {
                    // si chargeur est vide

                    if (inventory[0].CurrentAmmoOnPlayer != 0)
                    {
                        //si j'ai des munitions
                        CurrentWeaponState = wState.NeedReload;
                    }
                }

            }
        }
        else if (inventory[0] == null && inventory[1] == null)
        {
            //si j'ai pas d'armes
            CurrentWeaponState = wState.NoWeapon;
        }
        
        
    }
    public void ApplyRecoil()
    {
        StartCoroutine(RecoilEffect(1f));
    }

    //_________________________________________________________________________________________________________________________________________________________________________________________
    
    private void OnTriggerEnter(Collider col)
    {
        col.GetComponent<Pickup>().give();
    } 

    //_________________________________________________________________________________________________________________________________________________________________________________________
    
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(new Vector3(transform.position.x, transform.position.y - 0.05f , transform.position.z), new Vector3(0.3f, 0.1f, 0.3f));
    } 

    //_________________________________________________________________________________________________________________________________________________________________________________________
    
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
    }
    public Vector3 Direction(int leftInput,int rightInput)
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

        Vector3 direction = new Vector3 (var * XSpeed * bonusSpeed * speedMultiply * Time.deltaTime, VerticalDirection * Time.deltaTime, 0);

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
    } //
    public Vector3 RecoilValue()
    {
        float x = transform.position.x;
        float y = transform.position.y;
        float xs = GetMousePosition().x;
        float ys = GetMousePosition().y;

        Vector2 a = new Vector2(x - xs, y - ys);

        a.x = a.x / Mathf.Abs(a.x);
        a.y = a.y / Mathf.Abs(a.y);

        a *= recoilEffect.Evaluate(RecoilTime);

        Vector3 var = new Vector3 (a.x, a.y ,0);

        return var;
    } //

    //_________________________________________________________________________________________________________________________________________________________________________________________
   
    public void ToggleStateToPause()
    {
        if (CurrentPlayerState == pState.In_Game)
        {
            CurrentPlayerState = pState.In_Pause;
        }
        else if (CurrentPlayerState == pState.In_Pause)
        {
            CurrentPlayerState = pState.In_Game;
        }
    }

    //_________________________________________________________________________________________________________________________________________________________________________________________
   
    public IEnumerator Octofinity(float Timer)
    {
        Timer -= Time.deltaTime;
        Timer = Mathf.Clamp(Timer,0,999);
        yield return Time.deltaTime;
        if(Timer != 0)
        {
            StartCoroutine(Octofinity(Timer));
        }
    } //manque X8 
    public IEnumerator CantShoot(float Timer)
    {
        canShoot = false;
        Timer -= Time.deltaTime;
        Timer = Mathf.Clamp(Timer,0,999);
        yield return Time.deltaTime;
        if (Timer != 0)
        {
            StartCoroutine(CantShoot(Timer));
        }
        else
        {
            canShoot = true;
            CheckWeaponState();
        }
    } //
    public IEnumerator InvincibilityOnRecoil(float Timer)
    {
        if(Timer <= 0)
        {
        hittable = true;
        }
        else
        {
            hittable = false;
            Timer -= Time.deltaTime;
            yield return Time.deltaTime;
            StartCoroutine(InvincibilityOnRecoil(Timer));
        }
    } //
    public  IEnumerator RecoilEffect(float Timer)
    {
        if (Timer <= 0)
        {
            RecoilTime = Timer;
        }
        else
        {
            Timer -= Time.deltaTime;
            yield return Time.deltaTime;
            StartCoroutine(RecoilEffect(Timer));
        }
    } //
}
