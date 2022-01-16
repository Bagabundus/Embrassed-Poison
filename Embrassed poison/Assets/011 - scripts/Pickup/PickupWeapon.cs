using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime;


[CreateAssetMenu(fileName = "Pickup Weapon", menuName = "Pickup/Weapon Pickup")]
public class PickupWeapon : Pickup, Ipickable
{
    //donne une arme si le joueur ne l'a pas 
    //donne munitions si le joueur l'a déjà

    public Weapon weapon;
    public Sprite spr;

    public new void give()
    {
        Player player = FindObjectOfType<Player>();

        for (int i = 0; i < player.inventory.Length; i++) // check tout
        {
            if (weapon.weaponCategory == player.inventory[i].weaponCategory) // si a déjà l'arme
            {
                player.inventory[i].CurrentAmmoOnPlayer += weapon.CurrentAmmoOnPlayer + weapon.CurrentBulletInMagazine; 
                player.inventory[i].CurrentAmmoOnPlayer = Mathf.Clamp(player.inventory[i].CurrentAmmoOnPlayer, 0, weapon.MaxAmmoOnPlayer); //++ammo

                break;
            }

            else if (i == player.inventory.Length && weapon.weaponCategory != player.inventory[i].weaponCategory) //si check toute la liste et pas d'amre similaire
            {
                for (int j = 0; j < player.inventory.Length; j++)
                {
                    if (player.inventory[j] == null) //cheche une place libre
                    {
                        Weapon var;
                        player.inventory[j] = weapon;  //rammasse l'arme 
                        var = player.inventory[0];
                        player.inventory[0] = weapon;  //et met en première place de la hotbar
                        player.inventory[j] = var;

                    }
                }
            }
        }
    }
}
