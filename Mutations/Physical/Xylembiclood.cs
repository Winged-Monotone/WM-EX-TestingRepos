using System;

namespace XRL.World.Parts.Mutation
{
    public class XylemicBlood : BaseMutation
    {
        public XylemicBlood()
        {
            this.DisplayName = "Xylemic Blood";
        }

        public override bool AllowStaticRegistration()
        {
            return true;
        }

        public override string GetDescription()
        {
            return "Instead of blood, sap runs through your xylem conduits.";
        }

        public override string GetLevelText(int Level)
        {
            return "You release {{yellow|sap}} upon being wounded by an attack that would make you bleed.";
        }

        public override bool CanLevel()
        {
            return false;
        }

        public override bool Mutate(GameObject GO, int Level)
        {
            ParentObject.SetStringProperty("BleedLiquid", "sap-1000");
            Unmutate(GO);
            return base.Mutate(GO, Level);
        }
    }
}