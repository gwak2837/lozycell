using System.Collections.Generic;
using UnityEngine;

public struct AminoAcidData
{
    public string FullName;
    public string SkillDescription;
    public Color Color;

    public AminoAcidData(string fullName, string skillDesc, Color color)
    {
        FullName = fullName;
        SkillDescription = skillDesc;
        Color = color;
    }
}

public static class CodonTable
{
    private static readonly Dictionary<string, string> codonMap = new Dictionary<string, string>
    {
        { "UUU", "Phe" },
        { "UUC", "Phe" },
        { "UUA", "Leu" },
        { "UUG", "Leu" },
        { "CUU", "Leu" },
        { "CUC", "Leu" },
        { "CUA", "Leu" },
        { "CUG", "Leu" },
        { "AUU", "Ile" },
        { "AUC", "Ile" },
        { "AUA", "Ile" },
        { "AUG", "Met" },
        { "GUU", "Val" },
        { "GUC", "Val" },
        { "GUA", "Val" },
        { "GUG", "Val" },
        { "UCU", "Ser" },
        { "UCC", "Ser" },
        { "UCA", "Ser" },
        { "UCG", "Ser" },
        { "CCU", "Pro" },
        { "CCC", "Pro" },
        { "CCA", "Pro" },
        { "CCG", "Pro" },
        { "ACU", "Thr" },
        { "ACC", "Thr" },
        { "ACA", "Thr" },
        { "ACG", "Thr" },
        { "GCU", "Ala" },
        { "GCC", "Ala" },
        { "GCA", "Ala" },
        { "GCG", "Ala" },
        { "UAU", "Tyr" },
        { "UAC", "Tyr" },
        { "UAA", "Stop" },
        { "UAG", "Stop" },
        { "CAU", "His" },
        { "CAC", "His" },
        { "CAA", "Gln" },
        { "CAG", "Gln" },
        { "AAU", "Asn" },
        { "AAC", "Asn" },
        { "AAA", "Lys" },
        { "AAG", "Lys" },
        { "GAU", "Asp" },
        { "GAC", "Asp" },
        { "GAA", "Glu" },
        { "GAG", "Glu" },
        { "UGU", "Cys" },
        { "UGC", "Cys" },
        { "UGA", "Stop" },
        { "UGG", "Trp" },
        { "CGU", "Arg" },
        { "CGC", "Arg" },
        { "CGA", "Arg" },
        { "CGG", "Arg" },
        { "AGU", "Ser" },
        { "AGC", "Ser" },
        { "AGA", "Arg" },
        { "AGG", "Arg" },
        { "GGU", "Gly" },
        { "GGC", "Gly" },
        { "GGA", "Gly" },
        { "GGG", "Gly" },
    };

    // Visual Data Mapping
    private static readonly Dictionary<string, AminoAcidData> aminoAcidData = new Dictionary<string, AminoAcidData>
    {
        // Group A: Non-polar (Physical/Gray/Orange)
        { "Gly", new AminoAcidData("Glycine", "MINIGUN!", Color.gray) },
        { "Ala", new AminoAcidData("Alanine", "SHOOT!", Color.gray) },
        { "Val", new AminoAcidData("Valine", "POWER SHOT!", new Color(0.8f, 0.5f, 0.2f)) }, // Orange
        { "Leu", new AminoAcidData("Leucine", "MUSCLE UP!", new Color(0.8f, 0.5f, 0.2f)) }, // Orange
        { "Ile", new AminoAcidData("Isoleucine", "IMPACT!", new Color(0.8f, 0.5f, 0.2f)) }, // Orange
        { "Pro", new AminoAcidData("Proline", "BOOMERANG!", Color.gray) },
        // Group B: Polar (Water/Blue/Cyan)
        { "Ser", new AminoAcidData("Serine", "SLOW FIELD!", Color.cyan) },
        { "Thr", new AminoAcidData("Threonine", "FREEZE!", Color.cyan) },
        { "Asn", new AminoAcidData("Asparagine", "WAVE!", new Color(0.2f, 0.6f, 1f)) }, // Light Blue
        { "Gln", new AminoAcidData("Glutamine", "TIDAL!", new Color(0.2f, 0.6f, 1f)) }, // Light Blue
        // Group C: Basic (Lightning/Yellow)
        { "Lys", new AminoAcidData("Lysine", "LIGHTNING!", Color.yellow) },
        { "Arg", new AminoAcidData("Arginine", "THUNDER SMASH!", Color.yellow) },
        { "His", new AminoAcidData("Histidine", "OVERCHARGE!", Color.yellow) },
        // Group D: Acidic (Fire/Acid/Red-Green)
        { "Asp", new AminoAcidData("Aspartic Acid", "POISON POOL!", new Color(0.3f, 1f, 0.3f)) }, // Green Acid
        { "Glu", new AminoAcidData("Glutamic Acid", "EXPLOSIVE!", new Color(1f, 0.3f, 0.3f)) }, // Red Fire
        // Group E: Special (Purple/Magenta)
        { "Phe", new AminoAcidData("Phenylalanine", "HOMING MISSILES!", new Color(1f, 0f, 1f)) }, // Magenta
        { "Tyr", new AminoAcidData("Tyrosine", "CRITICAL!", new Color(0.8f, 0f, 0.8f)) },
        { "Trp", new AminoAcidData("Tryptophan", "METEOR STRIKE!", new Color(0.6f, 0f, 0.8f)) }, // Deep Purple
        { "Cys", new AminoAcidData("Cysteine", "LASER LINK!", new Color(0.8f, 0.8f, 0f)) },
        // Start/Stop
        { "Met", new AminoAcidData("Methionine", "START - SHIELD!", Color.green) },
        { "Stop", new AminoAcidData("STOP CODON", "SELF DESTRUCT!", Color.red) },
        // Fallback
        { "Unknown", new AminoAcidData("Unknown", "FAILED", Color.white) },
    };

    public static string GetAminoAcid(BaseType b1, BaseType b2, BaseType b3)
    {
        string key = $"{b1}{b2}{b3}";
        if (codonMap.TryGetValue(key, out string aminoAcid))
        {
            return aminoAcid;
        }
        return "Unknown";
    }

    public static AminoAcidData GetData(string shortName)
    {
        if (aminoAcidData.TryGetValue(shortName, out AminoAcidData data))
        {
            return data;
        }
        return new AminoAcidData(shortName, "SYNTHESIZED!", Color.white);
    }
}
