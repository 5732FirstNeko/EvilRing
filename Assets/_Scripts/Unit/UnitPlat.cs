using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class UnitPlat : MonoBehaviour
{
    public static Vector3 topDistance = new Vector3(0, 1.25f, 0);
    public static Vector3 bottomDistance = new Vector3(0, -1.25f, 0);
    public static Vector3 originScale = new Vector3(0.22f, 0.32f, 1f);

    public Unit unit;
    public UnitDataSo unitData;
    public UnitSite site;
    public SpriteRenderer iconSpriteRender;

    [SerializeField] private GameObject recoveryprefab;

    private GameObject recoveryEffect;
    public bool isDead 
    {
        get => _isDead; 
        set 
        { 
            _isDead = value;
            if (_isDead)
            {
                unit.DeadAction();
            }
        }
    }

    public Dictionary<UnitBuff, int> buffList = new Dictionary<UnitBuff, int>();

    private bool _isDead;

    public int costumvalue_first
    {
        get => _costumvalue_first;
        set 
        {
            _costumvalue_first = value;
            OnFirstValueChange?.Invoke(value);
        }
    }
    public int costumvalue_second;
    public int costumvlue_third;
    public int costumvlue_fourth;

    private int _costumvalue_first;
    public Action<int> OnFirstValueChange;

    private void Start()
    {
        iconSpriteRender = GetComponentsInChildren<SpriteRenderer>()[1];
        recoveryEffect = Instantiate(recoveryprefab, transform.position, Quaternion.identity);
        recoveryEffect.transform.SetParent(transform);
        recoveryEffect.SetActive(false);
    }

    public void UnitPlatInit(UnitDataSo unitData, UnitSite site)
    {
        unit = new Unit(unitData, this);
        isDead = false;
        this.unitData = unitData;
        this.site = site;

        costumvalue_first = 0;
        costumvalue_second = 0;
        costumvlue_third = 0;
        costumvlue_fourth = 0;

        unit.OnHPChange += OnHPChangeAction;
        OnFirstValueChange = null;

        GameObject hpBar = transform.Find("HPBar").gameObject;
        hpBar.SetActive(true);
        Text HPtext = hpBar.GetComponentInChildren<Text>();
        HPtext.text = unit.HP.ToString();
    }

    public void UnitPlatClear()
    {
        isDead = false;

        foreach (var buff in buffList)
        {
            buff.Key.UnBindBuff(this);
        }
        buffList.Clear();

        costumvalue_first = -1;
        costumvalue_second = -1;
        costumvlue_third = -1;
        costumvlue_fourth = -1;

        unit.OnHPChange = null;
        OnFirstValueChange = null;
    }

    public void OnHPChangeAction(int HPChange, UnitPlat user)
    {
        UnitDamageText damageText = GetComponentInChildren<UnitDamageText>();
        GameObject hpBar = transform.Find("HPBar").gameObject;
        hpBar.SetActive(true);
        Text HPtext = hpBar.GetComponentInChildren<Text>();
       
        if (HPChange > 0)
        {
            damageText.DamageFontDisplay(HPChange, false);
        }
        else if(HPChange < 0)
        {
            damageText.DamageFontDisplay(HPChange, true);
        }

        HPtext.transform.DOScale(new Vector3(1.5f, 1.5f, 1.5f), 0.15f).
            OnComplete(
            () => { HPtext.text = user.unit.HP.ToString(); HPtext.transform.DOScale(Vector3.one, 0.1f); });
    }

    public void DamageTextJump(string text, Color color, float animationTime = 1f)
    {
        UnitDamageText damageText = GetComponentInChildren<UnitDamageText>();
        damageText.FontDisPlay(text, color, animationTime);
    }

    public void AddBuff(UnitBuff buff)
    {
        bool isFirstBindBuff = true;
        UnitBuff addbuff = null;
        foreach (var buf in buffList)
        {
            if (buf.Key.Equals(buff))
            {
                isFirstBindBuff = false;
                addbuff = buf.Key;
            }
        }

        if (!isFirstBindBuff)
        {
            buffList[addbuff] += buff.ContinuousRound;
        }
        else
        {
            buffList.Add(buff, buff.ContinuousRound);
        }

        buff.BindBuff(this, isFirstBindBuff);
    }

    public int BuffConsumption(UnitBuff buff)
    {
        if (!buffList.ContainsKey(buff)) return 0;

        buffList[buff]--;
        if (buffList[buff] <= 0)
        {
            buff.UnBindBuff(this);
            buffList.Remove(buff);
            return 0;
        }

        return buffList[buff];
    }

    public void UnitPlatHurtAnimation(int frequency = 1, float frameStopTime = 0, Action callback = null)
    {
        Sequence sequence = DOTween.Sequence();

        Color originColor = iconSpriteRender.color;

        for (int i = 0; i < frequency; i++)
        {
            Tween colortoRed = iconSpriteRender.DOColor(Color.red, 0.1f).Pause().
                OnComplete(
                () =>
                {
                    callback?.Invoke();
                    CameraShakeManager.instance.TriggerHitStop(frameStopTime);
                });
            Tween colortowhite = iconSpriteRender.DOColor(originColor, 0.15f).Pause();

            Tween shark = transform.DOShakePosition(0.15f, 0.5f).Pause();

            sequence.Append(colortoRed);
            sequence.Join(shark);
            sequence.Append(colortowhite);
        }

        sequence.Play();
    }

    public void UnitPlatRecoveryAnimation()
    {
        recoveryEffect.SetActive(true);

        ParticleSystem director = recoveryEffect.GetComponentInChildren<ParticleSystem>();
        director.Play();

        recoveryEffect.transform.DOMove(transform.position, director.main.duration).OnComplete(() => 
        {
            recoveryEffect.SetActive(false);
        });
    }
}
