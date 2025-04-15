using System.Reflection;
using HarmonyLib;
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.Equipping;
using Il2CppScheduleOne.UI;
using Il2CppSystem.Runtime.Serialization.Formatters.Binary;
using MelonLoader;
using UnityEngine;

[assembly: MelonInfo(typeof(BetterWeapons.EntryPoint), "BetterWeapons", "1.0.1", "_peron")]

namespace BetterWeapons
{
    public class EntryPoint : MelonMod
    {
        public override void OnInitializeMelon()
        {
            Config.OnLoad();
        }
        
    }

    public class Config
    {
        public static int Damage = 120;
        public static bool SuperAccuracy = true;
        public static bool NoCockPit = true;
        public static bool InfiniteAmmo = true;
        

        public static string createConfig()
        {
            string directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string text = Path.Combine(Path.Combine(directoryName, "BetterWeapons"));
            MelonLogger.Msg("BetterWeapons Config File Path: " + text);
            bool flag = !Directory.Exists(text);
            if (flag)
            {
                Directory.CreateDirectory(text);
            }
            string path = Path.Combine(text, "config.ini");
            bool flag2 = !File.Exists(path);
            if (flag2)
            {
                string[] contents = new string[]
                {
                    "# Better weapons mod configuration",
                    "#How many damage should guns do? Default 60 (An npc has 100HP)",
                    "Damage=60",
                    "#If this is true, bullet will not spread",
                    "SuperAccuracy=false",
                    "#This will get you infinite ammo",
                    "InfiniteAmmo=False",
                    "#If this is true you wont need to cock the revolver",
                    "NoCockPit=True",
                };
                File.WriteAllLines(path, contents);
                MelonLogger.Msg("Config file created with default values.");
            }

            return path;
        }

        public static void OnLoad()
        {
            
            bool lastupdated = false;
            string[] array = File.ReadAllLines(createConfig());
            foreach (string text2 in array)
            {
                bool flag3 = string.IsNullOrWhiteSpace(text2) || text2.TrimStart().StartsWith("#");
                if (!flag3)
                {
                    string[] array3 = text2.Split('=', StringSplitOptions.None);
                    bool flag4 = array3.Length < 2;
                    if (!flag4)
                    {
                        string text3 = array3[0].Trim();
                        string text4 = array3[1].Trim();
                        if (text3.Equals("Damage", StringComparison.CurrentCultureIgnoreCase))
                        {
                            Damage = int.Parse(text4);
                        }
                        else if (text3.Equals("SuperAccuracy", StringComparison.OrdinalIgnoreCase))
                        {
                            SuperAccuracy = bool.Parse(text4);
                        } else if (text3.Equals("InfiniteAmmo", StringComparison.OrdinalIgnoreCase))
                        {
                            InfiniteAmmo = bool.Parse(text4);
                        }
                        else if (text3.Equals("NoCockPit", StringComparison.OrdinalIgnoreCase))
                        {
                            NoCockPit = bool.Parse(text4);
                        }
                    }
                }
                
            }
        }
    }
        
    [HarmonyPatch(typeof(Il2CppScheduleOne.PlayerScripts.Player))]
    [HarmonyPatch("PlayerLoaded")]
    class ConfigLoad
    {
        [HarmonyPostfix]
        static void PostfixPlayerLoaded(Il2CppScheduleOne.PlayerScripts.Player __instance)
        {
            Config.OnLoad();
        }
    }
    

    [HarmonyPatch(typeof(Il2CppScheduleOne.Equipping.Equippable_RangedWeapon))]
    public class EquippableRangedWeapons
    {
        [HarmonyPatch(nameof(Il2CppScheduleOne.Equipping.Equippable_RangedWeapon.Fire))]
        [HarmonyPrefix]
        public static bool FirePrefix(Il2CppScheduleOne.Equipping.Equippable_RangedWeapon __instance)
        {
            
            __instance.Damage = Config.Damage;
            __instance.MustBeCocked = !Config.NoCockPit;
            if (Config.SuperAccuracy)
            {
                __instance.MaxSpread = 0.2f;
                __instance.MinSpread = 0.1f;
            }

            
            return true;
        }

        [HarmonyPatch(nameof(Il2CppScheduleOne.Equipping.Equippable_RangedWeapon.Equip))]
        [HarmonyPostfix]
        public static void EquipPostFix(Il2CppScheduleOne.Equipping.Equippable_RangedWeapon __instance)
        {
            Singleton<HUD>.Instance.SetCrosshairVisible(true);
        }

        [HarmonyPatch(nameof(Equippable_RangedWeapon.Update))]
        [HarmonyPostfix]
        public static void Update(Equippable_RangedWeapon __instance)
        {
            Singleton<HUD>.Instance.SetCrosshairVisible(true);
            if (Config.InfiniteAmmo)
            {
                if (__instance.weaponItem.Value <=2)
                {
                    __instance.weaponItem.Value++;
                }
            }
            //__instance.Update();
            //__instance.UpdateInput();
            //__instance.UpdateAnim();
            //__instance.TimeSinceFire += Time.deltaTime;
            //return false;
        }
    }
    
    
}
