using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMtranslator
{
	class Program
	{
		static void Main(string[] args)
		{

			//String inputpath = Path.GetFullPath(args[0]);
			//String inputpath = @"C:\Users\10001176180\Documents\16FY_教育研修\コンピュータ実装教育\nand2tetris\projects\07\StackArithmetic\SimpleAdd\SimpleAdd.vm";
			//String inputpath = @"C:\Users\10001176180\Documents\16FY_教育研修\コンピュータ実装教育\nand2tetris\projects\07\StackArithmetic\StackTest\StackTest.vm";
			//String inputpath = @" C:\Users\10001176180\Documents\16FY_教育研修\コンピュータ実装教育\nand2tetris\projects\07\MemoryAccess\BasicTest\BasicTest.vm";
			String inputpath = @"C:\Users\10001176180\Documents\16FY_教育研修\コンピュータ実装教育\nand2tetris\projects\07\MemoryAccess\PointerTest\PointerTest.vm";
			//String inputpath = @"C:\Users\10001176180\Documents\16FY_教育研修\コンピュータ実装教育\nand2tetris\projects\07\MemoryAccess\StaticTest\StaticTest.vm";

			String outputpath = Path.ChangeExtension(inputpath, ".asm");

            Parser prs = new Parser(inputpath);
			CodeWriter cw = new CodeWriter(outputpath);

            while (prs.hasMoreCommands())
            {
                switch (prs.commandtype)
                {
                    case Parser.CommandTypes.C_ARITHMETIC:
						cw.WriteArithmetic(prs.arg1);
						break;
					case Parser.CommandTypes.C_PUSH:
					case Parser.CommandTypes.C_POP:
						cw.writePushPop(prs.commandtype,prs.arg1,prs.arg2);
                        break;
                    default:
                        break;
                }
            }
			cw.close();


		}
	}
}
