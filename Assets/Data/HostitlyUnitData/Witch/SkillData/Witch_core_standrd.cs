using System.Collections;
using System.Collections.Generic;
using Coffee.UIEffects;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;

[CreateAssetMenu(fileName = "newUnitData", menuName = "Data/Skill Data/" + nameof(Witch_core_standrd))]
public class Witch_core_standrd : UnitSkillDataSo
{
    [SerializeField] private GameObject potPrefab;

    [SerializeField] private int skillListIndex;
    [SerializeField] private UnitDataSo potCard;

    private GameObject potEffect;

    [SerializeField] private GameObject destoryPrefab;
    [SerializeField] private GameObject hitPrefab;

    private GameObject destoryEffect;
    private GameObject hitEffect;

    public override void GameStartInit()
    {
        base.GameStartInit();

        potEffect = Instantiate(potPrefab, Vector3.zero, Quaternion.identity);
        potEffect.SetActive(false);
        BattleSystem.instance.destoryEffect.Add(potEffect);

        destoryEffect = Instantiate(destoryPrefab, Vector3.zero, Quaternion.identity);
        destoryEffect.SetActive(false);
        BattleSystem.instance.destoryEffect.Add(destoryEffect);

        hitEffect = Instantiate(hitPrefab, Vector3.zero, Quaternion.identity);
        hitEffect.SetActive(false);
        BattleSystem.instance.destoryEffect.Add(hitEffect);
    }

    public override void GameEndAction()
    {
        potEffect = null;

        destoryEffect = null;
        hitEffect = null;
    }

    public override void Action(ICollection<UnitPlat> unitPlats, UnitPlat user)
    {
        user.costumvlue_third += 3;

        if (user.costumvlue_third >= 6)
        {
            SpAttack(unitPlats, user);
        }
        else
        {
            StandrdAttack(unitPlats, user);
        }

        user.costumvalue_second = 0;
    }

    private void StandrdAttack(ICollection<UnitPlat> unitPlats, UnitPlat user)
    {
        unitPlats = BattleSystem.instance.HostilityUnitPlatsQueue.GetAllUnitPlat();
        if (unitPlats.Count <= 0)
        {
            user.unit.unitSkills[skillListIndex].SkillTime = 0.5f;
            return;
        }

        UnitPlat pool = BattleSystem.instance.HostilityUnitPlatsQueue.GetUnitPlatByUnitSite(UnitSite.first).plat;

        if (pool.unitData != potCard)
        {
            user.unit.unitSkills[skillListIndex].SkillTime = 0.5f;
            return;
        }

        GameManager.instance.GlobalLightControll(0.5f, 0.5f);
        user.transform.DOScale(UnitPlat.originScale * attackScale, 0.5f);

        PlayableDirector director = potEffect.GetComponent<PlayableDirector>();
        TimerManager.instance.StartTimer(name + "PotEffect", 0.6f,
            () =>
            {
                potEffect.transform.position = pool.transform.position;
                potEffect.SetActive(true);
                director.Play();
                foreach (var unit in unitPlats)
                {
                    if (unit.unitData == FactorySystem.instance.EmptyHostitlyUnitData)
                    {
                        continue;
                    }

                    unit.costumvlue_third++;
                    unit.DamageTextJump("sp +1", Color.black);
                }
            });


        TimerManager.instance.StartTimer(name + "potEffectClose", 0.7f + (float)director.duration,
            () =>
            {
                potEffect.SetActive(false);
                GameManager.instance.GlobalLightControll(1f, 0.5f);
                user.transform.DOScale(UnitPlat.originScale, 0.5f);
            });

        if (user.costumvalue_second == 1) user.costumvalue_first--;

        if (user.costumvalue_first <= 0)
        {
            UnitPlat pot = BattleSystem.instance.HostilityUnitPlatsQueue.
                GetUnitPlatByUnitSite(UnitSite.first).plat;

            pot.UnitPlatInit(FactorySystem.instance.EmptyHostitlyUnitData, UnitSite.first);
            pot.iconSpriteRender.sprite = FactorySystem.instance.EmptyHostitlyUnitData.UnitSprite;
            user.costumvalue_second = 0;
        }

        user.unit.unitSkills[skillListIndex].SkillTime = 0.7f + (float)director.duration + 1f;
    }

    private void SpAttack(ICollection<UnitPlat> unitPlats, UnitPlat user)
    {
        if (user.costumvlue_third < 6)
        {
            user.unit.unitSkills[skillListIndex].SkillTime = 0.5f;
            return;
        }

        unitPlats = BattleSystem.instance.FriendlyUnitPlatsQueue.GetAllUnitPlat();
        UnitPlat target = null;
        foreach (var unit in unitPlats)
        {
            if (target == null)
            {
                target = unit;
                continue;
            }

            if (target.unit.HP > unit.unit.HP && 
                !unit.isDead && unit.unitData == FactorySystem.instance.EmptyFriendlyUnitData)
            {
                target = unit;
            }
        }

        UnitPlat spwanUnit = BattleSystem.instance.HostilityUnitPlatsQueue.
            GetUnitPlatByUnitSite(UnitSite.first).plat;
        if (target == null ||
            spwanUnit.unitData == FactorySystem.instance.EmptyFriendlyUnitData ||
            user.costumvalue_second == 1)
        {
            user.unit.unitSkills[skillListIndex].SkillTime = 0.5f;
            return;
        }

        GameManager.instance.GlobalLightControll(0.5f, 0.5f);
        user.transform.DOScale(UnitPlat.originScale * attackScale, 0.5f);
        user.costumvalue_first = 3;
        user.iconSpriteRender.material = GameManager.UnlitMaterial;

        PlayableDirector DestoryDirector = destoryEffect.GetComponent<PlayableDirector>();
        TimerManager.instance.StartTimer(name + "DestoryEffect", 0.6f,
            () =>
            {
                destoryEffect.transform.position = target.transform.position;
                destoryEffect.SetActive(true);
                DestoryDirector.Play();

                target.unit.HP = 0;
            });

        PlayableDirector HitDirector = hitEffect.GetComponent<PlayableDirector>();
        TimerManager.instance.StartTimer(name + "HitEffect", 0.7f + (float)DestoryDirector.duration,
            () =>
            {
                hitEffect.transform.position = target.transform.position;
                hitEffect.SetActive(true);
                HitDirector.Play();
            });

        TimerManager.instance.StartTimer(name + "UnitCardSpwan",
            0.7f + (float)DestoryDirector.duration + (float)HitDirector.duration,
            () =>
            {
                destoryEffect.SetActive(false);
                hitEffect.SetActive(false);

                spwanUnit.UnitPlatInit(potCard, UnitSite.first);

                spwanUnit.iconSpriteRender.sprite = potCard.UnitSprite;
                Vector3 originScale = spwanUnit.transform.localScale;
                spwanUnit.transform.localScale = Vector3.zero;
                spwanUnit.transform.DOScale(originScale, 1.5f);

                user.costumvlue_third -= 6;
                spwanUnit.costumvalue_first = 3;
                user.costumvalue_second = 1;
            });

        TimerManager.instance.StartTimer(name + "ActionEnd",
            0.7f + (float)DestoryDirector.duration + (float)HitDirector.duration + 1.6f,
            () =>
            {
                GameManager.instance.GlobalLightControll(1f, 0.5f);
                user.transform.DOScale(UnitPlat.originScale, 0.5f);
            });

        user.unit.unitSkills[skillListIndex].SkillTime =
            0.7f + (float)DestoryDirector.duration + (float)HitDirector.duration + 1.6f + 0.6f;
    }
}
