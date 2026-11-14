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
