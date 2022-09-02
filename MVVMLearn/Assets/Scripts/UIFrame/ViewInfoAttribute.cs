using System;

[AttributeUsage(AttributeTargets.Class)]
public class ViewInfoAttribute : Attribute
{
    private UILayer _layer;
    private UILife _life;
    private string _path;
    private string _name;

    public UILayer Layer
    {
        get => _layer;
    }

    public UILife Life
    {
        get => _life;
    }

    public string Path
    {
        get => _path;
    }

    public string Name
    {
        get => _name;
    }

    public ViewInfoAttribute(UILayer layer, UILife life, string path = "")
    {
        _layer = layer;
        _life = life;
        _path = path;
    }

    public void SetName(string name, string path)
    {
        _name = name;

        //如果没有定义路径则使用传入的默认路径
        if (string.IsNullOrEmpty(_path))
            _path = path;
    }
}