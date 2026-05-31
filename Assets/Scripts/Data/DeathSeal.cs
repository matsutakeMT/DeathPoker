public class DeathSeal
{
    private SealType sealType;
    public int TriggerCount
    {
        get
        {
            return (int)sealType;
        }
    }

    public string SpriteName { get
        {
            if (sealType == SealType.None)
            {
                return "";
            }
            return $"deathSeal{TriggerCount}";
        }
    }

    public DeathSeal(SealType _sealType)
    {
        sealType = _sealType;
    }
}
