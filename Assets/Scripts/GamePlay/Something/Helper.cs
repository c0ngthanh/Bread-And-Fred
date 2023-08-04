using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helper : MonoBehaviour
{
    public enum Element{
        Fire,
        Water,
        Elec,
        Dendro
    };
    public enum SellBy{
        Money,
        Gems
    };
    public enum TypeSell{
        ATK,
        ATKSPD,
        SPEED,
        HEALTH,
        SKILL
    };
    public enum SkillState{
        Unlocked,
        Locked
    }
    public enum WaypointState{
        Unlocked,
        Locked
    }
}
