﻿using TreeRoutine.FlaskComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using ExileCore.PoEMemory.Components;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared.Enums;

namespace TreeRoutine.DefaultBehaviors.Helpers
{
    public class FlaskHelper<TSettings, TCache>
        where TSettings : BaseTreeSettings, new()
        where TCache : BaseTreeCache, new()
    {
        public BaseTreeRoutinePlugin<TSettings, TCache> Core { get; set; }

        public const String ChargeReductionModName = "flaskchargesused";

        public List<PlayerFlask> GetAllFlaskInfo()
        {
            var flaskItems = Core.GameController.Game.IngameState.ServerData.PlayerInventories
                .FirstOrDefault(x => x.Inventory.InventType == InventoryTypeE.Flask)?.Inventory?.InventorySlotItems;

            if (flaskItems == null)
            {
                return null;
            }

            var flaskList = new List<PlayerFlask>();
            foreach (var flaskItem in flaskItems)
            {
                var flask = GetFlaskInfo((int)flaskItem.PosX, flaskItem.Item);
                if (flask != null)
                    flaskList.Add(flask);
            }
            return flaskList;
        }

        public PlayerFlask GetFlaskInfo(int index, Entity foundFlask=null)
        {
            if (Core.Cache.MiscBuffInfo == null)
            {
                Core.LogErr($"{Core.Name}: Error: Misc Buff Info cache was never initialized. This method will not function properly.", Core.ErrmsgTime);
                return null;
            }

            Entity currentFlask = foundFlask ?? Core.GameController.Game.IngameState.ServerData.PlayerInventories.FirstOrDefault(x => x.Inventory.InventType == InventoryTypeE.Flask)?.Inventory?.InventorySlotItems?.FirstOrDefault(x => x.InventoryPosition.X == index)?.Item;
            if (currentFlask == null || currentFlask.Address == 0x00)
            {
                return null;
            }

            if (string.IsNullOrEmpty(currentFlask.Path))
            {
                Core.LogErr($"{Core.Name}: Ignoring Flask {index} for an empty or null path.", 5);
                return null;
            }

            if (currentFlask.Path.Contains("Tinctures/Tincture"))
            {
                return null;
            }

            PlayerFlask simplePlayerFlask = new PlayerFlask();
            simplePlayerFlask.Index = index;

            if (string.IsNullOrEmpty(currentFlask.Path))
            {
                Core.LogErr($"{Core.Name}: Ignoring Flask {index} for an empty or null path.", 5);
                return null;
            }

            var baseItem = Core.GameController.Files.BaseItemTypes.Translate(currentFlask.Path);
            if (baseItem == null)
            {
                Core.LogErr($"{Core.Name}: Ignoring Flask {index}. No base item was found! Path: {currentFlask.Path}", 5);
                return null;
            }
            
            simplePlayerFlask.Name = baseItem.BaseName;


            Charges flaskChargesStruct = currentFlask.GetComponent<Charges>();
            Mods flaskMods = currentFlask.GetComponent<Mods>();

            var useCharge = CalculateUseCharges(flaskChargesStruct.ChargesPerUse, flaskMods.ItemMods);
            if (useCharge > 0)
                simplePlayerFlask.TotalUses = flaskChargesStruct.NumCharges / useCharge;

            //TreeRoutine.LogError("Flask: " + simplePlayerFlask.Name + "Num Charges: " + flaskChargesStruct.NumCharges + " Use Charges: " + useCharge + " Charges Per use: " + flaskChargesStruct.ChargesPerUse + " Total Uses: " + simplePlayerFlask.TotalUses, 5);


            var flaskBaseName = currentFlask.GetComponent<Base>().Name ?? "NULL";
            if (!Core.Cache.MiscBuffInfo.flaskNameToBuffConversion.TryGetValue(
                flaskBaseName, out string flaskBuffOut))
            {
                if (Core.Settings.Debug)
                    Core.LogErr($"{Core.Name}: Cannot find Flask Buff for flask on slot {index + 1} with base name: {flaskBaseName}", 5);
                return null;
            }

            simplePlayerFlask.BuffString1 = flaskBuffOut;

            // For Hybrid Flask as it have two buffs.
            simplePlayerFlask.BuffString2 = Core.Cache.MiscBuffInfo.flaskNameToBuffConversion2.TryGetValue(flaskBaseName, out flaskBuffOut) 
                ? flaskBuffOut 
                : "";

            simplePlayerFlask.Mods = currentFlask.GetComponent<Mods>();

            HandleFlaskMods(simplePlayerFlask);

            return simplePlayerFlask;
        }

        private int CalculateUseCharges(float BaseUseCharges, List<ItemMod> flaskMods)
        {
            var playerStats = Core.GameController.EntityListWrapper.Player.GetComponent<Stats>();

            if (playerStats == null || !playerStats.StatDictionary.TryGetValue(GameStat.FlaskChargesUsedPct, out int totalChargeReduction))
                totalChargeReduction = 0;

            if (totalChargeReduction > 0)
                BaseUseCharges = ((100 + totalChargeReduction) / 100) * BaseUseCharges;
            foreach (var mod in flaskMods)
            {
                if (mod.Name.Contains(ChargeReductionModName, StringComparison.OrdinalIgnoreCase))
                    BaseUseCharges = ((100 + (float)mod.Value1) / 100) * BaseUseCharges;
            }
            return (int)Math.Floor(BaseUseCharges);
        }

        private void HandleFlaskMods(PlayerFlask flask)
        {
            if (Core.Cache.FlaskInfo == null)
            {
                Core.LogErr($"{Core.Name}: Error: Flask Info cache was never initialized. This method will not function properly.", Core.ErrmsgTime);
                return;
            }

            //Checking flask action based on flask name type.
            if (!Core.Cache.FlaskInfo.FlaskTypes.TryGetValue(flask.Name, out FlaskActions flaskActionOut))
                Core.LogConfigErr($"{Core.Name}: Error: Flask type {flask.Name} not found. You can add it to config/flaskinfo.json.", Core.ErrmsgTime);
            else flask.Action1 = flaskActionOut;

            //Checking for unique flasks.
            if (flask.Mods.ItemRarity == ItemRarity.Unique)
            {
                flask.Name = flask.Mods.UniqueName;

                //Enabling Unique flask action 2.
                if (!Core.Cache.FlaskInfo.UniqueFlaskNames.TryGetValue(flask.Name, out flaskActionOut))
                    Core.LogConfigErr($"{Core.Name}: Error: Unique flask name {flask.Name} not found. You can add it to config/flaskinfo.json.", Core.ErrmsgTime);
                else flask.Action2 = flaskActionOut;
            }

            foreach (var mod in flask.Mods.ItemMods)
            {
                var modName = mod.Name;
                if (modName.Contains("instant", StringComparison.OrdinalIgnoreCase))
                {
                    if (modName.Contains("FlaskPartialInstantRecovery"))
                        flask.InstantType = FlaskInstantType.Partial;
                    else if (modName.Contains("FlaskInstantRecoveryOnLowLife"))
                        flask.InstantType = FlaskInstantType.LowLife;
                    else if (modName.Contains("FlaskFullInstantRecovery"))
                        flask.InstantType = FlaskInstantType.Full;
                }

                // We have already decided action2 for unique flasks.
                if (flask.Mods.ItemRarity == ItemRarity.Unique)
                    continue;

                if (modName == "FlaskEffectNotRemovedOnFullMana")
                {
                    flask.RemovedWhenFull = false;
                    flask.BuffString2 = "flask_effect_mana_not_removed_when_full";
                }

                //Checking flask mods.
                if (!Core.Cache.FlaskInfo.FlaskMods.TryGetValue(modName, out FlaskActions action2))
                    Core.LogConfigErr($"{Core.Name}: Error: Mod {modName} (called '{mod.DisplayName}' ingame) not found. You can add it to config/flaskinfo.json.", Core.ErrmsgTime);
                else if (action2 != FlaskActions.Ignore)
                    flask.Action2 = action2;
            }
        }

        public Boolean CanUsePotion(int flaskIndex, int reservedUses = 0, bool ignoreActionType = false)
        {
            PlayerFlask flask = this.GetFlaskInfo(flaskIndex);
            if (flask == null)
            {
                if (Core.Settings.Debug)
                    Core.Log($"{Core.Name}: Cannot use a null flask in slot {flaskIndex}.", 1);
                return false;
            }

            return CanUsePotion(flask, reservedUses, ignoreActionType);
        }

        public Boolean CanUsePotion(PlayerFlask flask, int reservedUses=0, bool ignoreActionType = false)
        {
            if (flask == null)
            {
                if (Core.Settings.Debug)
                    Core.Log($"{Core.Name}: Cannot use a null flask.", 1);
                return false;
            }
                

            if (flask.TotalUses - reservedUses <= 0)
            {
                if (Core.Settings.Debug)
                    Core.Log($"{Core.Name}: Don't have enough uses on flask {flask.Name} to use.", 1);
                return false;
            }

            if (ignoreActionType)
                return true;

            if (flask.Action1 == FlaskActions.Life && !(Core.PlayerHelper.isHealthBelowPercentage(99) || Core.PlayerHelper.isEnergyShieldBelowPercentage(99)))
            {
                if (Core.Settings.Debug)
                    Core.Log($"{Core.Name}: Can't use life flask {flask.Name} at full health and energy shiled.", 1);
                return false;
            }

            if (flask.Action1 == FlaskActions.Mana && !Core.PlayerHelper.isManaBelowPercentage(99))
            {
                if (Core.Settings.Debug)
                    Core.Log($"{Core.Name}: Can't use mana flask {flask.Name} at full mana.", 1);
                return false;
            }

            if (flask.Action1 == FlaskActions.Hybrid && !(Core.PlayerHelper.isHealthBelowPercentage(99) || Core.PlayerHelper.isManaBelowPercentage(99)))
            {
                if (Core.Settings.Debug)
                    Core.Log($"{Core.Name}: Can't use hybrid {flask.Name} at full health and mana.", 1);
                return false;
            }

            return true;
        }
    }
}
