using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyTimer3rd.Models
{
    public static class TimerListFactory
    {
        public static TimerValueListModel Create()
        {
            return TimerValueListModel.Instance();
        }
    }
}
