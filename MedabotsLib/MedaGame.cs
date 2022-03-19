using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GBALib;

namespace MedabotsLib
{
    public class MedaGame
    {
        public Game game;

        private MedaGame(Game game)
        {
            this.game = game;
        }

        public static MedaGame Load(string filename)
        {
            Game game = Game.Load(filename);
            GameData.LoadAll(game);
            return new MedaGame(game);
        }

        public void Save(string filename)
        {
            DataWriter writer = new DataWriter(0x800000);
            writer.Write(game, GameData.Data);
            game.Save(filename);
        }
    }
}
