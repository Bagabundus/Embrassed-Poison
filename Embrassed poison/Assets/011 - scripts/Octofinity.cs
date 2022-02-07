using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Octofinity : MonoBehaviour
{
    private Player p;
    private GameObject arm1;
    private GameObject arm2;
    private GameObject arm3;
    private GameObject arm4;
    private GameObject arm5;
    private GameObject arm6;
    private GameObject arm7;
    private ClonesParticleParent psParent;
    private AudioSource SFX;
    private bool BOOL = true;
    private KeyCode input;
    private BarrelExit barrelExit;


    private void Awake()
    {
        p = FindObjectOfType<Player>();
        arm1 = transform.GetChild(0).gameObject;
        arm2 = transform.GetChild(1).gameObject;
        arm3 = transform.GetChild(2).gameObject;
        arm4 = transform.GetChild(3).gameObject;
        arm5 = transform.GetChild(4).gameObject;
        arm6 = transform.GetChild(5).gameObject;
        arm7 = transform.GetChild(6).gameObject;
        psParent = FindObjectOfType<ClonesParticleParent>();
        SFX = p.GetComponent<AudioSource>();
        input = p.controls[2];
        barrelExit = FindObjectOfType<BarrelExit>();
        Destroy(this.gameObject, 2);

    }
    private void Update()
    {
        if (Input.GetKeyDown(input))
        {
            if (BOOL == true)
            {
             StartCoroutine(allShoots());
            }
        }
    }
    public IEnumerator allShoots()
    {
        BOOL = !BOOL;
        ParticleSystem ps1 = Instantiate(p.inventory[p.indexCurentWeapon].FXBullet, arm1.transform.position + new Vector3(.22f,.07f,0), barrelExit.transform.rotation, psParent.transform);
        ParticleSystem ps2 = Instantiate(p.inventory[p.indexCurentWeapon].FXBullet, arm2.transform.position + new Vector3(.22f,.07f,0), barrelExit.transform.rotation, psParent.transform);
        ParticleSystem ps3 = Instantiate(p.inventory[p.indexCurentWeapon].FXBullet, arm3.transform.position + new Vector3(.22f,.07f,0), barrelExit.transform.rotation, psParent.transform);
        ParticleSystem ps4 = Instantiate(p.inventory[p.indexCurentWeapon].FXBullet, arm4.transform.position + new Vector3(.22f,.07f,0), barrelExit.transform.rotation, psParent.transform);
        ParticleSystem ps5 = Instantiate(p.inventory[p.indexCurentWeapon].FXBullet, arm5.transform.position + new Vector3(.22f,.07f,0), barrelExit.transform.rotation, psParent.transform);
        ParticleSystem ps6 = Instantiate(p.inventory[p.indexCurentWeapon].FXBullet, arm6.transform.position + new Vector3(.22f,.07f,0), barrelExit.transform.rotation, psParent.transform);
        ParticleSystem ps7 = Instantiate(p.inventory[p.indexCurentWeapon].FXBullet, arm7.transform.position + new Vector3(.22f,.07f,0), barrelExit.transform.rotation, psParent.transform);
        ps1.Play();
        ps2.Play();
        ps3.Play();
        ps4.Play();
        ps5.Play();
        ps6.Play();
        ps7.Play();
        Destroy(ps1.gameObject, 2);
        Destroy(ps2.gameObject, 2);
        Destroy(ps3.gameObject, 2);
        Destroy(ps4.gameObject, 2);
        Destroy(ps5.gameObject, 2);
        Destroy(ps6.gameObject, 2);
        Destroy(ps7.gameObject, 2);
        SFX.clip = p.inventory[p.indexCurentWeapon].SFXFire;
        SFX.Play();
        SFX.Play();
        SFX.Play();
        SFX.Play();
        SFX.Play();
        SFX.Play();
        SFX.Play();

        yield return new WaitForSeconds(1/p.inventory[p.indexCurentWeapon].FireRate/p.bonusSpeed);
        BOOL = !BOOL;
        }

}
