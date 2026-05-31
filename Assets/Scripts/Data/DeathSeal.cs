public class DeathSeal
{
    public SealType SealType;
    public int TriggerCount
    {
        get
        {
            return (int)SealType;
        }
    }

    public string SpriteName { get
        {
            if (SealType == SealType.None)
            {
                return "";
            }
            return $"deathSeal{TriggerCount}";
        }
    }

    public DeathSeal(SealType _sealType)
    {
        SealType = _sealType;
    }
}
