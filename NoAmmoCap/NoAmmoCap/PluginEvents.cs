using EXILED;
using EXILED.ApiObjects;
using EXILED.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NoAmmoCap
{
    public class PluginEvents
    {
        private PluginMain plugin;
        private ItemType[] allowed = new ItemType[]
        {
            ItemType.GunCOM15,
            ItemType.GunE11SR,
            ItemType.GunLogicer,
            ItemType.GunMP7,
            ItemType.GunProject90,
            ItemType.GunUSP,
        };

        public PluginEvents(PluginMain pluginMain)
        {
            this.plugin = pluginMain;
        }

        internal void Shoot(ref ShootEvent ev)
        {
            if (!PluginMain.ammo.ContainsKey(ev.Shooter))
                PluginMain.ammo.Add(ev.Shooter, new Ammo(ev.Shooter));
            ItemType id = ev.Shooter.inventory.curItem;
            if (id.IsWeapon(false))
            {
                ev.Shooter.SetAmmo(AmmoType.Dropped5, 1);
                ev.Shooter.SetAmmo(AmmoType.Dropped7, 1);
                ev.Shooter.SetAmmo(AmmoType.Dropped9, 1);
            }
        }

        internal void Spawn(PlayerSpawnEvent ev)
        {
            if (PluginMain.ammo.ContainsKey(ev.Player))
                PluginMain.ammo.Remove(ev.Player);
        }

        internal void PlyCmd(ConsoleCommandEvent ev)
        {
            if (!PluginMain.ammo.ContainsKey(ev.Player))
                PluginMain.ammo.Add(ev.Player, new Ammo(ev.Player));
            if (ev.Command.ToLower() == "ammo")
            {
                ev.Player.SendConsoleMessage(PluginMain.ammo[ev.Player].ToString(), "cyan");
                ev.ReturnMessage = "";
            }
        }

        internal void Reset()
        {
            PluginMain.ammo.Clear();
        }

        public int MaxAmmo(ReferenceHub hub)
        {
            return hub.weaponManager.weapons.First(i => i.inventoryID == hub.GetCurrentItem().id).maxAmmo;
        }

        internal void ReloadItem(ref PlayerReloadEvent ev)
        {
            if (!PluginMain.ammo.ContainsKey(ev.Player))
                PluginMain.ammo.Add(ev.Player, new Ammo(ev.Player));
            var id = ev.Player.GetCurrentItem();
            if (PluginMain.ammo[ev.Player].Get() > 0)
            {
                ev.Allow = true;
                int dur = (int)Pickup.Inv.GetItemByID(ev.Player.inventory.curItem).durability;
                PluginMain.ammo[ev.Player].Add(-dur);
                if (PluginMain.ammo[ev.Player].Get() <= 0)
                    PluginMain.ammo[ev.Player].Set(0);
                int newammo = (int)id.durability + dur + PluginMain.ammo[ev.Player].Get();
                if (newammo > MaxAmmo(ev.Player))
                {
                    PluginMain.ammo[ev.Player].Set(-(MaxAmmo(ev.Player) - newammo));
                    newammo = MaxAmmo(ev.Player);
                }
                ev.Player.SetWeaponAmmo(id, newammo);
                ev.Player.SetAmmo(AmmoType.Dropped5, 1);
                ev.Player.SetAmmo(AmmoType.Dropped7, 1);
                ev.Player.SetAmmo(AmmoType.Dropped9, 1);
            }
            else
            {
                ev.Allow = false;
                ev.Player.SetAmmo(AmmoType.Dropped5, 1);
                ev.Player.SetAmmo(AmmoType.Dropped7, 1);
                ev.Player.SetAmmo(AmmoType.Dropped9, 1);
            }
        }

        internal void ItemChange(ItemChangedEvent ev)
        {
            if (!PluginMain.ammo.ContainsKey(ev.Player))
                PluginMain.ammo.Add(ev.Player, new Ammo(ev.Player));
            if (ev.NewItem.id.IsWeapon(false))
            {
                ev.Player.SetAmmo(AmmoType.Dropped5, 1);
                ev.Player.SetAmmo(AmmoType.Dropped7, 1);
                ev.Player.SetAmmo(AmmoType.Dropped9, 1);
            }
        }

        internal void PickupEvent(ref PickupItemEvent ev)
        {
            if (!PluginMain.ammo.ContainsKey(ev.Player))
                PluginMain.ammo.Add(ev.Player, new Ammo(ev.Player));
            if (ev.Item.ItemId.IsAmmo())
            {
                ev.Allow = false;
                PluginMain.ammo[ev.Player].Add(ev.Item.info.itemId, (int)ev.Item.info.durability);
                ev.Player.SetAmmo(AmmoType.Dropped5, 1);
                ev.Player.SetAmmo(AmmoType.Dropped7, 1);
                ev.Player.SetAmmo(AmmoType.Dropped9, 1);
                ev.Item.Delete();
            }
        }
    }
}