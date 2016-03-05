using System;
using System.Windows.Forms;
using System.Threading;
using System.Drawing;
using System.Runtime.InteropServices;
using Microsoft.DirectX;
using D3D = Microsoft.DirectX.Direct3D;

namespace Warband
{
    public partial class Form1 : Form
    {
        private Margins marg;
        private D3D.Device device = null;
        private static D3D.Line line;
        private static D3D.Font font;
        private Radar radar;

        //#region -----------#Addresses#-----------------
        //int MainPlayerBase = 0x31370F0;
        //int[] MainPlayerMultiLevel = new int[] { 0 };
        //PlayerDataAddr MainPlayerOffsets = new PlayerDataAddr(0);
        //#endregion

        #region -----------#const#-----------------
        public const int GWL_EXSTYLE = -20;
        public const int WS_EX_LAYERED = 0x80000;
        public const int WS_EX_TRANSPARENT = 0x20;
        public const int LWA_ALPHA = 0x2;
        public const int LWA_COLORKEY = 0x1;
        #endregion

        public Form1()
        {
            InitializeComponent();

            ReadAndInject.AssemblyInject();
            PlayerData.Mem = ReadAndInject.Mem;
            PlayerData.Offsets = ReadAndInject.Offsets;

            SetWindowLong(this.Handle, GWL_EXSTYLE, (IntPtr)(GetWindowLong(this.Handle, GWL_EXSTYLE) ^ WS_EX_LAYERED ^ WS_EX_TRANSPARENT));
            SetLayeredWindowAttributes(this.Handle, 0, 255, LWA_ALPHA);

            D3D.PresentParameters presentParameters = new D3D.PresentParameters();
            presentParameters.Windowed = true;
            presentParameters.SwapEffect = D3D.SwapEffect.Discard;
            presentParameters.BackBufferFormat = D3D.Format.A8R8G8B8;

            device = new D3D.Device(0, D3D.DeviceType.Hardware, Handle, D3D.CreateFlags.HardwareVertexProcessing, presentParameters);
            line = new D3D.Line(device);
            font = new D3D.Font(device, new System.Drawing.Font("Arial", 10, FontStyle.Regular));
            radar = new Radar(device);
            Thread dx = new Thread(new ThreadStart(dxThread));
            dx.IsBackground = true;
            dx.Start();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            marg.Left = 0;
            marg.Top = 0;
            marg.Right = this.Width;
            marg.Bottom = this.Height;
            DwmExtendFrameIntoClientArea(this.Handle, ref marg);
        }

        public static void DrawShadowText(string text, Point Position, Color color)
        {
            font.DrawText(null, text, new Point(Position.X + 1, Position.Y + 1), Color.Red);
            font.DrawText(null, text, Position, color);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            SetHook();
        }

        private void Form1_Closing(object sender, EventArgs e)
        {
            UnHook();
            this.device.Dispose();
        }

        private void dxThread()
        {
            while (true)
            {
                device.Clear(D3D.ClearFlags.Target, Color.FromArgb(0, 0, 0, 0), 1.0f, 0);
                device.RenderState.ZBufferEnable = false;
                device.RenderState.Lighting = false;
                device.RenderState.CullMode = D3D.Cull.None;
                device.Transform.Projection = Matrix.OrthoOffCenterLH(0, this.Width, this.Height, 0, 0, 1);
                device.BeginScene();

                MainPlayer.CheckPlayer();
                radar.SetRadar();

                device.EndScene();
                device.Present();
            }

        }

        //private void AngleVectors()
        //{
        //    float angle;
        //    float sr, sp, sy, cr, cp, cy,
        //        cpi = (3.141f * 2 / 360);

        //    angle = viewAngles.y * cpi;
        //    //cpi = same view angles.x isn't

        //    sy = (float)Math.Sin(angle);
        //    cy = (float)Math.Cos(angle);
        //    angle = viewAngles.x * cpi;
        //    sp = (float)Math.Sin(angle);
        //    cp = (float)Math.Cos(angle);
        //    angle = viewAngles.z * cpi;
        //    sr = (float)Math.Sin(angle);
        //    cr = (float)Math.Cos(angle);

        //    vForward.x = cp * cy;
        //    vForward.y = cp * sy;
        //    vForward.z = -sp;

        //    vRight.x = (-1 * sr * sp * cy + -1 * cr * -sy);
        //    vRight.y = (-1 * sr * sp * sy + -1 * cr * cy);
        //    vRight.z = -1 * sr * cp;

        //    vUpward.x = (cr * sp * cy + -sr * -sy);
        //    vUpward.y = (cr * sp * sy + -sr * cy);
        //    vUpward.z = cr * cp;
        //}

        //bool WorldToScreen(Vect3d WorldLocation, Vect3d mypos)
        //{
        //    Vect3d vLocal, vTransForm;
        //    vTransForm = new Vect3d();
        //    vForward = new Vect3d();
        //    vRight = new Vect3d();
        //    vUpward = new Vect3d();

        //    AngleVectors();
        //    vLocal = SubVectorDist(WorldLocation, mypos);

        //    vTransForm.x = vLocal.dotproduct(vRight);
        //    vTransForm.y = vLocal.dotproduct(vUpward);
        //    vTransForm.z = vLocal.dotproduct(vForward);

        //    if (vTransForm.z < 0.01)
        //        return false;

        //    ScreenX = screencenter[0] + (screencenter[0] / vTransForm.z * (1 / fov[0])) * vTransForm.x;
        //    ScreenY = screencenter[1] - (screencenter[1] / vTransForm.z * (1 / fov[1])) * vTransForm.y;

        //    return true;
        //}




        

        //private PlayerDataVec GetPlayerData(PlayerData updatePlayer)
        //{
        //    PlayerDataVec playerRet = new PlayerDataVec();
        //    int playerBase = Mem.ReadMultiLevelPointer(0x031370F0, 4, updatePlayer.multiLevel);
        //    playerRet.health = Mem.ReadFloat(playerBase + 0x6000);
        //    playerRet.X = Mem.ReadFloat(playerBase + 0x0040);
        //    playerRet.Y = Mem.ReadFloat(playerBase + 0x0044);
        //    playerRet.Z = Mem.ReadFloat(playerBase + 0x0048);
        //    return playerRet;
        //}

        internal struct Margins
        {
            public int Left, Right, Top, Bottom;
        }

        #region More pointless stuff

        [DllImport("user32.dll")]
        static extern IntPtr GetFocus();

        //[DllImport("user32.dll")]
        //[return: MarshalAs(UnmanagedType.Bool)]
        //public static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

        [DllImport("user32.dll")]
        static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(String sClassName, String sAppName);

        [DllImport("dwmapi.dll")]
        static extern void DwmExtendFrameIntoClientArea(IntPtr hWnd, ref Margins pMargins);

        [DllImport("user32.dll")]
        static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, UIntPtr dwExtraInfo);

        [DllImport("user32.dll")]
        static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc callback, IntPtr hInstance, uint threadId);

        [DllImport("user32.dll")]
        static extern bool UnhookWindowsHookEx(IntPtr hInstance);

        [DllImport("user32.dll")]
        static extern IntPtr CallNextHookEx(IntPtr idHook, int nCode, int wParam, IntPtr lParam);

        [DllImport("kernel32.dll")]
        static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("user32.dll", SetLastError = true)]

        private static extern UInt32 GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]

        static extern int SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll")]

        static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private static IntPtr hhook = IntPtr.Zero;

        public void SetHook()
        {
            IntPtr hInstance = LoadLibrary("User32");

        }

        public static void UnHook()
        {
            UnhookWindowsHookEx(hhook);
        }

        #endregion
      
    }
}
