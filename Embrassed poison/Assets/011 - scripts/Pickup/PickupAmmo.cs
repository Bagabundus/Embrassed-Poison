using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupAmmo : Pickup, Ipickable
{
    public new void give()
    {
        Player player = FindObjectOfType<Player>();

        player.inventory[0].CurrentAmmoOnPlayer += 100;
        player.inventory[0].CurrentAmmoOnPlayer = Mathf.Clamp(player.inventory[0].CurrentAmmoOnPlayer, 0, player.inventory[0].MaxAmmoOnPlayer);

    }

}
