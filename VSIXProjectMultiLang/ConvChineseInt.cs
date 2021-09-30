using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSIXProjectMultiLang
{
    public class ConvChineseInt
    {
        public static string NumToChinese(string inputNum)
        {
            string[] intArr = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", };
            string[] strArr = { "零", "一", "二", "三", "四", "五", "六", "七", "八", "九", };
            string[] Chinese = { "", "十", "百", "千", "萬", "十", "百", "千", "億" };
            //金額
            //string[] Chinese = { "元", "十", "百", "千", "萬", "十", "百", "千", "億" };
            char[] tmpArr = inputNum.ToString().ToArray();
            string tmpVal = "";
            for (int i = 0; i < tmpArr.Length; i++)
            {
                tmpVal += strArr[tmpArr[i] - 48];//ASCII編碼 0為48
                tmpVal += Chinese[tmpArr.Length - 1 - i];//根據對應的位數插入對應的單位
            }

            return tmpVal;
        }




        /// <summary>
        /// 將國字數字轉換為阿拉伯數字 - 僅支援一億以下之數字; 若格式有誤, 可能出現 Exception
        /// </summary>
        /// <param name="cnum">請輸入正確格式之國字數字, 如 "八千四百二十一萬三千五百六十三"</param>
        public static int ConvChineseNumber(string cnum)
        {
            cnum = cnum.Replace( "一十", "十" );

            cnum = cnum.TrimEnd( );
            char[] cnums = cnum.ToCharArray( );
            int sum = 0, sectionUnit = 0, sectionsum = 0;
            foreach (char c in cnums)
            {
                int arab = mapCnumLetters( c );
                if (isMultiplier( c ))
                {
                    if (isSegmentDelimeter( c )) // 萬/億
                    {
                        sectionsum = sum * arab;
                        sum = sectionsum;
                        if (sum < 0)
                            throw new Exception( "輸入的字串無法解析!" );
                        sectionsum = 0;
                    }
                    else // 十/百/千
                    {
                        if (sectionUnit == 0)
                            sectionsum = 10; // 特別處理 "十萬", "十一萬" 之類的狀況
                        else
                        {
                            sectionsum -= sectionUnit;
                            sum -= sectionUnit;
                            sectionsum = sectionUnit * arab;
                        }
                        sum += sectionsum;
                        if (sum < 0)
                            throw new Exception( "輸入的字串無法解析!" );
                    }
                }
                else
                {
                    sectionUnit = arab;
                    sum += arab;
                    if (sum < 0)
                        throw new Exception( "輸入的字串無法解析!" );
                    sectionsum += arab;
                }
            }
            return sum;
        }

        private static bool isSegmentDelimeter(char cnum)
        {
            switch (cnum)
            {
                case '萬':
                    return true;
                case '億':
                    return true;
                default:
                    return false;
            }
        }

        private static bool isMultiplier(char cnum)
        {
            switch (cnum)
            {
                case '十':
                    return true;
                case '拾':
                    return true;
                case '百':
                    return true;
                case '佰':
                    return true;
                case '千':
                    return true;
                case '仟':
                    return true;
                case '萬':
                    return true;
                case '億':
                    return true;
                default:
                    return false;
            }
        }

        private static int mapCnumLetters(char cnum)
        {
            switch (cnum)
            {
                case '零':
                    return 0;
                case '一':
                    return 1;
                case '壹':
                    return 1;
                case '二':
                    return 2;
                case '貳':
                    return 2;
                case '三':
                    return 3;
                case '參':
                    return 3;
                case '四':
                    return 4;
                case '肆':
                    return 4;
                case '五':
                    return 5;
                case '伍':
                    return 5;
                case '六':
                    return 6;
                case '陸':
                    return 6;
                case '七':
                    return 7;
                case '柒':
                    return 7;
                case '八':
                    return 8;
                case '捌':
                    return 8;
                case '九':
                    return 9;
                case '玖':
                    return 9;
                case '十':
                    return 10;
                case '拾':
                    return 10;
                case '廿':
                    return 20;
                case '丗':
                    return 30;
                case '百':
                    return 100;
                case '佰':
                    return 100;
                case '千':
                    return 1000;
                case '仟':
                    return 1000;
                case '萬':
                    return 10000;
                case '億':
                    return 100000000;
                default:
                    return 0;
            }
        }
    }
}
