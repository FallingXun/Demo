using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMP
{
    public class BruteForce
    {
        public static int Find(string main, string child)
        {
            // 主字符串 Main 索引
            int m = 0;
            // 子字符串 Child 索引
            int c = 0;
            // 当主字符串的索引小于主字符串的长度，且子字符串的索引小于子字符串的长度时，循环进行匹配检查
            while (m < main.Length && c < child.Length)
            {
                if (main[m].Equals(child[c]))
                {
                    // 如果 Main[m] 和 Child[c] 相同，则移动到下一个位置
                    ++m;
                    ++c;
                }
                else
                {
                    // 如果 Main[m] 和 Child[c] 不相同，则主字符串的索引回退到首个匹配成功字符的下一个位置，子字符串则重置
                    m = m - c + 1;
                    c = 0;
                }
            }
            if (c >= child.Length)
            {
                // 如果子字符串索引不小于子字符串的长度，则子字符串匹配完毕，找到了子字符串，匹配成功
                return m - c;
            }
            else
            {
                // 如果主字符串索引不小于主字符串的长度，则主字符串匹配完毕，没有找到子字符串，匹配失败
                return -1;
            }
        }
    }
}
