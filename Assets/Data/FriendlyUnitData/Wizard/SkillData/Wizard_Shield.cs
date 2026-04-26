using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;

[CreateAssetMenu(fileName = "newUnitData", menuName = "Data/Skill Data/" + nameof(Wizard_Shield))]
public class Wizard_Shield : UnitSkillDataSo
{
    [SerializeField] private GameObject shiledPrefab;
    [SerializeField] private GameObject standrdMagicPrefab;
    [SerializeField] private GameObject standrdhitPrefab;

    [SerializeField] private int skillListIndex;
    [SerializeField] private int explsionEnergy;
    [SerializeField] private float standrdAttackSpeed;

    private List<GameObject> shiledEffect;

    private GameObject standrdMagicAttackEffect;
    private GameObject standrdHitEffect;

    public override void GameStartInit()
    {
        base.GameStartInit();

        shiledEffect = new List<GameObject>();
        for (int i = 0; i < 6; i++)
        {
            GameObject effect = Instantiate(shiledPrefab, Vector3.zero, Quaternion.identity);
            effect.SetActive(false);
            shiledEffect.Add(effect);
        }

        standrdMagicAttackEffect = Instantiate(standrdMagicPrefab, Vector3.zero, Quaternion.identity);
        standrdMagicAttackEffect.SetActive(false);

        standrdHitEffect = Instantiate(standrdhitPrefab, Vector3.zero, Quaternion.identity);
        standrdHitEffect.SetActive(false);
    }

    public override void Action(ICollection<UnitPlat> unitPlats, UnitPlat user)
    {
        if (unitPlats.Count <= 0)
        {
            user.unit.unitSkills[skillListIndex].SkillTime = 0;
            return;
        }

        UnitPlat target = null;
        foreach (var unit in unitPlats)
        {
            if (target == null)
            {
                target = unit;
                continue;
            }

            if (unit.unit.MaxHP < 20 && unit.unit.MaxHP < target.unit.MaxHP)
            {
                target = unit;
            }
        }

        if (target == null)
        {
            user.unit.unitSkills[skillListIndex].SkillTime = 0;
            return;
        }

        user.transform.DOScale(UnitPlat.originScale * attackScale, 0.5f);
        GameManager.instance.GlobalLightControll(0.5f, 0.5f);

        if (user.site != UnitSite.first)
        {
            int index = BattleSystem.GetIndexByUnitSite(user.site);
            UnitSite toSite = BattleSystem.GetUnitSiteByIndex(
                (index + 1 + BattleSystem.unitPlatQueueCount) % BattleSystem.unitPlatQueueCount);
            BattleSystem.instance.UnitSiteChange(user.unit.faction, user.site, toSite);
        }

        int num = Random.Range(0, 100);
        if (num <= 25)
        {
            standrdMagicAttackEffect.transform.position = user.transform.position;
            standrdMagicAttackEffect.SetActive(true);

            float movedis = Mathf.Abs(target.transform.position.x -
                standrdMagicAttackEffect.transform.position.x);
            float movetime = movedis / standrdAttackSpeed;

            standrdMagicAttackEffect.transform.DOMoveX(target.transform.position.x, movetime).
                OnComplete(() =>
                {
                    target.UnitPlatHurtAnimation();
                    target.unit.HP -= Damage;

                    standrdHitEffect.transform.position = target.transform.position;
                    standrdHitEffect.SetActive(true);
                    standrdHitEffect.GetComponent<PlayableDirector>().Play();
                });

            TimerManager.instance.StartTimer(name + "StandrdHitEffectClose", movetime + 1f,
                () =>
                {
                    standrdMagicAttackEffect.SetActive(false);
                    standrdHitEffect.SetActive(false);
                    standrdMagicAttackEffect.transform.position = user.transform.position;
                });

            user.unit.unitSkills[skillListIndex].SkillTime = movetime + 1f;
            user.transform.DOScale(UnitPlat.originScale, 0.5f).SetDelay(movetime + 1f);
        }
        else
        {
            target.GetComponent<SpriteRenderer>().DOColor(Color.yellow, 0.5f);
            target.unit.OnHPChange += Shiled;

            for (int i = 0; i < shiledEffect.Count; i++)
            {
                if (!shiledEffect[i].activeSelf)
                {
                    shiledEffect[i].transform.position = target.transform.position;
                    shiledEffect[i].SetActive(true);
                    break;
                }
            }

            user.unit.unitSkills[skillListIndex].SkillTime = 1.5f;

            TimerManager.instance.StartTimer(name + "GlobalClose", 1f, () =>
            {
                user.transform.DOScale(UnitPlat.originScale, 0.5f);
                GameManager.instance.GlobalLightControll(1f, 0.5f);
            });
        }
    }

    private void Shiled(int hpchange, UnitPlat user)
    {
        if (hpchange < 0)
        {
            user.unit.HP -= hpchange;
            user.unit.OnHPChange -= Shiled;

            for (int i = 0; i < shiledEffect.Count; i++)
            {
                if (Vector3.Distance(shiledEffect[i].transform.position,
                    user.transform.position) < 0.1f)
                {
                    shiledEffect[i].SetActive(false);
                    break;
                }
            }

            return;
        }
    }
}
