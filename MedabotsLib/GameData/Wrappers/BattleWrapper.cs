using System;
using MedabotsLib.GameData.Raw;

namespace MedabotsLib.GameData.Wrappers
{
    public class BattleWrapper : BaseWrapper<Battle>
    {
        public MedabotWrapper[] bots;
        public BattleWrapper(int id, Battle data) : base(id, data) {
            this.bots = new MedabotWrapper[data.number_of_bots];
            for (int i = 0; i < data.number_of_bots; i++)
            {
                this.bots[i] = new MedabotWrapper(i, data.bots[i]);
            }
        }

        public string Character 
        { 
            get { return AllData.CharacterNames[data.characterId].Str; } 
            set { data.characterId = (byte)AllData.CharacterNames.IndexOf(value); } 
        }

        public int NumberOfBots
        {
            get { return data.number_of_bots; }
            set { data.number_of_bots = (byte)value; }
        }
    }
}
