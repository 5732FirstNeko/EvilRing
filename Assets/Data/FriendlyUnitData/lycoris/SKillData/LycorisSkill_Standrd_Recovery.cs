using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using UnityEngine;
using UnityEngine.Playables;

[CreateAssetMenu(fileName = "newUnitData", menuName = "Data/Skill Data/" + nameof(LycorisSkill_Standrd_Recovery))]
public class LycorisSkill_Standrd_Recovery : UnitSkillDataSo
{
    [SerializeField] private GameObject effectprefab;
    [SerializeField] private float frihurttime;
    [SerializeField] private float friRecoverytime;

    private List<GameObject> effects;

    private List<UnitPlat> units;
    public override void GameStartInit()
    {
        base.GameStartInit();

        effects = new List<GameObject>();
        for (int i = 0; i < 4; i++)
        {
            GameObject effect = Instantiate(effectprefab, Vector3.zero, Quaternion.identity);
            effect.SetActive(false);
            effects.Add(effect);
        }

        units = new List<UnitPlat>();
    }

    public override void Action(ICollection<UnitPlat> unitPlats, UnitPlat user)
    {
        units.Clear();
        foreach (var unit in unitPlats)
        {
            if (unit.unit.HP <= Mathf.RoundToInt(0.5f * unit.unit.MaxHP) && unit != user)
            {
                units.Add(unit);
            }
        }

        if (units.Count <= 0)
        {
            user.unit.unitSkills[0].SkillTime = 0.5f;
            return;
        }

        user.transform.DOScale(user.transform.localScale * attackScale, 0.5f);
        GameManager.instance.GlobalLightControll(0.5f, 0.5f);
        user.unit.unitSkills[0].SkillTime = frihurttime + friRecoverytime + 0.2f;

        TimerManager.instance.StartTimer(name + nameof(LycorisSkill_Standrd_Recovery) + "0",
            0.5f, () => 
            {
                int damage = units.Count * Mathf.RoundToInt(user.unit.MaxHP * 0.25f);
                user.unit.HP = user.unit.HP - damage <= 0 ? 1 : user.unit.HP - damage;
                Debug.Log(user.unit.HP);
                user.UnitPlatHurtAnimation();
            });


        TimerManager.instance.StartTimer(name + nameof(LycorisSkill_Standrd_Recovery) + "1",
            frihurttime, () => 
            {
                for (int i = 0; i < units.Count; i++)
                {
                    int index = i;
                    effects[index].transform.position = units[index].transform.position;
                    effects[index].SetActive(true);
                    effects[index].GetComponentInChildren<ParticleSystem>().Play();
                }
            });

        TimerManager.instance.StartTimer(name + nameof(LycorisSkill_Standrd_Recovery) + "2",
            frihurttime + friRecoverytime * 0.5f, () => 
            {
                for (int i = 0; i < units.Count; i++)
                {
                    Debug.Log(units[i].unit.HP);
                    units[i].unit.HP += Mathf.RoundToInt(0.5f * units[i].unit.MaxHP);
                    Debug.Log(units[i].unit.HP);
                    units[i].UnitPlatRecoveryAnimation();
                }
            });

        TimerManager.instance.StartTimer(name + nameof(LycorisSkill_Standrd_Recovery) + "3",
            frihurttime + friRecoverytime, () =>
            {
                foreach (var effect in effects)
                {
                    effect.SetActive(false);
                }
                GameManager.instance.GlobalLightControll(1, 0.75f);
                user.transform.DOScale(UnitPlat.originScale, 0.5f);
            });
    }
}
