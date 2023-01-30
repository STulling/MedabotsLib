using System;
using MedabotsLib.GameData.Raw;
using MedabotsLib;

namespace MedabotsLib.GameData.Wrappers
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
            get { return AllData.MedalNames[data.medal].Str; }
            set { data.head = (byte)AllData.MedalNames.IndexOf(value); }
        }
        public string Head
        {
            get { return AllData.PartNames[data.head * 4].Str; }
            set { data.head = (byte)(AllData.PartNames.IndexOf(value) / 4); }
        }
        public string LeftArm
        {
            get { return AllData.PartNames[data.left_arm * 4 + 1].Str; }
            set { data.head = (byte)(AllData.PartNames.IndexOf(value) / 4 + 1); }
        }

        public string RightArm
        {
            get { return AllData.PartNames[data.right_arm * 4 + 2].Str; }
            set { data.head = (byte)(AllData.PartNames.IndexOf(value) / 4 + 2); }
        }

        public string Legs
        {
            get { return AllData.PartNames[data.legs * 4 + 3].Str; }
            set { data.head = (byte)(AllData.PartNames.IndexOf(value) / 4 + 3); }
        }
    }
}
