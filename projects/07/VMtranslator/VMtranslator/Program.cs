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
			/*
			List<String> vmfiles = new List<string>() { };

			vmfiles.Add(@"C:\Users\10001176180\Documents\16FY_教育研修\コンピュータ実装教育\nand2tetris\projects\07\StackArithmetic\SimpleAdd\SimpleAdd.vm");
			vmfiles.Add(@"C:\Users\10001176180\Documents\16FY_教育研修\コンピュータ実装教育\nand2tetris\projects\07\StackArithmetic\StackTest\StackTest.vm");
			vmfiles.Add(@" C:\Users\10001176180\Documents\16FY_教育研修\コンピュータ実装教育\nand2tetris\projects\07\MemoryAccess\BasicTest\BasicTest.vm");
			vmfiles.Add(@"C:\Users\10001176180\Documents\16FY_教育研修\コンピュータ実装教育\nand2tetris\projects\07\MemoryAccess\PointerTest\PointerTest.vm");
			vmfiles.Add(@"C:\Users\10001176180\Documents\16FY_教育研修\コンピュータ実装教育\nand2tetris\projects\07\MemoryAccess\StaticTest\StaticTest.vm");
			*/
			//vmfiles.Add(@"C:\Users\c2010\Documents\git\nand2tetris\projects\08\ProgramFlow\BasicLoop\BasicLoop.vm");
			//vmfiles.Add(@"C:\Users\c2010\Documents\git\nand2tetris\projects\08\ProgramFlow\FibonacciSeries\FibonacciSeries.vm");

			//String dir = @"C:\github\nand2tetris\projects\08\FunctionCalls\StaticsTest";
			String dir = @"C:\Users\c2010\Documents\git\nand2tetris\projects\08\FunctionCalls\FibonacciElement";
			
			//指定フォルダのvmファイルを全部とってくる。サブフォルダはみない。
			string[] files = System.IO.Directory.GetFiles(dir, "*.vm", System.IO.SearchOption.TopDirectoryOnly);
			
			String outputpath = dir +"\\" +Path.GetFileName(dir)+".asm";
			CodeWriter cw = new CodeWriter(outputpath);

			//もしSys.vmがあればWriteInitを呼ぶ
			foreach(String filepath in files)
			{
				if(filepath.EndsWith("Sys.vm"))
				{
					cw.WriteInit();
				}
					
			}
			//
			for (int i = 0; i < files.Length; i++)
			{
				String inputpath = files[i];
				Parser prs = new Parser(inputpath);

				while (prs.hasMoreCommands())
				{
#if DEBUG
					cw.m_file_writer.WriteLine("\t//"+prs.line);
#endif
					switch (prs.commandtype)
					{
						case Parser.CommandTypes.C_ARITHMETIC:
							cw.writeArithmetic(prs.arg1);
							break;
						case Parser.CommandTypes.C_PUSH:
						case Parser.CommandTypes.C_POP:
							cw.writePushPop(prs.commandtype,prs.arg1,prs.arg2);
							break;
                        case Parser.CommandTypes.C_LABEL:
                            cw.writeLabel(prs.arg1);
                            break;
						case Parser.CommandTypes.C_GOTO:
							cw.writeGoto(prs.arg1);
							break;
						case Parser.CommandTypes.C_IF:
							cw.writeIf(prs.arg1);
							break;
						case Parser.CommandTypes.C_FUNCTION:
							cw.writeFunction(prs.arg1,prs.arg2);
							break;
						case Parser.CommandTypes.C_RETURN:
							cw.writeReturn();
							break;
						case Parser.CommandTypes.C_CALL:
							cw.writeCall(prs.arg1, prs.arg2);
							break;
						default:
							break;
					}
				}
				Console.WriteLine("Done[{0}/{1}]:" + inputpath,i+1, files.Length);
			}
			cw.close();
			Console.WriteLine("Complete:" + outputpath);
			Console.WriteLine("press enter to exit");
			Console.ReadLine(); 
		}
	}
}
