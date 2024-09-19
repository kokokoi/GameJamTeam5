
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
 
public class OverridePostProcess : MonoBehaviour
{
    [SerializeField]
    private PostProcessVolume postProcessVolume;
    private PostProcessProfile postProcessProfile;

    // Start is called before the first frame update
    void Start()
    {
        //�@���̃{�����[���̂ݓK�p
        postProcessProfile = postProcessVolume.profile;
        //�@�����v���t�@�C�����g�p�������̃{�����[���ɂ��K�p
        //postProcessProfile = postProcessVolume.sharedProfile;

        Bloom bloom = postProcessProfile.GetSetting<Bloom>();
        bloom.enabled.Override(true);
        bloom.intensity.Override(20f);

        //�@MotionBlur�G�t�F�N�g�̒ǉ�
        MotionBlur motionBlur = postProcessProfile.AddSettings<MotionBlur>();
        motionBlur.enabled.Override(true);
        motionBlur.shutterAngle.Override(360f);

        //�@�G�t�F�N�g�����݂��邩�ǂ����H
        bool hasVignetteEffect = postProcessProfile.HasSettings<Vignette>();
        Debug.Log("hasVignetteEffect: " + hasVignetteEffect);

        //�@�G�t�F�N�g�����邩�ǂ����̔��f�Ǝ擾�𓯎��ɍs��
        Grain grain;
        bool hasGrainEffect = postProcessProfile.TryGetSettings<Grain>(out grain);

        if (hasGrainEffect)
        {
            grain.enabled.Override(true);
            grain.intensity.Override(1f);
        }
        //�@�G�t�F�N�g���폜
        postProcessProfile.RemoveSettings<Vignette>();
    }

    void OnDestroy()
    {
        //�@�쐬�����v���t�@�C���̍폜
        RuntimeUtilities.DestroyProfile(postProcessProfile, true);
    }
}