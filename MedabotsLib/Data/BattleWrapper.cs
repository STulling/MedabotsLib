using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedabotsLib.Data
{
    public class BattleWrapper : BaseWrapper<Battle>
    {
        private MedabotWrapper[] bots;
        public BattleWrapper(Battle data) : base(data) {
            this.bots = new MedabotWrapper[data.number_of_bots];
            for (int i = 0; i < data.number_of_bots; i++)
            {
                this.bots[i] = new MedabotWrapper(data.bots[i]);
            }
        }

        public string Character 
        { 
            get { return GameData.CharacterNames[data.characterId].Str; } 
            set { data.characterId = (byte)GameData.CharacterNames.IndexOf(value); } 
        }
    }
}
