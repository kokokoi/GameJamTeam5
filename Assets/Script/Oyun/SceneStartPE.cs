using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class SceneStartPE : MonoBehaviour
{
    [SerializeField] private PostProcessVolume activeVolume;
    const float START_INTENSITY_VALUE = -100.0f;
    const float GAME_INTENSITY_VALUE = 35.0f;
    private float intensityValue = -100.0f;
    public void Toggle(bool value)
    {
        LensDistortion lensDistortion;
        activeVolume.profile.TryGetSettings(out lensDistortion);
        if (value)
        {
            lensDistortion.active = true;
        }
        lensDistortion.intensity.value = START_INTENSITY_VALUE;
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
        activeVolume.profile.TryGetSettings(out lensDistortion);
 
        intensityValue += Time.deltaTime*5.5f;
        if (intensityValue >= GAME_INTENSITY_VALUE)
            intensityValue = GAME_INTENSITY_VALUE;

        lensDistortion.intensity.value = intensityValue;

    }
}
