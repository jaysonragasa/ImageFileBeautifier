using IFBMVVM.Common.Enums;
using System;

namespace IFBMVVM.Interface
{
    public interface iNamingFormat
    {
        string YearPrefix { get; set; }
        string MonthPrefix { get; set; }
        string FileNameFormat { get; set; }
        TODO_WHEN_DUPLICATE TodoWhenDuplicate { get; set; }
        //bool UseDateModified { get; set; }
    }
}
