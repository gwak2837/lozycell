using System.Collections.Generic;
using UnityEngine;

public struct AminoAcidData
{
    public string ShortName;
    public string FullName;
    public string SkillDescription;
    public Color Color;

    public AminoAcidData(string shortName, string fullName, string skillDesc, Color color)
    {
        ShortName = shortName;
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
        // 산성 아미노산
        { "Asp", new AminoAcidData("Asp", "Aspartic Acid", "ACID POOL", new Color(0.2f, 0.8f, 0.2f)) }, // 현재 위치에 피해를 주는 장판 생성 (지속시간 5초)
        { "Glu", new AminoAcidData("Glu", "Glutamic Acid", "SYNAPTIC BOOST", new Color(1f, 0.4f, 0.4f)) }, // 플레이어 이동속도 증가 (지속시간 10초)
        // 염기성 아미노산
        { "Arg", new AminoAcidData("Arg", "Arginine", "TESLA COIL", new Color(1f, 0.9f, 0.2f)) }, // 플레이어 주위에 피해를 주는 전기장 생성 (지속시간 5초)
        { "His", new AminoAcidData("His", "Histidine", "ANAPHYLAXIS", new Color(1f, 0.6f, 0.2f)) }, // 화면에 보이는 모든 적에게 큰 피해를 줌
        { "Lys", new AminoAcidData("Lys", "Lysine", "CHAIN LIGHTNING", new Color(0.4f, 0.8f, 1f)) }, // 근처 적끼리 연결해서 피해를 줌 (최대 4명)
        // 극성 아미노산
        { "Asn", new AminoAcidData("Asn", "Asparagine", "GRASS KNOT", new Color(0.4f, 0.8f, 0.4f)) }, // 현재 위치에 느려지는 장판 생성 (지속시간 5초)
        { "Cys", new AminoAcidData("Cys", "Cysteine", "S-S DEATH BOND", new Color(1f, 0.84f, 0f)) }, // 근처 적과 연결하는 선을 생성해 넘을 때마다 피해를 줌 (지속시간 5초)
        { "Gln", new AminoAcidData("Gln", "Glutamine", "HEAL", new Color(0.4f, 1f, 0.6f)) }, // 플레이어가 잃은 체력의 1/3만큼 회복
        { "Ser", new AminoAcidData("Ser", "Serine", "PHOSPHO MARK", new Color(0.8f, 0.2f, 0.8f)) }, // 화면에 보이는 모든 적의 방어력을 0으로 만듦 (지속시간 10초)
        { "Thr", new AminoAcidData("Thr", "Threonine", "ALCOHOL BURN", new Color(1f, 0.3f, 0f)) }, // 화면에 보이는 모든 적에게 지속 피해를 줌 (지속시간 5초)
        { "Tyr", new AminoAcidData("Tyr", "Tyrosine", "HOMING MISSILES", new Color(0.9f, 0.4f, 0.9f)) }, // 타겟팅하는 투사체 3개 발사
        // 비극성 아미노산
        { "Ala", new AminoAcidData("Ala", "Alanine", "MULTISHOT", new Color(0.8f, 0.8f, 0.8f)) }, // 작고 약한 투사체 5개 발사
        { "Gly", new AminoAcidData("Gly", "Glycine", "SYNAPSE SHUTDOWN", new Color(0.4f, 0.6f, 0.9f)) }, // 화면에 보이는 적 이동속도 느려짐 (지속시간 10초)
        { "Ile", new AminoAcidData("Ile", "Isoleucine", "MIRROR IMAGE", new Color(0.5f, 0.8f, 0.9f)) }, // 플레이어 주위에서 공격하는 도플갱어 소환 (지속시간 5초)
        { "Leu", new AminoAcidData("Leu", "Leucine", "MUSCLE UP", new Color(0.8f, 0.2f, 0.2f)) }, // 플레이어 공격력 증가 (지속시간 10초)
        { "Phe", new AminoAcidData("Phe", "Phenylalanine", "ORBITAL SHIELD", new Color(0.3f, 0.6f, 1f)) }, // 플레이어 주변에 최대 체력의 100% 보호막 생성 (지속시간 3초)
        { "Pro", new AminoAcidData("Pro", "Proline", "BOOMERANG", new Color(0.6f, 0.4f, 0.2f)) }, // 관통해서 돌아오는 투사체 1개 발사
        { "Trp", new AminoAcidData("Trp", "Tryptophan", "GRAVITATIONAL COLLAPSE", new Color(0.2f, 0f, 0.4f)) }, // 플레이어 주변에 중력 장판 생성 (지속시간 5초)
        { "Val", new AminoAcidData("Val", "Valine", "POWER SHOT", new Color(1f, 0.5f, 0f)) }, // 강력하고 큰 투사체 1개 발사
        // 개시/종결 코돈
        { "Met", new AminoAcidData("Met", "Methionine", "METHYL TRAIL", new Color(0.2f, 1f, 0.4f)) }, // 플레이어 이동 경로를 따라 피해를 주는 장판을 생성함 (방구차) (지속시간 5초, 장판 지속시간 3초)
        { "Stop", new AminoAcidData("Stop", "STOP CODON", "UNLIMITED VOID", new Color(1f, 0.1f, 0.1f)) }, // 모든 적을 멈춤 (지속시간 5초)
    };

    public static string GetAminoAcid(BaseType b1, BaseType b2, BaseType b3)
    {
        string key = $"{b1}{b2}{b3}";
        return codonMap[key];
    }

    private static AminoAcidData GetAminoAcidData(string shortName)
    {
        return aminoAcidData[shortName];
    }

    public static AminoAcidData GetCodonData(BaseType b1, BaseType b2, BaseType b3)
    {
        string shortName = GetAminoAcid(b1, b2, b3);
        return GetAminoAcidData(shortName);
    }
}
