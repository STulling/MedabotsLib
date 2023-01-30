using System;
using MedabotsLib.GameData.Raw;

namespace MedabotsLib.GameData.Wrappers
{
    public class LegsWrapper : BaseWrapper<Legs>
    {
        public LegsWrapper(int id, Legs data) : base(id, data)
        { }

        public byte Legtype {
            get { return data.legtype; }
            set { data.legtype = value; }
        }

        public byte Propulsion {
            get { return data.propulsion; }
            set { data.propulsion = value; }
        }

        public byte Evasion {
            get { return data.evasion; }
            set { data.evasion = value; }
        }

        public byte Defense {
            get { return data.defense; }
            set { data.defense = value; }
        }

        public byte Proximity {
            get { return data.proximity; }
            set { data.proximity = value; }
        }

        public byte Remoteness {
            get { return data.remoteness; }
            set { data.remoteness = value; }
        }

        public string Name {
            get { return AllData.PartNames[id*4].Str; }
            set { AllData.PartNames[id*4].Str = value; }
        }

        public byte Speciality {
            get { return data.speciality; }
            set { data.speciality = value; }
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
    }
}
