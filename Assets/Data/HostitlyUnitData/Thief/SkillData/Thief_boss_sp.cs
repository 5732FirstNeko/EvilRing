using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(fileName = "newUnitData", menuName = "Data/Skill Data/" + nameof(Thief_boss_sp))]
public class Thief_boss_sp : UnitSkillDataSo
{
    [SerializeField] private UnitDataSo Thief_bossCard;

    public override void Action(ICollection<UnitPlat> unitPlats, UnitPlat user)
    {
        BattleSystem.instance.OnRoundStart += OnRoundStartAction;
    }

    private float OnRoundStartAction(int round)
    {
        if (round < 4)
        {
            return 0;
        }

        List<UnitPlat> unitplats = BattleSystem.instance.HostilityUnitPlatsQueue.GetAllUnitPlat();
        UnitPlat user = null;
        foreach (var unit in unitplats)
        {
            if (user == null && unit.unitData == Thief_bossCard)
            {
                user = unit;
                break;
            }
        }

        user.transform.DOScale(UnitPlat.originScale * attackScale, 0.5f);

        TimerManager.instance.StartTimer(name + "ThiefItem", 0.6f, 
            () => 
            {
                user.DamageTextJump("µÁÇÔ", Color.black);
                int index = Random.Range(0, InventoryManager.instance.itemList.Count);
                InventoryManager.instance.RemoveInventryFromList(InventoryManager.instance.itemList[index]);
            });

        TimerManager.instance.StartTimer(name + "ThiefRun", 0.6f + 1f, 
            () => 
            {
                user.DamageTextJump("ÌÓÅÜ", Color.black);

                
            });

        TimerManager.instance.StartTimer(name + "CardUpDate", 0.6f + 1f + 1f, 
            () =>
            {
                user.UnitPlatInit(FactorySystem.instance.EmptyHostitlyUnitData, user.site);

                user.iconSpriteRender.sprite = FactorySystem.instance.EmptyHostitlyUnitData.UnitSprite;
                Vector3 originScale = user.transform.localScale;
                user.transform.localScale = Vector3.zero;
                user.transform.DOScale(originScale, 1.5f).OnComplete(
                    () => 
                    {
                        user.transform.DOScale(UnitPlat.originScale, 0.5f);
                    });
            });

        return 0.6f + 1f + 1f + 1.5f + 0.6f;
    }
}
