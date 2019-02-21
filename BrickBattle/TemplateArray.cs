using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Drawing;
namespace WindowsFormsApplication4
{
    class TemplateArray//模組陣列
    {
        private ArrayList list = new ArrayList();//集合列表
        
        //取得模組數量
        public int Count
        {
            get { return list.Count; }
        }
        //取得一個模組
        public BrickTemplate this[int index]
        {
            get { return (BrickTemplate)list[index]; }
        }
        //新增模組
        public void add(string code,Color color)
        {
            list.Add(new BrickTemplate(code, color));
        }
        //清除模組
        public void Clear()
        {
            list.Clear();
        }
    }
}
