using System;
using XRL.UI;
using XRL.Rules;
using XRL.World.Effects;
using System.Collections.Generic;
using ConsoleLib.Console;
using XRL.Core;
using XRL.Liquids;
using XRL.World.Capabilities;
using XRL.World.Anatomy;


namespace XRL.World.Parts.Mutation
{
    [Serializable]
    public class GelatinousFormPoison : BaseDefaultEquipmentMutation
    {
        public int nNextLimb = 100;
        public string ManagerID => ParentObject.id + "::GelatinousFormPoison";
        public string PoisonIchorObj = "poisonichorpool";
        public int Density = 1;
        public bool PlayerRepModified = false;

        public GelatinousFormPoison()
        {
            DisplayName = "Gelatinous Form {{poisonous|(Poison)}}";
        }

        public override bool CanLevel()
        {
            return true;
        }

        public override bool ChangeLevel(int NewLevel)
        {
            Density = GetDensity();
            return base.ChangeLevel(NewLevel);
        }

        public int GetDensity()
        {
            int NewDensity = Density * (Level / 2);
            return (NewDensity);
        }

        public override bool AffectsBodyParts()
        {
            return true;
        }

        public override bool AllowStaticRegistration()
        {
            return true;
        }

        public override void Register(GameObject go)
        {
            go.RegisterPartEvent((IPart)this, "AIGetPassiveMutationList");
            go.RegisterPartEvent((IPart)this, "BeforeApplyDamage");
            go.RegisterPartEvent((IPart)this, "EndTurn");
            go.RegisterPartEvent((IPart)this, "Regenerating");
            go.RegisterPartEvent((IPart)this, "BeginTakeAction");
            go.RegisterPartEvent((IPart)this, "BeforeDecapitate");
        }

        public override string GetDescription()
        {
            return "You lack a muscuskeletal system, your genome chose an amorphous eukaryote's physique. Yours is especially {{poisonous|poisonous.}}\n";
        }

        public void AddPoisonGlobul()
        {
            Body SourceBody = ParentObject.GetPart("Body") as Body;
            if (SourceBody != null)
            {
                BodyPart ReadyBody = SourceBody.GetBody();
                var AttatchPseudotemplate = ReadyBody.AddPartAt(
                    Base: "Oral Arm",
                    Laterality: Laterality.UPPER,
                    Manager: ManagerID,
                    OrInsertBefore: new string[1]
                {
                "Head",
                });
                if (Stat.Random(1, 100) <= 50)
                {
                    var mBodyPart = AttatchPseudotemplate.AddPart(
                        Base: "Pseudopod",
                        Laterality: Laterality.UPPER,
                        DefaultBehavior: "Poison_Humor_Pseudopod",
                        Manager: ManagerID);
                    mBodyPart.DefaultBehaviorBlueprint = "Poison_Humor_Pseudopod";
                }
                else
                {
                    var mBodyPart = AttatchPseudotemplate.AddPart(
                       Base: "Tentacle",
                       Laterality: Laterality.UPPER,
                       DefaultBehavior: "Poison_Humor_Tentacle",
                       Manager: ManagerID);
                    mBodyPart.DefaultBehaviorBlueprint = "Poison_Humor_Tentacle";
                }
            }
            SourceBody.UpdateBodyParts();
        }

        public void RemoveGlobul()
        {
            Body SourceBody = ParentObject.GetPart("Body") as Body;
            if (SourceBody != null)
            {
                ParentObject.Body.RemovePartsByManager(ManagerID, EvenIfDismembered: true);
            }
            SourceBody.UpdateBodyParts();
        }

        public override bool Mutate(GameObject GO, int Level)
        {
            if (ParentObject.HasPropertyOrTag("Role"))
                if (ParentObject.HasPropertyOrTag("Hero") && ParentObject.HasPropertyOrTag("Plant"))
                {
                    ParentObject.RemovePart<GelatinousFormPoison>();
                    LogError("REMOVED GEL FROM HERO.");
                    this.Unmutate(ParentObject);
                }
            if (PlayerRepModified == false && ParentObject.IsPlayer())
            {
                XRL.Core.XRLCore.Core.Game.PlayerReputation.modify("Oozes", 400, true);
                PlayerRepModified = true;
            }
            ParentObject.SetStringProperty("BleedLiquid", "poisonichor-1000");
            ParentObject.AddPart<PoisonImmunity>(true);
            AddPoisonGlobul();
            if (!ParentObject.HasIntProperty("Slimewalking"))
            {
                ParentObject.SetIntProperty("Slimewalking", 1);
            }
            return base.Mutate(GO, Level);
        }

        public override bool Unmutate(GameObject GO)
        {
            ParentObject.RemovePart<PoisonImmunity>();
            ParentObject.SetStringProperty("BleedLiquid", "BloodSplash");
            RemoveGlobul();
            if (!ParentObject.HasIntProperty("Slimewalking"))
            {
                ParentObject.SetIntProperty("Slimewalking", 0);
            }
            return base.Unmutate(GO);
        }


        public override string GetLevelText(int Level)
        {
            string text = string.Empty;


            if (Level == base.Level)
            {
                text += "You gain a {{cyan|25%}} damage resistance bonus to melee weapons.\n";
                text += "Immunity from poison.\n";
                text += "Take more damage from projectiles and explosives.\n";
                text += "When dealt damage, there's a random chance you release poison ichor in tiles around you.\n";
                text += "You quickly regenerate lost limbs.\n\n";
                text += "{{cyan|+200 reputation with oozes}}";
            }
            else
            {
                text += "Increased chance of poison release by {{cyan|5%}}, and the amount of poison release upon being struck by an enemy.\n";
                text += "You regenerate lost limbs more quickly.\n";
            }
            return text;
        }

        public override bool FireEvent(Event E)
        {
            if (E.ID == "BeforeApplyDamage")
            {
                Damage parameter = E.GetParameter("Damage") as Damage;
                if (parameter.HasAttribute("Slashing"))
                    parameter.Amount -= (int)((double)parameter.Amount * (0.25 * (int)new Decimal(2)));
                else if (parameter.HasAttribute("Melee"))
                    parameter.Amount -= (int)((double)parameter.Amount * (0.25 * (int)new Decimal(2)));
                else if (parameter.HasAttribute("Ranged"))
                    parameter.Amount += (int)((double)parameter.Amount * (0.25 * (int)new Decimal(2)));

                if (ParentObject.CurrentCell != null && parameter.Amount != 0)
                {
                    List<Cell> adjacentCells1 = ParentObject.CurrentCell.GetAdjacentCells(true);
                    adjacentCells1.Add(ParentObject.CurrentCell);
                    foreach (Cell cell in adjacentCells1)
                    {
                        if (!cell.IsOccluding() && Stat.Random(1, 100) <= 10 + (5 * Level / 2))
                        {
                            GameObject IchorContainer = GameObject.create(this.PoisonIchorObj);
                            var IchorProperties = IchorContainer.GetPart<LiquidVolume>();
                            IchorProperties.Volume *= Level;
                            cell.AddObject(IchorContainer);
                        }
                    }
                }
            }
            if (E.ID == "Regenerating")
            {
                int intParameter = E.GetIntParameter("Amount");
                intParameter += (int)Math.Ceiling((float)intParameter * GetRegenerationBonus(base.Level));
                E.SetParameter("Amount", intParameter);
                return true;
            }
            else if (E.ID == "BeginTakeAction")
            {
                --nNextLimb;
                var parBody = ParentObject.Body.AnyDismemberedMortalParts();
                var misBody = ParentObject.Body.DismemberedParts;
                if (nNextLimb <= 0 && (parBody == true || misBody != null))
                {
                    ParentObject.FireEvent(Event.New("Regenera", "Level", base.Level, "Involuntary", 1));
                    GenericNotifyEvent.Send(ParentObject, "RegenerateLimb");
                    nNextLimb = 100;
                }
                else
                {
                    nNextLimb = 100;
                }
                if (ParentObject.HasEffect<Poisoned>())
                {
                    ParentObject.RemoveEffect("Poisoned");
                }
            }
            if (E.ID == "BeforeDecapitate")
            {
                if (ParentObject.IsPlayer())
                {
                    IComponent<GameObject>.AddPlayerMessage("{{G|You were decapitated, but a new head regrew immediately!}}");
                }
                return false;
            }
            return base.FireEvent(E);
        }
        public float GetRegenerationBonus(int Level)
        {
            return 0.05f + 0.5f;
        }
    }
}