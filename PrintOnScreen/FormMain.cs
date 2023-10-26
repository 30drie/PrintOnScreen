using Gma.System.MouseKeyHook;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace PrintOnScreen;

public partial class FormMain : Form
{

	[DllImport("User32.dll")]
	public static extern IntPtr GetDC(IntPtr hwnd);

	[DllImport("User32.dll")]
	public static extern void ReleaseDC(IntPtr dc);

	public FormMain()
	{
		InitializeComponent();
		InitializeHook();
		WindowState = FormWindowState.Minimized;
	}
	
	private IKeyboardMouseEvents _globalHook;
	private ControlEnum _pressedKey;
	private Point _location;

	private void button1_Click(object sender, EventArgs e)
	{
		Refresh();
	}

	private void InitializeHook()
	{
		var assignment = new Dictionary<Combination, Action>
		{
			{ Combination.FromString("Control+D1"), () => { HandleHotkey(ControlEnum.Ctrl1); } },
			{ Combination.FromString("Control+D2"), () => { HandleHotkey(ControlEnum.Ctrl2); } },
			{ Combination.FromString("Control+D3"), () => { HandleHotkey(ControlEnum.Ctrl3); } },
			{ Combination.FromString("Control+D4"), () => { HandleHotkey(ControlEnum.Ctrl4); } },
			{ Combination.FromString("Control+D5"), () => { HandleHotkey(ControlEnum.Ctrl5); } },
			{ Combination.FromString("Control+D6"), () => { HandleHotkey(ControlEnum.Ctrl6); } },
			{ Combination.FromString("Control+D7"), () => { HandleHotkey(ControlEnum.Ctrl7); } },
			{ Combination.FromString("Control+D8"), () => { HandleHotkey(ControlEnum.Ctrl8); } },
			{ Combination.FromString("Control+D9"), () => { HandleHotkey(ControlEnum.Ctrl9); } },
			{ Combination.FromString("Control+D0"), () => { HandleHotkey(ControlEnum.Ctrl10); } },
		};

		_globalHook = Hook.GlobalEvents();
		_globalHook.OnCombination(assignment);
		_globalHook.MouseMoveExt += GlobalHookMouseDownExt;
	}

	private void HandleHotkey(ControlEnum key)
	{
		_pressedKey = key;

		this.Invoke(new Action(() => {
			try
			{
				Refresh();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
		}));
	}
	private void GlobalHookMouseDownExt(object? sender, MouseEventExtArgs e)
	{
		_location = e.Location;
	}

	protected override void OnPaint(PaintEventArgs e)
	{
		if (_pressedKey == ControlEnum.Nothing)
		{
			base.OnPaint(e);
		}
		else
		{
			IntPtr desktopDC = GetDC(IntPtr.Zero);

			Graphics g = Graphics.FromHdc(desktopDC);
			DetermineText(g);
			g.Dispose();

			ReleaseDC(desktopDC);

			_pressedKey = ControlEnum.Nothing;
		}
	}

	private void DetermineText(Graphics g)
	{
		switch (_pressedKey)
		{
			case ControlEnum.Ctrl1:
				DrawHeader(g, "Test 1234");
				break;
			case ControlEnum.Ctrl2:
				DrawAtLocation(g, "At Mousepoint", _location);
				break;
			case ControlEnum.Ctrl3:
				DrawAtLocation(g, "At Mousepoint\n123", _location);
				break;
			case ControlEnum.Ctrl4:
				break;
			case ControlEnum.Ctrl5:
				break;
			case ControlEnum.Ctrl6:
				break;
			case ControlEnum.Ctrl7:
				break;
			case ControlEnum.Ctrl8:
				break;
			case ControlEnum.Ctrl9:
				break;
			case ControlEnum.Ctrl10:
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	private static void DrawHeader(Graphics g, string text)
	{
		var font = new Font(FontFamily.GenericSansSerif, 48);
		var size = g.MeasureString(text, font);

		var background = new SolidBrush(Color.FromArgb(128, 255, 255, 255));
		var left = Convert.ToInt32(0.5 * (g.VisibleClipBounds.Width - size.Width));
		const int top = 200;

		g.FillRectangle(background, left - 10, top - 10, size.Width + 20, size.Height + 20);
		g.DrawString(text, font, Brushes.Black, left, top);
	}

	private static void DrawAtLocation(Graphics g, string text, Point location)
	{
		var font = new Font(FontFamily.GenericSansSerif, 18);
		var size = g.MeasureString(text, font);

		var background = new SolidBrush(Color.FromArgb(128, 255, 255, 255));

		g.FillRectangle(background, location.X - 5, location.Y - 5, size.Width + 10, size.Height + 10);
		g.DrawString(text, font, Brushes.Black, location);
	}

}

