using MedabotsLib.GameData.Raw;

namespace MedabotsLib.GameData.Wrappers
{
    public class RightArmWrapper : BaseWrapper<Arm>
    {
        public RightArmWrapper(int id, Arm data) : base(id, data)
        { }
        
        public string Name {
            get { return AllData.PartNames[id*4 + 1].Str; }
            set { AllData.PartNames[id*4 + 1].Str = value; }
        }

        public byte Technique {
            get { return data.technique; }
            set { data.technique = value; }
        }

        public byte Speciality {
            get { return data.speciality; }
            set { data.speciality = value; }
        }

        public byte RateOfSuccess {
            get { return data.RoS; }
            set { data.RoS = value; }
        }

        public byte Power
        {
            get { return data.power; }
            set { data.power = value; }
        }

        public byte Charge
        {
            get { return data.charge; }
            set { data.charge = value; }
        }

        public byte Radiation
        {
            get { return data.radiation; }
            set { data.radiation = value; }
        }

        public string MedalCompatibility
        {
            get { return AllData.MedalNames[data.medal_compatibility].Str; }
            set { data.medal_compatibility = (byte)AllData.MedalNames.IndexOf(value); }
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
