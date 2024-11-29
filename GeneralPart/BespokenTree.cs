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


namespace XRL.World.Parts
{
    [Serializable]
    public class BespokenTree : IPart
    {
        public Dictionary<string, bool> Visited = new Dictionary<string, bool>();
        public Action FunctionToCall;
        public bool GivenSecrets = false;
        public bool SpokenOnce = false;


        public override bool WantEvent(int ID, int cascade)
        {

            return ID == BeginConversationEvent.ID
            || ID == EnteringZoneEvent.ID
            || base.WantEvent(ID, cascade);

        }

        public override void Register(GameObject Object)
        {
            Object.RegisterPartEvent(this, "ShowConversationChoices");
            base.Register(Object);
        }
        public override bool FireEvent(Event E)
        {
            if (E.ID == "ShowConversationChoices")
            {
                int Cast = Stat.Random(0, 100);
                var eCurrentNode = E.GetParameter<ConversationNode>("CurrentNode");
                var eChoice = E.GetParameter<List<ConversationChoice>>("Choices");

                var choice = new ConversationChoice();

                choice.ParentNode = eCurrentNode;
                choice.GotoID = choice.ParentNode.ID;
                choice.Text = "Speak the Dendrid Tongue?";
                choice.onAction = () =>
                    {
                        if (SpokenOnce == false)
                            if (Cast <= 10)
                            {
                                if (GivenSecrets == false)
                                {
                                    IBaseJournalEntry randomUnrevealedNote = JournalAPI.GetRandomUnrevealedNote();
                                    JournalMapNote obj = randomUnrevealedNote as JournalMapNote;
                                    string text = "";
                                    text = ((obj == null) ? randomUnrevealedNote.text : ("the location of " + Grammar.InitLowerIfArticle(randomUnrevealedNote.text)));
                                    Popup.Show("You hear whispers amongst the roots of the " + ParentObject.DisplayName + ". They speak of " + text);
                                    randomUnrevealedNote.Reveal();
                                    SpokenOnce = true;
                                }
                                else
                                {
                                    SpokenOnce = true;
                                }
                            }
                            else if (Cast <= 15)
                            {
                                SpokenOnce = true;
                                Popup.Show("To be able to stride like you, I can only dream of what wonders you've beholden, kin. May your adventures lead you to great insight.");
                            }
                            else if (Cast <= 20)
                            {
                                SpokenOnce = true;
                                Popup.Show("We’ve spoken much of you, kin. And we intend to speak more. ( +50 Relations with Plants)");
                                XRL.Core.XRLCore.Core.Game.PlayerReputation.modify("arachnids", +50, true);
                            }
                            else if (Cast <= 30)
                            {
                                SpokenOnce = true;
                                Popup.Show("I've always wondered what its like to have fur, not that I get cold mind you, but being soft and fuzzy seems comfortable.");
                            }
                            else if (Cast <= 40)
                            {
                                SpokenOnce = true;
                                Popup.Show("Beware the amethyst glow, kin. Or you might find yourself a thrall of a nightmarish power.");
                            }
                            else if (Cast <= 50)
                            {
                                SpokenOnce = true;
                                Popup.Show("If you find yourself ill, seek the village hidden in the wilderness to south whose homes are carved from ancient mushrooms, there its master holds tomes for whom might aid you.");
                            }
                            else if (Cast <= 60)
                            {
                                SpokenOnce = true;
                                Popup.Show("I hear to the northeast, across the salt-swelling dunes, there lies a commune who made their homes amongst the corpse of an ancient beast. It prospers with commerce and may be of use to you, kin.");
                            }
                            else if (Cast <= 70)
                            {
                                SpokenOnce = true;
                                Popup.Show("Bethesda Susa bears the frost of the forebearers wrath to smother its ancient halls, tread with care their, kin. And find warmth in any way you can.");
                            }
                            else if (Cast <= 80)
                            {
                                SpokenOnce = true;
                                Popup.Show("Golgotha, a horrid place, take not a drop of the toxins and poisons in the bowels of that hell, less you leave diseased and marred--easy prey in world like this.");
                            }
                            else if (Cast <= 90)
                            {
                                SpokenOnce = true;
                                Popup.Show("Safety lies in Grit Gate, but it takes a certain strength to find it. Prepare, kin. And there you might find many secrets.");
                            }
                            else if (Cast <= 100)
                            {
                                SpokenOnce = true;
                                Popup.Show("I sometimes dream that Joppa lies hidden in a twisted fate of pergatory. Here and not here, so much so I know not what to say of them.");
                            }
                        return base.FireEvent(E);
                    };
            }
            else if (E.ID == "EnteredCell")
            {
                {
                    if (Stat.Random(0, 100) == 1)
                    {

                    }
                }
            }
            return base.FireEvent(E);
        }
    }
}
