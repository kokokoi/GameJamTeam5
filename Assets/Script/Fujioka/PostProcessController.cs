using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessController : MonoBehaviour
{
    [SerializeField]
    private PostProcessVolume postProcessVolume;
    private PostProcessProfile postProcessProfile;

    float currentValue;
    float vignetteValue;

    public float duration;

    private LensDistortion lensDistortion;
    private Vignette vignette;

    // Start is called before the first frame update
    void Start()
    {
        //　このボリュームのみ適用
        postProcessProfile = postProcessVolume.profile;
        //　同じプロファイルを使用した他のボリュームにも適用
        //postProcessProfile = postProcessVolume.sharedProfile;

        //Bloom bloom = postProcessProfile.GetSetting<Bloom>();
        //bloom.enabled.Override(true);
        //bloom.intensity.Override(20f);

        ////　MotionBlurエフェクトの追加
        //MotionBlur motionBlur = postProcessProfile.AddSettings<MotionBlur>();
        //motionBlur.enabled.Override(true);
        //motionBlur.shutterAngle.Override(360f);

        ////　エフェクトが存在するかどうか？
        //bool hasVignetteEffect = postProcessProfile.HasSettings<Vignette>();
        //Debug.Log("hasVignetteEffect: " + hasVignetteEffect);

        ////　エフェクトがあるかどうかの判断と取得を同時に行う
        //Grain grain;
        //bool hasGrainEffect = postProcessProfile.TryGetSettings<Grain>(out grain);

        //if (hasGrainEffect)
        //{
        //    grain.enabled.Override(true);
        //    grain.intensity.Override(1f);
        //}
        ////　エフェクトを削除
        //postProcessProfile.RemoveSettings<Vignette>();

        bool hasLensDistortionEffect = postProcessProfile.TryGetSettings(out lensDistortion);
        bool havignetteEffect = postProcessProfile.TryGetSettings(out vignette);

        // 目的の最終値
        float targetValue = 35;
        float targetVignetteValue = 0.4f;

        currentValue = -90;
        lensDistortion.intensity.value = currentValue;

        vignette.intensity.value = 1;

        // DOTweenを使って数値をOutElasticで変化させる
        DOTween.To(() => currentValue, x => currentValue = x, targetValue, duration)
            .SetEase(Ease.OutElastic,1.0f)
            .SetDelay(0.3f)
            .OnUpdate(() =>
            {
                //Debug.Log("Current value: " + currentValue);
                // ここでmyValueをUIやゲームの他の部分に反映させる
                // LensDistortionエフェクトの取得
                if (hasLensDistortionEffect)
                {
                    // 例えば、intensityの値を変える
                    lensDistortion.intensity.value = currentValue;
                }
            });
        DOTween.To(() => vignetteValue, x => vignetteValue = x, targetVignetteValue, duration)
    .SetEase(Ease.OutCubic)
                .SetDelay(0.3f)
    .OnUpdate(() =>
    {
        if (havignetteEffect)
        {
            vignette.intensity.value = vignetteValue;

        }
    });
    }

    private void Update()
    {

    }

    void OnDestroy()
    {
        //　作成したプロファイルの削除
        //RuntimeUtilities.DestroyProfile(postProcessProfile, true);
    }
}
