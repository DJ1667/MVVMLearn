using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInfoViewModel : ViewModelBase
{
    public BindableProperty<string> InputVal = new BindableProperty<string>();
    public BindableProperty<float> SliderVal = new BindableProperty<float>();
}