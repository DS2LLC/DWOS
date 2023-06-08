using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DWOS.UI.Sales.ViewModels
{
    public interface ICustomFieldViewModel
    {
        int CustomFieldId { get; }

        string Name { get; }

        bool IsRequired { get; }

        string Description { get; }

        string Value { get; set; }
    }

}
