using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace DWOS.Dashboard.Production
{

    /// <summary>
    /// Used in showing a colored strip in an
    /// <see cref="Infragistics.Controls.Charts.XamDataChart"/> instance.
    /// </summary>
    public class InfoStrip : DependencyObject, INotifyPropertyChanged
    {
        #region Constructors
        public InfoStrip()
        {
            _id = Guid.NewGuid();
            StartX = double.NaN;
            EndX = double.NaN;
            StartY = double.NaN;
            EndY = double.NaN;
        }
        private InfoStrip(Guid id)
        {
            _id = id;
            StartX = double.NaN;
            EndX = double.NaN;
            StartY = double.NaN;
            EndY = double.NaN;
        }
        #endregion

        #region Properties
        private Guid _id;
        public Guid Id
        {
            get { return _id; }
            private set { _id = value; }
        }

        public double StartX { get; set; }
        public double EndX { get; set; }
        public double StartY { get; set; }
        public double EndY { get; set; }

        private string _startDateString;
        public string StartDateString
        {
            get
            {
                return _startDateString;
            }
            set
            {
                if (string.IsNullOrEmpty(value)) return;
                _startDateString = value;
                StartDate = DateTime.Parse(_startDateString);
            }
        }
        private string _endDateString;
        public string EndDateString
        {
            get
            {
                return _endDateString;
            }
            set
            {
                if (string.IsNullOrEmpty(value)) return;
                _endDateString = value;
                EndDate = DateTime.Parse(_endDateString);
            }
        }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool UseDates { get; set; }

        public Brush Fill { get; set; }
        public DataTemplate StripTemplate { get; set; }

        #region Dependency Properties
        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register(
         "Label", typeof(string), typeof(InfoStrip), new PropertyMetadata(new PropertyChangedCallback(OnLabelChanged)));

        private static void OnLabelChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            InfoStrip owner = (InfoStrip)obj;
            owner.OnPropertyChanged("Label");
        }
        public string Label
        {
            get { return (string)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        #endregion
        public double Width
        {
            get { return System.Math.Abs(EndX - StartX); }
        }
        public double Height
        {
            get { return System.Math.Abs(EndY - StartY); }
        }

        #endregion

        #region Event Handlers
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string name)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
        #endregion

        #region Methods
        public InfoStrip Clone()
        {
            InfoStrip newStrip = new InfoStrip(_id);
            newStrip.StartX = StartX;
            newStrip.StartY = StartY;
            newStrip.EndX = EndX;
            newStrip.EndY = EndY;
            newStrip.Label = Label;
            newStrip.StripTemplate = StripTemplate;
            newStrip.Fill = Fill;
            newStrip.UseDates = UseDates;
            newStrip.StartDate = StartDate;
            newStrip.EndDate = EndDate;

            return newStrip;
        }
        #endregion
    }
}
