using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Fool
{
    /// <summary>
    /// Логика взаимодействия для Card.xaml
    /// </summary>
    public partial class Card : Window
    {
        public CardClass thisCard;
        MainWindow mw;
        public Card(CardClass cardclass, MainWindow mw)
        {
            this.thisCard = cardclass;
            InitializeComponent();
            number.Content = (thisCard._values.ToString()).Remove(0, 1);
            number1.Content = (thisCard._values.ToString()).Remove(0, 1);
            mast_label.Content = CardClass.SuitIcon(thisCard._suits);
            if (thisCard._suits == suits.Hearts || thisCard._suits == suits.Diamonds)
            {
                number.Foreground = new SolidColorBrush(Colors.Red);
                number1.Foreground = new SolidColorBrush(Colors.Red);
                mast_label.Foreground = new SolidColorBrush(Colors.Red);
            }
            Loaded += DisableCloseButton;
            this.mw = mw;   
        }



        // Отключение крестика чтобы игрок случайно не выключил окно с картой!
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("user32.dll")]
        private static extern int EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);

        private const uint SC_CLOSE = 0xF060; // Команда для закрытия
        private const uint MF_BYCOMMAND = 0x00000000;
        private const uint MF_GRAYED = 0x00000001; // Отключить 

        private void DisableCloseButton(object sender, RoutedEventArgs e)
        {
            var hWnd = new System.Windows.Interop.WindowInteropHelper(this).Handle;
            var hMenu = GetSystemMenu(hWnd, false);

            // Отключаем кнопку закрытия
            EnableMenuItem(hMenu, SC_CLOSE, MF_BYCOMMAND | MF_GRAYED);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (mw.canMove && mw.Play(thisCard))
                {
                    this.Left = mw.screenHeight / 1.25;
                    this.Top = mw.screenHeight / 2.8;
                mw.tableWindows.Add(this);
                }
                else MessageBox.Show("Не удается покрыть!");
        }
    }
}

