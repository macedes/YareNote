using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.ComponentModel;
using System.IO;

namespace YareNote
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private const int HOTKEY_ID = 9000;

        //CTRL Modifier:
        private const uint MOD_CONTROL = 0x0002; //CTRL

        //SPACEBAR
        private const uint VK_SPACE = 0x20;
        public MainWindow()
        {
            InitializeComponent();
            textBox.Focus();
            string text = ReadText();
            textBox.Text = text;
            textBox.Select(textBox.Text.Length, 0);
            PreviewKeyDown += new KeyEventHandler(HandleEsc);
        }

        private void HandleEsc(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                this.WindowState = WindowState.Minimized;
        }

        private IntPtr _windowHandle;
        private HwndSource _source;
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            _windowHandle = new WindowInteropHelper(this).Handle;
            _source = HwndSource.FromHwnd(_windowHandle);
            _source.AddHook(HwndHook);
            
            RegisterHotKey(_windowHandle, HOTKEY_ID, MOD_CONTROL, VK_SPACE); //CTRL + SPACE
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;
            switch (msg)
            {
                case WM_HOTKEY:
                    switch (wParam.ToInt32())
                    {
                        case HOTKEY_ID:
                            int vkey = (((int)lParam >> 16) & 0xFFFF);
                            if (vkey == VK_SPACE)
                            {
                                        WindowState = WindowState.Normal;
                                        Activate();
                                        textBox.Focus();
                            }
                            handled = true;
                            break;
                    }
                    break;
            }
            return IntPtr.Zero;
        }

        protected override void OnClosed(EventArgs e)
        {
            _source.RemoveHook(HwndHook);
            UnregisterHotKey(_windowHandle, HOTKEY_ID);
            SaveText(textBox.Text);
            base.OnClosed(e);
        }

        private void SaveText(string content)
        {
            File.WriteAllText("YareNote.txt", content);
        }

        private string ReadText()
        {
            string text = "";
            if (File.Exists("YareNote.txt"))
            {
                text = File.ReadAllText("YareNote.txt");
            }
            return text;
        }

    }
}