using DWOS.UI.Sales.Models;
using NLog;
using System.Collections.Generic;
using System.Linq;

namespace DWOS.UI.Sales.ViewModels
{
    public class PartViewModel : Utilities.ViewModelBase
    {
        private const int NEW_ID = -1;
        private int _partId;

        public int PartId
        {
            get => _partId;
            set => Set(nameof(PartId), ref _partId, value);
        }

        public string Name { get; set; }

        public decimal? Weight { get; }

        public bool IsPartMarking { get; }

        public List<PartProcess> Processes { get; private set; }

        public bool IsNew => PartId == NEW_ID;

        public PartViewModel()
        {
            _partId = NEW_ID;
        }

        public PartViewModel(int partId, string name, decimal? weight, bool isPartMarking)
        {
            _partId = partId;
            Name = name;
            Weight = weight;
            IsPartMarking = isPartMarking;
        }

        public void LoadProcesses(IEnumerable<PartProcess> processes)
        {
            Processes = processes?.ToList();
        }

        public static PartViewModel From(Part part)
        {
            if (part == null)
            {
                return null;
            }

            return new PartViewModel(part.PartId, part.Name, part.Weight, part.IsPartMarking);
        }
    }
}
