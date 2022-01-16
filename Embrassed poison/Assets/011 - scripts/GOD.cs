using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;



public class GOD : MonoBehaviour,IPausable
{
    public enum States { In_Game,In_Pause}
    public States CurrentState = States.In_Game;
    public ScriptableObject[] allWeapons = new ScriptableObject[8];

    private Player player;
    private MonoBehaviour[] MB;
    private Animator anim;


    

    public void Start()
    {
        MB = FindObjectsOfType<MonoBehaviour>();
        player = FindObjectOfType<Player>();
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
        AmmoRecovery();
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
        Time.timeScale = player.GameSpeed;
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
    public void AmmoRecovery()
    {
        if(Input.GetKeyDown(KeyCode.F1))
        player.inventory[0].CurrentAmmoOnPlayer += 100;
    }
}
