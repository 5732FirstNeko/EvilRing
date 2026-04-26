using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;

[CreateAssetMenu(fileName = "newUnitData", menuName = "Data/Skill Data/" + nameof(LycorisSkill_standrd_Damage))]
public class LycorisSkill_standrd_Damage : UnitSkillDataSo
{
    [SerializeField] private GameObject attackprefab;
    [SerializeField] private GameObject hostitlyHitprefab;
    [SerializeField] private GameObject lycorisInstantitePrefab;

    [SerializeField] private float attackEffectSpeed;
    [SerializeField] private int skillListIndex;

    private GameObject attackEffect;
    private GameObject lycorisInstantiteEffect;
    private List<GameObject> hostitlyHitEffects;

    public override void GameStartInit()
    {
        base.GameStartInit();

        attackEffect = Instantiate(attackprefab, Vector3.zero, Quaternion.identity);
        attackEffect.SetActive(false);

        lycorisInstantiteEffect = Instantiate(lycorisInstantitePrefab, Vector3.zero, Quaternion.identity);
        lycorisInstantiteEffect.SetActive(false);

        hostitlyHitEffects = new List<GameObject>();
        for (int i = 0; i < BattleSystem.unitPlatQueueCount; i++)
        {
            GameObject effect = Instantiate(hostitlyHitprefab, Vector3.zero, Quaternion.identity);
            effect.SetActive(false);
            hostitlyHitEffects.Add(effect);
        }
    }

    public override void Action(ICollection<UnitPlat> unitPlats, UnitPlat user)
    {
        if (unitPlats.Count <= 0)
        {
            user.unit.unitSkills[skillListIndex].SkillTime = 0;
            return;
        }

        GameManager.instance.GlobalLightControll(0.5f, 0.5f);
        user.transform.DOScale(UnitPlat.originScale * attackScale, 0.5f);

        lycorisInstantiteEffect.transform.position = user.transform.position;
        lycorisInstantiteEffect.SetActive(true);
        lycorisInstantiteEffect.GetComponent<PlayableDirector>().Play();

        Sequence sequence = DOTween.Sequence();

        float skilltime = 0;

        UnitPlat lastUnit = null;
        for (int i = 0; i < Range.Count; i++)
        {
            UnitSite site = BattleSystem.GetUnitSiteByIndex(i);
            UnitPlat target = null;
            foreach (var unit in unitPlats)
            {
                if (unit.site == site)
                {
                    target = unit;
                    break;
                }
            }
            if (target == null || !target.isDead) continue;

            lastUnit = target;

            float movetime = Mathf.Abs(target.transform.position.x -
                attackEffect.transform.position.x) / attackEffectSpeed;

            skilltime += movetime;

            int index = i;

            TimerManager.instance.StartTimer(name + string.Empty + i, movetime + 2f,
                () =>
                {
                    target.UnitPlatHurtAnimation();
                    target.unit.HP -= Damage;
                    hostitlyHitEffects[index].transform.position = target.transform.position;
                    hostitlyHitEffects[index].SetActive(true);
                    hostitlyHitEffects[index].GetComponent<PlayableDirector>().Play();
                });
        }

        TimerManager.instance.StartTimer(name + "flowerInstantite", 2f, () => 
        {
            attackEffect.transform.position = user.transform.position;
            attackEffect.SetActive(true);
            attackEffect.transform.DOMoveX(lastUnit.transform.position.x - 2, skilltime * 0.2f);
        });

        TimerManager.instance.StartTimer(name + "closeEffect", skilltime + 0.5f + 2f,
            () => 
            {
                attackEffect.SetActive(false);
                attackEffect.transform.position = user.transform.position;
                foreach (var effect in hostitlyHitEffects)
                {
                    effect.SetActive(false);
                }
                GameManager.instance.GlobalLightControll(1f, 0.5f);
                user.transform.DOScale(UnitPlat.originScale, 0.5f);
            });

        user.unit.unitSkills[skillListIndex].SkillTime = skilltime + 0.5f + 1.5f + 2f;
    }
}
