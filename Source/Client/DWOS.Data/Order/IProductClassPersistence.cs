namespace DWOS.Data.Order
{
    public interface IProductClassPersistence
    {
        ProductClassItem RetrieveForOrder(int orderId);

        void Update(ProductClassItem item);
    }
}
