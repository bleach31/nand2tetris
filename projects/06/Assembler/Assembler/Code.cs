using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler
{
	static class Code
	{
        public static Dictionary<string, string> compDict = new Dictionary<string, string>()
        {
            {"0", "0101010"},
            {"1", "0111111"},
            {"-1", "0111010"},
            {"D", "0001100"},
            {"A", "0110000"},
            {"!D", "0001101"},
            {"!A", "0110001"},
            {"-D", "0001111"},
            {"-A", "0110011"},
            {"D+1", "0011111"},
            {"A+1", "0110111"},
            {"D-1", "0001110"},
            {"A-1", "0110010"},
            {"D+A", "0000010"},
            {"A+D", "0000010"},
            {"D-A", "0010011"},
            {"A-D", "0000111"},
            {"D&A", "0000000"},
            {"D|A", "0010101"},
            {"M", "1110000"},
            {"!M", "1110001"},
            {"-M", "1110011"},
            {"M+1", "1110111"},
            {"M-1", "1110010"},
            {"D+M", "1000010"},
            {"M+D", "1000010"}, //do we need both variations for addition?
            {"D-M", "1010011"},
            {"M-D", "1000111"},
            {"D&M", "1000000"},
            {"D|M", "1010101"},
        };

        public static Dictionary<string, string> jumpDict = new Dictionary<string, string>()
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

		public static Dictionary<string, string> destDict = new Dictionary<string, string>()
        {
            {"","000"},

            {"M","001"},
            {"D","010"},
            {"A","100"},

            {"MD","011"}, //2 variations
            {"DM","011"},

            {"AM","101"}, //2 variations
            {"MA","101"},

            {"AD","110"}, //2 variations
            {"DA","110"},

            {"AMD","111"}, //6 variations
            {"ADM","111"},
            {"MAD","111"},
            {"MDA","111"},
            {"DMA","111"},
            {"DAM","111"},
        };

      
	}
}
