using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;

[CreateAssetMenu(fileName = "newUnitData", menuName = "Data/Skill Data/Daimon/" + nameof(Daimon_believer))]
public class Daimon_believer : UnitSkillDataSo
{
    [SerializeField] private GameObject hitPrefab;

    private List<GameObject> hitEffects;

    public override void GameStartInit()
    {
        base.GameStartInit();

        hitEffects = new List<GameObject>();
        for (int i = 0; i < 2; i++)
        {
            GameObject effect = Instantiate(hitPrefab, Vector3.zero, Quaternion.identity);
            effect.SetActive(false);
            hitEffects.Add(effect);
        }
    }

    public override void Action(ICollection<UnitPlat> unitPlats, UnitPlat user)
    {
        int index = Random.Range(0, 101);

        if (index <= 75)
        {
            Attack(user);
        }
        else
        {
            DeepAttack(user);
        }
    }

    private void Attack(UnitPlat user)
    {
        UnitPlat target = null;
        foreach (var unit in 
            BattleSystem.instance.FriendlyUnitPlatsQueue.GetAllUnitPlat())
        {
            if (!unit.isDead && unit.unitData != FactorySystem.instance.EmptyFriendlyUnitData)
            {
                target = unit;
                break;
            }
        }

        if (target == null)
        {
            user.unit.unitSkills[0].SkillTime = 0.5f;
            return;
        }

        GameManager.instance.GlobalLightControll(0.5f, 0.5f);
        user.transform.DOScale(UnitPlat.originScale * attackScale, 0.5f);

        TimerManager.instance.StartTimer(name + "AttackEffect", 0.6f, 
            () => 
            {
                hitEffects[0].transform.position = target.transform.position;
                hitEffects[0].SetActive(true);
                hitEffects[0].GetComponent<PlayableDirector>().Play();

                target.unit.HP -= Damage;
                target.UnitPlatHurtAnimation();

                hitEffects[1].transform.position = target.transform.position;
                hitEffects[1].SetActive(true);
                hitEffects[1].GetComponent<PlayableDirector>().Play();

                user.unit.HP -= Damage;
                user.UnitPlatHurtAnimation();
            });

        TimerManager.instance.StartTimer(name + "EffectClose", 0.6f + 0.6f,
            () => 
            {
                for (int i = 0; i < hitEffects.Count; i++)
                {
                    hitEffects[i].SetActive(false);
                }

                GameManager.instance.GlobalLightControll(1f, 0.5f);
                user.transform.DOScale(UnitPlat.originScale, 0.5f);
            });

        user.unit.unitSkills[0].SkillTime = 0.6f + 0.6f + 0.6f;
    }

    private void DeepAttack(UnitPlat user)
    {
        UnitPlat target = null;
        foreach (var unit in
            BattleSystem.instance.FriendlyUnitPlatsQueue.GetAllUnitPlat())
        {
            if (!unit.isDead && unit.unitData != FactorySystem.instance.EmptyFriendlyUnitData)
            {
                target = unit;
                break;
            }
        }

        if (target == null)
        {
            user.unit.unitSkills[0].SkillTime = 0.5f;
            return;
        }

        GameManager.instance.GlobalLightControll(0.5f, 0.5f);
        user.transform.DOScale(UnitPlat.originScale * attackScale, 0.5f);

        TimerManager.instance.StartTimer(name + "AttackEffect", 0.6f,
            () =>
            {
                hitEffects[0].transform.position = target.transform.position;
                hitEffects[0].SetActive(true);
                hitEffects[0].GetComponent<PlayableDirector>().Play();

                target.unit.HP -= Damage;
                target.UnitPlatHurtAnimation();

                hitEffects[1].transform.position = target.transform.position;
                hitEffects[1].SetActive(true);
                hitEffects[1].GetComponent<PlayableDirector>().Play();

                user.unit.HP = 1;
                user.UnitPlatHurtAnimation();
            });

        TimerManager.instance.StartTimer(name + "EffectClose", 0.6f + 0.6f,
            () =>
            {
                for (int i = 0; i < hitEffects.Count; i++)
                {
                    hitEffects[i].SetActive(false);
                }

                GameManager.instance.GlobalLightControll(1f, 0.5f);
                user.transform.DOScale(UnitPlat.originScale, 0.5f);
            });

        user.unit.unitSkills[0].SkillTime = 0.6f + 0.6f + 0.6f;
    }
}
