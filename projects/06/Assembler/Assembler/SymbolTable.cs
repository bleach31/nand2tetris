using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler
{
	static class SymbolTable
	{
        /// <summary>
        /// 変数がはいるRamのアドレス
        /// </summary>
        static int variableAddress = 16;

        static public Dictionary<string, int> symbolDict = new Dictionary<string, int>
        {
            {"SP", 0},  //2進数のStringにすればよかったと後悔
            {"LCL", 1},
            {"ARG", 2},
            {"THIS", 3},
            {"THAT", 4},

            {"R0", 0},
            {"R1", 1},
            {"R2", 2},
            {"R3", 3},
            {"R4", 4},
            {"R5", 5},
            {"R6", 6},
            {"R7", 7},
            {"R8", 8},
            {"R9", 9},
            {"R10", 10},
            {"R11", 11},
            {"R12", 12},
            {"R13", 13},
            {"R14", 14},
            {"R15", 15},

            {"SCREEN", 16384},
            {"KBD", 24576}

        };

        /// <summary>
        /// symbolDictへ新しいシンボルを追加する。
        /// 新しいシンボルはRam16番地から順に割り当てられる。
        /// （規定のシンボルは追加済み）
        /// </summary>
        /// <param name="symbol"></param>
        static public void addVariable(string symbol) {
            if(!symbolDict.ContainsKey(symbol))
                symbolDict.Add(symbol, variableAddress);
                variableAddress++;
            }


        /// <summary>
        /// ラベルシンボルとアドレスの割り当てを行う
        /// romアドレスでもramアドレスでもどっちでもいい。使う人に任せる
        /// </summary>
        static public void addEntry(string symbol,int Address)
        {
            symbolDict.Add(symbol, Address);
        }
        
	}
}
