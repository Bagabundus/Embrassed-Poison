using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Weapon/Create Weapon")]
public class Weapon : ScriptableObject
{
    public enum mode {Automatic, semiAutomatic, semiAutoWithRecoil}
    [Space(5)]
    public int ID;
    [Space(5)]
    public string weaponCategory;
    [Space(5)]
    public Sprite sprite;
    [Space(5)]
    public AudioClip SFX;
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


}
