using System.Collections;
using UnityEngine;
using UnityEngine.UI; 

public class HealthBar : MonoBehaviour
{
    
    private Slider slider;
    private Sprite gunImage;


    public void SetMaxValue(float health)
    {
        slider = this.gameObject.GetComponent<Slider>();
        slider.maxValue = health;
        slider.minValue = health * 0.25f;
        slider.value = health;
    }
    public void SetHealth(float health, bool hard_set=false)
    {
        //slider.value = health;
        if (hard_set)
        {
            slider.value = health;
            return;
        }
        StartCoroutine(SmoothingValue(health));
    }

    IEnumerator SmoothingValue(float health)
    {
        slider = this.gameObject.GetComponent<Slider>();
        slider.value = Mathf.Lerp(slider.value, health, 2 * Time.deltaTime);

        while (Mathf.Abs(slider.value) != Mathf.Abs(health)) // This is your target size of object.
        {
            slider.value = Mathf.Lerp(slider.value, health, 1 * Time.deltaTime);
            yield return null;
        }

        slider.value = health;
    }

}
