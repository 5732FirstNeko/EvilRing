using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;

[CreateAssetMenu(fileName = "newUnitData", menuName = "Data/Skill Data/Necro/" + nameof(Necro_SkeletalArc))]
public class Necro_SkeletalArc : UnitSkillDataSo
{
    [SerializeField] private GameObject arrowAttackPrefab;
    [SerializeField] private GameObject arrowHitPrefab;

    [SerializeField] private int skillListIndex;

    private List<GameObject> arrowAttcakEffect;
    private List<GameObject> arrowHitEffects;

    public override void GameStartInit()
    {
        base.GameStartInit();

        arrowAttcakEffect = new List<GameObject>();
        for (int i = 0; i < 2; i++)
        {
            GameObject effect = Instantiate(arrowAttackPrefab, Vector3.zero, Quaternion.identity);
            effect.SetActive(false);
            arrowAttcakEffect.Add(effect);
        }

        arrowHitEffects = new List<GameObject>();
        for (int i = 0; i < 2; i++)
        {
            GameObject effect = Instantiate(arrowHitPrefab, Vector3.zero, Quaternion.identity);
            effect.SetActive(false);
            arrowHitEffects.Add(effect);
        }
    }

    public override void Action(ICollection<UnitPlat> unitPlats, UnitPlat user)
    {
        int index = Random.Range(0, 101);
        if (index <= 80)
        {
            TwoArcShoot(user);
        }
        else
        {
            ArcShoot(user);
        }
    }

    private void TwoArcShoot(UnitPlat user)
    {
        UnitPlat first = null;
        UnitPlat second = null;

        foreach (var unit in
            BattleSystem.instance.FriendlyUnitPlatsQueue.GetAllUnitPlat())
        {
            if (!unit.isDead && unit.unitData != FactorySystem.instance.EmptyFriendlyUnitData)
            {
                if (first == null)
                {
                    first = unit;
                    continue;
                }

                if (first != null && second == null)
                {
                    second = unit;
                    break;
                }
            }
        }

        if (first == null)
        {
            user.unit.unitSkills[skillListIndex].SkillTime = 0.5f;
            return;
        }

        GameManager.instance.GlobalLightControll(0.5f, 0.5f);
        user.transform.DOScale(UnitPlat.originScale * attackScale, 0.5f);

        TimerManager.instance.StartTimer(name + "ArrowAttackEffetc", 0.6f,
            () =>
            {
                float height = 1.5f;
                Vector3[] bulletPath = new Vector3[]
                {
                        user.transform.position,
                        user.transform.position + new Vector3(1.5f, height, 0),
                        first.transform.position,
                };

                Vector3 startDir_first = bulletPath[1] - arrowAttcakEffect[0].transform.position;
                arrowAttcakEffect[0].transform.rotation = Quaternion.FromToRotation(Vector3.up, startDir_first);

                arrowAttcakEffect[0].transform.position = user.transform.position;
                arrowAttcakEffect[0].SetActive(true);
                arrowAttcakEffect[0].transform.DOPath(bulletPath, 1f, PathType.CatmullRom).
                    SetEase(Ease.InQuart).SetOptions(false).SetDelay(0.5f).OnComplete(
                    () =>
                    {
                        arrowAttcakEffect[0].SetActive(false);
                        arrowHitEffects[0].transform.position = first.transform.position;
                        arrowHitEffects[0].SetActive(true);
                        arrowHitEffects[0].GetComponent<PlayableDirector>().Play();

                        first.unit.HP -= Damage;
                        first.UnitPlatHurtAnimation();
                    });

                if (second == null) return;

                Vector3[] path = new Vector3[]
                {
                        user.transform.position,
                        user.transform.position + new Vector3(1.5f, height, 0),
                        second.transform.position,
                };

                Vector3 startDir_second = bulletPath[1] - arrowAttcakEffect[1].transform.position;
                arrowAttcakEffect[1].transform.rotation = Quaternion.FromToRotation(Vector3.up, startDir_first);

                arrowAttcakEffect[1].transform.position = user.transform.position;
                arrowAttcakEffect[1].SetActive(true);
                arrowAttcakEffect[1].transform.DOPath(path, 1f, PathType.CatmullRom).
                    SetEase(Ease.InQuart).SetOptions(false).SetDelay(0.5f).OnComplete(
                    () =>
                    {
                        arrowAttcakEffect[1].SetActive(false);
                        arrowHitEffects[1].transform.position = second.transform.position;
                        arrowHitEffects[1].SetActive(true);
                        arrowHitEffects[1].GetComponent<PlayableDirector>().Play();

                        second.unit.HP -= Damage;
                        second.UnitPlatHurtAnimation();
                    });
            });

        TimerManager.instance.StartTimer(name + "ArrowDirChange", 0.6f + 1.5f * 0.5f,
            () =>
            {
                Vector3 startDir_first = first.transform.position - arrowAttcakEffect[0].transform.position;
                arrowAttcakEffect[0].transform.rotation = Quaternion.FromToRotation(Vector3.up, startDir_first);

                if (second == null) return;

                Vector3 startDir_second = second.transform.position - arrowAttcakEffect[1].transform.position;
                arrowAttcakEffect[1].transform.rotation = Quaternion.FromToRotation(Vector3.up, startDir_first);
            });

        TimerManager.instance.StartTimer(name + "EffectClose", 0.6f + 1.6f,
            () =>
            {
                for (int i = 0; i < arrowAttcakEffect.Count; i++)
                {
                    arrowAttcakEffect[i].SetActive(false);
                }

                for (int i = 0; i < arrowHitEffects.Count; i++)
                {
                    arrowHitEffects[i].SetActive(false);
                }

                GameManager.instance.GlobalLightControll(1f, 0.5f);
                user.transform.DOScale(UnitPlat.originScale, 0.5f);
            });

        user.unit.unitSkills[skillListIndex].SkillTime = 0.6f + 1.6f + 0.6f;
    }

    private void ArcShoot(UnitPlat user)
    {
        UnitPlat target = null;

        foreach (var unit in
            BattleSystem.instance.FriendlyUnitPlatsQueue.GetAllUnitPlat())
        {
            if (!unit.isDead && unit.unitData != FactorySystem.instance.EmptyFriendlyUnitData)
            {
                if (target == null)
                {
                    target = unit;
                    continue;
                }
            }
        }

        if (target == null)
        {
            user.unit.unitSkills[skillListIndex].SkillTime = 0.5f;
            return;
        }

        GameManager.instance.GlobalLightControll(0.5f, 0.5f);
        user.transform.DOScale(UnitPlat.originScale * attackScale, 0.5f);

        TimerManager.instance.StartTimer(name + "ArrowAttackEffetc", 0.6f,
            () =>
            {
                Vector3 startDir_first = target.transform.position - arrowAttcakEffect[0].transform.position;
                arrowAttcakEffect[0].transform.rotation = Quaternion.FromToRotation(Vector3.up, startDir_first);

                arrowAttcakEffect[0].transform.position = user.transform.position;
                arrowAttcakEffect[0].SetActive(true);
                arrowAttcakEffect[0].transform.DOMove(target.transform.position, 1f).
                    SetEase(Ease.InQuart).SetDelay(0.5f).OnComplete(
                    () =>
                    {
                        arrowAttcakEffect[0].SetActive(false);
                        arrowHitEffects[0].transform.position = target.transform.position;
                        arrowHitEffects[0].SetActive(true);
                        arrowHitEffects[0].GetComponent<PlayableDirector>().Play();

                        target.unit.HP -= Damage * 2;
                        target.UnitPlatHurtAnimation();
                    });
            });

        TimerManager.instance.StartTimer(name + "EffectClose", 0.6f + 1.6f,
            () =>
            {
                for (int i = 0; i < arrowAttcakEffect.Count; i++)
                {
                    arrowAttcakEffect[i].SetActive(false);
                }

                for (int i = 0; i < arrowHitEffects.Count; i++)
                {
                    arrowHitEffects[i].SetActive(false);
                }

                GameManager.instance.GlobalLightControll(1f, 0.5f);
                user.transform.DOScale(UnitPlat.originScale, 0.5f);
            });

        user.unit.unitSkills[skillListIndex].SkillTime = 0.6f + 1.6f + 0.6f;
    }
}
