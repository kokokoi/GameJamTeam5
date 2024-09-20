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
        //�@���̃{�����[���̂ݓK�p
        postProcessProfile = postProcessVolume.profile;
        //�@�����v���t�@�C�����g�p�������̃{�����[���ɂ��K�p
        //postProcessProfile = postProcessVolume.sharedProfile;

        //Bloom bloom = postProcessProfile.GetSetting<Bloom>();
        //bloom.enabled.Override(true);
        //bloom.intensity.Override(20f);

        ////�@MotionBlur�G�t�F�N�g�̒ǉ�
        //MotionBlur motionBlur = postProcessProfile.AddSettings<MotionBlur>();
        //motionBlur.enabled.Override(true);
        //motionBlur.shutterAngle.Override(360f);

        ////�@�G�t�F�N�g�����݂��邩�ǂ����H
        //bool hasVignetteEffect = postProcessProfile.HasSettings<Vignette>();
        //Debug.Log("hasVignetteEffect: " + hasVignetteEffect);

        ////�@�G�t�F�N�g�����邩�ǂ����̔��f�Ǝ擾�𓯎��ɍs��
        //Grain grain;
        //bool hasGrainEffect = postProcessProfile.TryGetSettings<Grain>(out grain);

        //if (hasGrainEffect)
        //{
        //    grain.enabled.Override(true);
        //    grain.intensity.Override(1f);
        //}
        ////�@�G�t�F�N�g���폜
        //postProcessProfile.RemoveSettings<Vignette>();

        bool hasLensDistortionEffect = postProcessProfile.TryGetSettings(out lensDistortion);
        bool havignetteEffect = postProcessProfile.TryGetSettings(out vignette);

        // �ړI�̍ŏI�l
        float targetValue = 35;
        float targetVignetteValue = 0.4f;

        currentValue = -90;
        lensDistortion.intensity.value = currentValue;

        vignette.intensity.value = 1;

        // DOTween���g���Đ��l��OutElastic�ŕω�������
        DOTween.To(() => currentValue, x => currentValue = x, targetValue, duration)
            .SetEase(Ease.OutElastic,1.0f)
            .SetDelay(0.3f)
            .OnUpdate(() =>
            {
                //Debug.Log("Current value: " + currentValue);
                // ������myValue��UI��Q�[���̑��̕����ɔ��f������
                // LensDistortion�G�t�F�N�g�̎擾
                if (hasLensDistortionEffect)
                {
                    // �Ⴆ�΁Aintensity�̒l��ς���
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
        //�@�쐬�����v���t�@�C���̍폜
        //RuntimeUtilities.DestroyProfile(postProcessProfile, true);
    }
}
