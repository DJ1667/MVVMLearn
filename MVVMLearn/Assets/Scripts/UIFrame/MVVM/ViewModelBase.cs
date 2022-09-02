public class ViewModelBase
{
    private bool _isInitialized;
    public float Timer { get; set; } //存活时间

    public ViewModelBase ParentViewModel { get; set; }

    public bool IsShowed { get; private set; }

    public bool IsShowInProgress { get; private set; }

    public bool IsHideInProgress { get; private set; }

    protected virtual void OnInitialize()
    {
    }

    public virtual void OnShowStart()
    {
        IsShowInProgress = true;

        //第一次显示时初始化
        if (!_isInitialized)
        {
            OnInitialize();
            _isInitialized = true;
        }
    }

    public virtual void OnShowFinish()
    {
        IsShowInProgress = false;
        IsShowed = true;
    }

    public virtual void OnHideStart()
    {
        IsHideInProgress = true;
    }

    public virtual void OnHideFinish()
    {
        IsHideInProgress = false;
        IsShowed = false;
    }

    public virtual void OnDestroy()
    {
    }
}