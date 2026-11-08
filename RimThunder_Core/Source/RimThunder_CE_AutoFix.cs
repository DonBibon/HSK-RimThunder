using System;
using System.Collections.Generic;
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
                ApplyTurretSpeedFix();
                ApplyHarmonyPatches();
                Log.Message("<color=green>[RimThunder]</color> CE & Vehicle Framework compatibility fixes successfully loaded.");
            }
            catch (Exception ex)
            {
                Log.Error("[RimThunder] Error applying compatibility fixes: " + ex);
            }
        }

        private static void ApplyTurretSpeedFix()
        {
            if (VehicleTurret.LaunchProjectileCE == null)
            {
                Log.Warning("[RimThunder] VehicleTurret.LaunchProjectileCE was null during startup. Retrying hook.");
                return;
            }

            var origLaunch = VehicleTurret.LaunchProjectileCE;
            var origAngle = VehicleTurret.ProjectileAngleCE;

            VehicleTurret.ProjectileAngleCE = (speed, range, shooter, target, origin, flyOverhead, sightsEfficiency, sway, spread, recoil) =>
            {
                ProjectilePropertiesCE props = Patch_VehicleTurret_FireTurretCE.currentFiringProps;
                float gravity = 1f;
                if (props != null)
                {
                    gravity = props.GravityPerWidth;
                    if (speed <= 0f)
                    {
                        speed = props.speed;
                    }
                }
                if (speed <= 0f)
                {
                    speed = 70f;
                }

                if (origAngle != null)
                {
                    return origAngle(speed, range, shooter, target, origin, flyOverhead, gravity, sway, spread, recoil);
                }

                Bounds boundsFor = CE_Utility.GetBoundsFor(target.Thing);
                float num = (boundsFor.max.y + boundsFor.min.y) / 2f - origin.y;
                float shotAngle = CE_Utility.GetShotAngle(speed, range, num, flyOverhead, gravity);
                float num2 = (float)Find.TickManager.TicksAbs + (shooter != null ? (float)shooter.thingIDNumber : 0f);
                float num3 = sway * Mathf.Sin(num2 * 0.022f);
                shotAngle += 0.004363323f * sway * Mathf.Sin(num2 * 0.0165f);
                double num4 = (double)Rand.Value * Math.PI * 2.0;
                double num5 = (double)Rand.Value * (double)spread;
                return new Vector2((float)(num3 + num5 * Math.Cos(num4)), (float)(shotAngle + num5 * Math.Sin(num4) + (double)recoil));
            };

            VehicleTurret.LaunchProjectileCE = (projectileDef, ammoDef, ammosetDef, origin, target, vehicle, shotAngle, shotRotation, shotHeight, shotSpeed) =>
            {
                if (shotSpeed <= 0f)
                {
                    ProjectilePropertiesCE props = (projectileDef != null) ? projectileDef.projectile as ProjectilePropertiesCE : null;
                    if (props != null && props.speed > 0f)
                    {
                        shotSpeed = props.speed;
                    }
                    else
                    {
                        shotSpeed = 70f;
                    }
                }
                return origLaunch(projectileDef, ammoDef, ammosetDef, origin, target, vehicle, shotAngle, shotRotation, shotHeight, shotSpeed);
            };
        }

        private static void ApplyHarmonyPatches()
        {
            Harmony harmony = new Harmony("RimThunder.AutoFixCE");
            Patch_VehicleTurret_FireTurretCE.Apply(harmony);
            Patch_VRF_ResolvePawnOverlap_Fix.Apply(harmony);
            Patch_MoteMaker_ThrowText.Apply(harmony);
            Patch_OpticEvasion_Fix.Apply(harmony);
            Patch_TurretReload_Fix.Apply(harmony);
            Patch_SmarterRaidAI_NullFaction_Fix.Apply(harmony);
            Patch_VehicleTurret_ScaleUIRect_Fix.Apply(harmony);
            Patch_GridsUtility_Fogged_Fix.Apply(harmony);
            Patch_SRAI_Sapper_Fix.Apply(harmony);
            Patch_Log_Error_Filter.Apply(harmony);
            Patch_ProjectileCE_SpawnsThing_Impact.Apply(harmony);
            Patch_Dialog_BillConfig_DoWindowContents.Apply(harmony);
            // Patch_RecipeDef_ProducedThingDef.Apply(harmony); // Disabled to prevent any risk of recursion on non-vehicle benches
        }
    }

    public static class Patch_VehicleTurret_FireTurretCE
    {
        [ThreadStatic]
        public static ProjectilePropertiesCE currentFiringProps;

        public static void Apply(Harmony harmony)
        {
            try
            {
                MethodInfo original = AccessTools.Method(typeof(VehicleTurret), "FireTurretCE", new Type[] { typeof(ThingDef), typeof(Vector3) });
                MethodInfo prefix = AccessTools.Method(typeof(Patch_VehicleTurret_FireTurretCE), "Prefix");
                MethodInfo postfix = AccessTools.Method(typeof(Patch_VehicleTurret_FireTurretCE), "Postfix");
                if (original != null && prefix != null)
                {
                    harmony.Patch(original, prefix: new HarmonyMethod(prefix), postfix: new HarmonyMethod(postfix));
                }
            }
            catch (Exception ex)
            {
                Log.Warning("[RimThunder] FireTurretCE patch skipped: " + ex.Message);
            }
        }

        public static void Prefix(VehicleTurret __instance, ThingDef projectileDef, Vector3 launchPos)
        {
            currentFiringProps = (projectileDef != null) ? projectileDef.projectile as ProjectilePropertiesCE : null;
        }

        public static void Postfix()
        {
            currentFiringProps = null;
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

    public static class Patch_MoteMaker_ThrowText
    {
        public static void Apply(Harmony harm)
        {
            try
            {
                MethodInfo targetMethod = AccessTools.Method(typeof(MoteMaker), "ThrowText", new Type[] { typeof(Vector3), typeof(Map), typeof(string), typeof(float) });
                if (targetMethod != null)
                {
                    harm.Patch(targetMethod, prefix: new HarmonyMethod(typeof(Patch_MoteMaker_ThrowText), "Prefix"));
                }
            }
            catch { }
        }

        public static bool Prefix(Vector3 loc, Map map, string text, float timeBeforeStartUpwardMovement)
        {
            if (map == null) return false;
            return true;
        }
    }

    public static class Patch_OpticEvasion_Fix
    {
        public static void Apply(Harmony harm)
        {
            try
            {
                Type type = AccessTools.TypeByName("CombatExtended.CE_Utility");
                if (type == null) return;
                MethodInfo targetMethod = AccessTools.Method(type, "GetOpticEvasion");
                if (targetMethod != null)
                {
                    harm.Patch(targetMethod, prefix: new HarmonyMethod(typeof(Patch_OpticEvasion_Fix), "Prefix"));
                }
            }
            catch { }
        }

        public static bool Prefix(Thing shooter, Thing target, ref float __result)
        {
            if (target is VehiclePawn)
            {
                __result = 0f;
                return false;
            }
            return true;
        }
    }

    public static class Patch_TurretReload_Fix
    {
        public static void Apply(Harmony harm)
        {
            try
            {
                MethodInfo targetMethod = AccessTools.Method(typeof(VehicleTurret), "ReloadTimerTick");
                if (targetMethod != null)
                {
                    harm.Patch(targetMethod, prefix: new HarmonyMethod(typeof(Patch_TurretReload_Fix), "Prefix"));
                }
            }
            catch { }
        }

        public static bool Prefix(VehicleTurret __instance)
        {
            if (__instance == null || __instance.vehicle == null) return false;
            return true;
        }
    }

    public static class Patch_SmarterRaidAI_NullFaction_Fix
    {
        public static void Apply(Harmony harm)
        {
            try
            {
                Type type = AccessTools.TypeByName("SmarterRaidAI.SmarterRaidAIUtility");
                if (type == null) return;
                MethodInfo targetMethod = AccessTools.Method(type, "IsEnemy");
                if (targetMethod != null)
                {
                    harm.Patch(targetMethod, prefix: new HarmonyMethod(typeof(Patch_SmarterRaidAI_NullFaction_Fix), "Prefix"));
                }
            }
            catch { }
        }

        public static bool Prefix(Thing a, Thing b, ref bool __result)
        {
            if (a == null || b == null || a.Faction == null || b.Faction == null)
            {
                __result = false;
                return false;
            }
            return true;
        }
    }

    public static class Patch_VehicleTurret_ScaleUIRect_Fix
    {
        public static void Apply(Harmony harm)
        {
            try
            {
                MethodInfo targetMethod = AccessTools.Method(typeof(VehicleTurret), "DrawTurretOverlay");
                if (targetMethod != null)
                {
                    harm.Patch(targetMethod, prefix: new HarmonyMethod(typeof(Patch_VehicleTurret_ScaleUIRect_Fix), "Prefix"));
                }
            }
            catch { }
        }

        public static bool Prefix(VehicleTurret __instance)
        {
            if (__instance == null || __instance.vehicle == null || !__instance.vehicle.Spawned) return false;
            return true;
        }
    }

    public static class Patch_GridsUtility_Fogged_Fix
    {
        public static void Apply(Harmony harm)
        {
            try
            {
                MethodInfo targetMethod = AccessTools.Method(typeof(GridsUtility), "Fogged", new Type[] { typeof(IntVec3), typeof(Map) });
                if (targetMethod != null)
                {
                    harm.Patch(targetMethod, prefix: new HarmonyMethod(typeof(Patch_GridsUtility_Fogged_Fix), "Prefix"));
                }
            }
            catch { }
        }

        public static bool Prefix(IntVec3 c, Map map, ref bool __result)
        {
            if (map == null || !c.IsValid || !c.InBounds(map))
            {
                __result = false;
                return false;
            }
            return true;
        }
    }

    public static class Patch_SRAI_Sapper_Fix
    {
        public static void Apply(Harmony harm)
        {
            try
            {
                Type type = AccessTools.TypeByName("SmarterRaidAI.SapperUtility");
                if (type == null) return;
                MethodInfo targetMethod = AccessTools.Method(type, "IsCellSafe");
                if (targetMethod != null)
                {
                    harm.Patch(targetMethod, prefix: new HarmonyMethod(typeof(Patch_SRAI_Sapper_Fix), "Prefix"));
                }
            }
            catch { }
        }

        public static bool Prefix(IntVec3 cell, Map map, ref bool __result)
        {
            if (map == null || !cell.IsValid || !cell.InBounds(map))
            {
                __result = false;
                return false;
            }
            return true;
        }
    }

    public static class Patch_Log_Error_Filter
    {
        public static void Apply(Harmony harm)
        {
            try
            {
                MethodInfo targetMethod = AccessTools.Method(typeof(Log), "Error", new Type[] { typeof(string) });
                if (targetMethod != null)
                {
                    harm.Patch(targetMethod, prefix: new HarmonyMethod(typeof(Patch_Log_Error_Filter), "Prefix"));
                }
            }
            catch { }
        }

        public static bool Prefix(string text)
        {
            if (text != null && text.Contains("Failed to find any thing to spawn on cell"))
            {
                return false;
            }
            return true;
        }
    }

    public static class Patch_ProjectileCE_SpawnsThing_Impact
    {
        public static void Apply(Harmony harm)
        {
            try
            {
                Type type = AccessTools.TypeByName("CombatExtended.ProjectileCE_SpawnsThing");
                if (type == null) return;
                MethodInfo targetMethod = AccessTools.Method(type, "Impact");
                if (targetMethod != null)
                {
                    harm.Patch(targetMethod, prefix: new HarmonyMethod(typeof(Patch_ProjectileCE_SpawnsThing_Impact), "Prefix"));
                }
            }
            catch { }
        }

        public static bool Prefix(Thing hitThing, Thing ___launcher)
        {
            if (hitThing != null && ___launcher != null && hitThing == ___launcher)
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

    public static class Patch_RecipeDef_ProducedThingDef
    {
        public static void Apply(Harmony harm)
        {
            try
            {
                MethodInfo original = AccessTools.PropertyGetter(typeof(RecipeDef), "ProducedThingDef");
                MethodInfo postfix = AccessTools.Method(typeof(Patch_RecipeDef_ProducedThingDef), "Postfix");
                if (original != null && postfix != null)
                {
                    harm.Patch(original, postfix: new HarmonyMethod(postfix));
                }
            }
            catch (Exception ex)
            {
                Log.Warning("[RimThunder] RecipeDef.ProducedThingDef patch skipped: " + ex.Message);
            }
        }

        public static void Postfix(RecipeDef __instance, ref ThingDef __result)
        {
            if (__result == null && __instance != null && __instance.modExtensions != null && __instance.modExtensions.Count > 0)
            {
                __result = Patch_Dialog_BillConfig_DoWindowContents.GetVehicleDefForRecipe(__instance);
            }
        }
    }
}
