using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ViewInfo(UILayer.Menu, UILife.Permanent)]
public class UIInfoView : ViewBase<UIInfoViewModel>
{
    public InputField _InputField;
    public Slider _Slider;
    public Text _txtInputField;
    public Text _txtSlider;

    protected override void OnInitialized()
    {
        base.OnInitialized();

        Binder.Add<string>("InputVal", OnChangedInputVal);
        Binder.Add<float>("SliderVal", OnChangedSliderVal);

        _InputField.onValueChanged.AddListener(OnInputFieldChanged);
        _Slider.onValueChanged.AddListener(OnSliderChanged);
    }

    private void OnChangedInputVal(string oldVal, string newVal)
    {
        _InputField.text = newVal;
        _txtInputField.text = newVal;
    }

    private void OnChangedSliderVal(float oldVal, float newVal)
    {
        _Slider.value = newVal;
        _txtSlider.text = newVal.ToString();
    }

    private void OnInputFieldChanged(string str)
    {
        Context.InputVal.Value = str;
    }

    private void OnSliderChanged(float val)
    {
        Context.SliderVal.Value = val;
    }

    public void BtnOnClick_InputValChange()
    {
        UIManager.Instance.OpenView<UIModifyDataView>(() => { UIManager.Instance.GetViewModel<UIModifyDataViewModel>().isSlider = false; });
    }

    public void BtnOnClick_SliderValChange()
    {
        UIManager.Instance.OpenView<UIModifyDataView>(() => { UIManager.Instance.GetViewModel<UIModifyDataViewModel>().isSlider = true; });
    }
}