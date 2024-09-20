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

        // 目的の最終値
        float targetValue = 100f;

        // DOTweenを使って数値をOutElasticで変化させる
        DOTween.To(() => currentValue, x => currentValue = x, targetValue, 2f)
            .SetEase(Ease.OutElastic)
            .OnUpdate(() =>
            {
                Debug.Log("Current value: " + currentValue);
                // ここでmyValueをUIやゲームの他の部分に反映させる
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
