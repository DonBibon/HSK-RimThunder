using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Vehicles;
using Verse;
using CombatExtended;

namespace RimThunder.AutoFixCE
{
    [StaticConstructorOnStartup]
    public static class AutoFixInit
    {
        static AutoFixInit()
        {
            try
            {
                ApplyHarmonyPatches();
                InjectVehicleAmmoRecipes();
                Log.Message("<color=green>[RimThunder]</color> CE & Vehicle Framework compatibility fixes successfully loaded.");
            }
            catch (Exception ex)
            {
                Log.Error("[RimThunder] Error applying compatibility fixes: " + ex);
            }
        }

        private static void ApplyHarmonyPatches()
        {
            Harmony harmony = new Harmony("RimThunder.AutoFixCE");
            Patch_VRF_ResolvePawnOverlap_Fix.Apply(harmony);
            Patch_Dialog_BillConfig_DoWindowContents.Apply(harmony);
        }

        private static void InjectVehicleAmmoRecipes()
        {
            try
            {
                ThingDef heavyBench = DefDatabase<ThingDef>.GetNamed("TableHeavyAmmunition", false);
                ThingDef ammoBench = DefDatabase<ThingDef>.GetNamed("AmmoBench", false);
                ThingDef machiningTable = DefDatabase<ThingDef>.GetNamed("TableMachining", false);

                int injectedCount = 0;
                var allRecipes = DefDatabase<RecipeDef>.AllDefsListForReading;

                foreach (AmmoDef ammo in DefDatabase<AmmoDef>.AllDefsListForReading)
                {
                    if (ammo.tradeTags == null || ammo.tradeTags.Count == 0)
                        continue;

                    RecipeDef recipe = DefDatabase<RecipeDef>.GetNamed("Make" + ammo.defName, false);
                    if (recipe == null)
                    {
                        recipe = DefDatabase<RecipeDef>.GetNamed("MakeAmmo_" + ammo.defName, false);
                    }
                    if (recipe == null)
                    {
                        for (int i = 0; i < allRecipes.Count; i++)
                        {
                            var r = allRecipes[i];
                            if (r.products != null)
                            {
                                for (int j = 0; j < r.products.Count; j++)
                                {
                                    if (r.products[j].thingDef == ammo)
                                    {
                                        recipe = r;
                                        break;
                                    }
                                }
                                if (recipe != null) break;
                            }
                        }
                    }

                    if (recipe == null)
                        continue;

                    bool injectedForThisAmmo = false;
                    foreach (string tag in ammo.tradeTags)
                    {
                        if (tag == "CE_AutoEnableCrafting_TableHeavyAmmunition")
                        {
                            ThingDef target = heavyBench ?? machiningTable ?? ammoBench;
                            if (target != null)
                            {
                                AddRecipeToBench(recipe, target);
                                injectedForThisAmmo = true;
                            }
                        }
                        else if (tag == "CE_AutoEnableCrafting")
                        {
                            ThingDef target = ammoBench ?? heavyBench ?? machiningTable;
                            if (target != null)
                            {
                                AddRecipeToBench(recipe, target);
                                injectedForThisAmmo = true;
                            }
                            if (heavyBench != null && heavyBench != target && (ammo.isMortarAmmo || ammo.statBases.GetStatValueFromList(StatDefOf.Mass, 0f) > 0.5f))
                            {
                                AddRecipeToBench(recipe, heavyBench);
                            }
                        }
                        else if (tag.StartsWith("CE_AutoEnableCrafting_"))
                        {
                            string benchName = tag.Substring("CE_AutoEnableCrafting_".Length);
                            ThingDef customBench = DefDatabase<ThingDef>.GetNamed(benchName, false);
                            if (customBench != null)
                            {
                                AddRecipeToBench(recipe, customBench);
                                injectedForThisAmmo = true;
                            }
                        }
                    }

                    if (injectedForThisAmmo)
                    {
                        injectedCount++;
                    }
                }

                if (heavyBench != null) ClearBenchCache(heavyBench);
                if (ammoBench != null) ClearBenchCache(ammoBench);
                if (machiningTable != null) ClearBenchCache(machiningTable);

                Log.Message(string.Format("<color=green>[RimThunder]</color> Successfully verified/injected {0} ammo recipes into crafting benches.", injectedCount));

                try
                {
                    Type hbType = AccessTools.TypeByName("HelpTab.HelpBuilder");
                    if (hbType != null)
                    {
                        MethodInfo resolveRecipes = AccessTools.Method(hbType, "ResolveRecipes");
                        if (resolveRecipes != null)
                        {
                            resolveRecipes.Invoke(null, null);
                            Log.Message("<color=green>[RimThunder]</color> WikiRim HelpBuilder.ResolveRecipes() re-indexed.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Warning("[RimThunder] WikiRim re-index skipped: " + ex.Message);
                }
            }
            catch (Exception ex)
            {
                Log.Error("[RimThunder] Error in InjectVehicleAmmoRecipes: " + ex);
            }
        }

        private static void AddRecipeToBench(RecipeDef recipe, ThingDef bench)
        {
            if (recipe.recipeUsers == null)
            {
                recipe.recipeUsers = new List<ThingDef>();
            }
            if (!recipe.recipeUsers.Contains(bench))
            {
                recipe.recipeUsers.Add(bench);
            }
            if (bench.recipes != null && !bench.recipes.Contains(recipe))
            {
                bench.recipes.Add(recipe);
            }
        }

        private static void ClearBenchCache(ThingDef bench)
        {
            try
            {
                FieldInfo fi = AccessTools.Field(typeof(ThingDef), "allRecipesCached");
                if (fi != null)
                {
                    fi.SetValue(bench, null);
                }
            }
            catch { }
        }
    }

    public static class Patch_VRF_ResolvePawnOverlap_Fix
    {
        public static void Apply(Harmony harm)
        {
            try
            {
                Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                Type type = null;
                for (int i = 0; i < assemblies.Length; i++)
                {
                    type = assemblies[i].GetType("VehicleRaidFramework.VehicleRaidStructureUtility");
                    if (type != null) break;
                }
                if (type == null) return;

                MethodInfo targetMethod = AccessTools.Method(type, "ResolvePawnOverlap");
                if (targetMethod == null) return;

                harm.Patch(targetMethod, prefix: new HarmonyMethod(typeof(Patch_VRF_ResolvePawnOverlap_Fix), "Prefix"));
            }
            catch { }
        }

        public static bool Prefix(Pawn pawn, IntVec3 currentPos, Map map)
        {
            if (pawn == null || map == null || !currentPos.IsValid || !currentPos.InBounds(map))
            {
                return false;
            }
            return true;
        }
    }

    public static class Patch_Dialog_BillConfig_DoWindowContents
    {
        private static readonly FieldInfo f_products = AccessTools.Field(typeof(RecipeDef), "products");
        private static readonly FieldInfo f_uiIconThing = AccessTools.Field(typeof(RecipeDef), "uiIconThing");
        private static readonly MethodInfo m_helper = AccessTools.Method(typeof(Patch_Dialog_BillConfig_DoWindowContents), "Helper_GetProducts");

        public static void Apply(Harmony harm)
        {
            try
            {
                MethodInfo original = AccessTools.Method(typeof(Dialog_BillConfig), "DoWindowContents", new Type[] { typeof(Rect) });
                MethodInfo transpiler = AccessTools.Method(typeof(Patch_Dialog_BillConfig_DoWindowContents), "Transpiler");
                if (original != null && transpiler != null)
                {
                    harm.Patch(original, transpiler: new HarmonyMethod(transpiler));
                    Log.Message("<color=green>[RimThunder]</color> Dialog_BillConfig InfoCard patch successfully applied.");
                }
            }
            catch (Exception ex)
            {
                Log.Warning("[RimThunder] Dialog_BillConfig patch skipped: " + ex.Message);
            }
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (var instruction in instructions)
            {
                if (instruction.LoadsField(f_products))
                {
                    yield return new CodeInstruction(OpCodes.Call, m_helper);
                }
                else
                {
                    yield return instruction;
                }
            }
        }

        public static List<ThingDefCountClass> Helper_GetProducts(RecipeDef recipe)
        {
            if (recipe == null) return null;
            if (recipe.products != null && recipe.products.Count > 0)
                return recipe.products;

            ThingDef vehDef = GetVehicleDefForRecipe(recipe);
            if (vehDef != null)
            {
                return new List<ThingDefCountClass> { new ThingDefCountClass(vehDef, 1) };
            }
            return recipe.products;
        }

        public static ThingDef GetVehicleDefForRecipe(RecipeDef recipe)
        {
            if (recipe == null) return null;
            if (recipe.modExtensions != null)
            {
                for (int i = 0; i < recipe.modExtensions.Count; i++)
                {
                    var ext = recipe.modExtensions[i];
                    if (ext != null && ext.GetType().Name == "ModExt_RTCVehicleRecipe")
                    {
                        var field = ext.GetType().GetField("thing", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                        if (field != null)
                        {
                            ThingDef td = field.GetValue(ext) as ThingDef;
                            VehicleBuildDef vbd = td as VehicleBuildDef;
                            if (vbd != null && vbd.thingToSpawn != null)
                                return vbd.thingToSpawn;
                            return td;
                        }
                    }
                }
            }
            if (f_uiIconThing != null)
            {
                ThingDef rawIcon = f_uiIconThing.GetValue(recipe) as ThingDef;
                if (rawIcon != null)
                {
                    VehicleBuildDef vbd2 = rawIcon as VehicleBuildDef;
                    if (vbd2 != null && vbd2.thingToSpawn != null)
                        return vbd2.thingToSpawn;
                    if (rawIcon.thingClass != null && typeof(VehiclePawn).IsAssignableFrom(rawIcon.thingClass))
                        return rawIcon;
                }
            }
            return null;
        }
    }
}
