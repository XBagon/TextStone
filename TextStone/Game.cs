using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TextStone.EventManager;

namespace TextStone
{
    public class Game
    {
        public enum ZoneType
        {
            Deck, Hand, Field, Grave, Removed
        }

        public Player[] players { get; }

        public int turn { get; private set; }

        public static Random RNG = new Random();

        public Game(int startingPlayer, params Player[] players)
        {
            TurnEvent.Subscribe(this, 0, NextTurn);
            this.players = players;
            turn = Convert.ToInt32(startingPlayer == 0);
            foreach (var player in players)
            {
                player.ExecuteDraw(3);
            }
            ExecuteNextTurn();
        }

        public void ExecuteNextTurn()
        {
            TurnEvent.Call(new TurnEventArgs(players[Convert.ToInt32(turn == 0)]));
        }

        public Action<Object, TurnEventArgs> NextTurn { get; set; } = (me, args) =>
        {
            var game = (Game) me;
            game.turn = Convert.ToInt32(game.turn == 0);
            game.players[game.turn].maxMana++;
            game.players[game.turn].mana = game.players[game.turn].maxMana;
            game.players[game.turn].ExecuteDraw(1);
        };


    }
}
