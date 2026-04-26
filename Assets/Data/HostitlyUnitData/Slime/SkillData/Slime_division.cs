using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(fileName = "newUnitData", menuName = "Data/Skill Data/" + nameof(Slime_division))]
public class Slime_division : UnitSkillDataSo
{
    [SerializeField] private int skillListIndex;
    [SerializeField] private UnitDataSo slimeCard;

    public override void Action(ICollection<UnitPlat> unitPlats, UnitPlat user)
    {
        if (user.unit.HP >= user.unit.MaxHP * 0.5f)
        {
            user.unit.SPSkill.SkillTime = 0.5f;
            BattleSystem.instance.UnitReEnqueue(user);
            return;
        }

        UnitPlat nearPlat = null;
        int userIndex = BattleSystem.GetIndexByUnitSite(user.site);
        UnitSite rightSite = BattleSystem.GetUnitSiteByIndex(userIndex + 1);
        UnitSite leftSite = BattleSystem.GetUnitSiteByIndex(userIndex - 1);
        UnitPlat rightPlat = BattleSystem.instance.HostilityUnitPlatsQueue.GetUnitPlatByUnitSite(rightSite).plat;
        UnitPlat leftPlat = BattleSystem.instance.HostilityUnitPlatsQueue.GetUnitPlatByUnitSite(leftSite).plat;
        if (rightPlat != null && rightPlat.unitData == FactorySystem.instance.EmptyHostitlyUnitData)
        {
            nearPlat = rightPlat;
        }

        if (nearPlat == null && leftPlat != null && leftPlat.unitData == FactorySystem.instance.EmptyHostitlyUnitData)
        {
            nearPlat = leftPlat;
        }

        GameManager.instance.GlobalLightControll(0.5f, 0.5f);
        user.transform.DOScale(UnitPlat.originScale * attackScale, 0.5f);

        TimerManager.instance.StartTimer(name + "SlimeDivision", 0.6f, 
            () =>
            {
                user.UnitPlatInit(slimeCard, user.site);

                user.iconSpriteRender.sprite = slimeCard.UnitSprite;
                Vector3 originScale = user.transform.localScale;
                user.transform.localScale = Vector3.zero;
                user.transform.DOScale(originScale, 1.5f).OnComplete(
                    () => 
                    {
                        GameManager.instance.GlobalLightControll(1f, 0.5f);
                        user.transform.DOScale(UnitPlat.originScale, 0.5f);
                    });

                if (nearPlat == null) return;

                Material originMaterial = nearPlat.iconSpriteRender.material;
                nearPlat.iconSpriteRender.material = GameManager.UnlitMaterial;
                nearPlat.UnitPlatInit(slimeCard, nearPlat.site);

                nearPlat.iconSpriteRender.sprite = slimeCard.UnitSprite;
                Vector3 nearoriginScale = nearPlat.transform.localScale;
                nearPlat.transform.localScale = Vector3.zero;
                nearPlat.transform.DOScale(nearoriginScale, 1.5f).OnComplete(
                    () => 
                    {
                        nearPlat.iconSpriteRender.material = originMaterial;
                    });
            });

        user.unit.SPSkill.SkillTime = 0.6f + 1.5f + 0.5f + 0.1f;
    }
}
