using System;
using System.Collections.Generic;
using System.Linq;

namespace PokerHand
{
    public enum Suit
    {
        Clubs,
        Diamonds,
        Hearts,
        Spades
    }

    public enum Value
    {
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        Ten,
        Jack,
        Queen,
        King,
        Ace
    }

    public class Card : IComparable<Card>
    {
        public Suit Suit { get; }
        public Value Value { get; }

        public Card(Suit suit, Value value)
        {
            this.Suit = suit;
            this.Value = value;
        }

        public Card(string cardInfo)
        {
            // Converts a string representation of a card (e.g. "4C", "KD", "10S") into a Card object
            var suitDictionary = new Dictionary<string, Suit>() { { "C", Suit.Clubs }, { "D", Suit.Diamonds }, { "H", Suit.Hearts }, { "S", Suit.Spades } };
            var valueDictionary = new Dictionary<string, Value>()
            {
                { "2", Value.Two }, { "3", Value.Three }, { "4", Value.Four }, { "5", Value.Five }, { "6", Value.Six },
                { "7", Value.Seven }, { "8", Value.Eight }, { "9", Value.Nine }, { "10", Value.Ten }, { "J", Value.Jack },
                { "Q", Value.Queen }, { "K", Value.King }, { "A", Value.Ace }
            };
            string valueString = cardInfo.Substring(0, cardInfo.Length - 1), suitString = cardInfo.Substring(cardInfo.Length - 1);
            if (!valueDictionary.ContainsKey(valueString) || !suitDictionary.ContainsKey(suitString))
                throw new ArgumentException("Invalid card info string.");
            this.Suit = suitDictionary[suitString];
            this.Value = valueDictionary[valueString];
        }

        public int CompareTo(Card other)
        {
            if (other == null) return 1;
            return this.Value.CompareTo(other.Value);
        }
    }

    public interface IHand : IComparable<IHand>
    {
        // The interface and base class are gratuitous, but will demonstrate SOLID principles
        List<Card> Cards { get; }
    }
    public class Hand : IHand
    {
        public List<Card> Cards { get; }

        public Hand(List<Card> cards)
        {
            Cards = cards;
        }

        public Hand(string cardListString)
        {
            // Converts a string representation of a hand (e.g. "2H 3D 5S 9C KD") into a Hand object
            this.Cards = cardListString.Split(' ').Select(cardString => new Card(cardString)).ToList();
        }
        public int CompareTo(IHand other)
        {
            // Compare the hand by seeing which hand has the highest card,
            // or highest 2nd card, etc.
            if (other == null) return 1;

            // If card counts differ, the zip will only iterate the length of the shorter enumerable.
            foreach (var (thisCard, otherCard) in this.Cards.OrderByDescending(c => c.Value).Zip(other.Cards.OrderByDescending(c => c.Value), (t, o) => (t, o)))
            {
                var cardCompareResult = thisCard.CompareTo(otherCard);
                if (cardCompareResult != 0) return cardCompareResult;
            }

            // If all the cards are equal, the bigger hand is the one with more cards
            return this.Cards.Count.CompareTo(other.Cards.Count);
        }
    }
    public enum PokerHandRank
    {
        HighCard,
        Pair,
        TwoPairs,
        ThreeOfKind,
        Flush
    }

    public class PokerHand : Hand
    {
        public PokerHand(List<Card> cards) : base(cards)
        {
            if (this.Cards.Count != 5)
                throw new ArgumentException("A poker hand must contain 5 cards.");
            Cards.Sort();
            Cards.Reverse();
        }

        public PokerHand(string cardInfo) : base(cardInfo)
        {
            // Example input: "2H 3D 5S 9C KD"
            if (this.Cards.Count != 5)
                throw new ArgumentException("A poker hand must contain 5 cards.");
            Cards.Sort();
            Cards.Reverse();
        }

        private static PokerHandRank GetHandRank(PokerHand pokerHand)
        {

            if (pokerHand.Cards.GroupBy(card => card.Suit).Count() == 1)
                return PokerHandRank.Flush;
            // 4 of a kind will be treated as 3 of a kind, not 2 pairs, by this logic
            //
            // Example of adding logic for 4 of a kind
            // if (hand.Cards.GroupBy(c => c.Value).Count(g => g.Count() == 4) == 1)
            //     return PokerHandRank.FourOfKind;
            if (pokerHand.Cards.GroupBy(c => c.Value).Count(g => g.Count() >= 3) == 1)
                return PokerHandRank.ThreeOfKind;
            if (pokerHand.Cards.GroupBy(card => card.Value).Count(g => g.Count() == 2) == 2)
                return PokerHandRank.TwoPairs;
            if (pokerHand.Cards.GroupBy(card => card.Value).Count(g => g.Count() == 2) == 1)
                return PokerHandRank.Pair;
            return PokerHandRank.HighCard;
        }

        public int CompareTo(PokerHand other)
        {
            if (other == null) return 1;
            PokerHandRank rankThis = GetHandRank(this), rankOther = GetHandRank(other);

            // First check if the hand ranks are different
            var compareValue = rankThis.CompareTo(rankOther);
            if (compareValue != 0) return compareValue;

            // If the hand ranks are the same, look for the higher three/pair/2nd pair value
            if (rankThis == PokerHandRank.ThreeOfKind)
            {
                // Grab the values of the triads, then compare
                Value threeValueThis = this.Cards.GroupBy(c => c.Value).Where(g => g.Count() >= 3)
                    .Select(g => g.First().Value).First(),
                    threeValueOther = other.Cards.GroupBy(c => c.Value).Where(g => g.Count() >= 3)
                        .Select(g => g.First().Value).First();
                compareValue = threeValueThis.CompareTo(threeValueOther);
                if (compareValue != 0) return compareValue;
            }

            if (rankThis == PokerHandRank.TwoPairs || rankThis == PokerHandRank.Pair)
            {
                // Pull the value of the pair(s) in descending order, then compare
                IEnumerable<Value> pairsThis = this.Cards.GroupBy(c => c.Value).Where(g => g.Count() == 2)
                    .Select(g => g.First().Value).OrderByDescending(v => v),
                    pairsOther = other.Cards.GroupBy(c => c.Value).Where(g => g.Count() == 2)
                        .Select(g => g.First().Value).OrderByDescending(v => v);
                foreach (var (pairValueThis, pairValueOther) in pairsThis.Zip(pairsOther, (t, o) => (t, o)))
                {
                    compareValue = pairValueThis.CompareTo(pairValueOther);
                    if (compareValue != 0) return compareValue;
                }
            }

            // Lastly, use the High Card result (no matter the hand rank type) to compare
            return base.CompareTo(other);
        }
    }
}
