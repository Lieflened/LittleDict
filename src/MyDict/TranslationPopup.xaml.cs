using System.Data;
using System.Data.Common;
using System.Windows;

namespace MyDict;

public partial class TranslationPopup : Window
{
    public TranslationPopup()
    {
        InitializeComponent();
    }

    public void ShowText(string sourceText, Point cursorPositionInDips)
    {
        // ContentTextBlock.Text = sourceText;
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
    }

    public void SetStatus(string status)
    {
        TranslationTextBlock.Text = status;
    }
    public void SetTranslation(string translation)
    {
        TranslationTextBlock.Text = translation;
        UpdateLayout();
    }
}
