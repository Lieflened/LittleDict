using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace MyDict;

public partial class TranslationPopup : Window
{
    private const double HideDelayMs = 1500;
    private const double FadeOutMs = 250;

    private readonly DispatcherTimer _hideTimer;
    private DateTime _lastMouseInPopupTime;
    private bool _isFadingOut;

    public TranslationPopup()
    {
        InitializeComponent();
        _hideTimer = new DispatcherTimer{Interval = TimeSpan.FromMilliseconds(200)};
        _hideTimer.Tick += HideTimer_Tick;
    }

    public void ShowText(string sourceText, Point cursorPositionInDips)
    {
        BeginAnimation(OpacityProperty, null);
        Opacity = 1;
        _isFadingOut = false;

        SourceTextBlock.Text = sourceText;
        TranslationTextBlock.Text = "";
        if (!IsVisible)
        {
            Left = -10000;
            Top = -10000;
            Show();
        }

        UpdateLayout();

        var workArea = SystemParameters.WorkArea;
        double left = cursorPositionInDips.X + 16;
        double top = cursorPositionInDips.Y + 16;

        if(left + ActualWidth > workArea.Right)
        {
            left = cursorPositionInDips.X - ActualWidth - 16;
        }
        if(top + ActualHeight > workArea.Bottom)
        {
            top = cursorPositionInDips.Y - ActualHeight - 16;
        }

        if(left < workArea.Left)
        {
            left = workArea.Left;
        }
        if(top < workArea.Top)
        {
            top = workArea.Top;
        }

        Left = left;
        Top = top;

        _lastMouseInPopupTime = DateTime.Now;
        _hideTimer.Start();
    }

    public void SetStatus(string status)
    {
        TranslationTextBlock.Text = status;
    }
    public void SetTranslation(string translation)
    {
        TranslationTextBlock.Text = translation;
        UpdateLayout();
        ConstrainToWorkArea();
    }

    private void ConstrainToWorkArea()
    {
        var workArea = SystemParameters.WorkArea;
        if (Left + ActualWidth > workArea.Right)
        {
            Left = workArea.Right - ActualWidth;
        }
        if (Top + ActualHeight > workArea.Bottom)
        {
            Top = workArea.Bottom - ActualHeight;
        }
        Left = Left < workArea.Left ? workArea.Left : Left;
        Top = Top < workArea.Top ? workArea.Top : Top;
    }

    private void HideTimer_Tick(object? sender, EventArgs e)
    {
        if (!IsVisible)
        {
            _hideTimer.Stop();
            return;
        }

        if (IsMouseOver)
        {
            _lastMouseInPopupTime = DateTime.Now;

            if (_isFadingOut)
            {
                BeginAnimation(OpacityProperty, null);
                Opacity = 1;
                _isFadingOut = false;
            }
            return;
        }

        if (_isFadingOut)
        {
            return;
        }

        if ((DateTime.Now - _lastMouseInPopupTime).TotalMilliseconds > HideDelayMs)
        {
            StartFadeOut();
        }
    }
    private void StartFadeOut()
    {
        _isFadingOut = true;
        _hideTimer.Stop();

        var fade = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(FadeOutMs));
        fade.Completed += (_, _) =>
        {
            Hide();
            BeginAnimation(OpacityProperty, null);
            Opacity = 1;
            _isFadingOut = false;
        };
        BeginAnimation(OpacityProperty, fade);
    }

    protected override void OnClosed(EventArgs e)
    {
        _hideTimer.Stop();
        base.OnClosed(e);
    }
}
