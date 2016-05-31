using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler
{
    class Program
    {
        static void Main(string[] args)
        {
            String inputpath = Path.GetFullPath(args[0]);
            String outputpath = Path.GetFileNameWithoutExtension(inputpath) + ".hack";

            StreamWriter outputwriter = new StreamWriter(outputpath);//, Encoding.UTF8);
            Parser prs = new Parser(inputpath);

            int romAddress = 0;
            while (prs.hasMoreCommands())
            {
                switch (prs.commandtype)
                {
                    case Parser.commandtypes.A_COMMAND:
                    case Parser.commandtypes.C_COMMAND:
                        romAddress++;
                        break;
                    case Parser.commandtypes.L_COMMAND:
                        SymbolTable.addEntry(prs.symbol, romAddress);
                        break;
                    default:
                        break;
                }
            }

            prs.rewind();

            int ramAddress = 16;
            while (prs.hasMoreCommands())
            {

                String line = "";
                switch (prs.commandtype)
                {
                    case Parser.commandtypes.A_COMMAND:
                        Int16 number;
                        //直値の時。string -> int16 -> 2進数 -> 15桁にパディング　の流れ
                        if (Int16.TryParse(prs.symbol, out number))
                        {
                            line = "0" + Convert.ToString(number, 2).PadLeft(15, '0');
                        }
                        //ラベルシンボルの時。symbolDictは10進intなので2進数stringに変換
                        else if (SymbolTable.symbolDict.ContainsKey(prs.symbol))
                        {
                            line = "0" + (Convert.ToString(SymbolTable.symbolDict[prs.symbol],2)).PadLeft(15, '0');
                        }
                        //変数の時
                        else
                        {
                            SymbolTable.addEntry(prs.symbol, ramAddress);
                            line = "0" + (Convert.ToString(ramAddress,2)).PadLeft(15, '0');
                            ramAddress++;
                        }


                        outputwriter.WriteLine(line);
                        break;
                    case Parser.commandtypes.C_COMMAND:
                        line = "111" + Code.compDict[prs.comp] + Code.destDict[prs.dest] + Code.jumpDict[prs.jump];
                        outputwriter.WriteLine(line);
                        break;
                    case Parser.commandtypes.L_COMMAND:
                        break;
                    default:
                        break;
                }
                
            }

            outputwriter.Close();
            prs.close();
        }



        /// <summary>
        /// 10進数のintかstringを2進数のStringして返す
        /// </summary>
        /// <param name="str"></param>
        /// <param name="totalWidth">桁数</param>
        /// <returns>digit桁の2進数のString、変換できないときは空</returns>
        /*
        static string ToBinaryString(object input, int totalWidth)// where T : Object
        {
            //string -> int(10進) -> int(2進) -> 15桁にパディング　の流れ
            int deci;
            if (input.GetType == typeof(String))
            {
                deci = int.Parse(input);
            }


            return "";
        }*/
    }
}
