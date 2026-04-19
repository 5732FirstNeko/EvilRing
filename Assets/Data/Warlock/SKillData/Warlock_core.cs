using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;

[CreateAssetMenu(fileName = "newUnitData", menuName = "Data/Skill Data/" + nameof(Warlock_core))]
public class Warlock_core : UnitSkillDataSo
{
    [SerializeField] private GameObject numberUpBuffPrefab;

    [SerializeField] private int skillListIndex;

    private List<GameObject> numberUpBuffEffect;

    public override void GameStartInit()
    {
        base.GameStartInit();

        numberUpBuffEffect = new List<GameObject>();
        for (int i = 0; i < 4; i++)
        {
            GameObject effect = Instantiate(numberUpBuffPrefab, Vector3.zero, Quaternion.identity);
            effect.SetActive(false);
            numberUpBuffEffect.Add(effect);
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

        TimerManager.instance.StartTimer(name + "UpBuffEffectInit", 0.6f, 
            () => 
            {
                int i = 0;
                foreach (var unit in unitPlats)
                {
                    int index = i;
                    numberUpBuffEffect[index].transform.position = unit.transform.position;
                    numberUpBuffEffect[index].SetActive(true);
                    numberUpBuffEffect[index].GetComponent<PlayableDirector>().Play();

                    if (FactorySystem.instance.warlockCards.Contains(unit.unitData))
                    {
                        if (unit.costumvalue_first <= 1)
                        {
                            unit.costumvalue_first = 2;
                        }
                        else
                        {
                            unit.costumvalue_first *= unit.costumvalue_first;
                        }
                    }

                    i++;
                }
            });

        float effecttime = (float)numberUpBuffEffect[0].GetComponent<PlayableDirector>().duration;
        TimerManager.instance.StartTimer(name + "UpEffectClose",0.7f + effecttime, 
            () => 
            {
                for (int i = 0; i < numberUpBuffEffect.Count; i++)
                {
                    numberUpBuffEffect[i].SetActive(false);
                }

                GameManager.instance.GlobalLightControll(1, 0.5f);
                user.transform.DOScale(UnitPlat.originScale, 0.5f);
            });

        user.unit.unitSkills[skillListIndex].SkillTime = 0.7f + 3f + 0.1f;
    }
}
