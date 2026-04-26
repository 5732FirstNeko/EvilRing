using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[CreateAssetMenu(fileName = "newUnitData", menuName = "Data/Skill Data/" + nameof(Shiled_core))]
public class Shiled_core : UnitSkillDataSo
{
    [SerializeField] private GameObject buffprefab;
    [SerializeField] private GameObject hitprefab;

    [SerializeField] private int skillListIndex;

    private GameObject buffEffect;
    private List<GameObject> hitEffect;

    public override void GameStartInit()
    {
        base.GameStartInit();

        buffEffect = Instantiate(buffprefab, Vector3.zero, Quaternion.identity);
        buffEffect.SetActive(false);

        hitEffect = new List<GameObject>();
        for (int i = 0; i < 4; i++)
        {
            GameObject effect = Instantiate(hitprefab, Vector3.zero, Quaternion.identity);
            effect.SetActive(false);
            hitEffect.Add(effect);
        }
    }

    public override void Action(ICollection<UnitPlat> unitPlats, UnitPlat user)
    {
        buffEffect.transform.position = user.transform.position;
        buffEffect.SetActive(true);

        PlayableDirector director = buffEffect.GetComponent<PlayableDirector>();
        director.Play();

        TimerManager.instance.StartTimer(name + "buffeffetClose", (float)director.duration + 0.1f, 
            ()=>
            {
                buffEffect.SetActive(false);
                user.unit.OnHPChange += HPChangeAction;
            });

        user.unit.unitSkills[skillListIndex].SkillTime = 0.5f;
    }

    private void HPChangeAction(int hpChange, UnitPlat user)
    {
        List<UnitPlat> units = BattleSystem.instance.HostilityUnitPlatsQueue.GetAllUnitPlat();

        if (units.Count <= 0) return;

        int damage = 0;
        if (hpChange > 0)
        {
            if (user.unit.HP > user.unit.MaxHP)
            {
                damage = Mathf.RoundToInt(hpChange * 5 * Damage * 0.1f);
            }
            else
            {
                damage = Mathf.RoundToInt(hpChange * 2 * Damage * 0.1f);
            }
        }
        else
        {
            damage = Damage;
        }

        buffEffect.transform.position = user.transform.position;
        buffEffect.SetActive(true);
        PlayableDirector director = buffEffect.GetComponent<PlayableDirector>();
        director.Play();

        float time = (float)hitEffect[0].GetComponent<PlayableDirector>().duration;
        for (int i = 0; i < units.Count; i++)
        {
            if (units[i].unitData == null ||
                units[i].unitData == FactorySystem.instance.EmptyFriendlyUnitData)
            {
                continue;
            }

            units[i].unit.HP -= damage;
            hitEffect[i].transform.position = units[i].transform.position;
            hitEffect[i].SetActive(true);
            hitEffect[i].GetComponent<PlayableDirector>().Play();
            units[i].UnitPlatHurtAnimation();
        }

        TimerManager.instance.StartTimer(name + "hitEffectClose" ,(float)director.duration + time + 0.1f, 
            () => 
            {
                buffEffect.SetActive(false);
                for (int i = 0; i < hitEffect.Count; i++)
                {
                    hitEffect[i].SetActive(false);
                }
            });
    }
}
