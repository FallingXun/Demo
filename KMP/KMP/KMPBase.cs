using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMP
{
    public class KMPBase
    {
        public static int Find(string main, string child)
        {
            // 初始化子字符串的每个索引对应的下个匹配位置
            var next = GetNext(child);
            // 主字符串 Main 索引
            int m = 0;
            // 子字符串 Child 索引
            int c = 0;
            // 当主字符串的索引小于主字符串的长度，且子字符串的索引小于子字符串的长度时，循环进行匹配检查
            while (m < main.Length && c < child.Length)
            {
                if (main[m].Equals(child[c]))
                {
                    ++m;
                    ++c;
                }
                else
                {
                    // 如果 Main[m] 和 Child[c] 不相同，则子字符串的索引回退到下一个需要检查的位置
                    c = next[c];
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

        /// <summary>
        /// 获取字符串的每个索引对应的下个匹配位置
        /// </summary>
        /// <param name="child"></param>
        /// <returns></returns>
        public static int[] GetNext(string child)
        {
            // 字符串前缀索引（为了简化代码，初始化为 -1，不需要单独判断 prefix 为 0 的情况）
            int prefix = -1;
            // 字符串后缀索引
            int postifx = 0;
            // 字符串每个索引不匹配时跳转的索引，即下一个匹配的索引
            int[] next = new int[child.Length];
            // 初始化索引 0 的 next 值
            next[0] = -1;
            while (postifx < child.Length - 1)
            {
                if (prefix < 0 || child[prefix].Equals(child[postifx]))
                {
                    ++prefix;
                    ++postifx;
                    // 前缀和后缀字符相同，则回退到相同前缀的下一个字符
                    next[postifx] = prefix;
                }
                else
                {
                    // 如果前缀和后缀不相同，则缩短前缀，回退到上一个索引
                    prefix = next[prefix];
                }
            }
            return next;
        }
    }
}
