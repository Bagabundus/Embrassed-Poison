using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Weapon/Create Weapon")]
public class Weapon : ScriptableObject
{
    public enum mode { Automatic,
                       semiAutomatic, 
                       semiAutoWithRecoil
                     }




    [Space(5)]
    public bool locked = false;
    [Space(5)]
    public int ID;
    [Space(5)]
    public string weaponCategory;
    [Space(5)]
    public Sprite sprite;
    [Space(5)]
    public Sprite R_Arm;
    [Space(5)]
    public Sprite L_Arm;
    [Space(5)]
    public AudioClip SFXFire;
    [Space(5)]
    public AudioClip SFXReload;
    [Space(5)]
    public ParticleSystem FXBullet;
    [Space(5)]
    public float Damage;
    [Space(5)]
    public mode shootingMode = mode.Automatic;
    [Space(25)]
    public int MaxBulletInMagazine;
    [Space(5)]
    public int CurrentBulletInMagazine;
    [Space(5)]
    public int MaxAmmoOnPlayer;
    [Space(5)]
    public int CurrentAmmoOnPlayer;
    [Space(25)]
    public float FireRate;
    [Space(5)]
    public float ReloadingCooldown;
    [Space(5)]
    public float recoilEffect;
    [Space(5)]
    public int bonusAmmo;



    
    public void PickupWeapon()
    {
        if (locked == true)
        {
            locked = false;
            CurrentAmmoOnPlayer += bonusAmmo;
            FindObjectOfType<Player>().Feedback("+" + weaponCategory , new Color(1, 0.45f, 0, 1));
        }
        else
        {
            FindObjectOfType<Player>().Feedback("+" + bonusAmmo , new Color(1, 0.45f, 0, 1));
            CurrentAmmoOnPlayer += bonusAmmo;
        }
    }

    public void Reload()
    {
        int a = MaxBulletInMagazine - CurrentBulletInMagazine - Mathf.Clamp(MaxBulletInMagazine - CurrentAmmoOnPlayer - CurrentBulletInMagazine, 0, 999999);
        CurrentBulletInMagazine += a;
        CurrentAmmoOnPlayer -= a;
    }



}
