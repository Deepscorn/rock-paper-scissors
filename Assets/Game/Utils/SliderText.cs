using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class SliderText : MonoBehaviour
{
	[SerializeField] 
	private Slider _slider;

	private Text _text;

	private void Awake()
	{
		_text = GetComponent<Text>();
	}

	private void OnEnable()
	{
		_text.text = GetFormattedSliderValue(_slider.value);
		_slider.onValueChanged.AddListener(OnSliderChanged);
	}

	private void OnDisable()
	{
		_slider.onValueChanged.RemoveListener(OnSliderChanged);
	}

	private void OnSliderChanged(float value)
	{
		_text.text = GetFormattedSliderValue(value);
	}

	private static string GetFormattedSliderValue(float value)
	{
		return $"{value:0.00}";
	}
}
