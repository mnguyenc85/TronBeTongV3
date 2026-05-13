using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace NMWPFControls.Controls;

/// <summary>
/// Interaction logic for CtrlMessages.xaml
/// </summary>
public partial class CtrlMessages : UserControl
{
    #region Action messages
    public ObservableCollection<MessageDO> Messages { get; set; } = new();
    private readonly DispatcherTimer _aniTmr = new();
    private TimeSpan _keepMsgTime;
    #endregion

    public CtrlMessages()
    {
        InitializeComponent();
        DataContext = this;

        _keepMsgTime = TimeSpan.FromSeconds(30);

        _aniTmr.Interval = _keepMsgTime;
        _aniTmr.Tick += _aniTmr_Tick;
    }

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        _aniTmr.Start();
    }

    public void End()
    {
        _aniTmr.Stop();
    }

    public void AddMessage(string s)
    {
        Messages.Add(new MessageDO(s));
    }

    private void _aniTmr_Tick(object? sender, EventArgs e)
    {
        int i = 0;
        while (i < Messages.Count)
        {
            if (DateTime.Now - Messages[i].T > _keepMsgTime)
            {
                Messages.RemoveAt(i);
            }
            else break;
        }
    }

    private void BdrMessage_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
        {
            Messages.Clear();
        }
        else
        {
            if (sender is FrameworkElement fe)
            {
                if (fe.DataContext is MessageDO m)
                {
                    Messages.Remove(m);
                }
            }
        }
    }

    /// <summary>
    /// t (s)
    /// </summary>
    /// <param name="t"></param>
    public void SetKeepTime(double t)
    {

    }
}
