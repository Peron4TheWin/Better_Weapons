using HarmonyLib;
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.Dialogue;
using Il2CppScheduleOne.Equipping;
using Il2CppScheduleOne.ItemFramework;
using Il2CppScheduleOne.UI;
using MelonLoader;
using UnityEngine;
using ModManagerPhoneApp;

[assembly: MelonInfo(typeof(BetterWeapons.EntryPoint), "BetterWeapons", "1.0.1", "_peron")]

namespace BetterWeapons
{
    public class EntryPoint : MelonMod
    {
        public static MelonPreferences_Category Maincategory;
        public static MelonPreferences_Category _1911category;
        public static MelonPreferences_Category Revolvercategory;
        public static MelonPreferences_Entry<int> Damage;
        public static MelonPreferences_Entry<bool> SuperAccuracy;
        public static MelonPreferences_Entry<bool> NoCockPit;
        public static MelonPreferences_Entry<bool> InfiniteAmmo;
        public static MelonPreferences_Entry<int> _1911Price;
        public static MelonPreferences_Entry<int> RevolverPrice;
        public static MelonPreferences_Entry<int> _1911AmmoCapacity;
        public static MelonPreferences_Entry<int> RevolverAmmoCapacity;
        public static MelonPreferences_Entry<int> _1911MagazineCapacity;
        public static MelonPreferences_Entry<int> RevolverMagazineCapacity;
        public static MelonPreferences_Entry<int> _1911MagazinePrice;
        public static MelonPreferences_Entry<int> RevolverMagazinePrice;
            
        public override void OnInitializeMelon()
        {
            _1911category = MelonPreferences.CreateCategory("BetterWeapons_1911", "Better Weapons - 1911");
            _1911Price = _1911category.CreateEntry("BetterWeapons_1911_price", 2500, "Price for 1911 pistol");
            _1911AmmoCapacity = _1911category.CreateEntry("BetterWeapons_1911_ammo_capacity", 7, "Ammo capacity for 1911");
            _1911MagazineCapacity = _1911category.CreateEntry("BetterWeapons_1911_magazine_capacity", 7, "Magazine capacity for 1911");
            _1911MagazinePrice = _1911category.CreateEntry("BetterWeapons_1911_magazine_price", 20, "Price for 1911 magazines");
            
            Revolvercategory = MelonPreferences.CreateCategory("BetterWeapons_Revolver", "Better Weapons - Revolver");
            RevolverPrice = Revolvercategory.CreateEntry("BetterWeapons_revolver_price", 1000, "Price for Revolver");
            RevolverAmmoCapacity = Revolvercategory.CreateEntry("BetterWeapons_revolver_ammo_capacity", 6, "Ammo capacity for Revolver");
            RevolverMagazineCapacity = Revolvercategory.CreateEntry("BetterWeapons_revolver_magazine_capacity", 6, "Magazine capacity for Revolver");
            RevolverMagazinePrice = Revolvercategory.CreateEntry("BetterWeapons_revolver_magazine_price", 10, "Price for Revolver magazines");
            
            Maincategory = MelonPreferences.CreateCategory("BetterWeapons", "Better Weapons");
            Damage = Maincategory.CreateEntry("BetterWeapons_1_damage", 60, "Weapon damage (For all weapons)");
            SuperAccuracy = Maincategory.CreateEntry("BetterWeapons_super_accuracy", false, "Improved weapon accuracy");
            NoCockPit = Maincategory.CreateEntry("BetterWeapons_no_cockpit", false, "Disable cockpit");
            InfiniteAmmo = Maincategory.CreateEntry("BetterWeapons_infinite_ammo", false, "Unlimited ammunition");
            _1911category.SetFilePath("UserData/BetterWeapons.cfg");
            Revolvercategory.SetFilePath("UserData/BetterWeapons.cfg");
            Maincategory.SetFilePath("UserData/BetterWeapons.cfg");
            _1911category.LoadFromFile();
            Revolvercategory.LoadFromFile();
            Maincategory.LoadFromFile();
            try
            {
                ModSettingsEvents.OnPreferencesSaved += LoadAllCategories;
                LoggerInstance.Msg("Successfully subscribed to Mod Manager save event.");
            }
            catch (Exception ex)
            {
                LoggerInstance.Warning($"Could not subscribe to Mod Manager event (Mod Manager may not be installed/compatible): {ex.Message}");
            }
        }
        
        public static void LoadAllCategories()
        {
            foreach (MelonPreferences_Category category in MelonPreferences.Categories)
            {
                category.LoadFromFile();
            }
        }


        [HarmonyPatch(typeof(Il2CppScheduleOne.Registry))]
        class LoadItems
        {
            [HarmonyPatch(nameof(Il2CppScheduleOne.Registry.Awake))]
            [HarmonyPostfix]
            static void PostRegistry()
            {
                //1911
                GameObject m1911Object = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(obj => obj.name == "M1911_Equippable" && obj.transform.parent == null);
                if (m1911Object != null)
                {
                    Equippable_RangedWeapon weaponComponent = m1911Object.GetComponent<Equippable_RangedWeapon>();

                    if (weaponComponent != null)
                    {
                        weaponComponent.MagazineSize = (int)_1911AmmoCapacity.BoxedValue;
                        StorableItemDefinition magazineDefinition = weaponComponent.Magazine;
                        if (magazineDefinition != null)
                        {
                            IntegerItemDefinition intMagazineDefinition =
                                magazineDefinition.TryCast<IntegerItemDefinition>();
                            if (intMagazineDefinition != null)
                            {
                                intMagazineDefinition.DefaultValue = (int)_1911MagazineCapacity.BoxedValue;
                            }
                        }
                    }
                }


                //revolver
                GameObject revolverObject = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(obj => obj.name == "Revolver_Equippable" && obj.transform.parent == null);
                if (revolverObject != null)
                {
                    Equippable_RangedWeapon weaponComponent = revolverObject.GetComponent<Equippable_RangedWeapon>();
                    if (weaponComponent != null)
                    {
                        weaponComponent.MagazineSize = (int)RevolverAmmoCapacity.BoxedValue;
                        StorableItemDefinition magazineDefinition = weaponComponent.Magazine;
                        if (magazineDefinition != null)
                        {
                            IntegerItemDefinition intMagazineDefinition =
                                magazineDefinition.TryCast<IntegerItemDefinition>();
                            if (intMagazineDefinition != null)
                            {
                                intMagazineDefinition.DefaultValue = (int)RevolverMagazineCapacity.BoxedValue;
                            }
                        }
                    }
                }

            }
        }

        [HarmonyPatch(typeof(Il2CppScheduleOne.Dialogue.DialogueController_ArmsDealer))]
        class ArmDealer
        {
            [HarmonyPatch(nameof(Il2CppScheduleOne.Dialogue.DialogueController_ArmsDealer.Awake))]
            [HarmonyPostfix]
            public static void DealerPrefix(Il2CppScheduleOne.Dialogue.DialogueController_ArmsDealer __instance)
            {
                Dictionary<string, int> weaponPriceMap = new Dictionary<string, int>
                {
                    { "M1911", (int)_1911Price.BoxedValue },
                    { "Revolver", (int)RevolverPrice.BoxedValue },
                    { "Revolver Ammo", (int)RevolverMagazinePrice.BoxedValue },
                    { "M1911 Magazine", (int)_1911MagazinePrice.BoxedValue }
                };

                foreach (DialogueController_ArmsDealer.WeaponOption weapon in __instance.allWeapons)
                {
                    if (weaponPriceMap.ContainsKey(weapon.Name))
                    {
                        weapon.Price = weaponPriceMap[weapon.Name];
                    }
                }
            }
        }

        


        [HarmonyPatch(typeof(Il2CppScheduleOne.Equipping.Equippable_RangedWeapon))]
        public class EquippableRangedWeapons
        {
            [HarmonyPatch(nameof(Il2CppScheduleOne.Equipping.Equippable_RangedWeapon.Fire))]
            [HarmonyPrefix]
            public static bool FirePrefix(Il2CppScheduleOne.Equipping.Equippable_RangedWeapon __instance)
            {

                __instance.Damage = (int)Damage.BoxedValue;
                if (__instance.MustBeCocked)
                {
                    __instance.MustBeCocked = !((bool)NoCockPit.BoxedValue);
                }

                if ((bool)SuperAccuracy.BoxedValue)
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
                if ((bool)InfiniteAmmo.BoxedValue)
                {
                    if (__instance.weaponItem.Value <= 2)
                    {
                        __instance.weaponItem.Value++;
                    }
                }
            }
    }
}
}
