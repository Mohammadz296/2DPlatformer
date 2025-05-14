using UnityEngine;
using UnityEngine.UI;
public class SliderScript : MonoBehaviour
{
    Slider slider;
    [SerializeField] Gradient gradient;
   Image image;
    private void Awake()
    {
        slider = GetComponent<Slider>();
        image = transform.Find("Fill").GetComponent<Image>();
    }
    public void SetValue(float health)
    {
        slider.value = health;
        image.color = gradient.Evaluate(slider.normalizedValue);
    }
    public void SetMaxValue(float max)
    {
        slider.maxValue = max;
        slider.value = max;
        image.color = gradient.Evaluate(1f);
    }


}
