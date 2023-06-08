using DWOS.Data.Datasets;
using System;

namespace DWOS.Data
{
    public interface IWorkScheduler
    {
        DateTime StartDate { get; }
        WorkScheduleSettings ScheduleSettings { get; set; }
        ScheduleDataset Data { get; }
        void LoadData();
    }


}
