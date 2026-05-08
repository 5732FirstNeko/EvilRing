using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;

[CreateAssetMenu(fileName = "newUnitData", menuName = "Data/Skill Data/" + nameof(Witch_core))]
public class Witch_core : UnitSkillDataSo
{
    [SerializeField] private GameObject destoryPrefab;
    [SerializeField] private GameObject hitPrefab;

    [SerializeField] private UnitDataSo potCard;
    [SerializeField] private int skillListIndex;

    private GameObject destoryEffect;
    private GameObject hitEffect;

    public override void GameStartInit()
    {
        base.GameStartInit();

        destoryEffect = Instantiate(destoryPrefab, Vector3.zero, Quaternion.identity);
        destoryEffect.SetActive(false);
        BattleSystem.instance.destoryEffect.Add(destoryEffect);

        hitEffect = Instantiate(hitPrefab, Vector3.zero, Quaternion.identity);
        hitEffect.SetActive(false);
        BattleSystem.instance.destoryEffect.Add(hitEffect);
    }

    public override void GameEndAction()
    {
        destoryEffect = null;
        hitEffect = null;
    }

    public override void Action(ICollection<UnitPlat> unitPlats, UnitPlat user)
    {
        
    }
}
