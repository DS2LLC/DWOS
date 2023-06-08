using System;

namespace DWOS.Data.Order
{
    public class ProductClassItem : IEquatable<ProductClassItem>
    {
        #region Properties

        public string Name { get; set; }

        public string AccountingCode { get; set; }

        #endregion

        public override bool Equals(object obj) =>
            Equals(obj as ProductClassItem);

        public override int GetHashCode() =>
            new[] { Name, AccountingCode }.GetHashCode();

        public static bool operator ==(ProductClassItem lhs, ProductClassItem rhs)
        {
            if (lhs is null)
            {
                return rhs is null;
            }

            return Equals(lhs, rhs);
        }

        public static bool operator !=(ProductClassItem lhs, ProductClassItem rhs) =>
            !(lhs == rhs);

        #region IEquatable<ProductClassItem> Members

        public bool Equals(ProductClassItem other)
        {
            if (other == null)
            {
                return false;
            }

            return Name == other.Name
                && AccountingCode == other.AccountingCode;
        }

        #endregion
    }
}
