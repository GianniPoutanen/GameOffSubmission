using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlocksManager : MonoBehaviour
{

    public enum eSkillType
    {

    }

    [SerializeField]
    public List<eSkillType> unlockedSkills = new List<eSkillType>();

    #region Constructor
    private static UnlocksManager _i;
    public static UnlocksManager Instance
    {
        get
        {
            if (_i == null)
            {
                _i = Instantiate(Resources.Load<UnlocksManager>("Managers/UnlocksManager"));
                GameAssets.Instance.AddToPool("Managers", _i.gameObject);
            }

            return _i;
        }
    }
    #endregion Constructor

    public void UnlockSkill(eSkillType skill)
    {
        if (!unlockedSkills.Contains(skill))
        {
            unlockedSkills.Add(skill);
        }
    }

    public bool CheckUnlocked(eSkillType skill)
    {
        return unlockedSkills.Contains(skill);
    }
}