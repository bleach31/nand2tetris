using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler
{
	class Program
	{
		static void Main(string[] args)
		{
			String filepath = @"C:\Users\10001176180\Documents\16FY_教育研修\コンピュータ実装教育\nand2tetris\projects\06\add\Add.asm";
			String filepath2 = @"C:\Users\10001176180\Documents\16FY_教育研修\コンピュータ実装教育\nand2tetris\projects\06\max\Max.asm";


			Parser test = new Parser(filepath2);
			while (test.hasMoreCommands())
			{
				Console.WriteLine("{0} \t\t {1},{2},{3},{4},{5}", test.line, test.commandtype, test.symbol, test.comp, test.dest, test.jump);
			}
			// Suspend the screen.
			Console.ReadLine();
		}
	}
}
