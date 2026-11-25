using System;
using System.Windows;
using System.Threading;
//using System.Windows.Forms;
using Gma.System.MouseKeyHook;

using WinForms = System.Windows.Forms;

namespace MyDict;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private IKeyboardMouseEvents? _globalHoock;
    private readonly TranslationPopup _popup = new();

    public MainWindow()
    {
        InitializeComponent();
        SubscribeGlobalHook();
    }

    private void SubscribeGlobalHook()
    {
        _globalHoock = Hook.GlobalEvents();
        _globalHoock.MouseDoubleClick += OnGlobalDoubleClick;
    }

    private void OnGlobalDoubleClick(object? sender, WinForms.MouseEventArgs e)
    {
        Thread.Sleep(50);
        try
        {
            string oldClipboard = "";
            if (Clipboard.ContainsText())
            {
                oldClipboard = Clipboard.GetText();
            }
            WinForms.SendKeys.SendWait("^c");
            Thread.Sleep(80);

            string selectedText = Clipboard.ContainsText() ? Clipboard.GetText() : "";

            if (!string.IsNullOrEmpty(oldClipboard))
            {
                Clipboard.SetText(oldClipboard);
            }
            if (string.IsNullOrWhiteSpace(selectedText))
            {
                return;
            }

            var cursorPos = CursorHelper.GetPositionInDips(this);

            Dispatcher.Invoke(
                () =>
                {
                    _popup.ShowText(selectedText.Trim(), cursorPos);
                }
            );
        }
        catch 
        {
        }
    }
    protected override void OnClosed(EventArgs e)
    {
        // 释放全局钩子
        _globalHoock?.Dispose();
        _popup.Close();

        base.OnClosed(e);
    }
}