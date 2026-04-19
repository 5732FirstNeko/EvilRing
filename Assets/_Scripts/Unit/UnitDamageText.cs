using System.Collections;
using System.Collections.Generic;
using System.Security.Principal;
using DG.Tweening;
using UnityEngine;

public class UnitDamageText : MonoBehaviour
{
    private TextMesh textMesh;

    [SerializeField] private Color recoveryFontColor;
    [SerializeField] private Color hurtFontColor;

    [SerializeField] private int maxfontSize;
    [SerializeField] private AnimationCurve fontSizeCurve;
    [SerializeField] private float maxMoveHeight;
    [SerializeField] private AnimationCurve heightCurve;
    [SerializeField] private AnimationCurve fadeCurve;

    private Vector3 startLocalPos;
    private int startFontSize;

    private void Awake()
    {
        textMesh = GetComponent<TextMesh>();
        startLocalPos = transform.position;
        startFontSize = textMesh.fontSize;
    }

    public void DamageFontDisplay(int damage, bool isRecovery, float animationTime = 1f)
    {
        transform.DOKill();

        transform.localPosition = startLocalPos;
        textMesh.fontSize = startFontSize;
        textMesh.color = isRecovery ? recoveryFontColor : hurtFontColor;
        textMesh.text = Mathf.Abs(damage).ToString();

        float animProgress = 0;
        Tweener progressTweener = DOTween.To(
            () => animProgress,
            x => animProgress = x,
            1,
            animationTime
        );

        progressTweener.OnUpdate(() =>
        {
            transform.position = new Vector3(
                startLocalPos.x,
                startLocalPos.y + heightCurve.Evaluate(animProgress) * maxMoveHeight,
                startLocalPos.z
            );

            textMesh.fontSize = Mathf.RoundToInt(fontSizeCurve.Evaluate(animProgress) * maxfontSize);

            Color color = textMesh.color;
            color.a = fadeCurve.Evaluate(animProgress);
            textMesh.color = color;
        });
    }

    public void FontDisPlay(string text, Color color, float animationTime = 1f)
    {
        transform.DOKill();

        transform.localPosition = startLocalPos;
        textMesh.fontSize = startFontSize;
        textMesh.color = color;
        textMesh.text = text;

        float animProgress = 0;
        Tweener progressTweener = DOTween.To(
            () => animProgress,
            x => animProgress = x,
            1,
            animationTime
        );

        progressTweener.OnUpdate(() =>
        {
            textMesh.fontSize = Mathf.RoundToInt(fontSizeCurve.Evaluate(animProgress) * maxfontSize);

            Color color = textMesh.color;
            color.a = fadeCurve.Evaluate(animProgress);
            textMesh.color = color;
        });
    }
}
