using System.Collections.Generic;
using Books.Infrastructure;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Books.Models
{
    public class FormulaEditViewModel
    {
        public int NodeID { get; set; }
        public bool ClearFormula { get; set; }
        public bool Reverse { get; set; }
        public bool ClearNumerator { get; set; }
        public bool ClearDenominator { get; set; }
        public bool BothIdent { get; set; }
        public bool BoldIdent { get; set; }
        public string Ident1 { get; set; }
        public string Ident2 { get; set; }
        public string Oper1 { get; set; }
        public string Oper2 { get; set; }
        public bool BothOper { get; set; }
        public string Num1 { get; set; }
        public string Num2 { get; set; }
        public bool BoldNum { get; set; }
        public bool BothNum { get; set; }
        public string Space { get; set; }
        public string Text { get; set; }
        public bool BoldText { get; set; }
        public bool BothText { get; set; }
        public bool Matrix { get; set; }
        public int? Row { get; set; }
        public int? Column { get; set; }
        public string Insert { get; set; }
        public string Insert1 { get; set; }
        public string Target { get; set; }
        public string Block { get; set; }
        public string Algebraic { get; set; }
        public string Calculus { get; set; }
        public string Ellipses { get; set; }
        public string Logic { get; set; }
        public string Vector { get; set; }
        public string Set { get; set; }
        public string Geometric { get; set; }
        public string GreekUpper { get; set; }
        public string GreekLower { get; set; }
        public bool ContainsSupRow1 { get; set; }
        public bool ContainsSupRow2 { get; set; }
        public bool ContainsSubRow1 { get; set; }
        public bool ContainsSubRow2 { get; set; }
        public bool ContainsMatrix { get; set; }
        public bool ContainsNumerator { get; set; }
        public bool ContainsDenominator { get; set; }
        public bool ContainsFenced { get; set; }
        public bool ContainsSquareRootRow { get; set; }
        public bool ContainsRootRow { get; set; }
        public bool ContainsOverRow { get; set; }
        public bool ContainsUnderRow { get; set; }
        public bool ContainsRow { get; set; }
        public bool Id2 { get; set; }
        public bool Op2 { get; set; }
        public bool N2 { get; set; }

    }
}