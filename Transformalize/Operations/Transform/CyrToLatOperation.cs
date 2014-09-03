using System.Collections.Generic;
using System.Linq;
using Transformalize.Libs.Rhino.Etl;

namespace Transformalize.Operations.Transform {
    public class CyrToLatOperation : ShouldRunOperation {

        private static readonly Dictionary<char, string> Map = new Dictionary<char, string>(){
            {'А',"A"},
            {'Б',"B"},
            {'В',"V"},
            {'Г',"G"},
            {'Ѓ',"G`"},
            {'Ґ',"G`"},
            {'Д',"D"},
            {'Е',"E"},
            {'Ё',"YO"},
            {'Є',"YE"},
            {'Ж',"ZH"},
            {'З',"Z"},
            {'Ѕ',"Z"},
            {'И',"I"},
            {'Й',"Y"},
            {'Ј',"J"},
            {'І',"I"},
            {'Ї',"YI"},
            {'К',"K"},
            {'Ќ',"K"},
            {'Л',"L"},
            {'Љ',"L"},
            {'М',"M"},
            {'Н',"N"},
            {'Њ',"N"},
            {'О',"O"},
            {'П',"P"},
            {'Р',"R"},
            {'С',"S"},
            {'Т',"T"},
            {'У',"U"},
            {'Ў',"U"},
            {'Ф',"F"},
            {'Х',"H"},
            {'Ц',"TS"},
            {'Ч',"CH"},
            {'Џ',"DH"},
            {'Ш',"SH"},
            {'Щ',"SHH"},
            {'Ъ',"``"},
            {'Ы',"YI"},
            {'Ь',"`"},
            {'Э',"E`"},
            {'Ю',"YU"},
            {'Я',"YA"},
            {'а',"a"},
            {'б',"b"},
            {'в',"v"},
            {'г',"g"},
            {'ѓ',"g"},
            {'ґ',"g"},
            {'д',"d"},
            {'е',"e"},
            {'ё',"yo"},
            {'є',"ye"},
            {'ж',"zh"},
            {'з',"z"},
            {'ѕ',"z"},
            {'и',"i"},
            {'й',"y"},
            {'ј',"j"},
            {'і',"i"},
            {'ї',"yi"},
            {'к',"k"},
            {'ќ',"k"},
            {'л',"l"},
            {'љ',"l"},
            {'м',"m"},
            {'н',"n"},
            {'њ',"n"},
            {'о',"o"},
            {'п',"p"},
            {'р',"r"},
            {'с',"s"},
            {'т',"t"},
            {'у',"u"},
            {'ў',"u"},
            {'ф',"f"},
            {'х',"h"},
            {'ц',"ts"},
            {'ч',"ch"},
            {'џ',"dh"},
            {'ш',"sh"},
            {'щ',"shh"},
            {'ь',""},
            {'ы',"yi"},
            {'ъ',"'"},
            {'э',"e`"},
            {'ю',"yu"},
            {'я',"ya"}
        };


        public CyrToLatOperation(string inKey, string outKey)
            : base(inKey, outKey) {
            Name = "Cyr-To-Lat (" + outKey + ")";
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                if (ShouldRun(row)) {
                    row[OutKey] = SlugOperation.GenerateSlug(CyrToLat(row[InKey].ToString()), 0);
                }
                yield return row;
            }
        }

        public static string CyrToLat(string cyrillic) {
            return string.Concat(cyrillic.Select(c => Map.ContainsKey(c) ? Map[c].ToLower() : " ")).Trim();
        }
    }
}