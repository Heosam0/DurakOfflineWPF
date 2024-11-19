using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace Fool
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<CardClass> deck = new List<CardClass>();
        List<CardClass> player = new List<CardClass>();
        List<CardClass> bot = new List<CardClass>();
        List<CardClass> table = new List<CardClass>();
        List<Card> cardsWindows = new List<Card>();
        List<Window> windows = new List<Window>();
        public List<Window> tableWindows = new List<Window>();
        public double screenWidth = SystemParameters.PrimaryScreenWidth; // Ширина экрана
        public double screenHeight = SystemParameters.PrimaryScreenHeight; // Высота экрана
        suits trump;
        public bool canMove = true;
        bool bots_turn = false;
        public MainWindow()
        {
            InitializeComponent();
            for (suits suit = 0; suit <= suits.Spades; suit++)
                for (values value = 0; value <= values._A; value++)
                    deck.Add(new CardClass(suit, value));
            Shuffle(deck);
            this.Closing += OnWindowClosed;
            this.Left = screenHeight / 2;
            this.Top = screenHeight / 4;
      
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 6; i++)
            {
                player.Add(deck[i]);

            }
            deck.RemoveRange(0, 6);
            for (int i = 0; i < 6; i++)
            {
                bot.Add(deck[i]);

            }
            deck.RemoveRange(0, 6);
            trump = DetermineTrumpSuit(deck);
            ArrangePlayerCards(player, 150, 200, 5);
            Start.Visibility = Visibility.Collapsed;
            Narrator.Visibility = Visibility.Visible;
            Label.Content = $"У противника {bot.Count} карт";
            suitt.Visibility = Visibility.Visible;
            suit_value.Visibility = Visibility.Visible;
            suit_value.Content = CardClass.SuitIcon(trump);
            if (trump == suits.Hearts || trump == suits.Diamonds)
                suit_value.Foreground = new SolidColorBrush(Colors.Red);
            deck_counter.Visibility = Visibility.Visible;
            deck_counter.Content = deck.Count();
        }


        public async Task ArrangePlayerCards(IEnumerable<CardClass> playerCards, double cardWidth, double cardHeight, double margin)
        {
           
            double difference = 10;
            double currentLeft = screenWidth / 4; // Текущая координата слева
            double currentTop = screenHeight - cardHeight - margin - 100; // Начальная координата сверху (снизу экрана)

            foreach (CardClass cardClass in playerCards)
            {
                Card cardWindow = new Card(cardClass, this);
                cardsWindows.Add(cardWindow);
                // Если карта выходит за пределы ширины экрана, переносим на следующую строку
                if (currentLeft + cardWidth > screenWidth)
                {
                    currentLeft = 0; // Сброс позиции слева
                    currentTop -= (cardHeight + margin); // Смещение вверх
                }

                // Устанавливаем положение окна
                cardWindow.Left = currentLeft;
                cardWindow.Top = currentTop + difference;
                windows.Add(cardWindow);
                cardWindow.Show();
                await Task.Delay(5);
                
                // Сдвигаем позицию для следующей карты
                currentLeft += (cardWidth + margin);
                difference *= -1;

            }

        }

        public bool Play(CardClass cardClass) {
            if (table.Count == 0)
            {
                table.Add(cardClass);
                canMove = false;
                foreach (Card c in cardsWindows)
                {
                    c.button.IsEnabled = false;
                    c.Cursor = Cursors.No;
                }
                player.Remove(cardClass);
                BotThink();
                return true;
            }
            else 
                if(cardClass == CardClass.CompareCards(table[0], cardClass, trump))
            {
                TakeCardButton.Visibility = Visibility.Collapsed;
                player.Remove(cardClass);
                foreach (Card c in cardsWindows)
                {
                    c.button.IsEnabled = false;
                    c.Cursor = Cursors.No;
                }
                Next.Visibility = Visibility.Visible;
                return true;
            } else return false;

            
        }

        public async Task BotThink()
        {
            Card card = null;
            await Task.Delay(1000);
            foreach (CardClass c in bot)
            {
               if(c == CardClass.CompareCards(c, table[0], trump)) {
                    Card a = new Card(c, this);
                    a.button.Visibility = Visibility.Hidden;
                    a.Title = "Карта противника";
                    a.Left = (screenHeight / 1.25) + 10;                               
                    a.Top = (screenHeight / 2.8) + 10;
                    a.Show(); 
                    Next.Visibility = Visibility.Visible;
                    card = a;
                    a.Cursor = Cursors.No;
                    bot.Remove(a.thisCard);
                    table.Add(c);
                    tableWindows.Add(a);
                    cardsWindows.Add(a);
                    windows.Add(a);
                    break;
                }
            }
            if(card == null)
            {
                bot.Add(table[0]);
                table.Clear();
                ClearTable();
                canMove = true;
                CanMove(canMove);
                GiveCards();
                Refresh();
                ArrangePlayerCards(player, 150, 200, 5);
            }
            Label.Content = $"У противника {bot.Count} карт";
        }

        public async Task BotTurn()
        {
            canMove = false;
            CanMove(canMove);
            Narrator.Content = "Ход противника...";
            await Task.Delay(1000);
            if (bot.Count > 0)
            {
                foreach (CardClass c in bot)
                {
                    table.Clear();
                    table.Add(c);
                    Card a = new Card(c, this);
                    a.button.Visibility = Visibility.Hidden;
                    a.Title = "Карта";
                    a.Left = (screenHeight / 1.25) + 10;
                    a.Top = (screenHeight / 2.8) + 10;
                    a.Show();
                    a.Cursor = Cursors.No;
                    bot.Remove(a.thisCard);
                    cardsWindows.Add(a);
                    tableWindows.Add(a);
                    windows.Add(a);
                    break;
                }
                TakeCardButton.Visibility = Visibility.Visible;
                canMove = true;
                CanMove(canMove);
                Label.Content = $"У противника {bot.Count} карт";
            }
            else { 
            MessageBox.Show("Вы проиграли!");
                new MainWindow().Show();
                this.Close();
        }
        }

        static suits DetermineTrumpSuit(List<CardClass> deck)
        {
            Random rnd = new Random();
            suits trumpCard = deck[rnd.Next(deck.Count)]._suits;
            return trumpCard;
        }

        static void Shuffle(List<CardClass> deck)
        {
            Random rnd = new Random();
            for (int i = deck.Count - 1; i > 0; i--)
            {
                int j = rnd.Next(i + 1);
                CardClass temp = deck[i];
                deck[i] = deck[j];
                deck[j] = temp;
            }
        }




        private void OnWindowClosed(object sender, EventArgs e)
        {
            Refresh();
        }

        void Refresh()
        {
            foreach (Window window in windows)
                window.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            ClearTable();
            GiveCards();
            Label.Content = $"У противника {bot.Count} карт";
            bots_turn = !bots_turn;
            if (bots_turn)
            {
                BotTurn();
            }
            else
            {
                Narrator.Content = "Ваш ход!";
                canMove = true;
                table.Clear();
            }
            Next.Visibility = Visibility.Collapsed;
            TakeCardButton.Visibility = Visibility.Collapsed;
            Refresh();
            ArrangePlayerCards(player, 150, 200, 5);
        }

        void GiveCards()
        {
            if (bot.Count < 6 && deck.Count > 0)
            {
                bot.Add(deck[0]);
                deck.RemoveAt(0);
            }
            if (player.Count < 6 && deck.Count > 0)
            {
                player.Add(deck[0]);
                deck.RemoveAt(0);
            }
            if(deck.Count == 0 && player.Count == 0)
            {
                MessageBox.Show("Вы победили!");
                new MainWindow().Show();
                this.Close();
            }
            deck_counter.Content = deck.Count;
        }


        void CanMove(bool cm)
        {
            foreach (Card c in cardsWindows)
            {
                c.button.IsEnabled = cm;
                c.Cursor = cm ? Cursors.Hand : Cursors.No;

            }
        }
        void ClearTable()
        {
            foreach (Window w in tableWindows)
            {
                w.Close();
            }
        }

        private void TakeCardButton_Click(object sender, RoutedEventArgs e)
        {
            TakeCardButton.Visibility = Visibility.Collapsed;
            player.Add(table[0]);
            table.Clear();
            ClearTable();
            BotTurn();
            GiveCards();
            Label.Content = $"У противника {bot.Count} карт";
            Refresh();
            ArrangePlayerCards(player, 150, 200, 5);
        }
    }
}
