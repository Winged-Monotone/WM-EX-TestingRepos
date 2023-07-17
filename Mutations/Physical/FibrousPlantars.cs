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
    public class FibrousPantars : BaseMutation
    {
        public int NOSAVE = 9999;
        public int NODURATION = 9999;
        public int EarthNutrition = 50;
        public bool IsCurrentlyRooted;
        public bool OsmosisAct = false;
        public Guid RootInPlaceID;

        public override bool Mutate(GameObject GO, int Level)
        {
            ActivatedAbilities RootInPlaceID = ParentObject.GetPart("ActivatedAbilities") as ActivatedAbilities;

            this.RootInPlaceID = RootInPlaceID.AddAbility(Name: "Root", Command: "CommandRootinPlaceWM", Class: "Physical Mutation", Description: "Root in place, extending your plantars deep into the earth, you cannot move but are immune to force effects and absorb nutrients from the earth overtime.", Icon: "(O)", Cooldown: -1);
            return base.Mutate(GO, Level);
        }

        public override bool WantEvent(int ID, int cascade)
        {
            return ID == ObjectLeavingCellEvent.ID
            || base.WantEvent(ID, cascade);
        }
        public override void Register(GameObject Object)
        {
            Object.RegisterPartEvent(this, "EndTurn");
            Object.RegisterPartEvent(this, "CommandRootinPlaceWM");
            base.Register(Object);
        }
        public override bool HandleEvent(ObjectLeavingCellEvent E)
        {
            var eActor = E.Actor;

            if (eActor == ParentObject && IsCurrentlyRooted == true)
            {
                if (E.Dragging == eActor || E.Forced == false)
                {
                    return false;
                }
            }

            return base.HandleEvent(E);
        }
        public void RootInPlace()
        {
            ParentObject.AddPart<Regeneration>();
            var PParams = ParentObject.GetEffect<Healing>();

            ParentObject.ApplyEffect(new Immobilized(NOSAVE));
            var EParams = ParentObject.GetEffect<Immobilized>();


            EParams._DisplayName = "Rooted";
            EParams.Text = "You are rooted in place, you cannot move but are more resistant to force effects and absorb nutrients from the earth overtime.";

            PParams.DisplayName = "Osmosis";
            PParams._Duration = NODURATION;

            IsCurrentlyRooted = true;

            OsmosisAct = true;
        }

        public void unRootInPlace()
        {
            ParentObject.RemoveEffect(new Immobilized());
            ParentObject.RemoveEffect(new Healing());

            IsCurrentlyRooted = false;
            OsmosisAct = false;
        }
        public void RootedAnim()
        {
            TextConsole textConsole = Look._TextConsole;
            ScreenBuffer scrapBuffer = TextConsole.ScrapBuffer;
            XRLCore.Core.RenderMapToBuffer(scrapBuffer);

            var parAdjacentCells = ParentObject.CurrentCell.GetAdjacentCells();
            foreach (var C in parAdjacentCells)
            {
                foreach (Cell item4 in parAdjacentCells)
                {
                    scrapBuffer.Goto(item4.X, item4.Y);
                    scrapBuffer.Write("&W" + (char)Stat.Random(191, 198));
                }
            }
        }
        public override bool FireEvent(Event E)
        {
            if (E.ID == "EndTurn")
            {
                if (OsmosisAct == true)
                {
                    var ParStomach = ParentObject.GetPart<Stomach>();
                    ParStomach.HungerLevel += EarthNutrition;

                    if (NODURATION <= 1)
                    {
                        NODURATION = 9999;
                    }
                }
            }
            else if (E.ID == "CommandRootinPlaceWM")
            {
                if (IsCurrentlyRooted == false)
                {
                    RootInPlace();
                }
                else if (IsCurrentlyRooted == true)
                {
                    unRootInPlace();
                }
            }
            return base.FireEvent(E);
        }



    }
}