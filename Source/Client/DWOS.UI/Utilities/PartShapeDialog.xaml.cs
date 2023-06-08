using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DWOS.UI.Utilities
{
    /// <summary>
    /// Interaction logic for PartShapeDialog.xaml
    /// </summary>
    public partial class PartShapeDialog : Window
    {
        #region Fields

        private WindowDataContext viewModel;

        #endregion

        #region Properties

        /// <summary>
        /// Gets part data for this instance.
        /// </summary>
        public PartShapeData Data
        {
            get
            {
                PartShapeData returnValue = null;

                if (viewModel != null)
                {
                    returnValue = viewModel.Data;
                }

                return returnValue;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Default constructor.
        /// </summary>
        public PartShapeDialog()
        {
            InitializeComponent();
            Icon = Properties.Resources.Part_32.ToWpfImage();
        }

        /// <summary>
        /// Loads part shape data into this instance.
        /// </summary>
        /// <param name="data"></param>
        public void LoadData(PartShapeData data)
        {
            viewModel = new WindowDataContext(data.Copy());
            viewModel.Exit += ViewModel_Exit;
            DataContext = viewModel;
        }

        #endregion

        #region Events

        void ViewModel_Exit(object sender, EventArgs e)
        {
            DialogResult = true;
        }

        #endregion

        #region WindowDataContext

        /// <summary>
        /// Class for the window's DataContext.
        /// </summary>
        private sealed class WindowDataContext : INotifyPropertyChanged
        {

            #region Fields

            /// <summary>
            /// Occurs when the data context receives a request to close the window.
            /// </summary>
            public event EventHandler Exit;

            #endregion

            #region Properties

            /// <summary>
            /// Gets the part shape data for this instance.
            /// </summary>
            public PartShapeData Data
            {
                get; private set;
            }

            /// <summary>
            /// Gets the list of part shapes to show.
            /// </summary>
            public IList<string> PartShapes
            {
                get; private set;
            }

            /// <summary>
            /// Gets the list of 'units of measure' combobox items to show.
            /// </summary>
            public IList<KeyValuePair<string, UnitOfMeasure>> UnitsOfMeasure
            {
                get; private set;
            }

            /// <summary>
            /// Source of the 'part shape' image.
            /// </summary>
            public string ImageSource
            {
                get
                {
                    string returnValue;

                    if (Data != null)
                    {
                        switch (Data.ShapeType)
                        {
                            case "Box":
                                returnValue = "/DWOS.UI;component/Resources/images/part_shape_editor/box.png";
                                break;
                            case "Cone":
                                returnValue = "/DWOS.UI;component/Resources/images/part_shape_editor/cone.png";
                                break;
                            case "Cylinder":
                                returnValue = "/DWOS.UI;component/Resources/images/part_shape_editor/cylinder.png";
                                break;
                            case "Washer":
                                returnValue = "/DWOS.UI;component/Resources/images/part_shape_editor/washer.png";
                                break;
                            case "Donut":
                                returnValue = "/DWOS.UI;component/Resources/images/part_shape_editor/donut.png";
                                break;
                            case "Plane":
                                returnValue = "/DWOS.UI;component/Resources/images/part_shape_editor/plane.png";
                                break;
                            case "Pyramid":
                                returnValue = "/DWOS.UI;component/Resources/images/part_shape_editor/pyramid.png";
                                break;
                            case "Sphere":
                                returnValue = "/DWOS.UI;component/Resources/images/part_shape_editor/sphere.png";
                                break;
                            case "Rotor":
                                returnValue = "/DWOS.UI;component/Resources/images/part_shape_editor/rotor.png";
                                break;
                            case "Screw":
                                returnValue = "/DWOS.UI;component/Resources/images/part_shape_editor/screw.png";
                                break;
                            case "Bolt":
                                returnValue = "/DWOS.UI;component/Resources/images/part_shape_editor/bolt.png";
                                break;
                            case "HexNut":
                                returnValue = "/DWOS.UI;component/Resources/images/part_shape_editor/hex_nut.png";
                                break;
                            case "SteelWire":
                                returnValue = "/DWOS.UI;component/Resources/images/part_shape_editor/steel_wire.png";
                                break;
                            case "SteelStamping":
                                returnValue = "/DWOS.UI;component/Resources/images/part_shape_editor/steel_stamping.png";
                                break;
                            default:
                                returnValue = string.Empty;
                                break;
                        }
                    }
                    else
                    {
                        returnValue = string.Empty;
                    }

                    return returnValue;
                }
            }

            /// <summary>
            /// Gets the confirm command.
            /// </summary>
            public ICommand ConfirmCommand
            {
                get; private set;
            }

            #endregion

            #region Methods

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="data">part shape data to use</param>
            public WindowDataContext(PartShapeData data)
            {
                Data = data;
                PartShapes = new List<string>(PartShapeData.SHAPES);
                UnitsOfMeasure = new List<KeyValuePair<string, UnitOfMeasure>>()
                {
                    new KeyValuePair<string, UnitOfMeasure>("Inches", UnitOfMeasure.Inch),
                    new KeyValuePair<string, UnitOfMeasure>("Feet", UnitOfMeasure.Feet),
                };

                Data.PropertyChanged += Data_PropertyChanged;

                ConfirmCommand = new ConfirmCommand(this);
            }

            private void OnPropertyChanged(string name)
            {
                var handler = PropertyChanged;

                if (handler != null)
                {
                    handler(this, new PropertyChangedEventArgs(name));
                }
            }

            internal void OnExit()
            {
                var handler = Exit;

                if (handler != null)
                {
                    handler(this, new EventArgs());
                }
            }

            #endregion

            #region Events

            void Data_PropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == "ShapeType")
                {
                    // Trigger update for calculated property.
                    OnPropertyChanged("ImageSource");
                }
            }

            #endregion

            #region INotifyPropertyChanged Members

            public event PropertyChangedEventHandler PropertyChanged;

            #endregion
        }

        #endregion

        #region ConfirmCommand

        /// <summary>
        /// Implementation of WindowDataContext's ConfirmCommand.
        /// </summary>
        private sealed class ConfirmCommand : ICommand
        {
            #region Fields

            private bool _canExecute = true;

            #endregion

            #region Properties

            /// <summary>
            /// Gets the parent context.
            /// </summary>
            /// <remarks>
            /// An instance of this command listens to changes in context
            /// data to change 'can execute' status.
            /// </remarks>
            public WindowDataContext Context
            {
                get; private set;
            }

            #endregion

            #region Methods

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="context">window data context to use</param>
            public ConfirmCommand(WindowDataContext context)
            {
                Context = context;
                Context.Data.PropertyChanged += Data_PropertyChanged;
            }


            private void OnCanExecuteChanged()
            {
                var handler = CanExecuteChanged;

                if (handler != null)
                {
                    handler(this, new EventArgs());
                }
            }

            #endregion

            #region Events

            void Data_PropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                bool canExecute = string.IsNullOrEmpty(Context.Data.Error);

                if (_canExecute != canExecute)
                {
                    _canExecute = canExecute;
                    OnCanExecuteChanged();
                }
            }

            #endregion

            #region ICommand Members

            public bool CanExecute(object parameter)
            {
                return _canExecute;
            }

            public event EventHandler CanExecuteChanged;

            public void Execute(object parameter)
            {
                if (_canExecute)
                {
                    Context.OnExit();
                }
            }

            #endregion
        }

        #endregion
    }
}
