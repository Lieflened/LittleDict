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
    private IKeyboardMouseEvents? _globalHook;
    private readonly TranslationPopup _popup = new();
    private readonly TranslationService _translator = new();
    public MainWindow()
    {
        InitializeComponent();
        SubscribeGlobalHook();
    }

    private void SubscribeGlobalHook()
    {
        _globalHook = Hook.GlobalEvents();
        _globalHook.MouseDoubleClick += OnGlobalDoubleClick;
    }

    private async void OnGlobalDoubleClick(object? sender, WinForms.MouseEventArgs e)
    {
        Thread.Sleep(50);
        string selectedText = "";
        try
        {
            string oldClipboard = "";
            if (Clipboard.ContainsText())
            {
                oldClipboard = Clipboard.GetText();
            }
            WinForms.SendKeys.SendWait("^c");
            Thread.Sleep(80);

            selectedText = Clipboard.ContainsText() ? Clipboard.GetText() : "";

            if (!string.IsNullOrEmpty(oldClipboard))
            {
                Clipboard.SetText(oldClipboard);
            }
        }
        catch
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(selectedText))
        {
            return;
        }

        var cursorPos = CursorHelper.GetPositionInDips(this);

        selectedText = selectedText.Trim();

        Dispatcher.Invoke(
            () =>
            {
                _popup.ShowText(selectedText, cursorPos);
                _popup.SetStatus("翻译中...");
            }
        );

        var translation = await _translator.TranslateAsync(selectedText);

        Dispatcher.Invoke(
            () =>
            {
                _popup.SetTranslation(translation);
            }
        );
    }
    protected override void OnClosed(EventArgs e)
    {
        // 释放全局钩子
        _globalHook?.Dispose();
        _popup.Close();

        base.OnClosed(e);
    }
}