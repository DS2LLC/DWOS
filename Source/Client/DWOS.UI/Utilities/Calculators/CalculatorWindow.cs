using System;
using System.Collections.Generic;
using System.Windows;

namespace DWOS.UI.Utilities.Calculators
{
    /// <summary>
    /// Base class for price calculation windows.
    /// </summary>
    /// <remarks>
    /// Implementing classes must include a <see cref="CalculatorData"/>
    /// constructor for use with <see cref="CreateWindow(CalculatorData)"/>.
    /// </remarks>
    public abstract class CalculatorWindow : Window
    {
        #region Fields

        public const string TYPE_RATE_V1 = "Rate_V1";
        public const string TYPE_LABOR_V1 = "Labor_V1";
        public const string TYPE_MARKUP_V1 = "Markup_V1";
        public const string TYPE_OVERHEAD_V1 = "Overhead_V1";
        public const string TYPE_MATERIAL_V1 = "Material_V1";
        protected const string JSON_SERIALIZATION_ERROR = "Could not restore calculator data.";

        #endregion

        #region Properties

        /// <summary>
        /// Gets the final result of this calculation window.
        /// </summary>
        protected abstract object Result
        {
            get;
        }

        /// <summary>
        /// Gets or sets calculator data.
        /// </summary>
        protected CalculatorData CalculatorData
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the calculation type represented by this window.
        /// </summary>
        public abstract string CalculationType { get; }

        /// <summary>
        /// Gets a JSON-formatted representation of calculation data.
        /// </summary>
        public abstract string JsonData { get; }


        #endregion

        #region Methods

        /// <summary>
        /// Instantiates a new instance of <see cref="CalculatorWindow"/>.
        /// </summary>
        /// <remarks>
        /// This constructor prevents a minor, designer-facing error.
        /// 
        /// Implementers should call the
        /// <see cref="CalculatorWindow(CalculatorData)" /> constructor instead
        /// of this one.
        /// </remarks>
        public CalculatorWindow()
        {
        }

        /// <summary>
        /// Instantiates a new instance of <see cref="CalculatorWindow"/>.
        /// </summary>
        /// <param name="calculatorData"></param>
        protected CalculatorWindow(CalculatorData calculatorData)
        {
            CalculatorData = calculatorData;
        }

        /// <summary>
        /// Gets a typed result value for this window.
        /// </summary>
        /// <typeparam name="T">Type to return.</typeparam>
        /// <exception cref="InvalidOperationException">
        /// Window does not support results of the given type.
        /// </exception>
        /// <returns>An instance of the type.</returns>
        public T GetResult<T>()
        {
            if (!SupportsResultType(typeof(T)))
            {
                string errorMsg = string.Format("This instance does not support a result of type {0}",
                    typeof(T).Name);

                throw new InvalidOperationException(errorMsg);
            }

            return (T)Result;
        }

        /// <summary>
        /// Determines if this instance supports the given result type.
        /// </summary>
        /// <param name="resultType"></param>
        /// <returns>True if yes; otherwise, false.</returns>
        protected abstract bool SupportsResultType(Type resultType);

        /// <summary>
        /// Generates a new window based on calculator data.
        /// </summary>
        /// <remarks>
        /// This method acts as a factory method for
        /// <see cref="CalculatorWindow"/> instances.
        /// </remarks>
        /// <param name="data">Calculator data to use for the new window.</param>
        /// <exception cref="ArgumentNullException">data is null</exception>
        /// <returns>A new window instance.</returns>
        public static CalculatorWindow CreateWindow(CalculatorData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data), "data cannot be null.");
            }

            Type windowType;

            switch (data.Step)
            {
                case CalculationStep.Rate:
                    windowType = RateWindowType(data.CalculationType);
                    break;
                case CalculationStep.Labor:
                    windowType = LaborWindowType(data.CalculationType);
                    break;
                case CalculationStep.Markup:
                    windowType = MarkupWindowType(data.CalculationType);
                    break;
                case CalculationStep.Overhead:
                    windowType = OverheadWindowType(data.CalculationType);
                    break;
                case CalculationStep.Material:
                    windowType = MaterialWindowType(data.CalculationType);
                    break;
                default:
                    windowType = null;
                    break;
            }

            CalculatorWindow instance = null;
            if (windowType != null)
            {
                // This call can throw an exception if the type lacks a
                // necessary constructor.
                instance = Activator.CreateInstance(windowType, data) as CalculatorWindow;
            }

            return instance;
        }

        private static Type RateWindowType(string calculationType)
        {
            var windowTypes = new Dictionary<string, Type>()
            {
                { TYPE_RATE_V1, typeof(RateWindow) }
            };

            Type returnType;
            if (windowTypes.TryGetValue(calculationType, out returnType))
            {
                return returnType;
            }
            else
            {
                return null;
            }
        }
        private static Type LaborWindowType(string calculationType)
        {
            var windowTypes = new Dictionary<string, Type>()
            {
                { TYPE_LABOR_V1, typeof(LaborWindow) }
            };

            Type returnType;
            if (windowTypes.TryGetValue(calculationType, out returnType))
            {
                return returnType;
            }
            else
            {
                return null;
            }
        }

        private static Type MarkupWindowType(string calculationType)
        {
            var windowTypes = new Dictionary<string, Type>()
            {
                { TYPE_MARKUP_V1, typeof(MarkupWindow) }
            };

            Type returnType;
            if (windowTypes.TryGetValue(calculationType, out returnType))
            {
                return returnType;
            }
            else
            {
                return null;
            }
        }

        private static Type OverheadWindowType(string calculationType)
        {
            var windowTypes = new Dictionary<string, Type>()
            {
                { TYPE_OVERHEAD_V1, typeof(OverheadWindow) }
            };

            Type returnType;
            if (windowTypes.TryGetValue(calculationType, out returnType))
            {
                return returnType;
            }
            else
            {
                return null;
            }
        }
        private static Type MaterialWindowType(string calculationType)
        {
            var windowTypes = new Dictionary<string, Type>()
            {
                { TYPE_MATERIAL_V1, typeof(MaterialWindow) }
            };

            Type returnType;
            if (windowTypes.TryGetValue(calculationType, out returnType))
            {
                return returnType;
            }
            else
            {
                return null;
            }
        }

        #endregion
    }
}
