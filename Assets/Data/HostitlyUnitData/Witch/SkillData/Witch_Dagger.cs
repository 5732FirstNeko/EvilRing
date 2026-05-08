using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;

[CreateAssetMenu(fileName = "newUnitData", menuName = "Data/Skill Data/" + nameof(Witch_Dagger))]
public class Witch_Dagger : UnitSkillDataSo
{
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private GameObject hitPrefab;

    [SerializeField] private int skillListIndex;
    [SerializeField] private UnitDataSo potCard;

    private GameObject arrowEffect;
    private GameObject hitEffect;

    public override void GameStartInit()
    {
        base.GameStartInit();

        arrowEffect = Instantiate(arrowPrefab, Vector3.zero, Quaternion.identity);
        arrowEffect.SetActive(false);
        BattleSystem.instance.destoryEffect.Add(arrowEffect);

        hitEffect = Instantiate(hitPrefab, Vector3.zero, Quaternion.identity);
        hitEffect.SetActive(false);
        BattleSystem.instance.destoryEffect.Add(hitEffect);
    }

    public override void GameEndAction()
    {
        arrowEffect = null;
        hitEffect = null;
    }

    public override void Action(ICollection<UnitPlat> unitPlats, UnitPlat user)
    {
        unitPlats = BattleSystem.instance.FriendlyUnitPlatsQueue.GetAllUnitPlat();
        if (unitPlats.Count <= 0)
        {
            user.unit.unitSkills[skillListIndex].SkillTime = 0.5f;
            return;
        }

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
        user.costumvlue_third += 2;
        if (user.costumvlue_third < 3)
        {
            Dodge(unitPlats, user);
            return;
        }

        user.costumvlue_third = 0;
        GameManager.instance.GlobalLightControll(0.5f, 0.5f);
        user.transform.DOScale(UnitPlat.originScale * attackScale, 0.5f);
        user.iconSpriteRender.material = GameManager.UnlitMaterial;

        UnitPlat target = null;

        foreach (var unit in unitPlats)
        {
            if (target == null && 
                unit.unitData != FactorySystem.instance.EmptyFriendlyUnitData)
            {
                target = unit;
                continue;
            }

            if (target.unit.HP > target.unit.HP &&
                unit.unitData != FactorySystem.instance.EmptyFriendlyUnitData)
            {
                target = unit;
            }
        }

        TimerManager.instance.StartTimer(name + "ArrowEffect", 0.6f,
            () =>
            {
                arrowEffect.transform.position = target.transform.position + UnitPlat.bottomDistance * 1.5f;
                arrowEffect.SetActive(true);

                arrowEffect.transform.DOMove(target.transform.position + UnitPlat.topDistance * 1.5f, 1f).
                OnComplete(() => { arrowEffect.SetActive(false); });
            });

        TimerManager.instance.StartTimer(name + "ArrowEffect", 0.6f + 0.5f, 
            () => 
            {
                hitEffect.transform.position = target.transform.position;
                hitEffect.SetActive(true);
                hitEffect.GetComponent<PlayableDirector>().Play();

                target.unit.HP -= 15;
                target.UnitPlatHurtAnimation();
            });

        TimerManager.instance.StartTimer(name + "EffectClose", 0.6f + 1f + 0.1f,
            () =>
            {
                user.unit.spCount -= 2;
                hitEffect.SetActive(false);

                user.iconSpriteRender.material = GameManager.litMaterial;
                GameManager.instance.GlobalLightControll(1f, 0.5f);
                user.transform.DOScale(UnitPlat.originScale, 0.5f);
            });

        user.unit.unitSkills[skillListIndex].SkillTime = 0.6f + 1f + 0.2f;
    }

    private void Dodge(ICollection<UnitPlat> unitPlats, UnitPlat user)
    {
        GameManager.instance.GlobalLightControll(0.5f, 0.5f);
        user.transform.DOScale(UnitPlat.originScale * attackScale, 0.5f);

        user.iconSpriteRender.material = GameManager.UnlitMaterial;

        user.DamageTextJump("ÉÁ±ÜÌáÉý", Color.black);
        user.unit.OnDefend = null;
        user.unit.OnDefend += DodgeAction;

        TimerManager.instance.StartTimer(name + "ScaleRecovery", 1.1f,
            () =>
            {
                user.iconSpriteRender.material = GameManager.litMaterial;
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
                user.DamageTextJump("ÉÁ±Ü", Color.yellow);
            }

            user.unit.OnDefend -= DodgeAction;
            return isDodge ? -hpchange : 0;
        }

        return 0;
    }
}
