using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MinimumEditDistance;
using TextStone.Cards;

namespace TextStone
{
    class Program
    {
        public static int currentPlayerID = 0;
        public static Game game { get; private set; }

        static void Main(string[] args)
        {
            var deck = new Card[30];
            for (var i = 0; i < deck.Length; i++)
            {
                switch (i % 3)
                {
                    case 0:
                        deck[i] = new AcolyteOfPain();
                        break;
                    case 1:
                        deck[i] = new FuriousWolf();
                        break;
                    case 2:
                        deck[i] = new BlackMagician();
                        break;
                }
            }

            game = new Game(0, new Player("XBagon", deck), new Player("Enemy", deck));

                while (true)
                    Commands.WaitForCommand();

            }
        }

        class Commands
        {
            internal interface ICommand
            {
                string[] names { get; }
                string Execute(int playerID, params string[] args);
            }

            private static ICommand[] commands;

            static Commands()
            {
                commands = Assembly.GetExecutingAssembly().GetTypes()
                    .Where(x => x.GetInterfaces().Contains(typeof(ICommand)) &&
                                x.GetConstructor(Type.EmptyTypes) != null)
                    .Select(x => Activator.CreateInstance(x) as ICommand).ToArray();
            }

            public static void WaitForCommand()
            {
                var input = Console.ReadLine().Split(' ');
                foreach (var command in commands)
                {
                    foreach (string name in command.names)
                    {
                        if (name == input[0])
                        {
                            Console.WriteLine(command.Execute(Program.currentPlayerID, input.Skip(1).ToArray()));
                            return;
                        }
                    }
                }
                Console.WriteLine("Unknown Command!");
            }

            public static Card GetCardByName(string input, ICollection<Card> collection)
            {
                float closest = float.MaxValue;
                Card closestCard = null;
                foreach (var card in collection)
                {
                    float distance = Levenshtein.CalculateDistance(input, card.name, 1) / (float) card.name.Length;
                    if (distance < closest)
                    {
                        closest = distance;
                        closestCard = card;
                    }
                }
                return closestCard;
            }

            internal class LookCommand : ICommand
            {
                public string[] names { get; } = new[] {"look", "l"};

                public string Execute(int playerID, params string[] args)
                {
                    if (args.Length == 1)
                    {
                        string output = "";
                        List<Card> location = null;
                        switch (args[0])
                        {
                            case "e": goto case "enemy";
                            case "p": goto case "player";
                            case "h": goto case "hand";
                            case "f": goto case "field";
                            case "d": goto case "deck";
                            case "g": goto case "grave";
                            case "enemy":
                                return Program.game.players[Convert.ToInt32(Program.currentPlayerID == 0)]
                                    .ToString("(m/mm) a / h/mh");
                            case "player":
                                return Program.game.players[playerID].ToString("(m/mm) a / h/mh");
                            case "hand":
                                location = Program.game.players[playerID].hand.cards;
                                break;
                            case "field":
                                output += "YOUR SIDE:\n";
                                foreach (var card in Program.game.players[playerID].field.cards)
                                {
                                    output += card.ToString("(#m) #a / #h #n [#s]") + "\n";
                                }
                                output += "ENEMY SIDE:\n";
                                foreach (var card in Program.game.players[Convert.ToInt32(Program.currentPlayerID == 0)]
                                    .field.cards)
                                {
                                    output += card.ToString("(#m) #a / #h #n [#s]") + "\n";
                                }
                                return output.Substring(0, output.Length - 1);
                            case "deck":
                                location = Program.game.players[playerID].deck.cards;
                                break;
                            case "grave":
                                location = Program.game.players[playerID].grave.cards;
                                break;
                            default:
                                return "Unknown Location.";
                        }
                        foreach (var card in location)
                        {
                            output += card.ToString("(#m) #a / #h #n [#s]") + "\n";
                        }
                        if (output == "") output = "EMPTY";
                        else return output.Substring(0, output.Length - 1);
                        return output;
                    }
                    else
                    {
                        return "Wrong Arguments!";
                    }
                }
            }

            internal class PlayCardCommand : ICommand
            {
                public string[] names { get; } = new[] {"play", "p"};

                public string Execute(int playerID, params string[] args)
                {
                    Player player = Program.game.players[playerID];
                    if (args.Length == 1)
                    {
                        int index;
                        if (int.TryParse(args[0], out index))
                        {
                            if (index < player.hand.cards.Count)
                            {
                                if (player.ExecutePlay(player.hand.cards[index]))
                                    return $"Played {player.hand.cards[index].name}.";
                                else
                                    return $"Couldn't play {player.hand.cards[index].name}.";
                            }
                            else
                            {
                                return "Wrong Index!";
                            }
                        }
                        else
                        {
                            var closestCard = GetCardByName(args[0], player.hand.cards);
                            if (player.ExecutePlay(closestCard))
                                return $"Played {closestCard.name}.";
                            else
                                return $"Couldn't play {closestCard.name}.";
                        }
                    }
                    else
                    {
                        return "Wrong Arguments!";
                    }

                }
            }

            internal class AttackCommand : ICommand
            {
                public string[] names { get; } = new[] {"attack", "a"};

                public string Execute(int playerID, params string[] args)
                {
                    Player player = Program.game.players[playerID];
                    Player enemy = Program.game.players[Convert.ToInt32(Program.currentPlayerID == 0)];
                    if (args.Length == 2)
                    {
                        IAttacker attacker = null;
                        IAttacker target = null;
                        int index;
                        if (int.TryParse(args[0], out index))
                        {
                            if (index == -1)
                            {
                                target = player;
                            }
                            else
                            {
                                if (index < player.field.cards.Count)
                                {
                                    attacker = player.field.cards[index];
                                }
                                else
                                {
                                    return "Wrong Index!";
                                }
                            }
                        }
                        else
                        {
                            if (args[0] == "hero" || args[0] == "face")
                            {
                                attacker = player;
                            }
                            else
                            {
                                attacker = GetCardByName(args[0], player.field.cards);
                            }
                        }

                        if (int.TryParse(args[1], out index))
                        {
                            if (index == -1)
                            {
                                target = enemy;
                            }
                            else
                            {
                                if (index < enemy.field.cards.Count)
                                {
                                    target = enemy.field.cards[index];
                                }
                                else
                                {
                                    return "Wrong Index!";
                                }
                            }
                        }
                        else
                        {
                            if (args[1] == "hero" || args[1] == "face" || args[1] == "enemy")
                            {
                                target = enemy;
                            }
                            else
                            {
                                target = GetCardByName(args[1], enemy.field.cards);
                            }
                        }
                        if (player.ExecuteAttackOrder(attacker, target))
                            return $"{attacker.name} attacks {target.name}";
                        else
                            return $"{attacker.name} couldn't attack {target.name}.";
                    }
                    else
                    {
                        return "Wrong Arguments!";
                    }

                }
            }

            class NextTurnCommand : ICommand
            {
                public string[] names { get; } = new[] {"nextturn", "nt", "endturn", "et"};

                public string Execute(int playerID, params string[] args)
                {
                    Program.game.ExecuteNextTurn();
                    Program.currentPlayerID = Program.game.turn;
                    return $"{Program.game.players[Program.currentPlayerID].name}'s Turn.";
                }
            }
        }




}
