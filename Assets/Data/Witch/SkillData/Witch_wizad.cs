using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;

[CreateAssetMenu(fileName = "newUnitData", menuName = "Data/Skill Data/" + nameof(Witch_wizad))]
public class Witch_wizad : UnitSkillDataSo
{
    [SerializeField] private GameObject flashPrefab;
    [SerializeField] private GameObject hitPrefab;

    [SerializeField] private int skillListIndex;
    [SerializeField] private UnitDataSo potCard;

    private GameObject flashEffect;
    private List<GameObject> smallFlashEffect;
    private List<GameObject> hitEffects;

    public override void GameStartInit()
    {
        base.GameStartInit();

        flashEffect = Instantiate(flashPrefab, Vector3.zero, Quaternion.identity);
        flashEffect.SetActive(false);

        smallFlashEffect = new List<GameObject>();
        for (int i = 0; i < 2; i++)
        {
            GameObject effect = Instantiate(flashEffect, Vector3.zero, Quaternion.identity);
            effect.transform.GetChild(0).transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
            effect.SetActive(false);
            smallFlashEffect.Add(effect);
        }

        hitEffects = new List<GameObject>();
        for (int i = 0; i < 3; i++)
        {
            GameObject effect = Instantiate(hitPrefab, Vector3.zero, Quaternion.identity);
            effect.SetActive(false);
            hitEffects.Add(effect);
        }
    }

    public override void Action(ICollection<UnitPlat> unitPlats, UnitPlat user)
    {
        if (unitPlats.Count <= 0)
        {
            user.unit.unitSkills[skillListIndex].SkillTime = 0.5f;
            return;
        }

        GameManager.instance.GlobalLightControll(0.5f, 0.5f);
        user.transform.DOScale(UnitPlat.originScale * attackScale, 0.5f);

        int index = UnityEngine.Random.Range(0, 101);
        if (index <= 25)
        {
            Dodge(unitPlats, user);
        }
        else
        {
            wizardAttack(unitPlats, user);
        }
    }

    private void wizardAttack(ICollection<UnitPlat> unitPlats, UnitPlat user)
    {
        if (user.unit.spCount < 2)
        {
            user.unit.unitSkills[skillListIndex].SkillTime = 0.5f;
            return;
        }

        UnitPlat first = null;
        UnitPlat second = null;
        UnitPlat third = null;

        foreach (var unit in unitPlats)
        {
            if (first == null &&
                unit.unitData != FactorySystem.instance.EmptyHostitlyUnitData)
            {
                first = unit;
            }
            else if (first != null &&
                unit.unitData != FactorySystem.instance.EmptyHostitlyUnitData)
            {
                second = unit;
            }
            else if (first != null && second != null &&
                unit.unitData != FactorySystem.instance.EmptyHostitlyUnitData)
            {
                third = unit;
            }
        }

        if (second == null)
        {
            second = first;
            third = second;
        }
        else if (second != null && third == null)
        {
            third = second;
        }

        PlayableDirector director = flashEffect.GetComponent<PlayableDirector>();
        TimerManager.instance.StartTimer(name + "FlashEffect", 0.6f,
            () =>
            {
                flashEffect.transform.position = first.transform.position;
                flashEffect.SetActive(true);

                first.unit.HP -= Damage;
                first.UnitPlatHurtAnimation();
                hitEffects[0].transform.position = first.transform.position;
                hitEffects[0].SetActive(true);
                hitEffects[0].GetComponent<PlayableDirector>().Play();

                director.Play();
            });

        TimerManager.instance.StartTimer(name + "smallFlashEffect_0", 0.6f + 0.1f + (float)director.duration,
            () =>
            {
                smallFlashEffect[0].transform.position = second.transform.position;
                smallFlashEffect[0].SetActive(true);
                smallFlashEffect[0].GetComponent<PlayableDirector>().Play();
                second.unit.HP -= Mathf.RoundToInt(Damage * 0.9f);
                second.UnitPlatHurtAnimation();

                hitEffects[1].transform.position = second.transform.position;
                hitEffects[1].SetActive(true);
                hitEffects[1].GetComponent<PlayableDirector>().Play();
            });

        TimerManager.instance.StartTimer(name + "smallFlashEffect_1", 0.6f + 0.1f + (float)director.duration,
            () =>
            {
                smallFlashEffect[1].transform.position = third.transform.position;
                smallFlashEffect[1].SetActive(true);
                smallFlashEffect[1].GetComponent<PlayableDirector>().Play();
                third.unit.HP -= Mathf.RoundToInt(Damage * 0.9f);
                third.UnitPlatHurtAnimation();

                hitEffects[2].transform.position = third.transform.position;
                hitEffects[2].SetActive(true);
                hitEffects[2].GetComponent<PlayableDirector>().Play();
            });

        TimerManager.instance.StartTimer(name + "EffectClose", 0.6f + 3 * (float)director.duration + 0.1f,
            () =>
            {
                user.unit.spCount -= 2;
                flashEffect.SetActive(false);
                for (int i = 0; i < smallFlashEffect.Count; i++)
                {
                    smallFlashEffect[i].SetActive(false);
                }
                for (int i = 0; i < hitEffects.Count; i++)
                {
                    hitEffects[i].SetActive(false);
                }

                GameManager.instance.GlobalLightControll(1f, 0.5f);
                user.transform.DOScale(UnitPlat.originScale, 0.5f);
            });

        user.unit.unitSkills[skillListIndex].SkillTime = 0.6f + 3 * (float)director.duration + 0.2f;
    }

    private void Dodge(ICollection<UnitPlat> unitPlats, UnitPlat user)
    {
        user.DamageTextJump("…¡±‹Ã·…˝", Color.black);

        Func<int, UnitPlat, int> dodgeActionDelegate = DodgeAction;
        foreach (var method in user.unit.OnDefend.GetInvocationList())
        {
            if (method.Target == dodgeActionDelegate.Target && 
                dodgeActionDelegate.GetInvocationList().Length == 1 &&
                method.Method == dodgeActionDelegate.Method)
            {
                user.unit.OnDefend -= DodgeAction;
            }
        }

        user.unit.OnDefend += DodgeAction;

        TimerManager.instance.StartTimer(name + "ScaleRecovery", 1.1f, 
            () => 
            {
                GameManager.instance.GlobalLightControll(1f, 0.5f);
                user.transform.DOScale(UnitPlat.originScale, 0.5f);
            });

        user.unit.unitSkills[skillListIndex].SkillTime = 1.7f;
    }

    private int DodgeAction(int hpchange, UnitPlat user)
    {
        if (hpchange < 0)
        {
            int index = UnityEngine.Random.Range(0, 101);
            bool isDodge = BattleSystem.instance.HostilityUnitPlatsQueue.
                GetUnitPlatByUnitSite(UnitSite.first).plat.unitData == potCard ? index <= 50 : index <= 25;

            if (isDodge)
            {
                user.DamageTextJump("…¡±‹", Color.yellow);
            }
            
            user.unit.OnDefend -= DodgeAction;
            return isDodge ? -hpchange : 0;
        }

        return 0;
    }
}
