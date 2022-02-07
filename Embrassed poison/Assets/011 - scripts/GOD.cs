using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;



public class GOD : MonoBehaviour,IPausable
{
    public enum States { In_Game,In_Pause}
    public States CurrentState = States.In_Game;

    private Player p;
    private MonoBehaviour[] MB;
    private Animator anim;


    

    public void Start()
    {
        MB = FindObjectsOfType<MonoBehaviour>();
        p = FindObjectOfType<Player>();
        anim = GetComponent<Animator>();

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(CurrentState == States.In_Game)
            {
                Pause();
                
            }
            else if (CurrentState == States.In_Pause)
            {
                Resume();
            }
        }
        Debug();
    }
    public void Pause()
    {
        anim.Play("Base Layer.Pause");
        Time.timeScale = 0;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        foreach (IPausable i in MB.Where(x => x is IPausable))
        {
            i.ToggleStateToPause();
        }
    }
    public void Resume()
    {
        anim.Play("Base Layer.Resume");
        Time.timeScale = p.GameSpeed;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        foreach (IPausable i in MB.Where(x => x is IPausable))
        {
            i.ToggleStateToPause();
        }
        
    }
    public void ToggleStateToPause()
    {
        if (CurrentState == States.In_Game)
        {
            CurrentState = States.In_Pause;
        }
        else if (CurrentState == States.In_Pause)
        {
            CurrentState = States.In_Game;
        }
    }
    public void Debug()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            p.inventory[p.indexCurentWeapon].CurrentAmmoOnPlayer += 100;
            p.CheckWeaponState();
        }

    }
}
