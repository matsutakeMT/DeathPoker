using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerCount : MonoBehaviour
{
    [SerializeField] private int initializeCount = 5;
    [Space(5)]
    [SerializeField] private TMP_Text countView;
    [SerializeField] private Slider countSlider;

    public int Count { get { return (int)countSlider.value; } }

    private void Start()
    {
        countView.text = initializeCount.ToString();
        countSlider.value = initializeCount;
        countSlider.onValueChanged.AddListener(OnSliderChanged);
    }

    private void OnSliderChanged(float value)
    {
        Debug.Log("changed");
        countView.text = ((int)value).ToString();
    }
}
