using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disorder
{
    public enum DisorderType
    {
        mirage = 0,
        injury = 1,
        poisoning = 2,
        thirst = 3,
        hunger = 4
    }
    public Disorder(DisorderType type)
    {
        disorderType = type;
        Health = 0;
        Stamina = 0;
        Defense = 0;
        Damage = 0;
        StaminaRegen = 0;
        HealthRegen = 0;
        MoveSpeed = 0;
        MaxHealth = 0;
        MaxStamina = 0;
        DisorerLevel = 0;
        if (type.Equals(DisorderType.mirage))
        {
            MirageEffect();
        }
        else if (type.Equals(DisorderType.injury))
        {
            InjuryEffect();
        }
        else if (type.Equals(DisorderType.poisoning))
        {
            PoisoningEffect();
        }
        else if (type.Equals(DisorderType.thirst))
        {
            ThirstEffect();
        }
        else
        {
            HungerEffect();
        }
    }

    public DisorderType disorderType;

    public float Health;
    public float Stamina;
    public float Defense;
    public float Damage;
    public int StaminaRegen;
    public int HealthRegen;
    public float MaxHealth;
    public float MaxStamina;
    public float MoveSpeed;
    public int DisorerLevel;
    public string description;
    public string triggerCondition;
    public string recoveryCondition;

    public void MirageEffect()
    {
        MaxStamina = -30;   
    }

    public void InjuryEffect()
    {
        MoveSpeed = -5;
        MaxHealth = -20;
        Defense = -30;
        Damage = -30;
        MoveSpeed = -4;

    }

    public void PoisoningEffect()
    {
        Health = -30;
        Stamina = -30;
        HealthRegen = -5;
        StaminaRegen = -5;
    }

    public void ThirstEffect()
    {
        Stamina = -40;
    }

    public void HungerEffect()
    {
        Stamina = -20;
        Damage = -40;
        MoveSpeed = -3;
    }
}
