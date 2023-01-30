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
            AllData.LoadAll(game);
            return new MedaGame(game);
        }

        public void Save(string filename)
        {
            DataWriter writer = new DataWriter(0x800000);
            
            game.Save(filename);
        }
    }
}
