using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ViewInfo(UILayer.PopUpWindow, UILife.S30)]
public class UIModifyDataView : ViewBase<UIModifyDataViewModel>
{
    public InputField _InputField;
    public Slider _Slider;
    public Text _txt;

    protected override void OnInitialized()
    {
        base.OnInitialized();

        _InputField.onValueChanged.AddListener(OnInputFieldChanged);
        _Slider.onValueChanged.AddListener(OnSliderChanged);
    }

    protected override void OnShowStart(bool immediate)
    {
        base.OnShowStart(immediate);

        _Slider.gameObject.SetActive(Context.isSlider);
        _InputField.gameObject.SetActive(!Context.isSlider);

        var viewModel = UIManager.Instance.GetViewModel<UIInfoViewModel>();
        if (Context.isSlider)
        {
            _txt.text = viewModel.SliderVal.Value.ToString();
            _Slider.value = viewModel.SliderVal.Value;
        }
        else
        {
            _txt.text = viewModel.InputVal.Value;
            _InputField.text = viewModel.InputVal.Value;
        }
    }

    private void OnInputFieldChanged(string str)
    {
        _txt.text = str;

        var viewModel = UIManager.Instance.GetViewModel<UIInfoViewModel>();
        viewModel.InputVal.Value = str;
    }

    private void OnSliderChanged(float val)
    {
        _txt.text = val.ToString();
        var viewModel = UIManager.Instance.GetViewModel<UIInfoViewModel>();
        viewModel.SliderVal.Value = val;
    }

    public void OnBtnClick_Close()
    {
        UIManager.Instance.CloseView<UIModifyDataView>();
    }
}