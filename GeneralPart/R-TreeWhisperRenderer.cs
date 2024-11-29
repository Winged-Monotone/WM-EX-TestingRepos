// XRL.World.Parts.HologramMaterial
using System;
using System.Collections.Generic;
using XRL.Core;
using XRL.Rules;
using XRL.UI;
using XRL.World;
using XRL.World.Capabilities;

[Serializable]
public class TreeWhispRenderer : IPart
{
    public string Placeholder;
    public string Placeholder2;
    public int Placeholder3;
    public int Placeholder4;

    public string ColorStrings = "&M,&m,&y,&Y";
    public string DetailColors = "m,M,y,y";

    [NonSerialized]
    public int FrameOffset;

    [NonSerialized]
    public int FlickerFrame;

    [NonSerialized]
    private List<string> ColorStringParts;

    [NonSerialized]
    private List<string> DetailColorParts;

    public override bool Render(RenderEvent E)
    {
        int num = (XRLCore.CurrentFrame + FrameOffset) % 200;
        if (!ColorStrings.IsNullOrEmpty())
        {
            if (ColorStringParts == null)
            {
                ColorStringParts = ColorStrings.CachedCommaExpansion();
            }
            int count = ColorStringParts.Count;
            int num2 = num / count;
            E.ColorString = ((num2 < count) ? ColorStringParts[num2] : ColorStringParts[count - 1]);
        }
        if (!DetailColors.IsNullOrEmpty())
        {
            if (DetailColorParts == null)
            {
                DetailColorParts = DetailColors.CachedCommaExpansion();
            }
            int count2 = DetailColorParts.Count;
            int num3 = num / count2;
            E.DetailColor = ((num3 < count2) ? DetailColorParts[num3] : DetailColorParts[count2 - 1]);
        }
        if (FlickerFrame > 0 || Stat.RandomCosmetic(1, 200) == 1)
        {
            E.Tile = null;
            if (FlickerFrame == 0)
            {
                E.RenderString = "_";
            }
            if (FlickerFrame == 1)
            {
                E.RenderString = "-";
            }
            if (FlickerFrame == 2)
            {
                E.RenderString = "|";
            }
            if (FlickerFrame == 0)
            {
                FlickerFrame = 3;
            }
            FlickerFrame--;
            E.ColorString = "&Y";
        }
        else if (Stat.RandomCosmetic(1, 400) == 1)
        {
            E.ColorString = "&Y";
        }
        if (!Options.DisableTextAnimationEffects)
        {
            FrameOffset += Stat.Random(0, 20);
        }
        return true;
    }
}
