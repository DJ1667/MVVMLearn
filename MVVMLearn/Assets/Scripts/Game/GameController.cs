using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private void Start()
    {
        UIManager.Instance.OpenView<UIInfoView>();
    }
}