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
    public static class GameData
    {
        public static BackRefList<Text> MedalNames;
        public static BackRefList<Text> BotNames;
        public static BackRefList<Text> PartNames;
        public static BackRefList<Text> PreBattleMessage;
        public static BackRefList<Text> PostBattleMessage;
        public static BackRefList<Text> CharacterNames;
        public static BackRefList<Text> Messages;
        public static OffsetList<BattleWrapper> Battles;

        public static List<IList<IByteable>> Data;

        public static void LoadAll()
        {
            MedalNames = GetROMTextData(0x3b65b0);
            BotNames = GetROMTextData(0x3ba678);
            PreBattleMessage = GetROMTextData(0x3c2d5c);
            PostBattleMessage = GetROMTextData(0x3c3b24);
            CharacterNames = GetROMTextData(0x3c40d8);
            PartNames = GetROMTextData(0x3bbb6c);
            Battles = GetROMStructData<BattleWrapper, Battle>(0x3c1ba0, 0xf5, is_ptr_table: true);
            Messages = GetAllMessages();
            

            Data = new List<IList<IByteable>> {
                MedalNames.Cast<IByteable>().ToList(),
                BotNames.Cast<IByteable>().ToList(),
                Messages.Cast<IByteable>().ToList(),
                Battles.Cast<IByteable>().ToList(),
                PartNames.Cast<IByteable>().ToList(),
                PreBattleMessage.Cast<IByteable>().ToList(),
                PostBattleMessage.Cast<IByteable>().ToList(),
                CharacterNames.Cast<IByteable>().ToList()
            };
        }

        private static BackRefList<Text> GetAllMessages()
        {
            int amount_of_ptrs = 16;
            int[] addresses = new int[amount_of_ptrs];
            for (int i = 0; i < amount_of_ptrs; i++)
            {
                Console.WriteLine(0x47e45c + 4 * i);
                addresses[i] = Game.GetInstance().ReadLocalAddress(0x47df44 + 4 * i);
            }
            return GetScatteredROMTextData(addresses);
        }

        public static BackRefList<Text> GetROMTextData(int address)
        {
            List<Text> texts = TextExtract.Extract(address);
            return new SequentialBackRefList<Text>(texts, address);
        }

        public static BackRefList<Text> GetScatteredROMTextData(int[] addresses)
        {
            RandomAccessBackRefList<Text> res = new RandomAccessBackRefList<Text>();
            foreach (int address in addresses)
            {
                List<Text> texts = TextExtract.Extract(address);
                res.Merge(new SequentialBackRefList<Text>(texts, address).ToRandomAccess());
            }
            res.Lock();
            return res;
        }

        public static OffsetList<T> GetROMStructData<T>(int address, int count) where T : IByteable
        {
            List<T> items = Game.GetInstance().ReadObjects<T>(address, count);
            return new OffsetList<T>(items, address);
        }

        public static OffsetList<W> GetROMStructData<W, T>(int address, int count, bool is_ptr_table = false) where T : IByteable where W : BaseWrapper<T>
        {
            List<T> items = Game.GetInstance().ReadObjects<T>(address, count, is_ptr_table);
            List<W> wrappers = new List<W>();
            foreach (T item in items)
            {
                wrappers.Add(Activator.CreateInstance(typeof(W), new object[] { item }) as W);
            }
            return new OffsetList<W>(wrappers, address);
        }
    }
}
