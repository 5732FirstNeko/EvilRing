using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(fileName = "newUnitData", menuName = "Data/Skill Data/Element/" + nameof(Element_core))]
public class Element_core : UnitSkillDataSo
{
    [SerializeField] private UnitDataSo fireUnitData;
    [SerializeField] private UnitDataSo iceUnitData;
    [SerializeField] private UnitDataSo flashUnitData;

    public override void GameStartInit()
    {
        base.GameStartInit();

        BattleSystem.instance.OnRoundStart += OnRoundAction;
    }

    public override void Action(ICollection<UnitPlat> unitPlats, UnitPlat user)
    {
        GameManager.instance.GlobalLightControll(0.5f, 0.5f);
        user.transform.DOScale(UnitPlat.originScale * attackScale, 0.5f);

        TimerManager.instance.StartTimer(name + "SP++Effect", 0.6f, 
            () => 
            {
                foreach (var unit in
                    BattleSystem.instance.HostilityUnitPlatsQueue.GetAllUnitPlat())
                {
                    if (!unit.isDead && FactorySystem.instance.ElementCards.Contains(unit.unitData))
                    {
                        unit.DamageTextJump("sp +1", new Color(0.5f, 0, 0.5f));
                        unit.unit.spCount++;
                    }
                }
            });

        TimerManager.instance.StartTimer(name + "EffectCLose",0.6f + 1f, 
            () => 
            {
                GameManager.instance.GlobalLightControll(1f, 0.5f);
                user.transform.DOScale(UnitPlat.originScale, 0.5f);
            });

        user.unit.unitSkills[0].SkillTime = 0.6f + 1f + 0.6f;
    }

    private float OnRoundAction(int round)
    {
        int index = Random.Range(0, 3);
        switch (index)
        {
            case 0:
                foreach (var unit in
                    BattleSystem.instance.HostilityUnitPlatsQueue.GetAllUnitPlat())
                {
                    if (unit.unitData == fireUnitData)
                    {
                        unit.DamageTextJump("»ðÑæ¹²Ãù", Color.red);
                        unit.costumvalue_first = 1;
                    }

                    if (unit.unitData == iceUnitData)
                    {
                        unit.costumvalue_first = 0;
                    }

                    if (unit.unitData == flashUnitData)
                    {
                        unit.costumvalue_first = 0;
                    }
                }

                break;
            case 1:
                foreach (var unit in
                    BattleSystem.instance.HostilityUnitPlatsQueue.GetAllUnitPlat())
                {
                    if (unit.unitData == fireUnitData)
                    {
                        unit.costumvalue_first = 0;
                    }

                    if (unit.unitData == iceUnitData)
                    {
                        unit.DamageTextJump("±ùËª¹²Ãù", Color.blue);
                        unit.costumvalue_first = 1;
                    }

                    if (unit.unitData == flashUnitData)
                    {
                        unit.costumvalue_first = 0;
                    }
                }
                break;
            case 2:
                foreach (var unit in
                    BattleSystem.instance.HostilityUnitPlatsQueue.GetAllUnitPlat())
                {
                    if (unit.unitData == fireUnitData)
                    {
                        unit.costumvalue_first = 0;
                    }

                    if (unit.unitData == iceUnitData)
                    {
                        unit.costumvalue_first = 0;
                    }

                    if (unit.unitData == flashUnitData)
                    {
                        unit.DamageTextJump("À×µç¹²Ãù", new Color(0.5f, 0, 0.5f));
                        unit.costumvalue_first = 1;
                    }
                }
                break;
        }

        return 1.1f;
    }
}
