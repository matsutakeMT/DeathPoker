using UnityEngine;

[CreateAssetMenu(fileName = "SealSettings", menuName = "Scriptable Objects/SealSettings")]
public class SealSettings : ScriptableObject
{
    public float ProbabilityDeathSeal = 0.25f;
    public float ProbabilityDeath1 = 0.02f;
    public float ProbabilityDeath3 = 0.10f;
    public float ProbabilityDeath5 = 0.20f;
}
