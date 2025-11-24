using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ProteinTier
{
    Common,
    Rare,
    Epic,
    Legendary,
}

public class ProteinData
{
    public string ProteinName;
    public string Description;
    public ProteinTier Tier;
    public string EffectID; // Identifier for the effect implementation
    public Color Color;

    public ProteinData(string name, string description, ProteinTier tier, string effectID, Color color)
    {
        ProteinName = name;
        Description = description;
        Tier = tier;
        EffectID = effectID;
        Color = color;
    }
}

public static class ProteinDatabase
{
    /// <summary>
    /// Analyzes the amino acid chain and determines the resulting protein.
    /// </summary>
    /// <param name="chain">List of amino acids collected between Start (Met) and Stop codons.</param>
    /// <returns>The matching ProteinData.</returns>
    public static ProteinData CheckRecipe(List<AminoAcidData> chain)
    {
        if (chain == null || chain.Count == 0)
        {
            return null;
        }

        // 1. Check for Exact Sequence Matches (High Priority)
        // Glutathione: Glu -> Cys -> Gly
        if (
            chain.Count == 3
            && chain[0].ShortName == "Glu"
            && chain[1].ShortName == "Cys"
            && chain[2].ShortName == "Gly"
        )
        {
            return new ProteinData(
                "Glutathione",
                "The Master Antioxidant. Purifies the body.",
                ProteinTier.Rare,
                "Glutathione",
                new Color(0.8f, 1f, 0.8f) // Pale Green
            );
        }

        // 2. Check for Composition-based Matches (Prioritize by length/complexity)

        int length = chain.Count;
        var counts = GetAminoAcidCounts(chain);

        // Hemoglobin Type: Length 15+, must contain His (Heme coordination)
        if (length >= 15 && counts.ContainsKey("His") && counts["His"] >= 1)
        {
            return new ProteinData(
                "Hemoglobin",
                "[Satellite] 4 Oxygen molecules rotate around you, shredding enemies.",
                ProteinTier.Legendary,
                "Hemoglobin",
                new Color(1f, 0.2f, 0.2f) // Blood Red
            );
        }

        // Keratin Type: Length 10-14, must contain 2+ Cys (Disulfide bonds)
        if (length >= 10 && length <= 14 && counts.ContainsKey("Cys") && counts["Cys"] >= 2)
        {
            return new ProteinData(
                "Keratin",
                "[Thorns] Increases Defense and reflects damage.",
                ProteinTier.Epic,
                "Keratin",
                new Color(0.9f, 0.8f, 0.5f) // Pale Skin/Hair tone
            );
        }

        // Insulin Type: Length 5-9, must contain Val & Glu (A-chain start reference)
        if (
            length >= 5
            && length <= 9
            && counts.ContainsKey("Val")
            && counts["Val"] >= 1
            && counts.ContainsKey("Glu")
            && counts["Glu"] >= 1
        )
        {
            return new ProteinData(
                "Insulin",
                "[Berserk] Increases Attack Speed drastically.",
                ProteinTier.Rare,
                "Insulin",
                new Color(0.4f, 0.6f, 1f) // Medical Blue
            );
        }

        // Collagen Type: Length 10+, rich in Pro (Proline) or Gly (Glycine)
        // Assuming "rich" means substantial presence, let's say at least 3 combined.
        int proCount = counts.ContainsKey("Pro") ? counts["Pro"] : 0;
        int glyCount = counts.ContainsKey("Gly") ? counts["Gly"] : 0;
        if (length >= 10 && (proCount + glyCount >= 3))
        {
            return new ProteinData(
                "Collagen",
                "[Hardened] Increases Max HP and reduces incoming damage.",
                ProteinTier.Epic,
                "Collagen",
                new Color(0.9f, 0.9f, 0.9f) // White/Bone
            );
        }

        // 3. Fallback based on Length
        if (length >= 10)
        {
            return new ProteinData(
                "Large Polypeptide",
                "A complex chain of amino acids. Grants significant stats.",
                ProteinTier.Epic,
                "LargePolypeptide",
                Color.magenta
            );
        }
        else if (length >= 5)
        {
            return new ProteinData(
                "Polypeptide",
                "A moderate chain of amino acids. Grants moderate stats.",
                ProteinTier.Rare,
                "Polypeptide",
                Color.cyan
            );
        }
        else
        {
            return new ProteinData(
                "Oligopeptide",
                "A short chain of amino acids. Grants minor stats.",
                ProteinTier.Common,
                "Oligopeptide",
                Color.gray
            );
        }
    }

    private static Dictionary<string, int> GetAminoAcidCounts(List<AminoAcidData> chain)
    {
        Dictionary<string, int> counts = new Dictionary<string, int>();
        foreach (var aa in chain)
        {
            if (counts.ContainsKey(aa.ShortName))
                counts[aa.ShortName]++;
            else
                counts[aa.ShortName] = 1;
        }
        return counts;
    }
}
