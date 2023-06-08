using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DWOS.UI.Admin.Processes
{
    //internal enum EqualityOperator { None, GreaterThan, LessThan, Equal, NotEqual }
    //internal enum ConditionInputType { None, ProcessQuestion, PartTag }

    ///*****************************************************
    // * New Table - ProcessStepCondition
    // * ProcessStepConditionId (int, PK)
    // * ProcessStepId (int, FK) - Step that is using this for evaluating enabled state or not
    // * ProcessQuestionId (int, FK) - Question comparative answer is compared to
    // * InputType (nvarchar(50)) {Number, String, OrderTag, PartTag}
    // * Operator (nvarchar(10)) {>, <, =, != }
    // * Value (nvarchar(255))
    // * StepOrder (int)
    // * 
    // ******************************************************/

    //internal abstract class ConditionEvaluator
    //{
    //    public abstract List<EqualityOperator> AvailableOperators { get; }

    //    public int ProcessQuestionId { get; set; }

    //    public EqualityOperator Op { get; set; }
       
    //    public string Answer { get; set; }

    //    public virtual bool Evaluate(string value)
    //    {
    //        return true;
    //    }

    //    public static string EqualityToString(EqualityOperator op)
    //    {
    //        switch(op)
    //        {
    //            case EqualityOperator.GreaterThan:
    //                return ">";
    //            case EqualityOperator.LessThan:
    //                return "<";
    //            case EqualityOperator.Equal:
    //                return "=";
    //            case EqualityOperator.NotEqual:
    //                return "<>";
    //            case EqualityOperator.None:
    //            default:
    //                return "None";
    //        }
    //    }
    //}
}
