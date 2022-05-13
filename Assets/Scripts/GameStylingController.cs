using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
public class GameStylingController : MonoBehaviour
{
    public PostProcessVolume ppVolume;
    private Bloom _Bloom;
    private ChromaticAberration _ChromaticAberration;

   [SerializeField]private float NormalBloomVal;
   [SerializeField]private float Normal_ChromaticAberrationVal;

   [HideInInspector]public static GameStylingController Instance;

    void Start()
    {
        Instance = this;

        ppVolume.profile.TryGetSettings(out _Bloom);
        ppVolume.profile.TryGetSettings(out _ChromaticAberration);

        _Bloom.intensity.value = NormalBloomVal;
    }

    public void EffectChromaticAb(float x)
    {
        _ChromaticAberration.intensity.value += x;
        if (PlayerController.IsDead)
            return;
        StartCoroutine(RemoveEffectChromaticAb());
    }


    public void EffectBloom(float x)
    {
        float val = (float)_Bloom.intensity.value;
        if (val + x < MaxValues.MAX_BLOOM_EFFECT || PlayerController.IsDead)
            _Bloom.intensity.value += x;
        else
            _Bloom.intensity.value = MaxValues.MAX_BLOOM_EFFECT;
        if (PlayerController.IsDead)
            return;       
        StartCoroutine(RemoveBloomEffect());
    }



        //GSC.EffectBloom(bulletPower * BloomEff >= MaxValues.MAX_BLOOM_EFFECT ? MaxValues.MAX_BLOOM_EFFECT : bulletPower * BloomEff);
    //
    IEnumerator RemoveEffectChromaticAb()
    {
        float _time = 0.0f;
        while (_time < 5 && _ChromaticAberration.intensity.value > Normal_ChromaticAberrationVal)
        {
            _ChromaticAberration.intensity.value -= Time.deltaTime / 2;
            _time += Time.deltaTime;
            yield return null;
        }
        _ChromaticAberration.intensity.value = Normal_ChromaticAberrationVal;
    }


    IEnumerator RemoveBloomEffect()
    {
        float _time = 0.0f;
        while (_time < 5 && _Bloom.intensity.value > NormalBloomVal +0.5f)
        {
            
            _Bloom.intensity.value -= Time.deltaTime * 20;
            _time += Time.deltaTime ;
            yield return null;
        }
        _Bloom.intensity.value = NormalBloomVal;
    }

    
    
}
