namespace DWOS.Android.Utilities
{
    public class ValueItem<T>
    {
        public T Value { get; }

        public string DisplayText { get; }

        public ValueItem(T value, string displayText)
        {
            Value = value;
            DisplayText = displayText;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return DisplayText;
        }
    }
}