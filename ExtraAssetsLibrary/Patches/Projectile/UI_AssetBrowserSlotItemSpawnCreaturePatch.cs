using System;
using System.Collections.Generic;
using Bounce.Unmanaged;
using HarmonyLib;
using UnityEngine;

namespace ExtraAssetsLibrary.Patches.Projectile
{
    internal static class ParticleStack
    {
        internal static Dictionary<(Creature, Creature), Func<NGuid,GameObject>> Replacements = new Dictionary<(Creature, Creature), Func<NGuid, GameObject>>();

        private static Creature origin;
        private static Creature target;

        internal static bool Check(NGuid arg1, NGuid arg2)
        {
            CreaturePresenter.TryGetAsset(new CreatureGuid(arg1), out var c1);
            CreaturePresenter.TryGetAsset(new CreatureGuid(arg2), out var c2);
            return true;
        }

        internal static void CustomParticle(MapMenuItem item, object o)
        {
            var tuple = ((Func<NGuid, GameObject>,Creature)) o;
            Replacements.Add((origin,target), tuple.Item1);

            CreatureBoardAsset creatureAsset;
            if (!CreaturePresenter.TryGetAsset(LocalClient.SelectedCreatureId, out creatureAsset))
                return;
            creatureAsset.Creature.StartTargetEmote(origin, item.name);
        }
    }

    [HarmonyPatch(typeof(VFXMissile), "OnPlayFromOriginToTarget",typeof(Transform), typeof(Transform))]
    public class VFXMissilePatch
    {
        private static void Postfix(Transform origin, Transform target, ref Transform ___visual, ref Renderer[] ___renderers)
        {
            foreach (var key in ParticleStack.Replacements.Keys)
            {
                Creature.Targets m = null;
                Creature.Targets t = null;
                foreach (var me in Enum.GetValues(typeof(Creature.HookTransform)))
                {
                    var temp = new Creature.Targets
                    {
                        target = Creature.Targets.Who.Me,
                    };
                    if (key.Item1.GetTransformFromTarget(temp) == origin)
                    {
                        m = temp;
                    }
                }

                foreach (var them in Enum.GetValues(typeof(Creature.HookTransform)))
                {
                    var temp = new Creature.Targets
                    {
                        target = Creature.Targets.Who.Me,
                    };
                    if (key.Item1.GetTransformFromTarget(temp) == origin)
                    {
                        t = temp;
                    }
                }

                if (m != null && t != null)
                {

                    break;
                }
            }
        }
    }
}