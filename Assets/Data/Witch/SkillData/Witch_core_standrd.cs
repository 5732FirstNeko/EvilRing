using System.Collections;
using System.Collections.Generic;
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

    public override void GameStartInit()
    {
        base.GameStartInit();

        potEffect = Instantiate(potPrefab, Vector3.zero, Quaternion.identity);
        potEffect.SetActive(false);
    }

    public override void Action(ICollection<UnitPlat> unitPlats, UnitPlat user)
    {
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
        TimerManager.instance.StartTimer(name + "PotEffect",0.6f, 
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

                    unit.unit.spCount++;
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

        if(user.costumvalue_second == 1) user.costumvalue_first--;

        if (user.costumvalue_first <= 0)
        {
            UnitPlat pot = BattleSystem.instance.HostilityUnitPlatsQueue.
                GetUnitPlatByUnitSite(UnitSite.first).plat;

            pot.UnitPlatInit(FactorySystem.instance.EmptyHostitlyUnitData, UnitSite.first);
            pot.iconSpriteRender.sprite = FactorySystem.instance.EmptyHostitlyUnitData.UnitSprite;
            user.costumvalue_second = 0;
        }

        user.unit.unitSkills[skillListIndex].SkillTime = 0.7f + (float)director.duration + 0.6f;
    }
}
