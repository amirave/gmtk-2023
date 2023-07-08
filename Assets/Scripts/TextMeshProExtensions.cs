using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public static class TextMeshProExtensions
{
    public static UniTask SetSmoothValue(this TextMeshProUGUI textMeshPro, long curValue, long newValue, float time,
        Func<long, string> formatter, CancellationToken ct = default)
    {
        return ApplySmoothValue(textMeshPro, curValue, newValue, time, formatter, ct);
    }

    private static async UniTask ApplySmoothValue(TextMeshProUGUI label, long curValue, long newValue, float time,
        Func<long, string> formatter, CancellationToken ct)
    {
        var startTime = Time.time;
        var progress = 0.0d;

        while (progress < 1)
        {
            var coins = (long)(curValue + (newValue - curValue) * progress);
            label.text = formatter.Invoke(coins);

            progress = (Time.time - startTime) / time;
            await UniTask.Yield();
            if (ct.IsCancellationRequested)
            {
                break;
            }
        }

        label.text = formatter.Invoke(newValue);
    }
}