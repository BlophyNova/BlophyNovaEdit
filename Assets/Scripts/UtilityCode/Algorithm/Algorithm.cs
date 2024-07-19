using System;
using System.Collections.Generic;
using UnityEngine;
using Event = Data.ChartData.Event;

namespace UtilityCode.Algorithm
{
    public class Algorithm
    {
        /// <summary>
        /// 二分查找算法
        /// 这里用的是左开右开的算法
        /// </summary>
        /// <param name="list">给我一个列表</param>
        /// <param name="match">查找规则</param>
        /// <param name="isLeft">返回左边界还是右边界，根据需求选择True或者False</param>
        /// <returns>返回下标</returns>
        public static int BinarySearch<T>(T[] list, Predicate<T> match, bool isLeft)
        {
            int left = -1;//左初始化为-1
            int right = list.Length;//右初始化为数量
            int middle;//m无默认值
            while (left + 1 != right)//如果l和r的下标没有挨在一起
            {
                middle = (left + right) / 2;//将数据除2
                if (match(list[middle]))
                {
                    left = middle;//更新右边界
                }
                else//否则
                {
                    right = middle;//更新左边界
                }
            }
            return isLeft switch
            {
                true => left,
                false => right
            };//返回最终结果
        }
        public delegate bool Pre(Event obj, ref float currentTime);
        public static int BinarySearch(Event[] list, Pre match, bool isLeft, ref float currentTime)
        {
            int left = -1;//左初始化为-1
            int right = list.Length;//右初始化为数量
            while (left + 1 != right)//如果l和r的下标没有挨在一起
            {
                int middle = (left + right) / 2;//m无默认值
                if (match(list[middle], ref currentTime))
                {
                    left = middle;//更新右边界
                }
                else//否则
                {
                    right = middle;//更新左边界
                }
            }
            return isLeft switch
            {
                true => left,
                false => right
            };//返回最终结果
        }
        /// <summary>
        /// 二分查找算法
        /// 这里用的是左开右开的算法
        /// </summary>
        /// <param name="list">给我一个列表</param>
        /// <param name="match">查找规则</param>
        /// <param name="isLeft">返回左边界还是右边界，根据需求选择True或者False</param>
        /// <returns>返回下标</returns>
        public static int BinarySearch<T>(List<T> list, Predicate<T> match, bool isLeft)
        {
            int left = -1;//左初始化为-1
            int right = list.Count;//右初始化为数量
            while (left + 1 != right)//如果l和r的下标没有挨在一起
            {
                int middle = (left + right) / 2;//m无默认值
                if (match(list[middle]))
                {
                    left = middle;//更新右边界
                }
                else//否则
                {
                    right = middle;//更新左边界
                }
            }
            return isLeft switch
            {
                true => left,
                false => right
            };//返回最终结果
        }
        public static int BinaryStrictSearch(Keyframe[] list, float targetTime)
        {
            int l = -1;//左初始化为-1
            int r = list.Length;//右初始化为数量
            int m;//m无默认值
            while (l + 1 != r)//如果l和r的下标没有挨在一起
            {
                m = (l + r) / 2;//将数据除2
                if (list[m].time > targetTime)//如果大于我要找的数据
                {
                    r = m;//更新右边界
                }
                else//否则
                {
                    l = m;//更新左边界
                }
            }
            if (list.Length == 0) return -1;
            return Math.Abs(list[l].time - targetTime) < 0.0000000001 ? l : -1;//返回最终结果
        }
    }
}
