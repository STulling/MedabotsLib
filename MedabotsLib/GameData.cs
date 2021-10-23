using GBALib;
using MedabotsLib.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MedabotsLib
{
    struct StructInfo
    {
        public Type type;
        public int address;
        public int count;

        public StructInfo(Type type, int address, int count)
        {
            this.type = type;
            this.address = address;
            this.count = count;
        }
    }
    public static class GameData
    {
        public static BackRefList<Text> MedalNames = GetROMTextData(0x3b65b0);
        public static OffsetList<Battle> Battles = GetROMStructData<Battle>(0x3c1a00, 20);

        public static List<IList<Byteable>> Data = new List<IList<Byteable>> {
            MedalNames.Cast<Byteable>().ToList(),
            Battles.Cast<Byteable>().ToList()
        };

        public static BackRefList<Text> GetROMTextData(int address)
        {
            List<Text> texts = TextExtract.Extract(address);
            return new BackRefList<Text>(texts, address, typeof(Text));
        }

        public static OffsetList<T> GetROMStructData<T>(int address, int count) where T : Byteable
        {
            List<T> items = Game.GetInstance().ReadObjects<T>(address, count);
            return new OffsetList<T>(items, address, typeof(T));
        }
    }
}
