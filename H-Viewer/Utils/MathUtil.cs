using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HViewer.Utils
{
    public class MathUtil
    {
        private static Regex InBracket = new Regex(@"\(([0-9\+\-\*\/\.]+)\)");//正则切割括号
        private static Regex TwoNumberMD = new Regex(@"\(?(-?\d+(\.\d+)?)\)?([\*\/])\(?(-?\d+(\.\d+)?)\)?");//正则切分乘除
        private static Regex TwoNumberAE = new Regex(@"\(?(-?\d+(\.\d+)?)\)?([+-])\(?(-?\d+(\.\d+)?)\)?");//正则切分加减

        private string CalcTwoNumber(string left, string oper, string right)//计算两个数
        {
            switch (oper)
            {
                case "+": return (Convert.ToDecimal(left) + Convert.ToDecimal(right)).ToString();
                case "-": return (Convert.ToDecimal(left) - Convert.ToDecimal(right)).ToString();
                case "*": return (Convert.ToDecimal(left) * Convert.ToDecimal(right)).ToString();
                case "/": return (Convert.ToDecimal(left) / Convert.ToDecimal(right)).ToString();
                default: return string.Empty;
            }
        }
        private string CalcExpressNoBracket(string exp)//计算数字表达式
        {
            Match m = null;
            while (true)
            {
                m = TwoNumberMD.Match(exp);//匹配乘除法
                if (m.Success)//成功
                    exp = CalcReplace(m, exp);//进行数字计算
                else//否则
                {
                    m = TwoNumberAE.Match(exp);//匹配加减
                    if (m.Success)//成功
                        exp = CalcReplace(m, exp);//进行数字计算
                    else
                        break;//两项都失败则退出
                }
            }
            return exp;
        }
        private string CalcReplace(Match m, string express)
        {
            string result = CalcTwoNumber(m.Groups[1].Value, m.Groups[3].Value, m.Groups[4].Value);//把匹配到的式子【数字+数字】
            express = express.Replace(m.Groups[0].Value, result);//把表达式替换为结果
            return express;//返回结果
        }
        public string RunExpress(string Exp)
        {
            Match m = null;
            while (true)
            {
                m = InBracket.Match(Exp);//匹配括号【（数字+数字）】
                if (m.Success)//成功以后
                    Exp = Exp.Replace(m.Groups[0].Value, CalcExpressNoBracket(m.Groups[1].Value));//对括号内的字符进行运算，并且用结果替换掉原来的式子
                else//没成功
                    break;//退出循环

            }
            return CalcExpressNoBracket(Exp);//没括号以后的最后结果进行计算
        }
    }
}
