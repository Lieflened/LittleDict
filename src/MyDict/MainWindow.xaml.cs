using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System;
using System.Threading;
using System.Windows.Forms;
using Gma.System.MouseKeyHook;

namespace MyDict;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private IKeyboardMouseEvents _globalHoock;
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
    private void OnGlobalDoubleClick(object? sender, MouseEventArgs e)
    {
        Thread.Sleep(50);
        try
        {
            string oldClipboard = "";
            if (System.Windows.Clipboard.ContainsText())
            {
                oldClipboard = System.Windows.Clipboard.GetText();
            }
            SendKeys.SendWait("^c");
            Thread.Sleep(80);

            string selectedText = System.Windows.Clipboard.ContainsText() ? System.Windows.Clipboard.GetText() : "";

            if (!string.IsNullOrEmpty(oldClipboard))
            {
                System.Windows.Clipboard.SetText(oldClipboard);
            }
            if (string.IsNullOrWhiteSpace(selectedText))
            {
                return;
            }
            Dispatcher.Invoke(
                () =>
                {
                    DisplayTextBlock.Text = selectedText.Trim();
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
        _globalHoock.Dispose();

        base.OnClosed(e);
    }
}