using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedabotsLib.Data
{
    public class MedabotWrapper : BaseWrapper<Medabot>
    {
        public MedabotWrapper(int id, Medabot data) : base(id, data) { }

        public byte MedalLevel
        {
            get { return data.medal_level; }
            set { data.medal_level = value; }
        }
        public string Medal
        {
            get { return GameData.MedalNames[data.medal].Str; }
            set { data.head = (byte)GameData.MedalNames.IndexOf(value); }
        }
        public string Head
        {
            get { return GameData.PartNames[data.head * 4].Str; }
            set { data.head = (byte)(GameData.PartNames.IndexOf(value) / 4); }
        }
        public string LeftArm
        {
            get { return GameData.PartNames[data.left_arm * 4 + 1].Str; }
            set { data.head = (byte)(GameData.PartNames.IndexOf(value) / 4 + 1); }
        }

        public string RightArm
        {
            get { return GameData.PartNames[data.right_arm * 4 + 2].Str; }
            set { data.head = (byte)(GameData.PartNames.IndexOf(value) / 4 + 2); }
        }

        public string Legs
        {
            get { return GameData.PartNames[data.legs * 4 + 3].Str; }
            set { data.head = (byte)(GameData.PartNames.IndexOf(value) / 4 + 3); }
        }
    }
}
