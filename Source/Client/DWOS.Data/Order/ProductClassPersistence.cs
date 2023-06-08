using DWOS.Data.Datasets;
using DWOS.Data.Datasets.OrdersDataSetTableAdapters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DWOS.Data.Order
{
    public class ProductClassPersistence : IProductClassPersistence
    {
        #region Fields

        private IDictionary<string, ProductClassItem> _productClassLookup;
        private readonly object _lookupLock = new object();

        #endregion

        #region Properties

        private static IDictionary<string, ProductClassItem> CreateProductClassLookup()
        {
            var lookup = new Dictionary<string, ProductClassItem>();
            using (var taProductClass = new ProductClassTableAdapter())
            {
                using (var dtProductClass = taProductClass.GetData())
                {
                    foreach (var productClassRow in dtProductClass)
                    {
                        var productClass = new ProductClassItem
                        {
                            Name = productClassRow.Name,
                            AccountingCode = productClassRow.IsAccountingCodeNull()
                                ? null
                                : productClassRow.AccountingCode
                        };

                        lookup.Add(productClassRow.Name,
                             productClass);
                    }
                }
            }

            return lookup;
        }

        #endregion

        #region IProductClassPersistence Members

        public ProductClassItem RetrieveForOrder(int orderId)
        {
            IDictionary<string, ProductClassItem> lookup;

            lock (_lookupLock)
            {
                if (_productClassLookup == null)
                {
                    _productClassLookup = CreateProductClassLookup();
                }

                lookup = _productClassLookup;
            }

            // Retrieve product class for order
            using (var dtOrderProductClass = new OrdersDataSet.OrderProductClassDataTable())
            {
                using (var taOrderProductClass = new OrderProductClassTableAdapter())
                {
                    taOrderProductClass.FillByOrder(dtOrderProductClass, orderId);
                }

                var orderProductClassRow = dtOrderProductClass.FirstOrDefault();

                string productClass = null;

                if (orderProductClassRow != null)
                {
                    productClass = orderProductClassRow.IsProductClassNull()
                        ? null
                        : orderProductClassRow.ProductClass;
                }

                if (string.IsNullOrEmpty(productClass))
                {
                    return null;
                }

                if (lookup.ContainsKey(productClass))
                {
                    return lookup[productClass];
                }

                return new ProductClassItem
                {
                    Name = productClass
                };
            }
        }

        public void Update(ProductClassItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            // Save to database
            using (var dtProductClass = new OrdersDataSet.ProductClassDataTable())
            {
                using (var taProductClass = new ProductClassTableAdapter())
                {
                    taProductClass.Fill(dtProductClass);

                    var existingRow = dtProductClass.FirstOrDefault(pc => pc.Name == item.Name);

                    if (existingRow == null)
                    {
                        dtProductClass.AddProductClassRow(
                            item.Name,
                            item.AccountingCode,
                            null);
                    }
                    else
                    {
                        existingRow.AccountingCode = item.AccountingCode;
                    }

                    taProductClass.Update(dtProductClass);
                }
            }

            // Update cache
            lock (_productClassLookup)
            {
                _productClassLookup[item.Name] = item;
            }
        }

        #endregion
    }
}
