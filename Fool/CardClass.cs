using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fool
{
    public enum suits {
        Hearts = 0,
        Diamonds = 1,
        Clubs = 2,
        Spades = 3
    };
    public enum values { _6 = 0, _7 = 1, _8 = 2, _9 = 3, _10 = 4, _J = 5, _Q = 6, _K = 7, _A = 8 };

   

public class CardClass
    {
        public suits _suits;
        public values _values;

        public CardClass(suits suits, values values)
        {
            _suits = suits;
            _values = values;
        }

        public static char SuitIcon(suits s)
        {
            switch (s)
            {
                case suits.Hearts:
                    return '❤';
                case suits.Diamonds:
                    return '♦';
                case suits.Clubs:
                    return '♣';
                case suits.Spades:
                    return '♠';
                default:
                    return '\0';

            }
        }

        public static CardClass CompareCards(CardClass card1, CardClass card2, suits trumpSuit)
        {
            // Если одна из карт козырная, она побеждает
            bool isCard1Trump = card1._suits == trumpSuit;
            bool isCard2Trump = card2._suits == trumpSuit;

            if (isCard1Trump && !isCard2Trump)
                return card1;

            if (isCard2Trump && !isCard1Trump)
                return card2;

            // Если обе карты одной масти, сравниваем по значению
            if (card1._suits == card2._suits)
            {
                if (card1._values > card2._values)
                    return card1;
                else if (card1._values < card2._values)
                    return card2;
                else
                    return null; // Карты равны
            }

            // Если карты разных мастей и не козырные, ни одна не побеждает
            return null;
        }

    }
}
  
