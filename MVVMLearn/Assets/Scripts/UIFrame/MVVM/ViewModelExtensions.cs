public static class ViewModelExtensions
{
    /// <summary>
    /// 向上查找第一个符合的ViewModel
    /// </summary>
    /// <param name="origin"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T GetAncestors<T>(this ViewModelBase origin) where T : ViewModelBase
    {
        if (origin == null) return null;

        var parentViewModel = origin.ParentViewModel;
        while (parentViewModel != null)
        {
            var targetViewModel = parentViewModel as T;
            if (targetViewModel != null)
                return targetViewModel;

            parentViewModel = parentViewModel.ParentViewModel;
        }

        return null;
    }
}