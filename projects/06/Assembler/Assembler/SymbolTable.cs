using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler
{
	class SymbolTable
	{
		static Dictionary<string, int> symbolDict = new Dictionary<string, int>
        {
			//R0～R15はコンストラクタで設定
            {"SCREEN", 16384},
            {"KBD", 24576},

            {"SP", 0},
            {"LCL", 1},
            {"ARG", 2},
            {"THIS", 3},
            {"THAT", 4}
        };
		public SymbolTable(){
			for (int i = 0; i < 16;i++)
			{
				symbols.Add("R" + i, i);
			}
		}
	}
}
