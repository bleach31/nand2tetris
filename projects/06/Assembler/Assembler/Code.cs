using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler
{
	class Code
	{
		static Dictionary<string, string> compDict = new Dictionary<string, string>()
        {
            {"0",  "101010"},
            {"1",  "111111"},
            {"-1", "111010"},
            {"D",  "001100"},
            {"A",  "110000"},
            {"!D", "001101"},
            {"!A", "110001"},
            {"-D", "001111"},
            {"-A", "110011"},
            {"D+1","011111"},
            {"A+1","110111"},
            {"D-1","001110"},
            {"A-1","110010"},
            {"D+A","000010"},
            {"D-A","010011"},
            {"A-D","000111"},
            {"D&A","000000"},
            {"D|A","010101"},
        };

		static Dictionary<string, string> jumpDict = new Dictionary<string, string>()
        {
            {"",     "000"},
            {"JGT",  "001"},
            {"JEQ",  "010"},
            {"JGE",  "011"},
            {"JLT",  "100"},
            {"JNE",  "101"},
            {"JLE",  "110"},
            {"JMP",  "111"},
        };

		static Dictionary<string, string> distDict = new Dictionary<string, string>()
        {
            {"",     "000"},
            {"M",    "001"},
            {"D",    "010"},
            {"A",    "100"},
        };

	}
}
