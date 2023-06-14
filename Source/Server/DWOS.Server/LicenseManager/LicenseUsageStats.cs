using System;
using System.Collections.Generic;
using System.Linq;

namespace DWOS.LicenseManager
{
    public class LicenseUsageStats
    {
        #region Properties
        
        public DateTime StatStartTime { get; set; }
        
        public int TotalActivations { get; set; }
        
        public int TotalDeActivations { get; set; }
        
        public int TotalPrunes { get; set; }
        
        public int TotalOutOfLicense { get; set; }
        
        public Dictionary<int, int> ActivationsByHour { get; set; }
      
        #endregion

        #region Methods
        
        public LicenseUsageStats()
        {
            ActivationsByHour = new Dictionary<int, int>();
            Reset();
        }

        public void AddActivation()
        {
            if (ActivationsByHour.ContainsKey(DateTime.Now.Hour))
                ActivationsByHour[DateTime.Now.Hour] = ActivationsByHour[DateTime.Now.Hour] + 1;
            else
                ActivationsByHour.Add(DateTime.Now.Hour, 1);
        }

        public int GetMaxActivations()
        {
             return this.ActivationsByHour.Count > 0 ? ActivationsByHour.Max(kvp => kvp.Value) : 0;
        }

        public int GetMinActivations()
        {
             return this.ActivationsByHour.Count > 0 ? ActivationsByHour.Min(kvp => kvp.Value) : 0;
        }

        public decimal GetAvgActivations()
        {
            return this.ActivationsByHour.Count > 0 ? Convert.ToDecimal(this.ActivationsByHour.Average(kvp => kvp.Value)) : 0;
        }

        public void Reset()
        {
            StatStartTime = DateTime.Now;
            this.TotalActivations = 0;
            this.TotalDeActivations = 0;
            this.TotalPrunes = 0;
            this.TotalOutOfLicense = 0;
            ActivationsByHour.Clear();
        }
          
        #endregion
  }
}
