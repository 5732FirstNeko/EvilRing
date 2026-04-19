using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;

[CreateAssetMenu(fileName = "newUnitData", menuName = "Data/Skill Data/" + nameof(Guards_core))]
public class Guards_core : UnitSkillDataSo
{
    [SerializeField] private GameObject attackPrefab;
    [SerializeField] private GameObject recoveryPrefab;
    [SerializeField] private GameObject hitPrefab;

    [SerializeField] private float effectActionRate;
    [SerializeField] private int skillListIndex;

    private GameObject attackEffect;
    private GameObject recoveryEffect;
    private List<GameObject> hitEffects;

    public override void GameStartInit()
    {
        base.GameStartInit();

        attackEffect = Instantiate(attackPrefab, Vector3.zero, Quaternion.identity);
        attackEffect.SetActive(false);

        recoveryEffect = Instantiate(recoveryPrefab, Vector3.zero, Quaternion.identity);
        recoveryEffect.SetActive(false);

        hitEffects = new List<GameObject>();
        for (int i = 0; i < 4; i++)
        {
            GameObject effect = Instantiate(hitPrefab, Vector3.zero, Quaternion.identity);
            effect.SetActive(false);
            hitEffects.Add(effect);
        }
    }

    public override void Action(ICollection<UnitPlat> unitPlats, UnitPlat user)
    {
        int index = Random.Range(0, 101);

        if (index <= 75)
        {
            List<UnitPlat> units = BattleSystem.instance.FriendlyUnitPlatsQueue.GetAllUnitPlat();

            if (units.Count <= 0)
            {
                user.unit.unitSkills[skillListIndex].SkillTime = 0.5f;
                return;
            }

            GameManager.instance.GlobalLightControll(0.5f, 0.5f);
            user.transform.DOScale(UnitPlat.originScale * attackScale, 0.5f);

            PlayableDirector director = attackEffect.GetComponent<PlayableDirector>();
            TimerManager.instance.StartTimer(name + "AttackEffect", 0.6f, 
                () => 
                {
                    attackEffect.transform.position = user.transform.position;
                    attackEffect.SetActive(true);
                    director.Play();
                });

            TimerManager.instance.StartTimer(name + "HitEffect",
                0.6f + (float)director.duration * effectActionRate, 
                () => 
                {
                    int i = 0;
                    foreach (var unit in units)
                    {
                        if (unit.isDead || unit.unitData == FactorySystem.instance.EmptyFriendlyUnitData)
                        {
                            continue;
                        }

                        int j = i;
                        hitEffects[j].transform.position = unit.transform.position;
                        hitEffects[j].SetActive(true);
                        hitEffects[j].GetComponent<PlayableDirector>().Play();

                        unit.unit.HP -= Damage;
                        unit.UnitPlatHurtAnimation();

                        i++;
                    }
                });

            TimerManager.instance.StartTimer(name + "HitEffectClose",
                0.6f + (float)director.duration + 0.6f,
                () => 
                {
                    attackEffect.SetActive(false);
                    for (int i = 0; i < hitEffects.Count; i++)
                    {
                        hitEffects[i].SetActive(false);
                    }

                    GameManager.instance.GlobalLightControll(1f, 0.5f);
                    user.transform.DOScale(UnitPlat.originScale, 0.5f);
                });

            user.unit.unitSkills[skillListIndex].SkillTime =
                0.6f + (float)director.duration + 0.6f + 0.6f;
        }
        else
        {
            List<UnitPlat> units = BattleSystem.instance.HostilityUnitPlatsQueue.GetAllUnitPlat();

            if (units.Count <= 0)
            {
                user.unit.unitSkills[skillListIndex].SkillTime = 0.5f;
                return;
            }

            GameManager.instance.GlobalLightControll(0.5f, 0.5f);
            user.transform.DOScale(UnitPlat.originScale * attackScale, 0.5f);

            PlayableDirector director = recoveryEffect.GetComponent<PlayableDirector>();
            TimerManager.instance.StartTimer(name + "AttackEffect", 0.6f,
                () =>
                {
                    recoveryEffect.transform.position = user.transform.position;
                    recoveryEffect.SetActive(true);
                    director.Play();
                });

            TimerManager.instance.StartTimer(name + "HitEffect",
                0.6f + (float)director.duration * effectActionRate,
                () =>
                {
                    foreach (var unit in units)
                    {
                        if (unit.isDead || unit.unitData == FactorySystem.instance.EmptyHostitlyUnitData)
                        {
                            continue;
                        }

                        unit.UnitPlatRecoveryAnimation();
                        unit.unit.HP += 
                            FactorySystem.instance.GuardsCards.Contains(unit.unitData) ? Damage : Damage * 2;
                    }
                });

            TimerManager.instance.StartTimer(name + "HitEffectClose",
                0.6f + (float)director.duration + 0.6f,
                () =>
                {
                    attackEffect.SetActive(false);
                    for (int i = 0; i < hitEffects.Count; i++)
                    {
                        hitEffects[i].SetActive(false);
                    }

                    GameManager.instance.GlobalLightControll(1f, 0.5f);
                    user.transform.DOScale(UnitPlat.originScale, 0.5f);
                });

            user.unit.unitSkills[skillListIndex].SkillTime =
                0.6f + (float)director.duration + 0.6f + 0.6f;
        }
    }
}
