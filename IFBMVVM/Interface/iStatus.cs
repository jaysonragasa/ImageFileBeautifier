using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace IFBMVVM.Interface
{
    public interface iStatus
    {
        string LogText { get; set; }
        int RenamedFiles { get; set; }
        bool TrialOver { get; set; }
    }

    public class Status
    {
        iStatus _stat;

        public Status(iStatus stat)
        {
            this._stat = stat;
        }

        public Status()
        {
        }
        static readonly Status _i = new Status();
        public static Status Instance { get { return _i; } }

        public void LogText(string text)
        {
            Debug.WriteLine("**** " + text);
        }
    }
}
