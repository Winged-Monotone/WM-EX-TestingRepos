
using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Text;

using XRL.World.Parts;
using XRL.World;
using XRL.World.Effects;
using XRL.World.AI.GoalHandlers;
using XRL.World.Parts.Mutation;
using XRL.World.Capabilities;

using XRL.Core;
using XRL.Rules;
using XRL.Messages;
using XRL.UI;

using UnityEngine;
using AiUnity.NLog.Core.Targets;
using HarmonyLib;
using ConsoleLib.Console;
using XRL.World.Anatomy;

namespace XRL.World.Parts.Mutation
{
    [Serializable]
    public class mThorny : BaseMutation
    {
        public mThorny()
        {
            this.DisplayName = "Thorny";
        }

        public override bool CanLevel()
        {
            return false;
        }

        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade)
            || ID == DefendMeleeHitEvent.ID
            || ID == AfterMentalAttackEvent.ID;
        }

        public override string GetDescription()
        {
            return "Your body is covered in sharp barbs, they deal damage to enemies and have a chance to inflict bleed.\n\n";
        }

        public override bool HandleEvent(AfterMentalAttackEvent E)
        {
            var eDefender = E.Defender;
            var eAttacker = E.Attacker;

            if (ParentObject.HasPart("MentalMirror"))
            {
                var MutGrab = ParentObject.GetPart<MentalMirror>();
                eAttacker.TakeDamage(1 + MutGrab.Level + Level, eDefender, "thought stuff is lacerated by " + ParentObject.its + "mental thorns.");
            }

            return true;
        }

        public override bool HandleEvent(DefendMeleeHitEvent E)
        {
            var eDefender = E.Defender;
            var eAttacker = E.Attacker;
            var eWeapon = E.Weapon;

            if (eWeapon.HasPart("MeleeWeapon") && eAttacker == ParentObject && eDefender.IsCombatObject())
            {
                E.Attacker.TakeDamage(2 + Level, eDefender, "is lacerated by " + ParentObject.its + "thorns.");
                if (!eAttacker.MakeSave("Toughness", (8 + Level)))
                {
                    eAttacker.ApplyEffect(new Bleeding("1", 8 + Level, eDefender, false));
                }
            }
            if (ParentObject.HasPart("Stinger"))
            {
                var MutGrab = ParentObject.GetPart<Stinger>();
                eAttacker.ApplyEffect(new Poisoned(1 + Stat.Random(1, 10), "1", MutGrab.Level, ParentObject));
            }
            if (ParentObject.HasPart("GelatinousFormPoison"))
            {
                var MutGrab = ParentObject.GetPart<GelatinousFormPoison>();
                eAttacker.ApplyEffect(new Poisoned(1 + Stat.Random(1, 10), "1", MutGrab.Level, ParentObject));
            }

            return true;
        }

        public override void Register(GameObject Object)
        {
            Object.RegisterPartEvent(this, "EndTurn");

            base.Register(Object);
        }
    }


}