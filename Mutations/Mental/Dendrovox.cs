using System;
using System.Collections.Generic;
using System.Linq;
using XRL.World.Effects;

using System.Text;
using XRL.World;
using XRL.World.Parts.Mutation;
using XRL.Rules;

using XRL.Language;
using XRL.World.Capabilities;
using UnityEngine;
using XRL.Messages;
using XRL.UI;

namespace XRL.World.Parts.Mutation
{
    [Serializable]
    public class Dendrovoxy : BaseMutation
    {
        public int nNextLimb = 100;
        public int Density = 1;
        public string nTrees;
        public static readonly List<string> nQualifyingTrees = new List<string>()
        {
            "Shimscale Mangrove Tree_blue",
            "Shimscale Mangrove Tree",
            "Banana Tree",
            "Dogthorn Tree",
            "Leafless Dogthorn Tree",
            "Starapple Tree",
            "Starapple Farm Tree",
            "Witchwood Tree",
            "Cloudy Tree",
            "Branchy Tree",
            "Dicalyptus Tree",
            "Swarmshade Tree_blue",
            "Swarmshade Tree",
            "Tanglewood Tree",
            "Tanglewood Tree_blue",
            "Nachash Tree",
            "n-Ary Tree",
            "Glitchwood Tree"
        };
        public Dendrovoxy()
        {
            DisplayName = "Dendrovoxy";
        }
        public override string GetDescription()
        {
            return "You feel the words of flora that others may not, learning secrets and locations from various plantlife.";
        }
        public void GetAllTreesInZone()
        {
            var nZoneCells = ParentObject.CurrentZone.GetCells();

            foreach (var C in nZoneCells)
            {
                var nObjectsinCells = C.GetObjects();
                foreach (var O in nObjectsinCells)
                {

                    if (nQualifyingTrees.Any(O.DisplayName.Contains))
                    {
                        foreach (var k in nQualifyingTrees)
                        {
                            if (Stat.Random(1, 100) == 1)
                            {
                                O.AddPart<BespokenTree>();
                                O.AddPart<TreeWhispRenderer>();
                            }
                        }
                    }
                }
            }
        }
        public override bool WantEvent(int ID, int cascade)
        {
            return ID == BeginConversationEvent.ID
            || ID == EnteringZoneEvent.ID
            || base.WantEvent(ID, cascade);

        }
        public override bool HandleEvent(EnteringZoneEvent E)
        {
            GetAllTreesInZone();
            return base.HandleEvent(E);
        }

        public override bool FireEvent(Event E)
        {

            return base.FireEvent(E);
        }
    }
}