
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
    public class mExplosiveBurs : BaseMutation
    {
        public Guid BursPlaceID;
        public mExplosiveBurs()
        {
            this.DisplayName = "Explosive Burs";
        }

        public override bool CanLevel()
        {
            return false;
        }
        public override string GetDescription()
        {
            return "Release burs that tumble and explode  .";
        }

        public override bool Mutate(GameObject GO, int Level)
        {
            ActivatedAbilities BursPlaceID = ParentObject.GetPart("ActivatedAbilities") as ActivatedAbilities;
            ParentObject.AddPart<FeralLah>();

            this.BursPlaceID = BursPlaceID.AddAbility(Name: "Release Burs", Command: "CommandReleaseBursWM", Class: "Physical Mutation", Description: "Detach burrs from burrows in your flesh with explosive properties.", Icon: "(O)", Cooldown: 100);
            return base.Mutate(GO, Level);
        }

        public override void Register(GameObject Object)
        {
            Object.RegisterPartEvent(this, "EndTurn");
            Object.RegisterPartEvent(this, "CommandReleaseBursWM");
            base.Register(Object);
        }

        public override bool FireEvent(Event E)
        {
            if (E.ID == "CommandReleaseBursWM")
            {
                var LahGrab = ParentObject.GetPart<FeralLah>();
                Cell rElement = ParentObject.CurrentCell.GetEmptyAdjacentCells().GetRandomElement();
                if (IsMyActivatedAbilityUsable(BursPlaceID))
                {
                    CooldownMyActivatedAbility(BursPlaceID, 100);
                    GameObject pObject = ParentObject.PartyLeader ?? ParentObject;
                    GameObject gObject1 = GameObject.create("Feral Lah Pod");
                    if (pObject.IsPlayer())
                    {
                        gObject1.SetPartyLeader(ParentObject, takeOnAttitudesOfLeader: true, trifling: true);
                    }
                    else
                    {
                        gObject1.SetPartyLeader(pObject, takeOnAttitudesOfLeader: true, trifling: true, copyTargetWithAttitudes: true);
                    }
                    rElement.AddObject(gObject1);
                    gObject1.MakeActive();
                    ParentObject.Splatter("&g.");
                    gObject1.ParticleBlip("&go");
                    ParentObject.UseEnergy(200);

                    var gCells = ParentObject.CurrentCell.GetAdjacentCells(1);

                    foreach (var C in gCells)
                    {
                        if (C.HasObject("Feral Lah Pod"))
                        {
                            var nObj = C.GetFirstObjectWithPart("FeralLahPod");
                            var aObjMutGrab = nObj.GetPart<FeralLahPod>();

                            aObjMutGrab.damage += (Level * 0.2);
                        }
                    }
                }
            }
            return base.FireEvent(E);
        }
    }
}