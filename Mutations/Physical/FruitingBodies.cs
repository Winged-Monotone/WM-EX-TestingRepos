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
using AiUnity.NLog.Core.LayoutRenderers;
using System.Globalization;



namespace XRL.World.Parts.Mutation
{
    [Serializable]
    public class FruitingBodies : BaseMutation
    {
        // Tier 1 fruit
        public int StarappleGrowthPeriod = 16800;
        public int StarAppleBaseYield = 10;

        public int SpineFruitGrowthPeriod = 8000;
        public int SpineFruitBaseYield = 5;

        public int RipeCucumberGrowthPeriod = 18000;
        public int RipeCucumberBaseYield = 10;

        public int PlumpMushroomGrowthPeriod = 7200;
        public int PlumpMushroomBaseYield = 20;

        // Tier 2 fruit

        public int BananaGrowthPeriod = 20000;
        public int BananaBaseYield = 15;

        public int YuckwheatStemGrowthPeriod = 7000;
        public int YuckwheatStemBaseYield = 3;

        public int YondercaneGrowthPeriod = 20000;
        public int YondercaneBaseYield = 5;

        public int HoarshroomGrowthPeriod = 14000;
        public int HoarshroomBaseYield = 10;

        // Tier 3 fruit

        public int LahPetalsGrowthPeriod = 30000;
        public int LahPetalsBaseYield = 7;

        public int VantaPetalsGrowthPeriod = 26000;
        public int VantaPetalsBaseYield = 7;

        public int GodshroomCapGrowthPeriod = 12000;
        public int GodshroomCapBaseYield = 2;

        // Tier 4 fruit

        public int UrberryGrowthPeriod = 60000;
        public int UrberryBaseYield = 1;

        public int MirrorShardGrowthPeriod = 50000;
        public int MirrorShardCapBaseYield = 7;

        // Tier 5 fruit

        public int ArspliceSeedGrowthPeriod = 3000000;
        public int ArspliceSeedCapBaseYield = 1;

        public string vFruitChoice = null;

        public int GrowthPeriod;
        public int uGrowthPeriod;
        public int BaseYield;
        public int BonusYield;

        public int NutritionalRatio;
        public bool PlayerRepModified = false;

        public Guid uAccessFruitMenuID;

        public static readonly List<string> MainOptions = new List<string>()
        {
            "Choose Fruit to Produce",
            "Harvest Fruit",
            "Cancel"
        };
        public static readonly List<string> Tier1FruitOptions = new List<string>()
        {
            "{{red|Starapple}}",
            "{{magenta|Spine Fruit}}",
            "{{green|Ripe Cucumber}}",
            "Plump Mushroom"
        };
        public static readonly List<string> Tier2FruitOptions = new List<string>()
        {
            "{{red|Starapple}}",
            "{{magenta|Spine Fruit}}",
            "{{green|Ripe Cucumber}}",
            "Plump Mushroom",
            "{{yellow|Banana}}",
            "{{brown|Yuckwheat Stem}}",
            "{{blue|Yondercane}}",
            "{{lightblue|Hoarshroom}}"
        };
        public static readonly List<string> Tier3FruitOptions = new List<string>()
        {
            "{{red|Starapple}}",
            "{{magenta|Spine Fruit}}",
            "{{green|Ripe Cucumber}}",
            "Plump Mushroom",
            "{{yellow|Banana}}",
            "{{brown|Yuckwheat Stem}}",
            "{{W|Yondercane}}",
            "{{luminous|Hoarshroom}}",
            "{{lah|lah petals}}",
            "{{K|vanta petals}}",
            "{{o|Eater's flesh}}",
        };
        public static readonly List<string> Tier4FruitOptions = new List<string>()
        {
            "{{red|Starapple}}",
            "{{magenta|Spine Fruit}}",
            "{{green|Ripe Cucumber}}",
            "Plump Mushroom",
            "{{yellow|Banana}}",
            "{{brown|Yuckwheat Stem}}",
            "{{blue|Yondercane}}",
            "{{luminous|Hoarshroom}}",
            "{{lah|lah petals}}",
            "{{K|vanta petals}}",
            "{{o|Eater's flesh}}",
            "{{purple|Urberry}}",
            "{{grey|Mirror Shard}}"
        };
        public static readonly List<string> Tier5FruitOptions = new List<string>()
        {
            "{{red|Starapple}}",
            "{{magenta|Spine Fruit}}",
            "{{green|Ripe Cucumber}}",
            "Plump Mushroom",
            "{{yellow|Banana}}",
            "{{brown|Yuckwheat Stem}}",
            "{{blue|Yondercane}}",
            "{{luminous|Hoarshroom}}",
            "{{lah|lah petals}}",
            "{{K|vanta petals}}",
            "{{o|Eater's flesh}}",
            "{{purple|Urberry}}",
            "{{grey|Mirror Shard}}",
            "{{C-C-c-m-m-c-C-C sequence|arsplice}} seed"
        };

        public FruitingBodies()
        {
            DisplayName = "Fruiting Bodies";
            var MaxLevel = GetMaxLevel();
            MaxLevel = 15;
        }
        public override bool CanLevel()
        {
            return true;
        }
        public override string GetDescription()
        {
            return "Branches and vines extend from your form, every here and then, they fruit various flora of your choosing.";
        }
        public override string GetLevelText(int Level)
        {
            string text = string.Empty;

            if (Level == base.Level)
            {
                text += "You produce fruit and flora of various forms. Overtime, you can harvest them for your own use.\n";
                text += "Keeping yourself wellfed and hydrated will yield larger batches while being hungry and being dehydrated often will yield lesser.\n";
                text += "Raising the mutation level will unlock more fruit to grow. Remember, selecting a new fruit will reset the growth rate of your previous batch.\n";
                text += "{{cyan|+400 reputation with plants.}}\n";
            }
            else
            {
                text += "You produce fruit and flora of various forms overtime and can harvest them for your own use.\n";
                text += "Remember, selecting a new fruit will reset your current growth process.\n";
            }
            return text;
        }

        public static string GetChoice(List<string> valuesToShow)
        {
            int result = Popup.ShowOptionList(
                Title: "Select an Option",
                Options: valuesToShow.ToArray(),
                AllowEscape: true);
            if (result < 0 || valuesToShow[result].ToUpper() == "CANCEL")
            {
                // The user escaped out of the menu, or chose "Cancel"
                return string.Empty;
            }
            else
            {
                // The user selected a value - return the select value as a string
                return valuesToShow[result];
            }
        }

        public void FruitingProcessMenu()
        {
            string mainChoice = GetChoice(MainOptions);

            if (!string.IsNullOrEmpty(mainChoice))
            {
                if (mainChoice == "Choose Fruit to Produce")
                {
                    if (this.Level <= 5)
                    {
                        string FruitChoice = GetChoice(Tier1FruitOptions);
                        if (!string.IsNullOrEmpty(FruitChoice))
                        {
                            vFruitChoice = FruitChoice;
                            SetGrowthFormulae(FruitChoice);
                        }
                    }
                }
                else if (mainChoice == "Choose Fruit to Produce")
                {
                    if (this.Level <= 7)
                    {
                        string FruitChoice = GetChoice(Tier2FruitOptions);
                        if (!string.IsNullOrEmpty(FruitChoice))
                        {
                            vFruitChoice = FruitChoice;
                            SetGrowthFormulae(FruitChoice);
                        }
                    }
                }
                else if (mainChoice == "Choose Fruit to Produce")
                {
                    if (this.Level <= 8)
                    {
                        string FruitChoice = GetChoice(Tier3FruitOptions);
                        if (!string.IsNullOrEmpty(FruitChoice))
                        {
                            vFruitChoice = FruitChoice;
                            SetGrowthFormulae(FruitChoice);
                        }
                    }
                }
                else if (mainChoice == "Choose Fruit to Produce")
                {
                    if (this.Level <= 10)
                    {
                        string FruitChoice = GetChoice(Tier4FruitOptions);
                        if (!string.IsNullOrEmpty(FruitChoice))
                        {
                            vFruitChoice = FruitChoice;
                            SetGrowthFormulae(FruitChoice);
                        }
                    }
                }
                else if (mainChoice == "Choose Fruit to Produce")
                {
                    if (this.Level <= 15)
                    {
                        string FruitChoice = GetChoice(Tier4FruitOptions);
                        if (!string.IsNullOrEmpty(FruitChoice))
                        {
                            vFruitChoice = FruitChoice;
                            SetGrowthFormulae(FruitChoice);
                        }
                    }
                }
                else if (mainChoice == "Harvest Fruit")
                {
                    if (GrowthPeriod >= 0 && vFruitChoice != null)
                    {
                        Popup.Show("Your fruit isn't ready to harvest.");
                    }
                    else if (GrowthPeriod <= 0 && vFruitChoice != null)
                    {
                        ProduceFruit(vFruitChoice);
                    }
                    else if (vFruitChoice != null)
                    {
                        Popup.Show("You must determine a fruit to bear.");
                    }
                }
            }
        }
        public void ProduceFruit(string FruitChoice)
        {
            BonusBatches(FruitChoice);
            if (NutritionalRatio <= -90)
            {
                var mHarvestYield = ParentObject.CurrentCell.GetFirstEmptyAdjacentCell();
                mHarvestYield.AddObject(FruitChoice, BaseYield / 2);
            }
            else if (NutritionalRatio >= -90 && NutritionalRatio <= 80)
            {
                var mHarvestYield = ParentObject.CurrentCell.GetFirstEmptyAdjacentCell();
                mHarvestYield.AddObject(FruitChoice, BaseYield);
            }
            else if (NutritionalRatio >= 80)
            {
                var mHarvestYield = ParentObject.CurrentCell.GetFirstEmptyAdjacentCell();
                mHarvestYield.AddObject(FruitChoice, BaseYield + BonusYield);
            }
        }
        public void SetGrowthFormulae(string FruitChoice)
        {
            if (FruitChoice == "{{red|Starapple}}")
            {
                BaseYield = StarAppleBaseYield;
                GrowthPeriod = StarappleGrowthPeriod;
                uGrowthPeriod = GrowthPeriod;
            }
            else if (FruitChoice == "{{magenta|Spine Fruit}}")
            {
                BaseYield = SpineFruitBaseYield;
                GrowthPeriod = SpineFruitGrowthPeriod;
                uGrowthPeriod = GrowthPeriod;
            }
            else if (FruitChoice == "{{green|Ripe Cucumber}}")
            {
                BaseYield = RipeCucumberBaseYield;
                GrowthPeriod = RipeCucumberGrowthPeriod;
                uGrowthPeriod = GrowthPeriod;
            }
            else if (FruitChoice == "Plump Mushroom")
            {
                BaseYield = PlumpMushroomBaseYield;
                GrowthPeriod = PlumpMushroomGrowthPeriod;
                uGrowthPeriod = GrowthPeriod;
            }
            else if (FruitChoice == "{{yellow|Banana}}")
            {
                BaseYield = BananaBaseYield;
                GrowthPeriod = BananaGrowthPeriod;
                uGrowthPeriod = GrowthPeriod;
            }
            else if (FruitChoice == "{{brown|Yuckwheat Stem}}")
            {
                BaseYield = YuckwheatStemBaseYield;
                GrowthPeriod = YuckwheatStemGrowthPeriod;
                uGrowthPeriod = GrowthPeriod;
            }
            else if (FruitChoice == "{{blue|Yondercane}}")
            {
                BaseYield = YondercaneBaseYield;
                GrowthPeriod = YondercaneGrowthPeriod;
                uGrowthPeriod = GrowthPeriod;
            }
            else if (FruitChoice == "{{luminous|Hoarshroom}}")
            {
                BaseYield = HoarshroomBaseYield;
                GrowthPeriod = HoarshroomGrowthPeriod;
                uGrowthPeriod = GrowthPeriod;
            }
            else if (FruitChoice == "{{lah|lah petals}}")
            {
                BaseYield = LahPetalsBaseYield;
                GrowthPeriod = LahPetalsGrowthPeriod;
                uGrowthPeriod = GrowthPeriod;
            }
            else if (FruitChoice == "{{o|Eater's flesh}}")
            {
                BaseYield = GodshroomCapBaseYield;
                GrowthPeriod = GodshroomCapGrowthPeriod;
                uGrowthPeriod = GrowthPeriod;
            }
            else if (FruitChoice == "{{purple|Urberry}}")
            {
                BaseYield = UrberryBaseYield;
                GrowthPeriod = UrberryGrowthPeriod;
                uGrowthPeriod = GrowthPeriod;
            }
            else if (FruitChoice == "{{grey|Mirror Shard}}")
            {
                BaseYield = MirrorShardCapBaseYield;
                GrowthPeriod = MirrorShardGrowthPeriod;
                uGrowthPeriod = GrowthPeriod;
            }
            else if (FruitChoice == "{{C-C-c-m-m-c-C-C sequence|arsplice}} seed")
            {
                BaseYield = ArspliceSeedCapBaseYield;
                GrowthPeriod = ArspliceSeedGrowthPeriod;
                uGrowthPeriod = GrowthPeriod;
            }
        }
        public void BonusBatches(string FruitChoice)
        {
            if (FruitChoice == "{{red|Starapple}}")
            {
                BonusYield = Stat.Random(1, 10);
            }
            else if (FruitChoice == "{{magenta|Spine Fruit}}")
            {
                BonusYield = Stat.Random(1, 10);
            }
            else if (FruitChoice == "{{green|Ripe Cucumber}}")
            {
                BonusYield = Stat.Random(1, 10);
            }
            else if (FruitChoice == "Plump Mushroom")
            {
                BonusYield = Stat.Random(1, 10);
            }
            else if (FruitChoice == "{{yellow|Banana}}")
            {
                BonusYield = Stat.Random(1, 7);
            }
            else if (FruitChoice == "{{brown|Yuckwheat Stem}}")
            {
                BonusYield = Stat.Random(1, 5);
            }
            else if (FruitChoice == "{{blue|Yondercane}}")
            {
                BonusYield = Stat.Random(1, 3);
            }
            else if (FruitChoice == "{{luminous|Hoarshroom}}")
            {
                BonusYield = Stat.Random(1, 5);
            }
            else if (FruitChoice == "{{lah|lah petals}}")
            {
                BonusYield = Stat.Random(1, 5);
            }
            else if (FruitChoice == "{{o|Eater's flesh}}")
            {
                BonusYield = Stat.Random(1, 3);
            }
            else if (FruitChoice == "{{purple|Urberry}}")
            {
                BonusYield = Stat.Random(0, 2);
            }
            else if (FruitChoice == "{{grey|Mirror Shard}}")
            {
                BonusYield = Stat.Random(1, 3);
            }
            else if (FruitChoice == "{{C-C-c-m-m-c-C-C sequence|arsplice}} seed")
            {
                BonusYield = Stat.Random(0, 1);
            }
        }

        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade) || ID == GetShortDescriptionEvent.ID;
        }

        public override bool HandleEvent(GetShortDescriptionEvent E)
        {
            string PreviewLookDescriptPreMature = "Fruiting Stage: {{red|Pre-Maturity.}}";
            string PreviewLookDescriptMature = "Fruiting Stage: {{orange|Maturity.}}";
            string PreviewLookDescriptRipening = "Fruiting Stage: {{yellow|Ripening.}}";
            string PreviewLookDescriptRipe = "{{green|Ripe.}}";
            if (E.Postfix.Length > 0 && E.Postfix[E.Postfix.Length - 1] != '\n')
            {
                E.Postfix.Append('\n');
            }
            if (GrowthPeriod >= uGrowthPeriod * 0.7 && IsPlayer())
            {
                E.Postfix.Append('\n').Append(PreviewLookDescriptPreMature);
            }
            else if (GrowthPeriod >= uGrowthPeriod * 0.3 && IsPlayer())
            {
                E.Postfix.Append('\n').Append(PreviewLookDescriptMature);
            }
            else if (GrowthPeriod >= uGrowthPeriod * 0.1 && IsPlayer())
            {
                E.Postfix.Append('\n').Append(PreviewLookDescriptRipening);
            }
            else if (GrowthPeriod == 0 && IsPlayer())
            {
                E.Postfix.Append('\n').Append(PreviewLookDescriptRipe);
            }
            return true;
        }

        public override bool Mutate(GameObject GO, int Level)
        {
            this.uAccessFruitMenuID = base.AddMyActivatedAbility(
             Name: "fruiting bodies",
             Command: "AccessFruitingCommand",
             Class: "Physical Mutation",
             Description: "Choose Fruit to grow from your various vines and branches.",
             Icon: "\u03A9");

            if (PlayerRepModified == false && ParentObject.IsPlayer())
            {
                XRL.Core.XRLCore.Core.Game.PlayerReputation.modify("plants", 100, true);
                PlayerRepModified = true;
            }

            this.ChangeLevel(Level);
            return base.Mutate(GO, Level);
        }

        public override void Register(GameObject Object)
        {
            Object.RegisterPartEvent(this, "EndTurn");
            Object.RegisterPartEvent(this, "AccessFruitingCommand");
            base.Register(Object);
        }

        public override bool FireEvent(Event E)
        {
            if (E.ID == "EndTurn")
            {
                if (GrowthPeriod > 0)
                {
                    var ParStomach = ParentObject.GetPart<Stomach>();

                    --GrowthPeriod;

                    if (ParentObject.HasPart("Barkflesh"))
                    {
                        GrowthPeriod -= 2;
                    }

                    if (ParentObject.HasPart("FibrousPantars"))
                    {
                        var MutCheck = ParentObject.GetPart<FibrousPantars>();

                        if (MutCheck.IsCurrentlyRooted == true)
                        {
                            GrowthPeriod -= (1 + MutCheck.Level);
                        }
                    }

                    // Thirst Effects on Nutrition

                    if (ParStomach.Water >= 30000 && NutritionalRatio < 100)
                    {
                        NutritionalRatio += 1;
                    }
                    else if (ParStomach.Water >= 40000 && NutritionalRatio < 100)
                    {
                        NutritionalRatio += 2;
                    }
                    else if (ParStomach.Water >= 50000 && NutritionalRatio < 100)
                    {
                        NutritionalRatio += 3;
                    }
                    else if (ParStomach.Water <= 30000 && NutritionalRatio > -100)
                    {
                        NutritionalRatio -= 1;
                    }
                    else if (ParStomach.Water <= 20000 && NutritionalRatio > -100)
                    {
                        NutritionalRatio -= 2;
                    }
                    else if (ParStomach.Water <= 10000 && NutritionalRatio > -100)
                    {
                        NutritionalRatio -= 3;
                    }

                    // Hunger Effects on Nutrition

                    if (ParStomach.HungerLevel == 0 && NutritionalRatio < 100)
                    {
                        NutritionalRatio += 3;
                    }
                    else if (ParStomach.HungerLevel == 1 && NutritionalRatio >= -100)
                    {
                        NutritionalRatio -= 1;
                    }
                    else if (ParStomach.HungerLevel >= 2 && NutritionalRatio >= -100)
                    {
                        NutritionalRatio -= 3;
                    }

                }
            }
            if (E.ID == "AccessFruitingCommand")
            {
                if (IsMyActivatedAbilityUsable(uAccessFruitMenuID))
                {
                    FruitingProcessMenu();
                }
            }
            return base.FireEvent(E);
        }
    }
}