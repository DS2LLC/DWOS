using System.Collections.Generic;
using System.Linq;
using DWOS.Data.Datasets;
using DWOS.Data.Process;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DWOS.Data.Tests.Process
{
    [TestClass]
    public sealed class ProcessUtilitiesTests
    {
        [TestMethod]
        public void MinValueTest()
        {
            // Nothing present
            var testData = NewOrderTestData(null, null);
            Assert.IsTrue(string.IsNullOrEmpty(ProcessUtilities.MinValue(testData.Question, testData.Order)));

            // Min on Question
            testData.Question.MinValue = "5";

            Assert.AreEqual("5", ProcessUtilities.MinValue(testData.Question, testData.Order));

            // Min on Field
            testData = NewOrderTestData(
                new List<QuestionFieldData>
                {
                    new QuestionFieldData("Token", QuestionField.MinValue)
                },
                new List<OrderFieldData>
                {
                    new OrderFieldData("Token", "6")
                });

            testData.Question.MinValue = "5";
            Assert.AreEqual("6", ProcessUtilities.MinValue(testData.Question, testData.Order));

            // Min calculated through tolerance
            testData = NewOrderTestData(
                new List<QuestionFieldData>
                {
                    new QuestionFieldData("Answer", QuestionField.Answer),
                    new QuestionFieldData("Tolerance", QuestionField.Tolerance)
                },
                new List<OrderFieldData>
                {
                    new OrderFieldData("Answer", "10"),
                    new OrderFieldData("Tolerance", "5")
                });

            testData.Question.MinValue = "500";
            Assert.AreEqual("5", ProcessUtilities.MinValue(testData.Question, testData.Order));

            testData = NewOrderTestData(
                new List<QuestionFieldData>
                {
                    new QuestionFieldData("MinValue", QuestionField.MinValue),
                    new QuestionFieldData("Answer", QuestionField.Answer),
                    new QuestionFieldData("Tolerance", QuestionField.Tolerance)
                },
                new List<OrderFieldData>
                {
                    new OrderFieldData("MinValue", "1000"),
                    new OrderFieldData("Answer", "10"),
                    new OrderFieldData("Tolerance", "5")
                });

            testData.Question.MinValue = "500";
            Assert.AreEqual("5", ProcessUtilities.MinValue(testData.Question, testData.Order));
        }

        [TestMethod]
        public void MinValueProcessTest()
        {
            var question = NewProcessQuestionRow();

            // Nothing present
            Assert.IsNull(ProcessUtilities.MinValue(question));

            // Min present
            question.MinValue = "5";
            Assert.AreEqual("5", ProcessUtilities.MinValue(question));

            // DefaultValue present, but not Tolerance
            question.DefaultValue = "10";
            Assert.AreEqual("5", ProcessUtilities.MinValue(question));

            // Tolerance present, but not DefaultValue
            question.SetDefaultValueNull();
            question.Tolerance = "1";
            Assert.AreEqual("5", ProcessUtilities.MinValue(question));

            // Tolerance and DefaultValue present
            question.DefaultValue = "10";
            Assert.AreEqual("9", ProcessUtilities.MinValue(question));
        }

        [TestMethod]
        public void MaxValueTest()
        {
            // Nothing present
            var testData = NewOrderTestData(null, null);
            Assert.IsTrue(string.IsNullOrEmpty(ProcessUtilities.MaxValue(testData.Question, testData.Order)));

            // Max on Question
            testData.Question.MaxValue = "5";

            Assert.AreEqual("5", ProcessUtilities.MaxValue(testData.Question, testData.Order));

            // Max on Field
            testData = NewOrderTestData(
                new List<QuestionFieldData>
                {
                    new QuestionFieldData("Token", QuestionField.MaxValue)
                },
                new List<OrderFieldData>
                {
                    new OrderFieldData("Token", "6")
                });

            testData.Question.MaxValue = "5";
            Assert.AreEqual("6", ProcessUtilities.MaxValue(testData.Question, testData.Order));

            // Max calculated through tolerance
            testData = NewOrderTestData(
                new List<QuestionFieldData>
                {
                    new QuestionFieldData("Answer", QuestionField.Answer),
                    new QuestionFieldData("Tolerance", QuestionField.Tolerance)
                },
                new List<OrderFieldData>
                {
                    new OrderFieldData("Answer", "10"),
                    new OrderFieldData("Tolerance", "5")
                });

            testData.Question.MaxValue = "500";
            Assert.AreEqual("15", ProcessUtilities.MaxValue(testData.Question, testData.Order));

            testData = NewOrderTestData(
                new List<QuestionFieldData>
                {
                    new QuestionFieldData("MaxValue", QuestionField.MaxValue),
                    new QuestionFieldData("Answer", QuestionField.Answer),
                    new QuestionFieldData("Tolerance", QuestionField.Tolerance)
                },
                new List<OrderFieldData>
                {
                    new OrderFieldData("MaxValue", "1000"),
                    new OrderFieldData("Answer", "10"),
                    new OrderFieldData("Tolerance", "5")
                });

            testData.Question.MaxValue = "500";
            Assert.AreEqual("15", ProcessUtilities.MaxValue(testData.Question, testData.Order));
        }

        [TestMethod]
        public void MaxValueProcessTest()
        {
            var question = NewProcessQuestionRow();

            // Nothing present
            Assert.IsNull(ProcessUtilities.MaxValue(question));

            // Min present
            question.MaxValue = "5";
            Assert.AreEqual("5", ProcessUtilities.MaxValue(question));

            // DefaultValue present, but not Tolerance
            question.DefaultValue = "10";
            Assert.AreEqual("5", ProcessUtilities.MaxValue(question));

            // Tolerance present, but not DefaultValue
            question.SetDefaultValueNull();
            question.Tolerance = "1";
            Assert.AreEqual("5", ProcessUtilities.MaxValue(question));

            // Tolerance and DefaultValue present
            question.DefaultValue = "10";
            Assert.AreEqual("11", ProcessUtilities.MaxValue(question));
        }

        [TestMethod]
        public void MinValueDecimalTest()
        {
            var question = NewProcessQuestionRow();

            // Nothing present
            Assert.IsNull(ProcessUtilities.MinValueDecimal(question));

            // Min present
            question.MinValue = "5";
            Assert.AreEqual(5, ProcessUtilities.MinValueDecimal(question));

            // DefaultValue present, but not Tolerance
            question.DefaultValue = "10";
            Assert.AreEqual(5, ProcessUtilities.MinValueDecimal(question));

            // Tolerance present, but not DefaultValue
            question.SetDefaultValueNull();
            question.Tolerance = "1";
            Assert.AreEqual(5, ProcessUtilities.MinValueDecimal(question));

            // Tolerance and DefaultValue present
            question.DefaultValue = "10";
            Assert.AreEqual(9, ProcessUtilities.MinValueDecimal(question));
        }

        [TestMethod]
        public void MaxValueDecimalTest()
        {
            var question = NewProcessQuestionRow();

            // Nothing present
            Assert.IsNull(ProcessUtilities.MaxValueDecimal(question));

            // Max present
            question.MaxValue = "5";
            Assert.AreEqual(5, ProcessUtilities.MaxValueDecimal(question));

            // DefaultValue present, but not Tolerance
            question.DefaultValue = "10";
            Assert.AreEqual(5, ProcessUtilities.MaxValueDecimal(question));

            // Tolerance present, but not DefaultValue
            question.SetDefaultValueNull();
            question.Tolerance = "1";
            Assert.AreEqual(5, ProcessUtilities.MaxValueDecimal(question));

            // Tolerance and DefaultValue present
            question.DefaultValue = "10";
            Assert.AreEqual(11, ProcessUtilities.MaxValueDecimal(question));
        }

        [TestMethod]
        public void DefaultValueTest()
        {
            const string expectedFieldDefault = "5";
            const string expectedOrderDefault = "6";

            // Case - Custom Field Matches
            var testData = NewOrderTestData(new List<QuestionFieldData>
                {
                    new QuestionFieldData("VALUE", QuestionField.Answer)
                },
                new List<OrderFieldData>
                {
                    new OrderFieldData("VALUE", expectedFieldDefault)
                });

            testData.Question.DefaultValue = expectedOrderDefault;

            var actualDefault = ProcessUtilities.DefaultValue(testData.Question, testData.Order);
            Assert.AreEqual(expectedFieldDefault, actualDefault);

            // Case - Custom Field is Empty
            testData = NewOrderTestData(new List<QuestionFieldData>
                {
                    new QuestionFieldData("VALUE", QuestionField.Answer)
                },
                new List<OrderFieldData>
                {
                    new OrderFieldData("VALUE", string.Empty)
                });

            testData.Question.DefaultValue = expectedOrderDefault;

            actualDefault = ProcessUtilities.DefaultValue(testData.Question, testData.Order);
            Assert.AreEqual(expectedOrderDefault, actualDefault);

            // Case - Custom Field Does Not Match
            testData = NewOrderTestData(new List<QuestionFieldData>
                {
                    new QuestionFieldData("VALUE", QuestionField.Answer)
                },
                new List<OrderFieldData>
                {
                    new OrderFieldData("VALUE2", expectedFieldDefault)
                });

            testData.Question.DefaultValue = expectedOrderDefault;

            actualDefault = ProcessUtilities.DefaultValue(testData.Question, testData.Order);
            Assert.AreEqual(expectedOrderDefault, actualDefault);

            // Case - No Custom Field
            testData = NewOrderTestData(null, null);
            testData.Question.DefaultValue = expectedOrderDefault;

            actualDefault = ProcessUtilities.DefaultValue(testData.Question, testData.Order);
            Assert.AreEqual(expectedOrderDefault, actualDefault);
        }

        [TestMethod]
        public void DefaultValueWithToleranceTest()
        {
            const string expectedFieldDefault = "5";
            const string expectedQuestionDefault = "6";

            // Case - Order has tolerance
            var testData = NewOrderTestData(new List<QuestionFieldData>
                {
                    new QuestionFieldData("VALUE", QuestionField.Answer),
                    new QuestionFieldData("Tolerance", QuestionField.Tolerance)
                },
                new List<OrderFieldData>
                {
                    new OrderFieldData("VALUE", expectedFieldDefault),
                    new OrderFieldData("Tolerance", "Test"),
                });

            testData.Question.DefaultValue = expectedQuestionDefault;

            var actualDefault = ProcessUtilities.DefaultValue(testData.Question, testData.Order);
            Assert.IsTrue(string.IsNullOrEmpty(actualDefault));

            // Case - Question has tolerance
            testData = NewOrderTestData(new List<QuestionFieldData>
                {
                    new QuestionFieldData("VALUE", QuestionField.Answer),
                    new QuestionFieldData("Tolerance", QuestionField.Tolerance)
                },
                new List<OrderFieldData>
                {
                    new OrderFieldData("VALUE", expectedFieldDefault),
                });

            testData.Question.DefaultValue = expectedQuestionDefault;
            testData.Question.Tolerance = "0";

            actualDefault = ProcessUtilities.DefaultValue(testData.Question, testData.Order);
            Assert.IsTrue(string.IsNullOrEmpty(actualDefault));
        }

        private static ProcessesDataset.ProcessQuestionRow NewProcessQuestionRow()
        {
            var dtQuestion = new ProcessesDataset.ProcessQuestionDataTable();
            var question = dtQuestion.NewProcessQuestionRow();
            return question;
        }

        private static OrderTestData NewOrderTestData(IEnumerable<QuestionFieldData> questionFields,
            IEnumerable<OrderFieldData> orderFields)
        {
            var dsOrderProcessing = new OrderProcessingDataSet();

            var processQuestion = dsOrderProcessing.ProcessQuestion.NewProcessQuestionRow();
            processQuestion.StepOrder = decimal.Zero;
            processQuestion.OperatorEditable = true;
            processQuestion.Required = true;
            processQuestion.ProcessStepID = 5;
            processQuestion.Name = "Test";
            processQuestion.InputType = InputType.Decimal.ToString();
            dsOrderProcessing.ProcessQuestion.AddProcessQuestionRow(processQuestion);

            foreach (var questionField in questionFields ?? Enumerable.Empty<QuestionFieldData>())
            {
                var row = dsOrderProcessing.ProcessQuestionField.NewProcessQuestionFieldRow();
                row.ProcessQuestionRow = processQuestion;
                row.FieldName = questionField.Field.ToString();
                row.TokenName = questionField.TokenName;
                dsOrderProcessing.ProcessQuestionField.AddProcessQuestionFieldRow(row);
            }

            var order = dsOrderProcessing.OrderSummary.NewOrderSummaryRow();
            order.OrderType = (int) OrderType.Normal;
            order.WorkStatus = "In Process";
            order.Status = "Open";
            order.CurrentLocation = "Chem";
            order.RequireCoc = false;
            dsOrderProcessing.OrderSummary.AddOrderSummaryRow(order);

            var orderFieldNumber = 0;
            foreach (var orderField in orderFields ?? Enumerable.Empty<OrderFieldData>())
            {
                var customFieldRow = dsOrderProcessing.CustomField.NewCustomFieldRow();
                customFieldRow.Name = $"Name{orderFieldNumber}";
                customFieldRow.TokenName = orderField.TokenName;
                customFieldRow.CustomFieldID = orderFieldNumber;
                customFieldRow.ProcessUnique = true;
                customFieldRow.CustomerID = 1;
                customFieldRow.IsVisible = true;
                dsOrderProcessing.CustomField.AddCustomFieldRow(customFieldRow);

                var orderFieldRow = dsOrderProcessing.OrderCustomFields.NewOrderCustomFieldsRow();
                orderFieldRow.OrderSummaryRow = order;
                orderFieldRow.Value = orderField.Value;
                orderFieldRow.CustomFieldRow = customFieldRow;
                dsOrderProcessing.OrderCustomFields.AddOrderCustomFieldsRow(orderFieldRow);

                ++orderFieldNumber;
            }

            return new OrderTestData
            {
                Order = order,
                Question = processQuestion
            };
        }

        private class OrderTestData
        {
            public OrderProcessingDataSet.ProcessQuestionRow Question { get; set; }

            public OrderProcessingDataSet.OrderSummaryRow Order { get; set; }
        }

        private class QuestionFieldData
        {
            public string TokenName { get; }

            public QuestionField Field { get; }

            public QuestionFieldData(string tokenName, QuestionField field)
            {
                TokenName = tokenName;
                Field = field;
            }
        }

        private class OrderFieldData
        {
            public string TokenName { get; }

            public string Value { get; }

            public OrderFieldData(string tokenName, string value)
            {
                TokenName = tokenName;
                Value = value;
            }
        }
    }
}