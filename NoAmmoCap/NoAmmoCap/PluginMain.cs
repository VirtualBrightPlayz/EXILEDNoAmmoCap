using EXILED;
using EXILED.ApiObjects;
using EXILED.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoAmmoCap
{
    public class PluginMain : Plugin
    {
        public override string getName => "NoAmmoCap";
        public static Dictionary<ReferenceHub, Ammo> ammo = new Dictionary<ReferenceHub, Ammo>();
        public PluginEvents PLEV;

        public override void OnDisable()
        {
            Events.ShootEvent -= PLEV.Shoot;
            Events.PickupItemEvent -= PLEV.PickupEvent;
            Events.ItemChangedEvent -= PLEV.ItemChange;
            Events.PlayerReloadEvent -= PLEV.ReloadItem;
            Events.RoundRestartEvent -= PLEV.Reset;
            Events.ConsoleCommandEvent -= PLEV.PlyCmd;
            Events.PlayerSpawnEvent -= PLEV.Spawn;
            Events.PlayerDeathEvent -= PLEV.Death;
            PLEV = null;
        }

        public override void OnEnable()
        {
            PLEV = new PluginEvents(this);
            Events.ShootEvent += PLEV.Shoot;
            Events.PickupItemEvent += PLEV.PickupEvent;
            Events.ItemChangedEvent += PLEV.ItemChange;
            Events.PlayerReloadEvent += PLEV.ReloadItem;
            Events.RoundRestartEvent += PLEV.Reset;
            Events.ConsoleCommandEvent += PLEV.PlyCmd;
            Events.PlayerSpawnEvent += PLEV.Spawn;
            Events.PlayerDeathEvent += PLEV.Death;
        }

        public override void OnReload()
        {
        }
    }

    public class Ammo
    {
        public int[] ammo;
        private ReferenceHub ply;

        public Ammo(ReferenceHub ply)
        {
            this.ply = ply;
            ammo = new int[ply.ammoBox.types.Length];
            for (int i = 0; i < ply.ammoBox.types.Length; i++)
                ammo[i] = ply.ammoBox.GetAmmo(i);
        }

        public void Add(int amount)
        {
            ammo[GetItemAmmoType(ply.GetCurrentItem().id)] += amount;
            ply.SetAmmo(AmmoType.Dropped5, 1);
            ply.SetAmmo(AmmoType.Dropped7, 1);
            ply.SetAmmo(AmmoType.Dropped9, 1);
        }

        public void Add(ItemType type, int amount)
        {
            for (int i = 0; i < ply.ammoBox.types.Length; i++)
            {
                if (ply.ammoBox.types[i].inventoryID == type)
                {
                    ammo[i] += amount;
                    break;
                }
            }
            ply.SetAmmo(AmmoType.Dropped5, 1);
            ply.SetAmmo(AmmoType.Dropped7, 1);
            ply.SetAmmo(AmmoType.Dropped9, 1);
        }

        public int GetItemAmmoType(ItemType gun)
        {
            for (int i = 0; i < ply.weaponManager.weapons.Length; i++)
            {
                if (ply.weaponManager.weapons[i].inventoryID == gun)
                    return ply.weaponManager.weapons[i].ammoType;
            }
            return -1;
        }

        public void Set(int amount)
        {
            ammo[GetItemAmmoType(ply.GetCurrentItem().id)] = amount;
            ply.SetAmmo(AmmoType.Dropped5, 1);
            ply.SetAmmo(AmmoType.Dropped7, 1);
            ply.SetAmmo(AmmoType.Dropped9, 1);
        }

        public int Get()
        {
            ply.SetAmmo(AmmoType.Dropped5, 1);
            ply.SetAmmo(AmmoType.Dropped7, 1);
            ply.SetAmmo(AmmoType.Dropped9, 1);
            return ammo[GetItemAmmoType(ply.GetCurrentItem().id)];
        }

        public override string ToString()
        {
            string str = "Ammo:";
            for (int i = 0; i < ply.ammoBox.types.Length; i++)
            {
                str += "\n" + ply.ammoBox.types[i].label + " - " + ammo[i];
            }
            return str;
        }
    }
}
