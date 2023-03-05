using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using PokerHand;
using PHand = PokerHand.PokerHand;

namespace PokerHandTest
{
    public class UnitTest1
    {
        private static PHand Judge(PHand hand1, PHand hand2)
        {
            if (hand1.CompareTo(hand2) > 0)
                return hand1;
            else if (hand1.CompareTo(hand2) < 0)
                return hand2;
            return null;
        }
        // Given test 1
        // BLACK: 2H 3D 5S 9C KD  &  WHITE: 2C 3H 4S 8C AH => WHITE WINS...high card: Ace
        [Fact]
        public void Given_Test_1()
        {
            PHand black = new PHand("2H 3D 5S 9C KD"),
                white = new PHand("2C 3H 4S 8C AH");

            Judge(white, black).Should().Be(white);
        }

        // Given test 2
        // BLACK: 2C 2S AS JC 4C  &  WHITE: AH AD 2H 3S 6S => WHITE WINS...higher pair (Ace > 2)
        [Fact]
        public void Given_Test_2()
        {
            PHand black = new PHand("2C 2S AS JC 4C"),
                white = new PHand("AH AD 2H 3S 6S");

            Judge(white, black).Should().Be(white);
        }
        // BLACK: 2H 4S 4C 3D 4H  &  WHITE: 2S 8S AS QS 3S => WHITE WINS...flush
        [Fact]
        public void Given_Test_3()
        {
            PHand black = new PHand("2H 4S 4C 3D 4H"),
                white = new PHand("2S 8S AS QS 3S");

            Judge(white, black).Should().Be(white);
        }
        // BLACK: 3C 7C 6C JC 4C  &  WHITE: 2S 8S 4S QS 3S => WHITE WINS... higher flush (Queen > Jack)
        [Fact]
        public void Given_Test_4()
        {
            PHand black = new PHand("3C 7C 6C JC 4C"),
                white = new PHand("2S 8S 4S QS 3S");

            Judge(white, black).Should().Be(white);
        }
        // BLACK: 2H 3D 5S 9C KD  &  WHITE: 2C 3H 4S 8C KH => BLACK WINS...high card: 9 (both hands have K, so next the highest card is evaluated)
        [Fact]
        public void Given_Test_5()
        {
            PHand black = new PHand("2H 3D 5S 9C KD"),
                white = new PHand("2C 3H 4S 8C KH");

            Judge(white, black).Should().Be(black);
        }
        // BLACK: 2H 3D 5S 9C KD  &  WHITE: 2D 3H 5C 9S KH => TIE
        [Fact]
        public void Given_Test_6()
        {
            PHand black = new PHand("2H 3D 5S 9C KD"),
                white = new PHand("2D 3H 5C 9S KH");

            Judge(white, black).Should().Be(null);
        }

        //
        // More tests
        //
        private readonly PHand _fourOfSuit = new PHand(new List<Card>()
        {
            new Card(Suit.Diamonds, Value.Seven),
            new Card(Suit.Hearts, Value.Six),
            new Card(Suit.Hearts, Value.Five),
            new Card(Suit.Hearts, Value.Four),
            new Card(Suit.Hearts, Value.Three),
        });

        private readonly PHand _royalFlush = new PHand(new List<Card>()
        {
            new Card(Suit.Clubs, Value.Ace),
            new Card(Suit.Clubs, Value.King),
            new Card(Suit.Clubs, Value.Queen),
            new Card(Suit.Clubs, Value.Jack),
            new Card(Suit.Clubs, Value.Ten),
        });

        private readonly PHand _lowFlush = new PHand(new List<Card>()
        {
            new Card(Suit.Clubs, Value.Six),
            new Card(Suit.Clubs, Value.Five),
            new Card(Suit.Clubs, Value.Four),
            new Card(Suit.Clubs, Value.Three),
            new Card(Suit.Clubs, Value.Two),
        });

        private readonly PHand _threeHighKicker = new PHand(new List<Card>()
        {
            new Card(Suit.Clubs, Value.Ace),
            new Card(Suit.Clubs, Value.Five),
            new Card(Suit.Spades, Value.Five),
            new Card(Suit.Diamonds, Value.Five),
            new Card(Suit.Clubs, Value.Two),
        });
        private readonly PHand _threeLowKicker = new PHand(new List<Card>()
        {
            new Card(Suit.Clubs, Value.Three),
            new Card(Suit.Clubs, Value.Five),
            new Card(Suit.Spades, Value.Five),
            new Card(Suit.Diamonds, Value.Five),
            new Card(Suit.Clubs, Value.Two),
        });

        [Fact]
        public void Should_Return_Larger_Card()
        {
            Card c1 = new Card(Suit.Clubs, Value.Eight),
                c2 = new Card(Suit.Hearts, Value.Four);
            c1.CompareTo(c2).Should().Be(1);
        }

        [Fact]
        public void Should_Return_Larger_Hand()
        {
            Hand h1 = new Hand(new List<Card>()
            {
                new Card(Suit.Clubs, Value.Ace)
            }),
                h2 = new Hand(new List<Card>()
            {
                new Card(Suit.Spades, Value.Ace),
                new Card(Suit.Spades, Value.King),
            });

            h1.CompareTo(h2).Should().Be(-1);
        }

        [Fact]
        public void Should_Ignore_Poker_Rules()
        {
            IHand h1 = _lowFlush;
            IHand h2 = _fourOfSuit;
            h1.CompareTo(h2).Should().Be(-1);
        }

        [Fact]
        public void Should_Ignore_Poker_Rules2()
        {
            Hand h1 = _lowFlush;
            Hand h2 = _fourOfSuit;
            h1.CompareTo(h2).Should().Be(-1);
        }

        [Fact]
        public void Should_Use_Poker_Rules()
        {
            PHand h1 = _lowFlush;
            PHand h2 = _fourOfSuit;
            h1.CompareTo(h2).Should().Be(1);
        }

        [Fact]
        public void Should_Return_Flush()
        {
            var result = Judge(_fourOfSuit, _royalFlush);
            result.Should().Be(_royalFlush);
        }

    }
}