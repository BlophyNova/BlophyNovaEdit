using System.Collections.Generic;

namespace Container.SortHeap
{
    public class SortHeap
    {
        public SortHeap()
        {

        }
        public SortHeap(string a)
        {
            string[] cmds = a.Split(' ');
            Count = cmds.Length;
            int idx = 0;
            foreach (string i in cmds)
            {
                orderToPosi[idx] = int.Parse(i);
                idx++;
            }
            for (int i = 0; i < cnt; i++)
            {
                posiToOrder[orderToPosi[i]] = i; 
            }
        }
        public int Count
        {
            get
            {
                return cnt;
            }
            set
            {
                int v = value;
                if (v > cnt)
                {
                    while (v > cnt)
                    {
                        posiToOrder.Add(cnt);
                        orderToPosi.Add(cnt);
                        cnt++;
                    }
                }
                else
                {
                    //强制剔除最后一个位置
                    while (v < cnt)
                    {
                        cnt--;
                        posiToOrder.Remove(orderToPosi[^1]);
                        orderToPosi.RemoveAt(cnt);
                    }
                }
            }
        }
        private int cnt = 0;
        private List<int> posiToOrder = new(), orderToPosi = new();
        public int GetOrder(int x)
        {
            return posiToOrder[x];
        }
        public void Add()
        {
            posiToOrder.Add(cnt);
            orderToPosi.Add(cnt);
            cnt++;
        }
        public void MoveUp(int x)
        {
            if (x == 0) return;
            (posiToOrder[orderToPosi[x - 1]], posiToOrder[orderToPosi[x]]) = (posiToOrder[orderToPosi[x]], posiToOrder[orderToPosi[x - 1]]);
            (orderToPosi[x], orderToPosi[x - 1]) = (orderToPosi[x - 1], orderToPosi[x]);
        }
        public void MoveDown(int x)
        {
            if (x == cnt - 1) return;
            (posiToOrder[orderToPosi[x + 1]], posiToOrder[orderToPosi[x]]) = (posiToOrder[orderToPosi[x]], posiToOrder[orderToPosi[x + 1]]);
            (orderToPosi[x], orderToPosi[x + 1]) = (orderToPosi[x + 1], orderToPosi[x]);
        }
        public void Remove(int x)
        {
            cnt--;
            posiToOrder.Remove(orderToPosi[x]);
            orderToPosi.RemoveAt(x);
            for (int i = 0; i < cnt; i++)
            {
                if (orderToPosi[i] > x) orderToPosi[i]--;
            }
        }
        public override string ToString()
        {
            string a = "";
            foreach (int i in orderToPosi)
            {
                a += $"{i} ";
            }
            return a;
        }
    }
}
