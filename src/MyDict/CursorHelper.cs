using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;

namespace MyDict;

public static class CursorHelper
{
    [DllImport("user32.dll")]
    private static extern bool GetCursorPos(out POINT lpPoint);
    [StructLayout(LayoutKind.Sequential)]
    private struct POINT
    {
        public int X;
        public int Y;
    }

    public static Point GetPositionInDips(Visual referenceVisual)
    {
        GetCursorPos(out var p);

        var source = PresentationSource.FromVisual(referenceVisual);
        if (source?.CompositionTarget != null)
        {
            var matrix = source.CompositionTarget.TransformFromDevice;
            return matrix.Transform(new Point(p.X, p.Y));
        }
        return new Point(p.X, p.Y);
    }
}