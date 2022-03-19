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
        public static BackRefList<Text> BattlefieldNames;
        public static BackRefList<Text> PreBattleMessage;
        public static BackRefList<Text> PostBattleMessage;
        public static BackRefList<Text> CharacterNames;
        public static BackRefList<Text> Messages;
        public static OffsetList<BattleWrapper> Battles;
        public static OffsetList<HeadWrapper> HeadParts;

        public static List<object> Data;

        public static void LoadAll(Game game)
        {
            MedalNames = GetROMTextData(game, 0x3b65b0);
            BotNames = GetROMTextData(game, 0x3ba678);
            PreBattleMessage = GetROMTextData(game, 0x3c2d5c);
            PostBattleMessage = GetROMTextData(game, 0x3c3b24);
            CharacterNames = GetROMTextData(game, 0x3c40d8);
            PartNames = GetROMTextData(game, 0x3bbb6c);
            BattlefieldNames = GetROMTextData(game, 0x3beb88);
            PartNames = GetROMTextData(game, 0x3bbb6c);
            Battles = GetROMStructData<BattleWrapper, Battle>(game, 0x3c1ba0, 0xf5, is_ptr_table: true);
            HeadParts = GetROMStructData<HeadWrapper, Head>(game, 0x3b841c, 120, jump: 4);
            Messages = GetAllMessages(game);


            Data = new List<object> {
                MedalNames,
                BotNames,
                Messages,
                Battles,
                PartNames,
                BattlefieldNames,
                PreBattleMessage,
                PostBattleMessage,
                CharacterNames
            };
        }

        private static BackRefList<Text> GetAllMessages(Game game)
        {
            int amount_of_ptrs = 16;
            int[] addresses = new int[amount_of_ptrs];
            for (int i = 0; i < amount_of_ptrs; i++)
            {
                addresses[i] = game.ReadLocalAddress(0x47df44 + 4 * i);
            }
            return GetScatteredROMTextData(game, addresses);
        }

        public static BackRefList<Text> GetROMTextData(Game game, int address)
        {
            List<Text> texts = TextExtract.Extract(game, address);
            return new SequentialBackRefList<Text>(texts, address);
        }

        public static BackRefList<Text> GetScatteredROMTextData(Game game, int[] addresses)
        {
            RandomAccessBackRefList<Text> res = new RandomAccessBackRefList<Text>();
            foreach (int address in addresses)
            {
                List<Text> texts = TextExtract.Extract(game, address);
                res.Merge(new SequentialBackRefList<Text>(texts, address).ToRandomAccess());
            }
            res.Lock();
            return res;
        }

        public static OffsetList<T> GetROMStructData<T>(Game game, int address, int count) where T : IByteable
        {
            List<T> items = game.ReadObjects<T>(address, count);
            return new OffsetList<T>(items, address);
        }

        public static OffsetList<W> GetROMStructData<W, T>(Game game, int address, int count, int jump = 1, bool is_ptr_table = false) where T : IByteable where W : BaseWrapper<T>
        {
            List<T> items = game.ReadObjects<T>(address, count, jump, is_ptr_table);
            List<W> wrappers = new List<W>();
            for (int i = 0; i < items.Count; i++)
            {
                wrappers.Add(Activator.CreateInstance(typeof(W), new object[] { i, items[i] }) as W);
            }
            return new OffsetList<W>(wrappers, address);
        }
    }
}
