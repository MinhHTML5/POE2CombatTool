using System.Drawing;
using System.Windows.Forms;

namespace POE2RuleTool.Forms;

public sealed class PointPickerForm : Form
{
    private readonly List<Point> _points = new();

    public PointPickerForm()
    {
        FormBorderStyle = FormBorderStyle.None;
        StartPosition = FormStartPosition.Manual;
        Bounds = Screen.PrimaryScreen?.Bounds ?? SystemInformation.VirtualScreen;
        TopMost = true;
        Opacity = 0.28;
        BackColor = Color.Black;
        Cursor = Cursors.Cross;
        DoubleBuffered = true;
        KeyPreview = true;
        ShowInTaskbar = false;
    }

    public IReadOnlyList<Point> PickedPoints => _points;

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);

        if (e.Button == MouseButtons.Left)
        {
            _points.Add(PointToScreen(e.Location));
            Invalidate();
        }
        else if (e.Button == MouseButtons.Right)
        {
            FinishPicking();
        }
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);

        if (e.KeyCode is Keys.Escape or Keys.Enter)
        {
            FinishPicking();
        }
        else if (e.KeyCode == Keys.Back && _points.Count > 0)
        {
            _points.RemoveAt(_points.Count - 1);
            Invalidate();
        }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        using var pen = new Pen(Color.White, 2);
        using var brush = new SolidBrush(Color.Yellow);
        foreach (Point screenPoint in _points)
        {
            Point point = PointToClient(screenPoint);
            e.Graphics.DrawLine(pen, point.X - 9, point.Y, point.X + 9, point.Y);
            e.Graphics.DrawLine(pen, point.X, point.Y - 9, point.X, point.Y + 9);
            e.Graphics.FillEllipse(brush, point.X - 3, point.Y - 3, 6, 6);
        }
    }

    private void FinishPicking()
    {
        DialogResult = _points.Count > 0 ? DialogResult.OK : DialogResult.Cancel;
        Close();
    }
}
