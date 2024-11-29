using System;
using XRL.World.Effects;
using XRL.World;
using XRL.World.Parts.Mutation;
using System.Linq;
using XRL.Core;
using XRL.Rules;
using XRL.World.AI;
using XRL.World.AI.GoalHandlers;
using System.Collections.Generic;
using Qud.API;
using XRL.Language;
using XRL.UI;
using XRL.World.Capabilities;
using ConsoleLib.Console;
using XRL.World.ZoneBuilders;
using XRL.World.Parts.Skill;

namespace XRL.World.Parts.Mutation
{
    [Serializable]
    public class Barkflesh : BaseMutation
    {
        public bool PlayerRepModified = false;
        public Barkflesh()
        {
            this.DisplayName = "Barkflesh";
        }

        public override bool CanLevel()
        {
            return false;
        }



        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade)
            || ID == BeforeApplyDamageEvent.ID
            || ID == EffectAppliedEvent.ID;
        }

        public override bool HandleEvent(EffectAppliedEvent E)
        {
            if (E.Actor == ParentObject && E.Actor.HasEffect<Burning>())
            {
                var wAflamedEffect = E.Actor.GetEffect<Burning>();
                wAflamedEffect._Duration *= 2;
            }

            return true;
        }

        public override bool HandleEvent(BeforeApplyDamageEvent E)
        {
            if (ParentObject.HasPart("Barkflesh"))
            {
                if (E.Damage.HasAttribute("Heat") || E.Damage.HasAttribute("Heat Fire"))
                {
                    E.Damage._Amount *= 2;
                }
            }

            return true;
        }

        public override bool Mutate(GameObject GO, int Level)
        {
            if (PlayerRepModified == false && ParentObject.IsPlayer())
            {
                XRL.Core.XRLCore.Core.Game.PlayerReputation.modify("Plants", 200, true);
                PlayerRepModified = true;
            }
            var HPStatParent = ParentObject.GetStat("Hitpoints");
            var AVStatParent = ParentObject.GetStat("AV");
            var MSPEEDStatParent = ParentObject.GetStat("MoveSpeed");

            HPStatParent._Value *= 2;
            AVStatParent._Value += 4;
            MSPEEDStatParent._Value += 20;
            return base.Mutate(GO, Level);
        }

        public override string GetDescription()
        {
            return "You bear the flesh of the tree. Hard and resilient; it is also slow and cumbersome and flames are your worst enemy. It synergizes well with other plant-like mutations.\n"
            + "\n +200 relation with plants.";
        }


    }
}