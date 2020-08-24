using System;
using System.Windows.Controls;

namespace IFBMVVM.Interface.UI
{
    public interface IMainWindow
    {
        Image OpenButtonControl { get; set; }
        ListBox ListBoxControl { get; set; }
    }
}
