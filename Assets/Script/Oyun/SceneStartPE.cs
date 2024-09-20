using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class SceneStartPE : MonoBehaviour
{
    [SerializeField] private PostProcessVolume activeVolume;
    [SerializeField] private bool Reset;
    [SerializeField] private float LDScaleSpeed;
    [SerializeField] private float LDIntensitySpeed;
    [SerializeField] private float VIntensitySpeed;
    [SerializeField] private float VSmoothnessSpeed;
    [SerializeField] private float VRoundnessSpeed;

    const float START_INTENSITY_VALUE = -100.0f;

    const float GAME_INTENSITY_VALUE = 35.0f;
    const float GAME_V_INTENSITY_VALUE = 0.5f;
    const float GAME_V_SMOOTHNESS_VALUE = 0.25f;
    const float GAME_V_ROUNDNESS_VALUE = 0.65f;
    private float LDIntensityValue = -100.0f;
    private float LDScaleValue = 0.01f;
    private float VIntensityValue = 1.0f;
    private float VSmoothnessValue = 1.0f;
    private float VRoundnessValue = 1.0f;
    private float colorFilterValue = 0.0f;



    public void Toggle(bool value)
    {
        LensDistortion lensDistortion;
        ColorGrading colorGrading;
        Vignette vignette;

        activeVolume.profile.TryGetSettings(out lensDistortion);
        activeVolume.profile.TryGetSettings(out colorGrading);
        activeVolume.profile.TryGetSettings(out vignette);
        if (value)
        {
            lensDistortion.active = true;
        }
        //colorGrading.colorFilter.value = new Color(colorFilterValue, colorFilterValue, colorFilterValue);
        lensDistortion.intensity.value = START_INTENSITY_VALUE;
        lensDistortion.scale.value = 0.01f;
        vignette.intensity.value = 1;
        vignette.smoothness.value = 1;

        LDIntensityValue = -100.0f;
        LDScaleValue = 0.01f;
        VIntensityValue = 1.0f;
        VSmoothnessValue = 1.0f;
        colorFilterValue = 0.0f;
    }

    public void SetReset(bool isRestarted)
    {
        if (isRestarted)
        {
            LensDistortion lensDistortion;
            ColorGrading colorGrading;
            Vignette vignette;

            activeVolume.profile.TryGetSettings(out lensDistortion);
            activeVolume.profile.TryGetSettings(out colorGrading);
            activeVolume.profile.TryGetSettings(out vignette);

            lensDistortion.intensity.value = START_INTENSITY_VALUE;
            lensDistortion.scale.value = 0.01f;
            vignette.intensity.value = 1.0f;
            vignette.smoothness.value = 1.0f;
            isRestarted = false;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Toggle(true);
    }

    // Update is called once per frame
    void Update()
    {
        LensDistortion lensDistortion;
        ColorGrading colorGrading;
        Vignette vignette;

        activeVolume.profile.TryGetSettings(out lensDistortion);
        activeVolume.profile.TryGetSettings(out colorGrading);
        activeVolume.profile.TryGetSettings(out vignette);

        SetReset(Reset);

        LDIntensityValue += Time.deltaTime * LDScaleSpeed;
        if (LDIntensityValue >= GAME_INTENSITY_VALUE)
            LDIntensityValue = GAME_INTENSITY_VALUE;
        lensDistortion.intensity.value = LDIntensityValue;

        LDScaleValue += Time.deltaTime * LDIntensitySpeed;
        if (LDScaleValue >= 1)
            LDScaleValue = 1;
        lensDistortion.scale.value = LDScaleValue;

        VIntensityValue -= Time.deltaTime * VIntensitySpeed;
        if (VIntensityValue <= GAME_V_INTENSITY_VALUE)
            VIntensityValue = GAME_V_INTENSITY_VALUE;
        vignette.intensity.value = VIntensityValue;

        VSmoothnessValue -= Time.deltaTime * VSmoothnessSpeed;
        if (VSmoothnessValue <= GAME_V_SMOOTHNESS_VALUE)
            VSmoothnessValue = GAME_V_SMOOTHNESS_VALUE;
        vignette.smoothness.value = VSmoothnessValue;

        VRoundnessValue -= Time.deltaTime * VRoundnessSpeed;
        if (VRoundnessValue <= GAME_V_ROUNDNESS_VALUE)
            VRoundnessValue = GAME_V_ROUNDNESS_VALUE;
        vignette.roundness.value = VRoundnessValue;

        // colorGrading.colorFilter.value = new Color(colorFilterValue, colorFilterValue, colorFilterValue);
    }
}
