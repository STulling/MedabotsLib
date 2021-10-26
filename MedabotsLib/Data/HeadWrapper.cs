using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedabotsLib.Data
{
    public class HeadWrapper : BaseWrapper<Head>
    {
        public HeadWrapper(int id, Head data) : base(id, data)
        { }

        public string Name => GameData.PartNames[id*4].Str;

        public byte RateOfSuccess {
            get { return data.RoS; }
            set { data.RoS = value; }
        }

        public byte Power
        {
            get { return data.power; }
            set { data.power = value; }
        }

        public byte AmountOfUses
        {
            get { return data.amount_of_uses; }
            set { data.amount_of_uses = value; }
        }

        public string MedalCompatibility
        {
            get { return GameData.MedalNames[data.medal_compatibility].Str; }
            set { data.medal_compatibility = (byte)GameData.MedalNames.IndexOf(value); }
        }

        public byte Armor
        {
            get { return data.armor; }
            set { data.armor = value; }
        }

        public bool Female
        {
            get { return data.gender == 1; }
            set { data.gender = (byte)(value ? 1 : 0); }
        }

        public bool ChainReaction
        {
            get { return data.chain_reaction == 1; }
            set { data.chain_reaction = (byte)(value ? 1 : 0); }
        }
    }
}
