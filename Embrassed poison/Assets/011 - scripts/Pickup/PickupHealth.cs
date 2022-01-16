using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupHealth : Pickup, Ipickable
{
    public new void give()
    {
        Player player = FindObjectOfType<Player>();

        player.health += 5;
        player.health = Mathf.Clamp(player.health, 0, player.MaxHealth);
    }
}
