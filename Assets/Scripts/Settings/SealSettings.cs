using UnityEngine;

[CreateAssetMenu(fileName = "SealSettings", menuName = "Scriptable Objects/SealSettings")]
public class SealSettings : ScriptableObject
{
    [Header("デスシールをつける確率")]
    [Range(0f, 1f)]
    public float ProbabilityDeathSeal = 0.42f;
    [Space(10)]
    [Header("1回見たら死ぬシールである確率")]
    [Range(0f, 1f)]
    public float ProbabilityDeath1 = 0.0476190f;
    [Header("3回見たら死ぬシールである確率 (残りが5回見たら死ぬシール)")]
    [Range(0f, 1f)]
    public float ProbabilityDeath3 = 0.2380952f;
}
