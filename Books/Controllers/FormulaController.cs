using Books.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace Books.Controllers
{
    public class FormulaController : Controller
    {
        private readonly HttpContext _context;
        public FormulaController(IHttpContextAccessor context)
        {
            _context = context.HttpContext;
        }
        public ViewResult Edit(int id)
        {
            string formula = "<math xmlns=" + '"' + "http://www.w3.org/1998/Math/MathML" + '"' + " display='inline'> </math>";
            ViewBag.Formula = formula;
            FormulaEditViewModel model = InitEditModel(id, formula);
            if (_context != null)
            {
                if (_context.Session.GetString("formula") == null)
                {
                    _context.Session.SetString("formula", formula);
                    InitSessionVariables(_context);
                    AddFormulaToUnDoList(_context);
                }
                else
                {
                    _context.Session.SetString("formula", formula);
                    RestoreFields(_context, model);
                }
            }
            ViewBag.matrix = "false";
            return View(model);
        }

        [HttpPost]
        public ViewResult Edit(FormulaEditViewModel form, string clear = "none", bool undo = false, bool restore = false)
        {
            string Id, Op, Num;
            string searchfor = " </math>";
            bool insert = true;
            bool Matrix_Row_Col_In_Range = false;
            bool No_Special_Maths_Symbol_Being_Inserted = form.Algebraic == "None" && form.Calculus == "None" && form.Ellipses == "None" && form.Geometric == "None" && form.GreekLower == "None" && form.GreekUpper == "None" && form.Logic == "None" && form.Set == "None" && form.Vector == "None";
            StringBuilder sb, repeat;
            if (_context != null)
            {
                if (_context.Session.GetString("matrix") == "true")
                {
                    form.Matrix = true;
                    Matrix_Row_Col_In_Range = form.Column > 0 && form.Row > 0 && form.Column <= _context.Session.GetInt32("cols") && form.Row <= _context.Session.GetInt32("rows");
                }
                else
                {
                    form.Matrix = false;
                }
            }

            if (No_Special_Maths_Symbol_Being_Inserted && !undo)
            {
                CheckForMissingFields(form);
            }

            // _context is null in Unit Tests

            if (_context != null)
            {
                if (undo && _context.Session.GetInt32("undoptr") > 0)
                {
                    sb = UnDoLastInsert(_context, form);
                    repeat = new StringBuilder(_context.Session.GetString("repeat"));

                    insert = false;
                }
                else
                {
                    sb = new StringBuilder(_context.Session.GetString("formula"));
                    repeat = new StringBuilder(_context.Session.GetString("repeat"));

                }
            }
            else
            {
                sb = new StringBuilder(form.Formula);
                repeat = new StringBuilder(_context.Session.GetString("repeat"));

            }

            WasAClearButtonPressed(form, clear);

            if (form.ClearFormula)
            {
                if (_context != null)
                {
                    sb = ClearFormula(_context);
                    repeat = new StringBuilder(sb.ToString());
                }
                else
                {
                    string formula = "<math xmlns=" + '"' + "http://www.w3.org/1998/Math/MathML" + '"' + " display='inline'> </math>";
                    sb = new StringBuilder(formula);
                    repeat = new StringBuilder(sb.ToString());
                }
                ClearEditFields(form);
            }
            else
            {
                WhereToInsertNewMathMarkUp(form, ref searchfor, ref insert, Matrix_Row_Col_In_Range);
                InlineOrBlock(form, ref sb, ref insert);

                if (insert)
                {
                    if (form.Insert1 == "None")
                    {
                        if (form.Insert == "Matrix")
                        {
                            if (!undo && CheckMatrix(form))
                            {
                                if (_context != null)
                                {
                                    _context.Session.SetString("matrix", "true");
                                    _context.Session.SetInt32("rows", form.Row ?? 0);
                                    _context.Session.SetInt32("cols", form.Column ?? 0);
                                }
                            }
                        }
                        if (_context != null)
                        {
                            HasAnyFieldChanged(_context, form);
                        }
                        SpecialMathsSymbols(form);
                        FirstOrSecondField(form, out Id, out Op, out Num);
                        InsertField(_context, form, Id, Op, Num, searchfor, sb, ref repeat);
                        _context.Session.SetString("repeat", repeat.ToString());
                    }
                    else
                    {
                        if (_context != null)
                        {
                            HasAnyFieldChanged(_context, form);
                        }
                        SpecialMathsSymbols(form);
                        FirstOrSecondField(form, out Id, out Op, out Num);
                        InsertObject(_context, form, Id, Op, Num, searchfor, sb, repeat);
                        _context.Session.SetString("repeat", repeat.ToString());
                        form.Insert1 = "None";
                    }
                    IsClearNumeratorOrClearDenominatorChecked(form);
                    if (form.Target == "Clear Matrix")
                    {
                        if (_context != null)
                        {
                            _context.Session.SetString("matrix", "false");
                            _context.Session.SetInt32("rows", 0);
                            _context.Session.SetInt32("matrix", 0);
                        }
                    }
                    ClearButtonPressedRemoveIndicator(form, sb);
                }

                WhatIndicatorsAreLeft(form, sb);
                ResetCheckboxes(form);
                ViewBag.formula = sb.ToString();
                form.Formula = sb.ToString();
                if (_context != null)
                {
                    if (restore)
                    {
                        RestoreFields(_context, form);
                    }
                    SetSessionVariables(_context, form);
                    if (!undo)
                    {
                        AddFormulaToUnDoList(_context);
                    }
                }
            }
            return View(form);
        }

        StringBuilder ClearFormula(HttpContext context)
        {
            context.Session.SetString("formula", "<math xmlns=" + '"' + "http://www.w3.org/1998/Math/MathML" + '"' + " display='inline'> </math>");
            InitSessionVariables(context);
            _context.Session.CommitAsync();
            return new StringBuilder(context.Session.GetString("formula"));
        }

        private static void IsClearNumeratorOrClearDenominatorChecked(FormulaEditViewModel form)
        {
            if (form.ClearNumerator)
            {
                form.Target = "Clear Numerator";
                form.ClearNumerator = false;
            }

            if (form.ClearDenominator)
            {
                form.Target = "Clear Denominator";
                form.ClearDenominator = false;
            }
        }

        private void AddFormulaToUnDoList(HttpContext context)
        {
            context.Session.SetInt32("undoptr", (context.Session.GetInt32("undoptr") ?? 0) + 1);
            context.Session.SetString("undo" + (context.Session.GetInt32("undoptr")).ToString(), context.Session.GetString("formula"));
            _context.Session.CommitAsync();
        }

        StringBuilder UnDoLastInsert(HttpContext context, FormulaEditViewModel form)
        {
            StringBuilder sb = new StringBuilder(context.Session.GetString("undo" + context.Session.GetInt32("undoptr").ToString()));
            if (Int32.Parse(context.Session.GetInt32("undoptr").ToString()) > 1)
            {
                context.Session.SetInt32("undoptr", (context.Session.GetInt32("undoptr") ?? 2) - 1);
                sb = new StringBuilder(context.Session.GetString("undo" + context.Session.GetInt32("undoptr").ToString()));
            }
            context.Session.CommitAsync();
            MoveTarget(form, sb);
            return sb;
        }

        StringBuilder GetLastFormula(HttpContext context)
        {
            StringBuilder sb = new StringBuilder(context.Session.GetString("undo" + context.Session.GetInt32("undoptr").ToString()));
            context.Session.CommitAsync();
            return sb;
        }

        private static void ResetCheckboxes(FormulaEditViewModel form)
        {
            if (form.ClearFormula) { form.ClearFormula = false; }
            if (form.BoldIdent) { form.BoldIdent = false; }
            if (form.BoldNum) { form.BoldNum = false; }
            if (form.BoldText) { form.BoldText = false; }
            if (form.BothIdent) { form.BothIdent = false; }
            if (form.BothOper) { form.BothOper = false; }
            if (form.BothNum) { form.BothNum = false; }
            if (form.BothText) { form.BothText = false; }
            if (form.EmbedText) { form.EmbedText = false; }
            if (form.Id2) { form.Id2 = false; }
            if (form.Op2) { form.Op2 = false; }
            if (form.N2) { form.N2 = false; }
        }

        private static void WhatIndicatorsAreLeft(FormulaEditViewModel form, StringBuilder sb)
        {
            if (sb.ToString().Contains("#suprow1")) { form.ContainsSupRow1 = true; } else { form.ContainsSupRow1 = false; }
            if (sb.ToString().Contains("#suprow2")) { form.ContainsSupRow2 = true; } else { form.ContainsSupRow2 = false; }
            if (sb.ToString().Contains("#subrow1")) { form.ContainsSubRow1 = true; } else { form.ContainsSubRow1 = false; }
            if (sb.ToString().Contains("#subrow2")) { form.ContainsSubRow2 = true; } else { form.ContainsSubRow2 = false; }
            if (sb.ToString().Contains("#col")) { form.ContainsMatrix = true; } else { form.ContainsMatrix = false; }
            if (sb.ToString().Contains("#numerator")) { form.ContainsNumerator = true; } else { form.ContainsNumerator = false; }
            if (sb.ToString().Contains("#denominator")) { form.ContainsDenominator = true; } else { form.ContainsDenominator = false; }
            if (sb.ToString().Contains("#fenced")) { form.ContainsFenced = true; } else { form.ContainsFenced = false; }
            if (sb.ToString().Contains("#sqrtrow")) { form.ContainsSquareRootRow = true; } else { form.ContainsSquareRootRow = false; }
            if (sb.ToString().Contains("#rootrow")) { form.ContainsRootRow = true; } else { form.ContainsRootRow = false; }
            if (sb.ToString().Contains("#overrow")) { form.ContainsOverRow = true; } else { form.ContainsOverRow = false; }
            if (sb.ToString().Contains("#underrow")) { form.ContainsUnderRow = true; } else { form.ContainsUnderRow = false; }
            if (sb.ToString().Contains("#row")) { form.ContainsRow = true; } else { form.ContainsRow = false; }
            if (sb.ToString().Contains("#introw")) { form.ContainsIntergral = true; } else { form.ContainsIntergral = false; }

        }

        private static void InlineOrBlock(FormulaEditViewModel form, ref StringBuilder sb, ref bool insert)
        {
            switch (form.Block)
            {
                case "Block":
                    if (sb.ToString().IndexOf("inline") > 0)
                    {
                        sb.Replace("inline", "block");
                        insert = false;
                    }
                    break;
                case "Inline":
                    if (sb.ToString().IndexOf("block") > 0)
                    {
                        sb.Replace("block", "inline");
                        insert = false;
                    }
                    break;
                default:
                    break;
            }

            if (form.BothText)
            {
                if (sb.ToString().IndexOf("inline") > 0)
                {
                    sb.Replace("inline", "block");
                }
                form.Insert1 = "Text";
                if (form.Space == null)
                {
                    form.Space = "10";
                }
            }
        }

        private void ClearButtonPressedRemoveIndicator(FormulaEditViewModel form, StringBuilder sb)
        {
            switch (form.Target)
            {
                case "Clear Numerator":
                    if (sb.ToString().Contains("#numerator")) { sb.Remove(sb.ToString().IndexOf("#numerator"), 10); };
                    form.Target = "Denominator";
                    form.Insert = "Identifier";
                    break;
                case "Clear Denominator":
                    if (sb.ToString().Contains("#denominator")) { sb.Remove(sb.ToString().IndexOf("#denominator"), 12); };
                    MoveTarget(form, sb);
                    break;
                case "Clear Subscript Row1":
                    if (sb.ToString().Contains("#subrow1")) { sb.Remove(sb.ToString().IndexOf("#subrow1"), 8); };
                    MoveTarget(form, sb);
                    break;
                case "Clear Subscript Row2":
                    if (sb.ToString().Contains("#subrow2")) { sb.Remove(sb.ToString().IndexOf("#subrow2"), 8); };
                    MoveTarget(form, sb);
                    break;
                case "Clear Matrix":
                    if (sb.ToString().Contains("#col"))
                    {
                        do
                        {
                            sb.Remove(sb.ToString().IndexOf("#col"), 9);
                        }
                        while (sb.ToString().IndexOf("#col") > 0);
                    };
                    MoveTarget(form, sb);
                    form.Matrix = false;
                    form.Row = null;
                    form.Column = null;
                    break;
                case "Clear Superscript Row1":
                    if (sb.ToString().Contains("#suprow")) { sb.Remove(sb.ToString().IndexOf("#suprow1"), 8); };
                    MoveTarget(form, sb);
                    break;
                case "Clear Superscript Row2":
                    if (sb.ToString().Contains("#suprow2")) { sb.Remove(sb.ToString().IndexOf("#suprow2"), 8); };
                    MoveTarget(form, sb);
                    break;
                case "Clear Square Root Row":
                    if (sb.ToString().Contains("#sqrtrow")) { sb.Remove(sb.ToString().IndexOf("#sqrtrow"), 8); };
                    form.Insert = "Operator";
                    MoveTarget(form, sb);
                    break;
                case "Clear Fenced":
                    if (sb.ToString().Contains("#fenced")) { sb.Remove(sb.ToString().IndexOf("#fenced"), 7); };
                    MoveTarget(form, sb);
                    break;
                case "Clear Root Row":
                    if (sb.ToString().Contains("#rootrow")) { sb.Remove(sb.ToString().IndexOf("#rootrow"), 8); };
                    MoveTarget(form, sb);
                    break;
                case "Clear Over Row":
                    if (sb.ToString().Contains("#overrow")) { sb.Remove(sb.ToString().IndexOf("#overrow"), 8); };
                    MoveTarget(form, sb);
                    break;
                case "Clear Under Row":
                    if (sb.ToString().Contains("#underrow")) { sb.Remove(sb.ToString().IndexOf("#underrow"), 9); };
                    if (sb.ToString().Contains("#overrow"))
                    {
                        form.Target = "Over Row";
                        form.Insert = "Identifier";
                    }
                    else
                    {
                        form.Target = "Append";
                        form.Insert = "Identifier";
                    }
                    break;
                case "Clear Row":
                    if (sb.ToString().Contains("#row")) { sb.Remove(sb.ToString().IndexOf("#row"), 4); };
                    MoveTarget(form, sb);
                    break;
                case "Clear Intergral":
                    if (sb.ToString().Contains("#introw")) { sb.Remove(sb.ToString().IndexOf("#introw"), 7); };
                    MoveTarget(form, sb);
                    break;
            }
        }

        private void InsertObject(HttpContext context, FormulaEditViewModel form, string Id, string Op, string Num, string searchfor, StringBuilder sb, StringBuilder repeat)
        {
            switch (form.Insert1)
            {
                case "Subscript (Identifier, Number)":
                    InsertSubscriptIdentifierNumber(form, Id, Num, searchfor, sb);
                    form.Insert = "Operator";
                    break;
                case "Subscript (Identifier, Identifier)":
                    InsertSubscriptIdentifierIdentifier(form, searchfor, sb);
                    form.Insert = "Operator";
                    break;
                case "Subscript (Identifier, Operator)":
                    InsertSubscriptIdentifierOperator(form, Id, Op, searchfor, sb);
                    form.Insert = "Operator";
                    break;
                case "Subscript (Identifier, Row)":
                    InsertSubscriptIdentifierRow(form, Id, searchfor, sb);
                    form.Target = "Subscript Row1";
                    form.Insert = "Identifier";
                    break;
                case "Subscript (Row, Identifier)":
                    InsertSubscriptRowIdentifier(form, Id, searchfor, sb);
                    form.Target = "Subscript Row1";
                    form.Insert = "Identifier";
                    break;
                case "Subscript (Row, Number)":
                    InsertSubscriptRowNumber(form, Num, searchfor, sb);
                    form.Target = "Subscript Row1";
                    form.Insert = "Identifier";
                    break;
                case "Subscript (Row, Row)":
                    InsertSubscriptRowRow(form, searchfor, sb);
                    form.Target = "Subscript Row1";
                    form.Insert = "Identifier";
                    break;
                case "Superscript (Identifier, Number)":
                    InsertSuperscriptIdentifierNumber(form, Id, Num, searchfor, sb);
                    form.Insert = "Operator";
                    break;
                case "Superscript (Operator, Number)":
                    InsertSuperscriptOperatorNumber(form, Op, Num, searchfor, sb);
                    form.Insert = "Identifier";
                    break;
                case "Superscript (Number, Number)":
                    InsertSuperscriptNumberNumber(form, searchfor, sb);
                    form.Insert = "Operator";
                    break;
                case "Superscript (Number, Row)":
                    InsertSuperscriptNumberRow(form, Num, searchfor, sb);
                    form.Target = "Superscript Row1";
                    form.Insert = "Identifier";
                    break;
                case "Superscript (Identifier, Identifier)":
                    InsertSuperscriptIdentifierIdentifier(form, searchfor, sb);
                    form.Insert = "Operator";
                    break;
                case "Superscript (Identifier, Row)":
                    InsertSuperscriptIdentifierRow(form, Id, searchfor, sb);
                    form.Target = "Superscript Row1";
                    form.Insert = "Identifier";
                    break;
                case "Superscript (Row, Identifier)":
                    InsertSuperscriptRowIdentifier(form, Id, searchfor, sb);
                    form.Target = "Superscript Row1";
                    form.Insert = "Identifier";
                    break;
                case "Superscript (Row, Number)":
                    InsertSuperscriptRowNumber(form, Num, searchfor, sb);
                    form.Target = "Superscript Row1";
                    form.Insert = "Identifier";
                    break;
                case "Superscript (Row, Row)":
                    InsertSuperscriptRowRow(form, searchfor, sb);
                    form.Target = "Superscript Row1";
                    form.Insert = "Identifier";
                    break;
                case "SubSup (Identifier, Number, Number)":
                    InsertSubSupIdentifierNumberNumber(form, Id, Num, searchfor, sb);
                    form.Insert = "Operator";
                    break;
                case "SubSup (Identifier, Number, Identifier)":
                    InsertSubSupIdentifierNumberIdentifier(form, Id, Num, searchfor, sb);
                    form.Insert = "Operator";
                    break;
                case "SubSup (Identifier, Identifier, Number)":
                    InsertSubSupIdentifierIdentifierNumber(form, Id, Num, searchfor, sb);
                    form.Insert = "Operator";
                    break;
                case "SubSup (Identifier, Identifier, Identifier)":
                    InsertSubSupIdentifierIdentifierIdentifier(form, Id, searchfor, sb);
                    form.Insert = "Operator";
                    break;
                case "SubSup (Identifier, Row, Row)":
                    InsertSubSupIdentifierRowRow(form, Id, searchfor, sb);
                    form.Target = "Subscript Row1";
                    form.Insert = "Identifier";
                    break;
                case "Row":
                    InsertRow(searchfor, sb);
                    form.Target = "Row";
                    form.Insert = "Identifier";
                    break;
                case "IntergralDefinite":
                    InsertIntergralDefinite(Id, searchfor, sb);
                    form.Target = "Under Row";
                    form.Insert = "Identifier";
                    break;
                case "IntergralInDefinite":
                    InsertIntergralInDefinite(Id, searchfor, sb);
                    form.Target = "Intergral";
                    form.Insert = "Identifier";
                    break;
                case "Fraction":
                    InsertFraction(searchfor, sb, repeat);
                    form.Target = "Numerator";
                    form.Insert = "Identifier";
                    break;
                case "FractionNum":
                    InsertFractionNum(form, searchfor, sb);
                    form.Insert = "Operator";
                    break;
                case "FractionVar":
                    InsertFractionVar(form, searchfor, sb);
                    form.Insert = "Operator";
                    break;
                case "FractionVarNum":
                    InsertFractionVarNum(form, Id, Num, searchfor, sb);
                    form.Insert = "Operator";
                    break;
                case "FractionDiff":
                    InsertFractionDiff(form, searchfor, sb);
                    form.Insert = "Operator";
                    break;
                case "FractionDiff2":
                    InsertFractionDiff2(form, searchfor, sb);
                    form.Insert = "Operator";
                    break;
                case "FractionPart":
                    InsertFractionPart(form, searchfor, sb);
                    form.Insert = "Operator";
                    break;
                case "FractionPart2":
                    InsertFractionPart2(form, searchfor, sb);
                    form.Insert = "Operator";
                    break;
                case "Square Root":
                    InsertSquareRoot(searchfor, sb);
                    form.Target = "Square Root Row";
                    form.Insert = "Identifier";
                    break;
                case "Root":
                    InsertRoot(form, searchfor, sb);
                    form.Target = "Root Row";
                    form.Insert = "Identifier";
                    break;
                case "Under Over":
                    InsertUnderOver(Op, searchfor, sb);
                    form.Target = "Under Row";
                    form.Insert = "Identifier";
                    break;
                case "Under":
                    InsertUnder(Op, searchfor, sb);
                    form.Target = "Under Row";
                    form.Insert = "Identifier";
                    break;
                case "Limit":
                    InsertLimit(searchfor, sb);
                    form.Target = "Under Row";
                    form.Insert = "Identifier";
                    break;
                case "Limit h->0":
                    InsertLimith0(searchfor, sb);
                    form.Insert = "Identifier";
                    break;
                case "Over":
                    InsertOver(form, Id, Op, searchfor, sb);
                    form.Insert = "Operator";
                    break;
                case "Over Row":
                    InsertOverRow(form, Op, searchfor, sb);
                    form.Target = "Over Row";
                    form.Insert = "Identifier";
                    break;
                case "Fenced 0":
                    InsertFenced0(form, searchfor, sb);
                    form.Target = "Fenced";
                    form.Insert = "Identifier";
                    break;
                case "Fenced 1":
                    InsertFenced1(Id, searchfor, sb);
                    form.Insert = "Operator";
                    break;
                case "Fenced 2":
                    InsertFenced2(form, searchfor, sb);
                    form.Insert = "Operator";
                    break;
                case "Fenced 3":
                    InsertFenced3(context, form, searchfor, sb);
                    form.Insert = "Operator";
                    break;
                case "FencedNum 1":
                    InsertFencedNum1(form, searchfor, sb);
                    form.Insert = "Operator";
                    break;
                case "FencedNum 2":
                    InsertFencedNum2(form, searchfor, sb);
                    form.Insert = "Operator";
                    break;
                case "FencedNum 3":
                    InsertFencedNum3(context, form, searchfor, sb);
                    form.Insert = "Operator";
                    break;
                case "Space":
                    InsertSpace(form, searchfor, sb);
                    form.Insert = "Identifier";
                    break;
                case "Text":
                    InsertText(context, form, searchfor, sb);
                    form.Insert = "Identifier";
                    break;
                case "Line Break 1":
                    InsertLineBreak1(sb);
                    break;
                case "Line Break 2":
                    InsertLineBreak2(sb);
                    break;
                case "New Line =":
                    InsertLineBreakForEqual(searchfor, sb);
                    break;
                default:
                    break;
            }
        }

        private void InsertField(HttpContext context, FormulaEditViewModel form, string Id, string Op, string Num, string searchfor, StringBuilder sb, ref StringBuilder repeat)
        {
            if (form.BothOper)
            {
                form.Insert = "Operator";
            }
            if (form.BothNum)
            {
                form.Insert = "Identifier";
            }
            if (form.BothIdent)
            {
                form.Insert = "Identifier";
            }
            switch (form.Insert)
            {
                case "Identifier":
                    InsertIdentifier(context, form, Id, Num, searchfor, sb, repeat);
                    form.Insert = "Operator";
                    break;
                case "Operator":
                    InsertOperator(context, form, Op, searchfor, sb, ref repeat);
                    form.Insert = "Identifier";
                    if (Op == "&hellip;" || Op == "&ctdot;")
                    {
                        form.Insert = "Operator";
                    }
                    break;
                case "Number":
                    InsertNumber(context, form, Num, searchfor, sb, repeat);
                    form.Insert = "Operator";
                    break;
                case "Text":
                    InsertText(context, form, searchfor, sb);
                    form.Insert = "Identifier";
                    break;
                case "Space":
                    InsertSpace(form, searchfor, sb);
                    form.Insert = "Identifier";
                    break;
                case "Matrix":
                    InsertMatrix(form, searchfor, sb);
                    form.Target = "Matrix";
                    form.Insert = "Number";
                    form.Matrix = true;
                    break;
                default:
                    break;
            }
        }

        private void HasAnyFieldChanged(HttpContext context, FormulaEditViewModel form)
        {
            if (form.Text != null && form.Text != context.Session.GetString("text"))
            {
                form.Insert = "Text";
            }
            if (form.Text != null && form.BothText)
            {
                form.Insert = "Text";
            }
            if (form.Text != null && form.EmbedText)
            {
                form.Insert = "Text";
            }
            if (form.Space != null && form.Space != context.Session.GetString("space"))
            {
                form.Insert = "Space";
            }
            if (form.Oper2 != null && form.Oper2 != context.Session.GetString("oper2"))
            {
                form.Insert = "Operator";
                form.Op2 = true;
            }
            if (form.Oper1 != null && form.Oper1 != context.Session.GetString("oper1") && form.Insert != "Matrix")
            {
                form.Insert = "Operator";
                form.Op2 = false;
            }
            if (form.Num2 != null && form.Num2 != context.Session.GetString("num2"))
            {
                form.Insert = "Number";
                form.N2 = true;
            }
            if (form.Num1 != null && form.Num1 != context.Session.GetString("num1"))
            {
                form.Insert = "Number";
                form.N2 = false;
            }
            if (form.Ident2 != null && form.Ident2 != context.Session.GetString("ident2"))
            {
                form.Insert = "Identifier";
                form.Id2 = true;
            }
            if (form.Ident1 != null && form.Ident1 != context.Session.GetString("ident1"))
            {
                form.Insert = "Identifier";
                form.Id2 = false;
            }
        }

        private static void FirstOrSecondField(FormulaEditViewModel form, out string Id, out string Op, out string Num)
        {
            if (form.Id2)
            {
                Id = form.Ident2;
                form.Insert = "Identifier";
            }
            else
            {
                Id = form.Ident1;
            }
            if (form.Op2)
            {
                Op = form.Oper2;
                form.Insert = "Operator";
            }
            else
            {
                Op = form.Oper1;
            }
            if (form.N2)
            {
                Num = form.Num2;
                form.Insert = "Number";
            }
            else
            {
                Num = form.Num1;
            }
            if (form.Ident3 != "")
            {
                Id = form.Ident3;
            }
            if (form.Oper3 != "")
            {
                Op = form.Oper3;
            }
            if (form.Num3 != "")
            {
                Op = form.Num3;
            }
        }

        private static void SpecialMathsSymbols(FormulaEditViewModel form)
        {
            form.Ident3 = "";
            form.Num3 = "";
            form.Oper3 = "";
            if (form.Algebraic != "None")
            {
                form.Oper3 = "&" + form.Algebraic;
                form.Insert = "Operator";
                form.Algebraic = "None";
            }
            if (form.Calculus != "None")
            {
                if (form.Calculus == "&infin;")
                {
                    form.Num3 = "&" + "infin;";
                    form.Insert = "Number";
                    form.Calculus = "None";
                }
                else
                {
                    form.Oper3 = "&" + form.Calculus;
                    form.Ident3 = "&" + form.Calculus;
                    form.Insert = "Operator";
                    form.Calculus = "None";
                }
            }
            if (form.Ellipses != "None")
            {
                form.Oper3 = "&" + form.Ellipses;
                form.Insert = "Operator";
                form.Ellipses = "None";
            }
            if (form.Logic != "None")
            {
                form.Oper3 = "&" + form.Logic;
                form.Insert = "Operator";
                form.Logic = "None";
            }
            if (form.Vector != "None")
            {
                form.Oper3 = "&" + form.Vector;
                form.Insert = "Operator";
                form.Vector = "None";
            }
            if (form.Set != "None")
            {
                form.Oper3 = "&" + form.Set;
                form.Insert = "Operator";
                form.Set = "None";
            }
            if (form.Geometric != "None")
            {
                form.Oper3 = "&" + form.Geometric;
                form.Insert = "Operator";
                form.Geometric = "None";
            }
            if (form.GreekUpper != "None")
            {
                form.Ident3 = "&" + form.GreekUpper;
                form.Oper3 = "&" + form.GreekUpper;
                form.Insert = "Identifier";
                form.GreekUpper = "None";
            }
            if (form.GreekLower != "None")
            {
                form.Ident3 = "&" + form.GreekLower;
                form.Oper3 = "&" + form.GreekLower;
                form.Insert = "Identifier";
                form.GreekLower = "None";
            }
        }

        private static void WhereToInsertNewMathMarkUp(FormulaEditViewModel form, ref string searchfor, ref bool insert, bool Matrix_Row_Col_In_Range)
        {
            if (form.Matrix)
            {
                if (Matrix_Row_Col_In_Range)
                {
                    searchfor = " #col" + form.Column + "row" + form.Row;
                    insert = true;
                }
            }
            switch (form.Target)
            {
                case "Append":
                    searchfor = " </math>";
                    insert = true;
                    break;
                case "Numerator":
                    searchfor = " #numerator";
                    insert = true;
                    break;
                case "Denominator":
                    searchfor = " #denominator";
                    insert = true;
                    break;
                case "Matrix":
                    if (Matrix_Row_Col_In_Range)
                    {
                        searchfor = " #col" + form.Column + "row" + form.Row;
                        insert = true;
                    }
                    break;
                case "Subscript Row1":
                    searchfor = " #subrow1";
                    insert = true;
                    break;
                case "Subscript Row2":
                    searchfor = " #subrow2";
                    insert = true;
                    break;
                case "Superscript Row1":
                    searchfor = " #suprow1";
                    insert = true;
                    break;
                case "Superscript Row2":
                    searchfor = " #suprow2";
                    insert = true;
                    break;
                case "Square Root Row":
                    searchfor = " #sqrtrow";
                    insert = true;
                    break;
                case "Root Row":
                    searchfor = " #rootrow";
                    insert = true;
                    break;
                case "Over Row":
                    searchfor = " #overrow";
                    insert = true;
                    break;
                case "Under Row":
                    searchfor = " #underrow";
                    insert = true;
                    break;
                case "Limit":
                    searchfor = " #underrow";
                    insert = true;
                    break;
                case "Limit h->0":
                    searchfor = " </math>";
                    insert = true;
                    break;
                case "Fenced":
                    searchfor = " #fenced";
                    insert = true;
                    break;
                case "Row":
                    searchfor = " #row";
                    insert = true;
                    break;
                case "Intergral":
                    searchfor = " #introw";
                    insert = true;
                    break;
                case "Clear Subscript Row1":
                    form.Insert = "None";
                    insert = true;
                    break;
                case "Clear Subscript Row2":
                    form.Insert = "None";
                    insert = true;
                    break;
                case "Clear Matrix":
                    form.Insert = "None";
                    insert = true;
                    break;
                case "Clear Superscript Row1":
                    form.Insert = "None";
                    insert = true;
                    break;
                case "Clear Superscript Row2":
                    form.Insert = "None";
                    insert = true;
                    break;
                case "Clear Square Root Row":
                    form.Insert = "None";
                    insert = true;
                    break;
                case "Clear Root Row":
                    form.Insert = "None";
                    insert = true;
                    break;
                case "Clear Over Row":
                    form.Insert = "None";
                    insert = true;
                    break;
                case "Clear Under Row":
                    form.Insert = "None";
                    insert = true;
                    break;
                case "Clear Row":
                    form.Insert = "None";
                    insert = true;
                    break;
                case "Clear Intergral":
                    form.Insert = "None";
                    insert = true;
                    break;
                case "Clear Fenced":
                    form.Insert = "None";
                    insert = true;
                    break;
                default:
                    searchfor = " </math>";
                    insert = true;
                    break;
            }
        }

        private static void WasAClearButtonPressed(FormulaEditViewModel form, string clear)
        {
            switch (clear)
            {
                case "Clear Superscript Row1":
                    form.Target = "Clear Superscript Row1";
                    break;
                case "Clear Subscript Row1":
                    form.Target = "Clear Subscript Row1";
                    break;
                case "Clear Superscript Row2":
                    form.Target = "Clear Superscript Row2";
                    break;
                case "Clear Subscript Row2":
                    form.Target = "Clear Subscript Row2";
                    break;
                case "Clear Matrix":
                    form.Target = "Clear Matrix";
                    break;
                case "Clear Under Row":
                    form.Target = "Clear Under Row";
                    break;
                case "Clear Over Row":
                    form.Target = "Clear Over Row";
                    break;
                case "Clear Square Root Row":
                    form.Target = "Clear Square Root Row";
                    break;
                case "Clear Root Row":
                    form.Target = "Clear Root Row";
                    break;
                case "Clear Fenced":
                    form.Target = "Clear Fenced";
                    break;
                case "Clear Row":
                    form.Target = "Clear Row";
                    break;
                case "Clear Intergral":
                    form.Target = "Clear Intergral";
                    break;
                default:
                    break;
            }
        }

        private bool CheckMatrix(FormulaEditViewModel form)
        {
            if (string.IsNullOrEmpty(form.Row.ToString()))
            {
                ModelState.AddModelError("Row", "Please enter Matrix Row");
                return false;
            }

            if (string.IsNullOrEmpty(form.Column.ToString()))
            {
                ModelState.AddModelError("Column", "Please enter Matrix Column");
                return false;
            }
            return true;
        }

        private static FormulaEditViewModel InitEditModel(int id, string formula)
        {
            return new FormulaEditViewModel
            {
                NodeID = id,
                ClearFormula = false,
                Reverse = false,
                ClearNumerator = false,
                ClearDenominator = false,
                BothIdent = false,
                BoldIdent = false,
                Ident1 = "x",
                Ident2 = "y",
                Ident3 = "",
                Oper1 = "+",
                Oper2 = "=",
                Oper3 = "",
                BothOper = false,
                Num1 = "2",
                Num2 = "1",
                Num3 = "",
                BoldNum = false,
                Space = "2",
                BoldText = false,
                BothText = false,
                EmbedText = false,
                Text = ",",
                Matrix = false,
                Row = null,
                Column = null,
                Insert = "Identifier",
                Insert1 = "None",
                Target = "Append",
                Block = "inline",
                Algebraic = "None",
                Calculus = "None",
                Ellipses = "None",
                Logic = "None",
                Vector = "None",
                Set = "None",
                Geometric = "None",
                GreekUpper = "None",
                GreekLower = "None",
                ContainsSupRow1 = false,
                ContainsSupRow2 = false,
                ContainsSubRow1 = false,
                ContainsSubRow2 = false,
                ContainsMatrix = false,
                ContainsNumerator = false,
                ContainsDenominator = false,
                ContainsFenced = false,
                ContainsSquareRootRow = false,
                ContainsRootRow = false,
                ContainsOverRow = false,
                ContainsUnderRow = false,
                ContainsRow = false,
                Id2 = false,
                Op2 = false,
                N2 = false,
                Formula = formula
            };
        }

        private void InitSessionVariables(HttpContext context)
        {
            int undoptr = 1;
            context.Session.SetInt32("undoptr", undoptr);
            string ident1 = "x";
            context.Session.SetString("ident1", ident1);
            string ident2 = "y";
            context.Session.SetString("ident2", ident2);
            string oper1 = "+";
            context.Session.SetString("oper1", oper1);
            string oper2 = "=";
            context.Session.SetString("oper2", oper2);
            string num1 = "2";
            context.Session.SetString("num1", num1);
            string num2 = "1";
            context.Session.SetString("num2", num2);
            string space = "2";
            context.Session.SetString("space", space);
            string text = ",";
            context.Session.SetString("text", text);
            string matrix = "false";
            context.Session.SetString("matrix", matrix);
            string plusorminusyet = "false";
            context.Session.SetString("plusorminusyet", plusorminusyet);
            string repeat = "<math xmlns=" + '"' + "http://www.w3.org/1998/Math/MathML" + '"' + " display='inline'> </math>";
            context.Session.SetString("repeat", repeat);
            context.Session.CommitAsync();
        }

        private void SetSessionVariables(HttpContext context, FormulaEditViewModel form)
        {
            context.Session.SetString("ident2", (form.Ident2 ?? "").ToString());
            context.Session.SetString("ident1", (form.Ident1 ?? "").ToString());
            context.Session.SetString("oper2", (form.Oper2 ?? "").ToString());
            context.Session.SetString("oper1", (form.Oper1 ?? "").ToString());
            context.Session.SetString("num2", (form.Num2 ?? "").ToString());
            context.Session.SetString("num1", (form.Num1 ?? "").ToString());
            context.Session.SetString("space", (form.Space ?? "").ToString());
            context.Session.SetString("text", (form.Text ?? "").ToString());
            context.Session.SetString("formula", (form.Formula ?? "").ToString());
            context.Session.CommitAsync();
        }

        private void RestoreFields(HttpContext context, FormulaEditViewModel form)
        {
            form.Ident1 = (context.Session.GetString("ident1") ?? "").ToString();
            form.Ident2 = (context.Session.GetString("ident2") ?? "").ToString();
            form.Oper1 = (context.Session.GetString("oper1") ?? "").ToString();
            form.Oper2 = (context.Session.GetString("oper2") ?? "").ToString();
            form.Num2 = (context.Session.GetString("num2") ?? "").ToString();
            form.Num1 = (context.Session.GetString("num1") ?? "").ToString();
            form.Space = (context.Session.GetString("space") ?? "").ToString();
            form.Text = (context.Session.GetString("text") ?? "").ToString();
            form.Oper2 = "=";
            string oper2 = "=";
            context.Session.SetString("oper2", oper2);
            string plusorminusyet = "false";
            context.Session.SetString("plusorminusyet", plusorminusyet);
            int repeatstart = (context.Session.GetString("formula") ?? ">  ").ToString().IndexOf(">") + 2;
            context.Session.SetInt32("repeatstart", repeatstart);
            context.Session.CommitAsync();
            form.Insert = "Identifier";
        }

        private static void MoveTarget(FormulaEditViewModel form, StringBuilder sb)
        {
            int indexOfSupRow1, indexOfSupRow2, indexOfSubRow1, indexOfSubRow2, indexOfFenced, indexOfSqrtRow, indexOfRootRow, indexOfOverRow, indexOfUnderRow, indexOfIntRow, indexOfRow, indexOfNumerator, indexOfDenominator, least;
            bool foundOne;

            foundOne = false;
            least = sb.ToString().Length;

            if (sb.ToString().Contains("#suprow1"))
            {
                foundOne = true;
                indexOfSupRow1 = sb.ToString().IndexOf("#suprow1");
                if (indexOfSupRow1 < least)
                {
                    least = indexOfSupRow1;
                    form.Target = "Superscript Row1";
                    form.Insert = "Identifier";
                }
            }
            if (sb.ToString().Contains("#suprow2"))
            {
                foundOne = true;
                indexOfSupRow2 = sb.ToString().IndexOf("#suprow2");
                if (indexOfSupRow2 < least)
                {
                    least = indexOfSupRow2;
                    form.Target = "Superscript Row2";
                    form.Insert = "Identifier";
                }
            }
            if (sb.ToString().Contains("#subrow1"))
            {
                foundOne = true;
                indexOfSubRow1 = sb.ToString().IndexOf("#subrow1");
                if (indexOfSubRow1 < least)
                {
                    least = indexOfSubRow1;
                    form.Target = "Subscript Row1";
                    form.Insert = "Identifier";
                }
            }
            if (sb.ToString().Contains("#subrow2"))
            {
                foundOne = true;
                indexOfSubRow2 = sb.ToString().IndexOf("#subrow2");
                if (indexOfSubRow2 < least)
                {
                    least = indexOfSubRow2;
                    form.Target = "Subscript Row2";
                    form.Insert = "Identifier";
                }
            }
            if (sb.ToString().Contains("#fenced"))
            {
                foundOne = true;
                indexOfFenced = sb.ToString().IndexOf("#fenced");
                if (indexOfFenced < least)
                {
                    least = indexOfFenced;
                    form.Target = "Fenced";
                    form.Insert = "Operator";
                }

            }
            if (sb.ToString().Contains("#sqrtrow"))
            {
                foundOne = true;
                indexOfSqrtRow = sb.ToString().IndexOf("#sqrtrow");
                if (indexOfSqrtRow < least)
                {
                    least = indexOfSqrtRow;
                    form.Target = "Square Root Row";
                    form.Insert = "Identifier";
                }
            }
            if (sb.ToString().Contains("#rootrow"))
            {
                foundOne = true;
                indexOfRootRow = sb.ToString().IndexOf("#rootrow");
                if (indexOfRootRow < least)
                {
                    least = indexOfRootRow;
                    form.Target = "Root Row";
                    form.Insert = "Identifier";
                }
            }
            if (sb.ToString().Contains("#overrow"))
            {
                foundOne = true;
                indexOfOverRow = sb.ToString().IndexOf("#overrow");
                if (indexOfOverRow < least)
                {
                    least = indexOfOverRow;
                    form.Target = "Over Row";
                    form.Insert = "Identifier";
                }
            }
            if (sb.ToString().Contains("#underrow"))
            {
                foundOne = true;
                indexOfUnderRow = sb.ToString().IndexOf("#underrow");
                if (indexOfUnderRow < least)
                {
                    least = indexOfUnderRow;
                    form.Target = "Under Row";
                    form.Insert = "Identifier";
                }
            }
            if (sb.ToString().Contains("#introw"))
            {
                foundOne = true;
                indexOfIntRow = sb.ToString().IndexOf("#introw");
                if (indexOfIntRow < least)
                {
                    least = indexOfIntRow;
                    form.Target = "Intergral";
                    form.Insert = "Identifier";
                }
            }
            if (sb.ToString().Contains("#row"))
            {
                indexOfRow = sb.ToString().IndexOf("#row");
                if (indexOfRow < least)
                {
                    least = indexOfRow;
                    form.Target = "Row";
                    form.Insert = "Identifier";
                }
            }
            if (sb.ToString().Contains("#numerator"))
            {
                indexOfNumerator = sb.ToString().IndexOf("#numerator");
                if (indexOfNumerator < least)
                {
                    least = indexOfNumerator;
                    form.Target = "Numerator";
                    form.Insert = "Identifier";
                }
            }
            if (sb.ToString().Contains("#denominator"))
            {
                foundOne = true;
                indexOfDenominator = sb.ToString().IndexOf("#denominator");
                if (indexOfDenominator < least)
                {
                    least = indexOfDenominator;
                    form.Target = "Denominator";
                    form.Insert = "Identifier";
                }
            }
            if (!foundOne)
            {
                if (!form.Matrix)
                {
                    form.Target = "Append";
                }
                else
                {
                    form.Target = "Matrix";
                }
                form.Insert = "Operator";
            }
        }

        private static void InsertLineBreakForEqual(string searchfor, StringBuilder sb)
        {
            if (sb.ToString().Contains(searchfor)) { sb.Insert(sb.ToString().IndexOf(searchfor), "<mspace width=2em /> <mo>=</mo> "); }
        }

        private static void InsertLineBreak1(StringBuilder sb)
        {
            if (sb.ToString().Contains("</math>")) { sb.Replace("</math>", " </math><br/>"); }
        }

        private static void InsertLineBreak2(StringBuilder sb)
        {
            if (sb.ToString().Contains("</math>")) { sb.Replace("</math>", " </math><br/><br/>"); }
        }

        private static void InsertText(HttpContext context, FormulaEditViewModel form, string searchfor, StringBuilder sb)
        {
            if (form.EmbedText && sb.ToString().Contains(searchfor))
            {
                sb.Insert(sb.ToString().IndexOf(searchfor), " <mspace width=" + form.Space + "em /> <mspace width=.2em />" + form.Text + "</mtext> <mspace width=.2em /> " + " <mspace width=" + form.Space + "em /> <mspace width=.2em />");
                form.Oper2 = "=";
                string oper2 = "=";
                context.Session.SetString("oper2", oper2);
                string plusorminusyet = "false";
                context.Session.SetString("plusorminusyet", plusorminusyet);
                context.Session.CommitAsync();
            }
            else if (form.BothText && sb.ToString().Contains(searchfor))
            {
                sb.Insert(sb.ToString().IndexOf(searchfor), " <mspace width=" + form.Space + "em /> <mspace width=.2em /> <mtext mathvariant='bold'>" + form.Text + "</mtext> <mspace width=.2em /> ");
            }
            else if (form.BoldText && sb.ToString().Contains(searchfor))
            {
                sb.Insert(sb.ToString().IndexOf(searchfor), " <mspace width=.2em /> <mtext mathvariant='bold'>" + form.Text + "</mtext> <mspace width=.2em /> ");
            }
            else
            {
                sb.Insert(sb.ToString().IndexOf(searchfor), " <mspace width=.2em /> <mtext>" + form.Text + "</mtext> <mspace width=.2em /> ");
            }
            form.Text = ",";
        }

        private static void InsertSpace(FormulaEditViewModel form, string searchfor, StringBuilder sb)
        {
            if (sb.ToString().Contains(searchfor)) { sb.Insert(sb.ToString().IndexOf(searchfor), " <mspace width=" + form.Space + "em />"); }
            form.Space = "2";
        }

        private static void InsertFenced3(HttpContext context, FormulaEditViewModel form, string searchfor, StringBuilder sb)
        {
            sb.Insert(sb.ToString().IndexOf(searchfor), " <mrow> <mo>(</mo> <mi>" + form.Ident1 + "</mi> <mo>,</mo> <mi>" + form.Ident2 + "</mi> <mo>,</mo> <mi>" + form.Oper2 + "</mi> <mo>)</mo> </mrow>");
            if (context.Session.GetString("plusorminusyet") == "true")
            {
                form.Oper2 = "-";
            }
            else
            {
                form.Oper2 = "=";
                string oper2 = "=";
                context.Session.SetString("oper2", oper2);
                context.Session.CommitAsync();
            }
        }

        private static void InsertFencedNum1(FormulaEditViewModel form, string searchfor, StringBuilder sb)
        {
            sb.Insert(sb.ToString().IndexOf(searchfor), " <mrow> <mo>(</mo> <mi>" + form.Num1 + "</mi> <mo>)</mo> </mrow>");
        }

        private static void InsertFencedNum2(FormulaEditViewModel form, string searchfor, StringBuilder sb)
        {
            if (form.Reverse)
            {
                sb.Insert(sb.ToString().IndexOf(searchfor), " <mrow> <mo>(</mo> <mi>" + form.Num2 + "</mi> <mo>,</mo> <mi>" + form.Num1 + "</mi> <mo>)</mo> </mrow>");
            }
            else
            {
                sb.Insert(sb.ToString().IndexOf(searchfor), " <mrow> <mo>(</mo> <mi>" + form.Num1 + "</mi> <mo>,</mo> <mi>" + form.Num2 + "</mi> <mo>)</mo> </mrow>");
            }
        }

        private static void InsertFencedNum3(HttpContext context, FormulaEditViewModel form, string searchfor, StringBuilder sb)
        {
            sb.Insert(sb.ToString().IndexOf(searchfor), " <mrow> <mo>(</mo> <mi>" + form.Num1 + "</mi> <mo>,</mo> <mi>" + form.Num2 + "</mi> <mo>,</mo> <mi>" + form.Oper2 + "</mi> <mo>)</mo> </mrow>");
            if (context.Session.GetString("plusorminusyet") == "true")
            {
                form.Oper2 = "-";
            }
            else
            {
                form.Oper2 = "=";
                string oper2 = "=";
                context.Session.SetString("oper2", oper2);
                context.Session.CommitAsync();
            }
        }

        private static void InsertFenced2(FormulaEditViewModel form, string searchfor, StringBuilder sb)
        {
            if (form.Reverse)
            {
                sb.Insert(sb.ToString().IndexOf(searchfor), " <mrow> <mo>(</mo> <mi>" + form.Ident2 + "</mi> <mo>,</mo> <mi>" + form.Ident1 + "</mi> <mo>)</mo> </mrow>");
            }
            else
            {
                sb.Insert(sb.ToString().IndexOf(searchfor), " <mrow> <mo>(</mo> <mi>" + form.Ident1 + "</mi> <mo>,</mo> <mi>" + form.Ident2 + "</mi> <mo>)</mo> </mrow>");
            }
        }

        private static void InsertFenced1(string Id, string searchfor, StringBuilder sb)
        {
            sb.Insert(sb.ToString().IndexOf(searchfor), " <mrow> <mo>(</mo> <mi>" + Id + "</mi> <mo>)</mo> </mrow>");
        }

        private static void InsertFenced0(FormulaEditViewModel form, string searchfor, StringBuilder sb)
        {
            if (form.Oper1 == "[")
            {
                sb.Insert(sb.ToString().IndexOf(searchfor), " <mo>[</mo> #fenced <mo>]</mo> ");
                form.Oper1 = "+";
            }
            else if (form.Oper1 == "{")
            {
                sb.Insert(sb.ToString().IndexOf(searchfor), " <mo>{</mo> #fenced <mo>}</mo> ");
                form.Oper1 = "+";
            }
            else if (form.Oper1 == "|")
            {
                sb.Insert(sb.ToString().IndexOf(searchfor), " <mo>|</mo> #fenced <mo>|</mo> ");
                form.Oper1 = "+";
            }
            else
            {
                sb.Insert(sb.ToString().IndexOf(searchfor), " <mo>(</mo> #fenced <mo>)</mo> ");
            }
        }

        private static void InsertOverRow(FormulaEditViewModel form, string Op, string searchfor, StringBuilder sb)
        {
            if (sb.ToString().Contains(searchfor))
            {
                sb.Insert(sb.ToString().IndexOf(searchfor), " <mover> <mrow> #overrow </mrow> <mo>" + Op + "</mo> </mover>");
            }
        }

        private static void InsertOver(FormulaEditViewModel form, string Id, string Op, string searchfor, StringBuilder sb)
        {
            if (form.BoldIdent && sb.ToString().Contains(searchfor))
            {
                sb.Insert(sb.ToString().IndexOf(searchfor), " <mover> <mi mathvariant='bold'>" + Id + "</mi> <mo>" + Op + "</mo> </mover>");
            }
            else
            {
                sb.Insert(sb.ToString().IndexOf(searchfor), " <mover> <mi>" + Id + "</mi> <mo>" + Op + "</mo> </mover>");
            }
        }

        private static void InsertLimit(string searchfor, StringBuilder sb)
        {
            if (sb.ToString().Contains(searchfor))
            {
                sb.Insert(sb.ToString().IndexOf(searchfor), " <munder> <mo>lim</mo> <mrow> #underrow </mrow> </munder>");
            }
        }

        private static void InsertLimith0(string searchfor, StringBuilder sb)
        {
            if (sb.ToString().Contains(searchfor))
            {
                sb.Insert(sb.ToString().IndexOf(searchfor), " <munder> <mo>lim</mo> <mrow> <mi>h</mi> <mo>&rarr;</mo> <mn>0</mo> </mrow> </munder>");
            }
        }

        private static void InsertUnder(string Op, string searchfor, StringBuilder sb)
        {
            if (sb.ToString().Contains(searchfor))
            {
                sb.Insert(sb.ToString().IndexOf(searchfor), " <munder> <mo>" + Op + "</mo> <mrow> #underrow </mrow> </munder>");
            }
        }

        private static void InsertUnderOver(string Op, string searchfor, StringBuilder sb)
        {
            if (sb.ToString().Contains(searchfor))
            {
                sb.Insert(sb.ToString().IndexOf(searchfor), " <munderover> <mo>" + Op + "</mo> <mrow> #underrow </mrow> <mrow> #overrow </mrow> </munderover>");
            }
        }
        private static void InsertIntergralDefinite(string Id, string searchfor, StringBuilder sb)
        {
            if (sb.ToString().Contains(searchfor))
            {
                sb.Insert(sb.ToString().IndexOf(searchfor), " <munderover> <mo>&int;</mo> <mrow> #underrow </mrow> <mrow> #overrow </mrow> </munderover> #introw <mspace width=.2em /> <mi>d</mi> <mi>" + Id + "</mi>");
            }
        }
        private static void InsertIntergralInDefinite(string Id, string searchfor, StringBuilder sb)
        {
            if (sb.ToString().Contains(searchfor))
            {
                sb.Insert(sb.ToString().IndexOf(searchfor), " <mo>&int;</mo> #introw <mspace width=.2em /> <mi>d</mi> <mi>" + Id + "</mi>");
            }
        }

        private static void InsertRoot(FormulaEditViewModel form, string searchfor, StringBuilder sb)
        {
            if (sb.ToString().Contains(searchfor))
            {
                sb.Insert(sb.ToString().IndexOf(searchfor), " <mroot> <mrow> #rootrow </mrow> <mn>" + form.Num1 + "</mn></mroot>");
            }
        }

        private static void InsertSquareRoot(string searchfor, StringBuilder sb)
        {
            if (sb.ToString().Contains(searchfor)) { sb.Insert(sb.ToString().IndexOf(searchfor), " <msqrt> <mrow> #sqrtrow </mrow> </msqrt>"); }
        }
        private static void InsertFraction(string searchfor, StringBuilder sb, StringBuilder repeat)
        {
            if (sb.ToString().Contains(searchfor))
            {
                sb.Insert(sb.ToString().IndexOf(searchfor), " <mstyle mathsize='1.2em'> <mfrac> <mrow> #numerator </mrow> <mrow> #denominator </mrow> </mfrac> </mstyle>");
                repeat.Insert(repeat.ToString().IndexOf(searchfor), " <mstyle mathsize='1.2em'> <mfrac> <mrow> #numerator </mrow> <mrow> #denominator </mrow> </mfrac> </mstyle>");
            }
        }
        private static void InsertFractionNum(FormulaEditViewModel form, string searchfor, StringBuilder sb)
        {
            if (form.Reverse)
            {
                if (sb.ToString().Contains(searchfor)) { sb.Insert(sb.ToString().IndexOf(searchfor), " <mstyle mathsize='1.2em'> <mfrac> <mrow> <mn>" + form.Num2 + "</mn> </mrow> <mrow> <mn>" + form.Num1 + "</mn> </mrow> </mfrac> </mstyle>"); }
            }
            else
            {
                if (sb.ToString().Contains(searchfor)) { sb.Insert(sb.ToString().IndexOf(searchfor), " <mstyle mathsize='1.2em'> <mfrac> <mrow> <mn>" + form.Num1 + "</mn> </mrow> <mrow> <mn>" + form.Num2 + "</mn> </mrow> </mfrac> </mstyle>"); }
            }
        }
        private static void InsertFractionVar(FormulaEditViewModel form, string searchfor, StringBuilder sb)
        {
            if (form.Reverse)
            {
                if (sb.ToString().Contains(searchfor)) { sb.Insert(sb.ToString().IndexOf(searchfor), " <mstyle mathsize='1.2em'> <mfrac> <mrow> <mi>" + form.Ident2 + "</mi> </mrow> <mrow> <mi>" + form.Ident1 + "</mi> </mrow> </mfrac> </mstyle>"); }
            }
            else
            {
                if (sb.ToString().Contains(searchfor)) { sb.Insert(sb.ToString().IndexOf(searchfor), " <mstyle mathsize='1.2em'> <mfrac> <mrow> <mi>" + form.Ident1 + "</mi> </mrow> <mrow> <mi>" + form.Ident2 + "</mi> </mrow> </mfrac> </mstyle>"); }
            }
        }
        private static void InsertFractionVarNum(FormulaEditViewModel form, string Id, string Num, string searchfor, StringBuilder sb)
        {
            if (form.Reverse)
            {
                if (sb.ToString().Contains(searchfor)) { sb.Insert(sb.ToString().IndexOf(searchfor), " <mstyle mathsize='1.2em'> <mfrac> <mrow> <mn>" + Num + "</mn> </mrow> <mrow> <mi>" + Id + "</mi> </mrow> </mfrac> </mstyle>"); }
            }
            else
            {
                if (sb.ToString().Contains(searchfor)) { sb.Insert(sb.ToString().IndexOf(searchfor), " <mstyle mathsize='1.2em'> <mfrac> <mrow> <mi>" + Id + "</mi> </mrow> <mrow> <mn>" + Num + "</mn> </mrow> </mfrac> </mstyle>"); }
            }
        }
        private static void InsertFractionDiff(FormulaEditViewModel form, string searchfor, StringBuilder sb)
        {
            if (form.Reverse)
            {
                if (sb.ToString().Contains(searchfor)) { sb.Insert(sb.ToString().IndexOf(searchfor), " <mstyle mathsize='1.2em'> <mfrac> <mrow> <mi>d</mi> <mi>" + form.Ident2 + "</mi> </mrow> <mrow> <mi>d</mi> <mi>" + form.Ident1 + "</mi> </mrow> </mfrac> </mstyle>"); }
            }
            else
            {
                if (sb.ToString().Contains(searchfor)) { sb.Insert(sb.ToString().IndexOf(searchfor), " <mstyle mathsize='1.2em'> <mfrac> <mrow> <mi>d</mi> <mi>" + form.Ident1 + "</mi> </mrow> <mrow> <mi>d</mi> <mi>" + form.Ident2 + "</mi> </mrow> </mfrac> </mstyle>"); }
            }
        }
        private static void InsertFractionDiff2(FormulaEditViewModel form, string searchfor, StringBuilder sb)
        {
            if (form.Reverse)
            {
                if (sb.ToString().Contains(searchfor)) { sb.Insert(sb.ToString().IndexOf(searchfor), " <mstyle mathsize='1.2em'> <mfrac> <mrow> <msup> <mi>d</mi> <mn>2</mn> </msup> <mi>" + form.Ident2 + "</mi> </mrow> <mrow> <mi>d</mi> <msup> <mi>" + form.Ident1 + "</mi> <mn>2</mn> </msup> </mrow> </mfrac> </mstyle>"); }
            }
            else
            {
                if (sb.ToString().Contains(searchfor)) { sb.Insert(sb.ToString().IndexOf(searchfor), " <mstyle mathsize='1.2em'> <mfrac> <mrow> <msup> <mi>d</mi> <mn>2</mn> </msup> <mi>" + form.Ident1 + "</mi> </mrow> <mrow> <mi>d</mi> <msup> <mi>" + form.Ident2 + "</mi> <mn>2</mn> </msup> </mrow> </mfrac> </mstyle>"); }
            }
        }
        private static void InsertFractionPart(FormulaEditViewModel form, string searchfor, StringBuilder sb)
        {
            if (form.Reverse)
            {
                if (sb.ToString().Contains(searchfor)) { sb.Insert(sb.ToString().IndexOf(searchfor), " <mstyle mathsize='1.2em'> <mfrac> <mrow> <mo>&part;</mo> <mi>" + form.Ident2 + "</mi> </mrow> <mrow> <mo>&part;</mo> <mi>" + form.Ident1 + "</mi> </mrow> </mfrac> </mstyle>"); }
            }
            else
            {
                if (sb.ToString().Contains(searchfor)) { sb.Insert(sb.ToString().IndexOf(searchfor), " <mstyle mathsize='1.2em'> <mfrac> <mrow> <mo>&part;</mo> <mi>" + form.Ident1 + "</mi> </mrow> <mrow> <mo>&part;</mo> <mi>" + form.Ident2 + "</mi> </mrow> </mfrac> </mstyle>"); }
            }
        }
        private static void InsertFractionPart2(FormulaEditViewModel form, string searchfor, StringBuilder sb)
        {
            if (form.Reverse)
            {
                if (sb.ToString().Contains(searchfor)) { sb.Insert(sb.ToString().IndexOf(searchfor), " <mstyle mathsize='1.2em'> <mfrac> <mrow> <msup> <mo>&part;</mo> <mn>2</mn> </msup> <mi>" + form.Ident2 + "</mi> </mrow> <mrow> <mo>&part;</mo> <msup> <mi>" + form.Ident1 + "</mi> <mn>2</mn> </msup> </mrow> </mfrac> </mstyle>"); }
            }
            else
            {
                if (sb.ToString().Contains(searchfor)) { sb.Insert(sb.ToString().IndexOf(searchfor), " <mstyle mathsize='1.2em'> <mfrac> <mrow> <msup> <mo>&part;</mo> <mn>2</mn> </msup> <mi>" + form.Ident1 + "</mi> </mrow> <mrow> <mo>&part;</mo> <msup> <mi>" + form.Ident2 + "</mi> <mn>2</mn> </msup> </mrow> </mfrac> </mstyle>"); }
            }
        }
        private static void InsertRow(string searchfor, StringBuilder sb)
        {
            if (sb.ToString().Contains(searchfor)) { sb.Insert(sb.ToString().IndexOf(searchfor), " <mrow> #row </mrow>"); }
        }

        private static void InsertSubSupIdentifierRowRow(FormulaEditViewModel form, string Id, string searchfor, StringBuilder sb)
        {
            if (form.BothNum && sb.ToString().Contains(searchfor))
            {
                sb.Insert(sb.ToString().IndexOf(searchfor), " <mn>" + form.Num1 + "</mn>");
            }
            if (form.BoldIdent && sb.ToString().Contains(searchfor))
            {
                sb.Insert(sb.ToString().IndexOf(searchfor), " <msubsup> <mi mathvariant='bold'>" + Id + "</mi> <mrow> #subrow1 </mrow> <mrow> #suprow2 </mrow> </msubsup>");
            }
            else
            {
                sb.Insert(sb.ToString().IndexOf(searchfor), " <msubsup> <mi>" + Id + "</mi> <mrow> #subrow1 </mrow> <mrow> #suprow2 </mrow> </msubsup>");
            }
        }

        private static void InsertSubSupIdentifierIdentifierIdentifier(FormulaEditViewModel form, string Id, string searchfor, StringBuilder sb)
        {
            if (form.BothNum && sb.ToString().Contains(searchfor))
            {
                sb.Insert(sb.ToString().IndexOf(searchfor), " <mn>" + form.Num2 + "</mn>");
            }
            if (form.BoldIdent && sb.ToString().Contains(searchfor))
            {
                if (form.Reverse)
                {
                    sb.Insert(sb.ToString().IndexOf(searchfor), " <msubsup> <mi mathvariant='bold'>" + Id + "</mi> <mi>" + form.Ident2 + "</mi> <mi>" + form.Num1 + "</mi> </msubsup>");
                }
                else
                {
                    sb.Insert(sb.ToString().IndexOf(searchfor), " <msubsup> <mi mathvariant='bold'>" + Id + "</mi> <mi>" + form.Num1 + "</mi> <mi>" + form.Ident2 + "</mi> </msubsup>");
                }
            }
            else
            {
                if (form.Reverse)
                {
                    sb.Insert(sb.ToString().IndexOf(searchfor), " <msubsup> <mi>" + Id + "</mi> <mi>" + form.Ident2 + "</mi> <mi>" + form.Num1 + "</mi> </msubsup>");
                }
                else
                {
                    sb.Insert(sb.ToString().IndexOf(searchfor), " <msubsup> <mi>" + Id + "</mi> <mi>" + form.Num1 + "</mi> <mi>" + form.Ident2 + "</mi> </msubsup>");
                }
            }
        }

        private static void InsertSubSupIdentifierIdentifierNumber(FormulaEditViewModel form, string Id, string Num, string searchfor, StringBuilder sb)
        {
            if (form.BothNum && sb.ToString().Contains(searchfor))
            {
                sb.Insert(sb.ToString().IndexOf(searchfor), " <mn>" + form.Num2 + "</mn>");
            }
            if (form.BoldIdent && sb.ToString().Contains(searchfor))
            {
                sb.Insert(sb.ToString().IndexOf(searchfor), " <msubsup> <mi mathvariant='bold'>" + Id + "</mi> <mi>" + form.Ident2 + "</mi> <mn>" + Num + "</mn> </msubsup>");
            }
            else
            {
                sb.Insert(sb.ToString().IndexOf(searchfor), " <msubsup> <mi>" + Id + "</mi> <mi>" + form.Ident2 + "</mi> <mn>" + Num + "</mn> </msubsup>");
            }
        }

        private static void InsertSubSupIdentifierNumberIdentifier(FormulaEditViewModel form, string Id, string Num, string searchfor, StringBuilder sb)
        {
            if (form.BothNum && sb.ToString().Contains(searchfor))
            {
                sb.Insert(sb.ToString().IndexOf(searchfor), " <mn>" + form.Num2 + "</mn>");
            }
            if (form.BoldIdent && sb.ToString().Contains(searchfor))
            {
                sb.Insert(sb.ToString().IndexOf(searchfor), " <msubsup> <mi mathvariant='bold'>" + Id + "</mi> <mn>" + Num + "</mn> <mi>" + form.Ident2 + "</mi> </msubsup>");
            }
            else
            {
                sb.Insert(sb.ToString().IndexOf(searchfor), " <msubsup> <mi>" + Id + "</mi> <mn>" + Num + "</mn> <mi>" + form.Ident2 + "</mi> </msubsup>");
            }
        }

        private static void InsertSubSupIdentifierNumberNumber(FormulaEditViewModel form, string Id, string Num, string searchfor, StringBuilder sb)
        {
            if (form.BoldIdent && sb.ToString().Contains(searchfor))
            {
                if (form.Reverse)
                {
                    if (form.Ident3 != "")
                    {
                        sb.Insert(sb.ToString().IndexOf(searchfor), " <msubsup> <mi mathvariant='bold'>" + form.Ident3 + "</mi> <mn>" + form.Num2 + "</mn> <mn>" + Num + "</mn> </msubsup>");
                    }
                    else
                    {
                        sb.Insert(sb.ToString().IndexOf(searchfor), " <msubsup> <mi mathvariant='bold'>" + Id + "</mi> <mn>" + form.Num2 + "</mn> <mn>" + Num + "</mn> </msubsup>");
                    }
                }
                else
                {
                    if (form.Ident3 != "")
                    {
                        sb.Insert(sb.ToString().IndexOf(searchfor), " <msubsup> <mi mathvariant='bold'>" + form.Ident3 + "</mi> <mn>" + Num + "</mn> <mn>" + form.Num2 + "</mn> </msubsup>");
                    }
                    else
                    {
                        sb.Insert(sb.ToString().IndexOf(searchfor), " <msubsup> <mi mathvariant='bold'>" + Id + "</mi> <mn>" + Num + "</mn> <mn>" + form.Num2 + "</mn> </msubsup>");
                    }
                }
            }
            else
            {
                if (form.Reverse)
                {
                    if (form.Ident3 != "")
                    {
                        sb.Insert(sb.ToString().IndexOf(searchfor), " <msubsup> <mi>" + form.Ident3 + "</mi> <mn>" + form.Num2 + "</mn> <mn>" + Num + "</mn> </msubsup>");
                    }
                    else
                    {
                        sb.Insert(sb.ToString().IndexOf(searchfor), " <msubsup> <mi>" + Id + "</mi> <mn>" + form.Num2 + "</mn> <mn>" + Num + "</mn> </msubsup>");
                    }
                }
                else
                {
                    if (form.Ident3 != "")
                    {
                        sb.Insert(sb.ToString().IndexOf(searchfor), " <msubsup> <mi>" + form.Ident3 + "</mi> <mn>" + Num + "</mn> <mn>" + form.Num2 + "</mn> </msubsup>");
                    }
                    else
                    {
                        sb.Insert(sb.ToString().IndexOf(searchfor), " <msubsup> <mi>" + Id + "</mi> <mn>" + Num + "</mn> <mn>" + form.Num2 + "</mn> </msubsup>");
                    }
                }
            }
        }

        private static void InsertSuperscriptRowRow(FormulaEditViewModel form, string searchfor, StringBuilder sb)
        {
            if (form.BothNum && sb.ToString().Contains(searchfor))
            {
                sb.Insert(sb.ToString().IndexOf(searchfor), " <mn>" + form.Num1 + "</mn>");
            }
            if (sb.ToString().Contains(searchfor))
            {
                if (form.Oper1 == "(")
                {
                    sb.Insert(sb.ToString().IndexOf(searchfor), " <msup> <mrow> <mo>(</mo> #suprow1 <mo>)</mo> </mrow> <mrow> #suprow2 </mrow> </msup>");
                    form.Oper1 = "+";
                }
                else if (form.Oper1 == "[")
                {
                    sb.Insert(sb.ToString().IndexOf(searchfor), " <msup> <mrow> <mo>[</mo> #suprow1 <mo>]</mo> </mrow> <mrow> #suprow2 </mrow> </msup>");
                    form.Oper1 = "+";
                }
                else if (form.Oper1 == "|")
                {
                    sb.Insert(sb.ToString().IndexOf(searchfor), " <msup> <mrow> <mo>|</mo> #suprow1 <mo>|</mo> </mrow> <mrow> #suprow2 </mrow> </msup>");
                    form.Oper1 = "+";
                }
                else if (form.Oper1 == "{")
                {
                    sb.Insert(sb.ToString().IndexOf(searchfor), " <msup> <mrow> <mo>{</mo> #suprow1 <mo>}</mo> </mrow> <mrow> #suprow2 </mrow> </msup>");
                    form.Oper1 = "+";
                }
                else
                {
                    sb.Insert(sb.ToString().IndexOf(searchfor), " <msup> <mrow> #suprow1 </mrow> <mrow> #suprow2 </mrow> </msup>");
                }
            }
        }

        private static void InsertSuperscriptRowNumber(FormulaEditViewModel form, string Num, string searchfor, StringBuilder sb)
        {
            if (form.BothNum && sb.ToString().Contains(searchfor))
            {
                sb.Insert(sb.ToString().IndexOf(searchfor), " <mn>" + form.Num2 + "</mn>");
            }
            if (sb.ToString().Contains(searchfor))
            {
                if (form.Oper1 == "(")
                {
                    sb.Insert(sb.ToString().IndexOf(searchfor), " <msup> <mrow> <mo>(</mo> #suprow1 <mo>)</mo> </mrow> <mn>" + Num + "</mn>  </msup>");
                    form.Oper1 = "+";
                }
                else if (form.Oper1 == "[")
                {
                    sb.Insert(sb.ToString().IndexOf(searchfor), " <msup> <mrow> <mo>[</mo> #suprow1 <mo>]</mo> </mrow> <mn>" + Num + "</mn>  </msup>");
                    form.Oper1 = "+";
                }
                else if (form.Oper1 == "|")
                {
                    sb.Insert(sb.ToString().IndexOf(searchfor), " <msup> <mrow> <mo>|</mo> #suprow1 <mo>|</mo> </mrow> <mn>" + Num + "</mn>  </msup>");
                    form.Oper1 = "+";
                }
                else if (form.Oper1 == "{")
                {
                    sb.Insert(sb.ToString().IndexOf(searchfor), " <msup> <mrow> <mo>{</mo> #suprow1 <mo>}</mo> </mrow> <mrow> #suprow2 </mrow> </msup>");
                    form.Oper1 = "+";
                }
                else
                {
                    sb.Insert(sb.ToString().IndexOf(searchfor), " <msup> <mrow> #suprow1 </mrow> <mn>" + Num + "</mn>  </msup>");
                }
            }
        }

        private static void InsertSuperscriptRowIdentifier(FormulaEditViewModel form, string Id, string searchfor, StringBuilder sb)
        {
            if (form.BothNum && sb.ToString().Contains(searchfor))
            {
                sb.Insert(sb.ToString().IndexOf(searchfor), " <mn>" + form.Num1 + "</mn>");
            }
            if (form.BoldIdent && sb.ToString().Contains(searchfor))
            {
                sb.Insert(sb.ToString().IndexOf(searchfor), " <msup> <mrow> #suprow1 </mrow> <mi mathvariant='bold'>" + Id + "</mi>  </msup>");
            }
            else
            {
                if (form.Oper1 == "(")
                {
                    sb.Insert(sb.ToString().IndexOf(searchfor), " <msup> <mrow> <mo>(</mo> #suprow1 <mo>)</mo> </mrow> <mi>" + Id + "</mi>  </msup>");
                    form.Oper1 = "+";
                }
                else if (form.Oper1 == "[")
                {
                    sb.Insert(sb.ToString().IndexOf(searchfor), " <msup> <mrow> <mo>[</mo> #suprow1 <mo>]</mo> </mrow> <mi>" + Id + "</mi>  </msup>");
                    form.Oper1 = "+";
                }
                else if (form.Oper1 == "|")
                {
                    sb.Insert(sb.ToString().IndexOf(searchfor), " <msup> <mrow> <mo>|</mo> #suprow1 <mo>|</mo> </mrow> <mi>" + Id + "</mi>  </msup>");
                    form.Oper1 = "+";
                }
                else if (form.Oper1 == "{")
                {
                    sb.Insert(sb.ToString().IndexOf(searchfor), " <msup> <mrow> <mo>{</mo> #suprow1 <mo>}</mo> </mrow> <mrow> #suprow2 </mrow> </msup>");
                    form.Oper1 = "+";
                }
                else
                {
                    sb.Insert(sb.ToString().IndexOf(searchfor), " <msup> <mrow> #suprow1 </mrow> <mi>" + Id + "</mi>  </msup>");
                }
            }
        }

        private static void InsertSuperscriptIdentifierRow(FormulaEditViewModel form, string Id, string searchfor, StringBuilder sb)
        {
            if (form.BothNum && sb.ToString().Contains(searchfor))
            {
                sb.Insert(sb.ToString().IndexOf(searchfor), " <mn>" + form.Num1 + "</mn>");
            }
            if (form.BoldIdent && sb.ToString().Contains(searchfor))
            {
                sb.Insert(sb.ToString().IndexOf(searchfor), " <msup> <mi mathvariant='bold'>" + Id + "</mi> <mrow> #suprow1 </mrow> </msup>");
            }
            else
            {
                sb.Insert(sb.ToString().IndexOf(searchfor), " <msup> <mi>" + Id + "</mi> <mrow> #suprow1 </mrow> </msup>");
            }
        }

        private static void InsertSuperscriptIdentifierIdentifier(FormulaEditViewModel form, string searchfor, StringBuilder sb)
        {
            if (form.BothNum && sb.ToString().Contains(searchfor))
            {
                sb.Insert(sb.ToString().IndexOf(searchfor), " <mn>" + form.Num1 + "</mn>");
            }
            if (form.Reverse)
            {
                if (form.BoldIdent && sb.ToString().Contains(searchfor))
                {
                    sb.Insert(sb.ToString().IndexOf(searchfor), " <msup> <mi mathvariant='bold'>" + form.Ident2 + "</mi> <mi mathvariant='bold'>" + form.Ident1 + "</mi> </msup>");
                }
                else
                {
                    sb.Insert(sb.ToString().IndexOf(searchfor), " <msup> <mi>" + form.Ident2 + "</mi> <mi>" + form.Ident1 + "</mi> </msup>");
                }
            }
            else
            {
                if (form.BoldIdent && sb.ToString().Contains(searchfor))
                {
                    sb.Insert(sb.ToString().IndexOf(searchfor), " <msup> <mi mathvariant='bold'>" + form.Ident1 + "</mi> <mi>" + form.Ident2 + "</mi> </msup>");
                }
                else
                {
                    sb.Insert(sb.ToString().IndexOf(searchfor), " <msup> <mi>" + form.Ident1 + "</mi> <mi>" + form.Ident2 + "</mi> </msup>");
                }
            }
        }

        private static void InsertSuperscriptNumberRow(FormulaEditViewModel form, string Num, string searchfor, StringBuilder sb)
        {
            if (form.BoldIdent && sb.ToString().Contains(searchfor))
            {
                sb.Insert(sb.ToString().IndexOf(searchfor), " <msup> <mn mathvariant='bold'>" + Num + "</mn> <mrow> #suprow1 </mrow> </msup>");
            }
            else
            {
                sb.Insert(sb.ToString().IndexOf(searchfor), " <msup> <mn>" + Num + "</mn> <mrow> #suprow1 </mrow> </msup>");
            }
        }

        private static void InsertSuperscriptNumberNumber(FormulaEditViewModel form, string searchfor, StringBuilder sb)
        {
            if (form.Reverse)
            {
                if (form.BoldIdent && sb.ToString().Contains(searchfor))
                {
                    sb.Insert(sb.ToString().IndexOf(searchfor), " <msup> <mn mathvariant='bold'>" + form.Num2 + "</mni> <mn>" + form.Num1 + "</mn> </msup>");
                }
                else
                {
                    sb.Insert(sb.ToString().IndexOf(searchfor), " <msup> <mn>" + form.Num2 + "</mn> <mn>" + form.Num1 + "</mn> </msup>");
                }
            }
            else
            {
                if (form.BoldIdent && sb.ToString().Contains(searchfor))
                {
                    sb.Insert(sb.ToString().IndexOf(searchfor), " <msup> <mn mathvariant='bold'>" + form.Num1 + "</mni> <mn>" + form.Num2 + "</mn> </msup>");
                }
                else
                {
                    sb.Insert(sb.ToString().IndexOf(searchfor), " <msup> <mn>" + form.Num1 + "</mn> <mn>" + form.Num2 + "</mn> </msup>");
                }
            }
        }

        private static void InsertSuperscriptOperatorNumber(FormulaEditViewModel form, string Op, string Num, string searchfor, StringBuilder sb)
        {
            if (form.BothNum && sb.ToString().Contains(searchfor))
            {
                sb.Insert(sb.ToString().IndexOf(searchfor), " <mn>" + form.Num2 + "</mn>");
            }
            if (form.BoldIdent && sb.ToString().Contains(searchfor))
            {
                sb.Insert(sb.ToString().IndexOf(searchfor), " <msup> <mo mathvariant='bold'>" + Op + "</mo> <mn>" + Num + "</mn> </msup>");
            }
            else
            {
                sb.Insert(sb.ToString().IndexOf(searchfor), " <msup> <mo>" + Op + "</mo> <mn>" + Num + "</mn> </msup>");
            }
        }

        private static void InsertSuperscriptIdentifierNumber(FormulaEditViewModel form, string Id, string Num, string searchfor, StringBuilder sb)
        {
            if (form.BothNum && sb.ToString().Contains(searchfor))
            {
                sb.Insert(sb.ToString().IndexOf(searchfor), " <mn>" + form.Num2 + "</mn>");
            }
            if (form.BoldIdent && sb.ToString().Contains(searchfor))
            {
                sb.Insert(sb.ToString().IndexOf(searchfor), " <msup> <mi mathvariant='bold'>" + Id + "</mi> <mn>" + Num + "</mn> </msup>");
            }
            else
            {
                sb.Insert(sb.ToString().IndexOf(searchfor), " <msup> <mi>" + Id + "</mi> <mn>" + Num + "</mn> </msup>");
            }
        }

        private static void InsertSubscriptRowRow(FormulaEditViewModel form, string searchfor, StringBuilder sb)
        {
            if (form.BothNum && sb.ToString().Contains(searchfor))
            {
                sb.Insert(sb.ToString().IndexOf(searchfor), " <mn>" + form.Num1 + "</mn>");
            }
            if (sb.ToString().Contains(searchfor)) { sb.Insert(sb.ToString().IndexOf(searchfor), " <msub> <mrow> #subrow1 </mrow> <mrow> #subrow2 </mrow> </msub>"); }
        }

        private static void InsertSubscriptRowNumber(FormulaEditViewModel form, string Num, string searchfor, StringBuilder sb)
        {
            if (form.BothNum && sb.ToString().Contains(searchfor))
            {
                sb.Insert(sb.ToString().IndexOf(searchfor), " <mn>" + form.Num2 + "</mn>");
            }
            if (sb.ToString().Contains(searchfor))
            {
                sb.Insert(sb.ToString().IndexOf(searchfor), " <msub> <mrow> #subrow1 </mrow> <mn>" + Num + "</mn> </msub>");
            }
        }

        private static void InsertSubscriptRowIdentifier(FormulaEditViewModel form, string Id, string searchfor, StringBuilder sb)
        {
            if (form.BothNum && sb.ToString().Contains(searchfor))
            {
                sb.Insert(sb.ToString().IndexOf(searchfor), " <mn>" + form.Num1 + "</mn>");
            }
            if (form.BoldIdent && sb.ToString().Contains(searchfor))
            {
                sb.Insert(sb.ToString().IndexOf(searchfor), " <msub> <mrow> #subrow1 </mrow> <mi mathvariant='bold'>" + Id + "</mi> </msub>");
            }
            else
            {
                sb.Insert(sb.ToString().IndexOf(searchfor), " <msub> <mrow> #subrow1 </mrow> <mi>" + Id + "</mi> </msub>");
            }
        }

        private static void InsertSubscriptIdentifierRow(FormulaEditViewModel form, string Id, string searchfor, StringBuilder sb)
        {
            if (form.BothNum && sb.ToString().Contains(searchfor))
            {
                sb.Insert(sb.ToString().IndexOf(searchfor), " <mn>" + form.Num1 + "</mn>");
            }
            if (form.BoldIdent && sb.ToString().Contains(searchfor))
            {
                sb.Insert(sb.ToString().IndexOf(searchfor), " <msub> <mi mathvariant='bold'>" + Id + "</mi> <mrow> #subrow1 </mrow> </msub>");
            }
            else
            {
                sb.Insert(sb.ToString().IndexOf(searchfor), " <msub> <mi>" + Id + "</mi> <mrow> #subrow1 </mrow> </msub>");
            }
        }

        private static void InsertSubscriptIdentifierOperator(FormulaEditViewModel form, string Id, string Op, string searchfor, StringBuilder sb)
        {
            if (form.BothNum && sb.ToString().Contains(searchfor))
            {
                sb.Insert(sb.ToString().IndexOf(searchfor), " <mn>" + form.Num2 + "</mn>");
            }
            if (form.BoldIdent && sb.ToString().Contains(searchfor))
            {
                sb.Insert(sb.ToString().IndexOf(searchfor), " <msub> <mi mathvariant='bold'>" + Id + "</mi> <mo>" + Op + "</mo> </msub>");
            }
            else
            {
                sb.Insert(sb.ToString().IndexOf(searchfor), " <msub> <mi>" + Id + "</mi> <mo>" + Op + "</mo> </msub>");
            }
        }

        private void InsertSubscriptIdentifierIdentifier(FormulaEditViewModel form, string searchfor, StringBuilder sb)
        {
            if (form.BothNum && sb.ToString().Contains(searchfor))
            {
                sb.Insert(sb.ToString().IndexOf(searchfor), " <mn>" + form.Num1 + "</mn>");
                ViewBag.num = form.Num1;
            }
            if (form.Reverse)
            {
                if (form.BoldIdent && sb.ToString().Contains(searchfor))
                {
                    sb.Insert(sb.ToString().IndexOf(searchfor), " <msub> <mi mathvariant='bold'>" + form.Ident2 + "</mi> <mi mathvariant='bold'>" + form.Ident1 + "</mi> </msub>");
                }
                else
                {
                    sb.Insert(sb.ToString().IndexOf(searchfor), " <msub> <mi>" + form.Ident2 + "</mi> <mi>" + form.Ident1 + "</mi> </msub>");
                }
            }
            else
            {
                if (form.BoldIdent && sb.ToString().Contains(searchfor))
                {
                    sb.Insert(sb.ToString().IndexOf(searchfor), " <msub> <mi mathvariant='bold'>" + form.Ident1 + "</mi> <mi mathvariant='bold'>" + form.Ident2 + "</mi> </msub>");
                }
                else
                {
                    sb.Insert(sb.ToString().IndexOf(searchfor), " <msub> <mi>" + form.Ident1 + "</mi> <mi>" + form.Ident2 + "</mi> </msub>");
                }

            }
        }

        private static void InsertSubscriptIdentifierNumber(FormulaEditViewModel form, string Id, string Num, string searchfor, StringBuilder sb)
        {
            if (form.BothNum && sb.ToString().Contains(searchfor))
            {
                sb.Insert(sb.ToString().IndexOf(searchfor), " <mn>" + form.Num2 + "</mn>");
            }
            if (form.BoldIdent && sb.ToString().Contains(searchfor))
            {
                sb.Insert(sb.ToString().IndexOf(searchfor), " <msub> <mi mathvariant='bold'>" + Id + "</mi> <mn>" + Num + "</mn> </msub>");
            }
            else
            {
                sb.Insert(sb.ToString().IndexOf(searchfor), " <msub> <mi>" + Id + "</mi> <mn>" + Num + "</mn> </msub>");
            }
        }

        private static void InsertMatrix(FormulaEditViewModel form, string searchfor, StringBuilder sb)
        {
            if (sb.ToString().Contains(searchfor))
            {
                if (form.Oper1 == "(")
                {
                    sb.Insert(sb.ToString().IndexOf(searchfor), " <mspace width=.2em /> <mrow> <mo>(</mo> <mtable>");
                    form.Oper1 = "+";
                }
                else if (form.Oper1 == "|")
                {
                    sb.Insert(sb.ToString().IndexOf(searchfor), " <mspace width=.2em /> <mrow> <mo>|</mo> <mtable>");
                    form.Oper1 = "+";
                }
                else if (form.Oper1 == "{")
                {
                    sb.Insert(sb.ToString().IndexOf(searchfor), " <mspace width=.2em /> <mrow> <mo>{</mo> <mtable>");
                    form.Oper1 = "+";
                }
                else if (form.Oper1 == "[")
                {
                    sb.Insert(sb.ToString().IndexOf(searchfor), " <mspace width=.2em /> <mrow> <mo>[</mo> <mtable>");
                    form.Oper1 = "+";
                }
                else
                {
                    sb.Insert(sb.ToString().IndexOf(searchfor), " <mspace width=.2em /> <mrow> <mo>[</mo> <mtable>");
                }
                for (int rows = 0; rows < form.Row; rows++)
                {
                    int row = rows + 1;
                    sb.Insert(sb.ToString().IndexOf(searchfor), " <mtr>");
                    for (int cols = 0; cols < form.Column; cols++)
                    {
                        int col = cols + 1;
                        sb.Insert(sb.ToString().IndexOf(searchfor), " <mtd>");
                        sb.Insert(sb.ToString().IndexOf(searchfor), " #col" + col + "row" + row);
                        sb.Insert(sb.ToString().IndexOf(searchfor), " </mtd>");
                    }
                    sb.Insert(sb.ToString().IndexOf(searchfor), " </mtr>");
                }
                if (form.Oper1 == "(")
                {
                    sb.Insert(sb.ToString().IndexOf(searchfor), " </mtable> <mo>)</mo> </mrow> <mspace width=.2em /> ");
                    form.Oper1 = "+";
                }
                else if (form.Oper1 == "|")
                {
                    sb.Insert(sb.ToString().IndexOf(searchfor), " </mtable> <mo>|</mo> </mrow> <mspace width=.2em /> ");
                    form.Oper1 = "+";
                }
                else if (form.Oper1 == "{")
                {
                    sb.Insert(sb.ToString().IndexOf(searchfor), " </mtable> <mo>}</mo> </mrow> <mspace width=.2em /> ");
                    form.Oper1 = "+";
                }
                else if (form.Oper1 == "[")
                {
                    sb.Insert(sb.ToString().IndexOf(searchfor), " <mspace width=.2em /> <mrow> <mo>[</mo> <mtable>");
                    form.Oper1 = "+";
                }
                else
                {
                    sb.Insert(sb.ToString().IndexOf(searchfor), " </mtable> <mo>]</mo> </mrow> <mspace width=.2em /> ");
                }
            }
            form.Oper1 = "+";
        }

        private static void InsertNumber(HttpContext context, FormulaEditViewModel form, string Num, string searchfor, StringBuilder sb, StringBuilder repeat)
        {
            if (sb.ToString().Contains(searchfor))
            {
                if (form.Num3 != "")
                {
                    repeat.Insert(repeat.ToString().IndexOf(searchfor), " <mn>Num3</mn>");
                    sb.Insert(sb.ToString().IndexOf(searchfor), " <mn>" + form.Num3 + "</mn>");
                }
                else
                {
                    if (form.BoldNum)
                    {
                        repeat.Insert(repeat.ToString().IndexOf(searchfor), " <mn mathvariant='bold'>Num</mn>");
                        sb.Insert(sb.ToString().IndexOf(searchfor), " <mn mathvariant='bold'>" + Num + "</mn>");
                    }
                    else
                    {
                        repeat.Insert(repeat.ToString().IndexOf(searchfor), " <mn>Num</mn>");
                        sb.Insert(sb.ToString().IndexOf(searchfor), " <mn>" + Num + "</mn>");
                    }
                }
            }
        }

        private static void InsertOperator(HttpContext context, FormulaEditViewModel form, string Op, string searchfor, StringBuilder sb, ref StringBuilder repeat)
        {
            int repeatstart;
            if (form.Oper3 != "")
            {

                sb.Insert(sb.ToString().IndexOf(searchfor), " <mo>" + form.Oper3 + "</mo>");
                if (form.Oper3 == "&ne;" || form.Oper3 == "&gt;" || form.Oper3 == "&ge;" || form.Oper3 == "&lt;" || form.Oper3 == "&le;")
                {
                    string plusorminusyet = "true";
                    context.Session.SetString("plusorminusyet", plusorminusyet);
                    context.Session.CommitAsync();
                    repeat = new StringBuilder("<math xmlns=" + '"' + "http://www.w3.org/1998/Math/MathML" + '"' + " display='inline'> </math>");
                }
            }
            else
            {
                if (!form.BothOper && sb.ToString().Contains(searchfor))
                {
                    sb.Insert(sb.ToString().IndexOf(searchfor), " <mo>" + Op + "</mo>");
                    if (Op == "+" || Op == "-" || Op == "=" || Op == "&ne;" || Op == "&gt;" || Op == "&ge;" || Op == "&lt;" || Op == "&le;")
                    {
                        string plusorminusyet = "true";
                        context.Session.SetString("plusorminusyet", plusorminusyet);
                        context.Session.CommitAsync();
                        repeat = new StringBuilder("<math xmlns=" + '"' + "http://www.w3.org/1998/Math/MathML" + '"' + " display='inline'> </math>");
                    }
                }
                if (form.BothOper && sb.ToString().Contains(searchfor))
                {
                    repeatstart = context.Session.GetInt32("repeatstart") ?? 0;
                    if (form.Reverse)
                    {
                        sb.Insert(sb.ToString().IndexOf(searchfor), " <mo>" + form.Oper2 + "</mo> <mo>" + form.Oper1 + "</mo>");
                    }
                    else
                    {
                        sb.Insert(sb.ToString().IndexOf(searchfor), " <mo>" + form.Oper1 + "</mo> <mo>" + form.Oper2 + "</mo>");
                    }
                    if (form.Oper1 == "+" || form.Oper2 == "+" || form.Oper1 == "-" || form.Oper2 == "-" || form.Oper1 == "=" || form.Oper2 == "=")
                    {
                        string plusorminusyet = "true";
                        context.Session.SetString("plusorminusyet", plusorminusyet);
                        context.Session.CommitAsync();
                        repeat = new StringBuilder("<math xmlns=" + '"' + "http://www.w3.org/1998/Math/MathML" + '"' + " display='inline'> </math>");
                    }
                }
            }
            if (context.Session.GetString("plusorminusyet") == "true")
            {
                form.Oper2 = "-";
            }
            else
            {
                form.Oper2 = "=";
                string oper2 = "=";
                context.Session.SetString("oper2", oper2);
                context.Session.CommitAsync();
            }
        }

        private static void InsertIdentifier(HttpContext context, FormulaEditViewModel form, string Id, string Num, string searchfor, StringBuilder sb, StringBuilder repeat)
        {
            if (sb.ToString().Contains(searchfor))
            {
                if (form.BothNum)
                {
                    repeat.Insert(repeat.ToString().IndexOf(searchfor), " <mn>Num</mn>");
                    sb.Insert(sb.ToString().IndexOf(searchfor), " <mn>" + Num + "</mn>");
                }
                if (form.Ident3 != "")
                {
                    repeat.Insert(repeat.ToString().IndexOf(searchfor), " <mi>Ident3</mi>");
                    sb.Insert(sb.ToString().IndexOf(searchfor), " <mi>" + form.Ident3 + "</mi>");
                }
                else
                {
                    if (!form.BothIdent)
                    {
                        if (form.BoldIdent)
                        {
                            repeat.Insert(repeat.ToString().IndexOf(searchfor), " <mi mathvariant='bold'>Id</mi>");
                            sb.Insert(sb.ToString().IndexOf(searchfor), " <mi mathvariant='bold'>" + Id + "</mi>");
                        }
                        else
                        {
                            repeat.Insert(repeat.ToString().IndexOf(searchfor), " <mi>Id</mi>");
                            sb.Insert(sb.ToString().IndexOf(searchfor), " <mi>" + Id + "</mi>");
                        }
                    }
                    else
                    {
                        if (form.Reverse)
                        {
                            if (form.BoldIdent)
                            {
                                repeat.Insert(repeat.ToString().IndexOf(searchfor), " <mi mathvariant='bold'>Ident2</mi> <mi mathvariant='bold'>Ident1</mi>");
                                sb.Insert(sb.ToString().IndexOf(searchfor), " <mi mathvariant='bold'>" + form.Ident2 + "</mi> <mi mathvariant='bold'>" + form.Ident1 + "</mi>");
                            }
                            else
                            {
                                repeat.Insert(repeat.ToString().IndexOf(searchfor), " <mi>Ident2</mi> <mi>Ident1</mi>");
                                sb.Insert(sb.ToString().IndexOf(searchfor), " <mi>" + form.Ident2 + "</mi> <mi>" + form.Ident1 + "</mi>");
                            }
                        }
                        else
                        {
                            if (form.BoldIdent)
                            {
                                repeat.Insert(repeat.ToString().IndexOf(searchfor), " <mi mathvariant='bold'>Ident1</mi> <mi mathvariant='bold'>Ident2</mi>");
                                sb.Insert(sb.ToString().IndexOf(searchfor), " <mi mathvariant='bold'>" + form.Ident1 + "</mi> <mi mathvariant='bold'>" + form.Ident2 + "</mi>");
                            }
                            else
                            {
                                repeat.Insert(repeat.ToString().IndexOf(searchfor), " <mi>Ident1</mi> <mi>Ident2</mi>");
                                sb.Insert(sb.ToString().IndexOf(searchfor), " <mi>" + form.Ident1 + "</mi> <mi>" + form.Ident2 + "</mi>");
                            }
                        }

                    }
                }
            }
        }

        private static void ClearEditFields(FormulaEditViewModel form)
        {
            form.Reverse = false;
            form.ClearNumerator = false;
            form.ClearNumerator = false;
            form.BothIdent = false;
            form.BoldIdent = false;
            //form.Ident1 = "";
            //form.Ident2 = "";
            form.Ident3 = "";
            //form.Oper1 = "";
            //form.Oper2 = "";
            form.Oper3 = "";
            form.BothOper = false;
            //form.Num1 = "";
            //form.Num2 = "";
            form.Num3 = "";
            form.BoldNum = false;
            //form.Space = "";
            form.BoldText = false;
            form.BothText = false;
            form.EmbedText = false;
            //form.Text = "";
            form.Matrix = false;
            //form.Row = null;
            //form.Column = null;
            form.Insert = "Identifier";
            form.Insert1 = "None";
            form.Target = "Append";
            form.Block = "inline";
            form.Algebraic = "None";
            form.Calculus = "None";
            form.Ellipses = "None";
            form.Logic = "None";
            form.Vector = "None";
            form.Set = "None";
            form.Geometric = "None";
            form.GreekUpper = "None";
            form.GreekLower = "None";
            form.ContainsSupRow1 = false;
            form.ContainsSupRow2 = false;
            form.ContainsSubRow1 = false;
            form.ContainsSubRow2 = false;
            form.ContainsMatrix = false;
            form.ContainsNumerator = false;
            form.ContainsDenominator = false;
            form.ContainsFenced = false;
            form.ContainsSquareRootRow = false;
            form.ContainsRootRow = false;
            form.ContainsOverRow = false;
            form.ContainsUnderRow = false;
            form.ContainsRow = false;
            form.Id2 = false;
            form.Op2 = false;
            form.N2 = false;
        }

        private void CheckForMissingFields(FormulaEditViewModel form)
        {
            if (form.Insert1 == "Subscript (Identifier, Identifier)")
            {
                if (string.IsNullOrEmpty(form.Ident1))
                {
                    ModelState.AddModelError("Ident1", "Please enter your Identifier");
                }
                if (string.IsNullOrEmpty(form.Ident2))
                {
                    ModelState.AddModelError("Ident2", "Please enter your Identifier in the 2nd Box");
                }
            }
            else if (form.Insert1 == "Superscript (Identifier, Identifier)")
            {
                if (string.IsNullOrEmpty(form.Ident1))
                {
                    ModelState.AddModelError("Ident1", "Please enter your Identifier");
                }
                if (string.IsNullOrEmpty(form.Ident2))
                {
                    ModelState.AddModelError("Ident2", "Please enter your Identifier in the 2nd Box");
                }
            }
            else if (form.Insert1 == "Space")
            {
                if (string.IsNullOrEmpty(form.Space))
                {
                    ModelState.AddModelError("Space", "Please enter a Number in the Space Box");
                }
            }
            else if (form.Insert1 == "Text")
            {
                if (string.IsNullOrEmpty(form.Text))
                {
                    ModelState.AddModelError("Text", "Please enter your Text");
                }
            }
            else if (form.Insert1 == "Fenced 1")
            {
                if (string.IsNullOrEmpty(form.Ident1))
                {
                    ModelState.AddModelError("Ident1", "Please enter your Identifier");
                }
            }
            else if (form.Insert1 == "Fenced 2")
            {
                if (string.IsNullOrEmpty(form.Ident1))
                {
                    ModelState.AddModelError("Ident1", "Please enter your Identifier");
                }
                if (string.IsNullOrEmpty(form.Ident2))
                {
                    ModelState.AddModelError("Ident2", "Please enter your Identifier in the 2nd Box");
                }
            }
            else if (form.Insert1 == "Fenced 3")
            {
                if (string.IsNullOrEmpty(form.Ident1))
                {
                    ModelState.AddModelError("Ident1", "Please enter your Identifier");
                }
                if (string.IsNullOrEmpty(form.Ident2))
                {
                    ModelState.AddModelError("Ident2", "Please enter your Identifier in the 2nd Box");
                }
                if (string.IsNullOrEmpty(form.Num1))
                {
                    ModelState.AddModelError("Num1", "Please enter your Identifier in the Number Box");
                }
            }
            else if (form.Insert1 == "SubSup (Identifier, Number, Number)")
            {
                if (string.IsNullOrEmpty(form.Ident1))
                {
                    ModelState.AddModelError("Ident1", "Please enter your Identifier");
                }
                if (string.IsNullOrEmpty(form.Num1))
                {
                    ModelState.AddModelError("Num1", "Please enter your Number");
                }
                if (string.IsNullOrEmpty(form.Num2))
                {
                    ModelState.AddModelError("Num2", "Please enter your Number in the 2nd Box");
                }
            }
            else if (form.Insert1 == "SubSup (Identifier, Identifier, Identifier)")
            {
                if (string.IsNullOrEmpty(form.Ident1))
                {
                    ModelState.AddModelError("Ident1", "Please enter your Identifier");
                }
                if (string.IsNullOrEmpty(form.Ident2))
                {
                    ModelState.AddModelError("Ident2", "Please enter your Identifier in the 2nd Box");
                }
                if (string.IsNullOrEmpty(form.Num1))
                {
                    ModelState.AddModelError("Num1", "Please enter your Identifier in the Number Box");
                }
            }
            else if (form.Insert1 == "SubSup (Identifier, Number, Identifier)")
            {
                if (string.IsNullOrEmpty(form.Ident1))
                {
                    ModelState.AddModelError("Ident1", "Please enter your Identifier");
                }
                if (string.IsNullOrEmpty(form.Num1))
                {
                    ModelState.AddModelError("Num1", "Please enter your Number");
                }
                if (string.IsNullOrEmpty(form.Ident2))
                {
                    ModelState.AddModelError("Ident2", "Please enter your Identifier in the 2nd Box");
                }
            }
            else if (form.Insert1 == "SubSup (Identifier, Identifier, Number)")
            {
                if (string.IsNullOrEmpty(form.Ident1))
                {
                    ModelState.AddModelError("Ident1", "Please enter your Identifier");
                }
                if (string.IsNullOrEmpty(form.Ident2))
                {
                    ModelState.AddModelError("Ident2", "Please enter your Identifier in the 2nd Box");
                }
                if (string.IsNullOrEmpty(form.Num1))
                {
                    ModelState.AddModelError("Num1", "Please enter your Number");
                }
            }
            else if (form.Insert1 == "Superscript (Number, Number)")
            {
                if (string.IsNullOrEmpty(form.Num1))
                {
                    ModelState.AddModelError("Num1", "Please enter your Number");
                }
                if (string.IsNullOrEmpty(form.Num2))
                {
                    ModelState.AddModelError("Num2", "Please enter your Number in the 2nd Box");
                }
            }
            else if (form.Insert1 != "None")
            {
                if (string.IsNullOrEmpty(form.Ident1) && !form.Id2 && form.Insert1.Contains("Identifier"))
                {
                    ModelState.AddModelError("Ident1", "Please enter your Identifier");
                }
                if (string.IsNullOrEmpty(form.Ident2) && form.Id2 && form.Insert1.Contains("Identifier"))
                {
                    ModelState.AddModelError("Ident2", "Please enter your Identifier in the 2nd Box (Remember to Check the Box again)");
                }
                if (string.IsNullOrEmpty(form.Oper1) && !form.Op2 && form.Insert1.Contains("Operator"))
                {
                    ModelState.AddModelError("Oper1", "Please enter your Operator");
                }
                if (string.IsNullOrEmpty(form.Oper2) && form.Op2 && form.Insert1.Contains("Operator"))
                {
                    ModelState.AddModelError("Oper2", "Please enter your Operator in the 2nd Box (Remember to Check the Box again)");
                }
                if (string.IsNullOrEmpty(form.Num1) && !form.N2 && form.Insert1.Contains("Number"))
                {
                    ModelState.AddModelError("Num1", "Please enter your Number");
                }
                if (string.IsNullOrEmpty(form.Num2) && form.N2 && form.Insert1.Contains("Number"))
                {
                    ModelState.AddModelError("Num", "Please enter your Number in the 2nd Box (Remember to Check the Box again)");
                }
            }
            else
            {
                if (string.IsNullOrEmpty(form.Ident1) && !form.Id2 && form.Insert == "Identifier")
                {
                    ModelState.AddModelError("Ident1", "Please enter your Identifier");
                }
                if (string.IsNullOrEmpty(form.Ident2) && form.Id2 && form.Insert == "Identifier")
                {
                    ModelState.AddModelError("Ident2", "Please enter your Identifier in the 2nd Box (Remember to Check the Box again)");
                }
                if (string.IsNullOrEmpty(form.Oper1) && !form.Op2 && form.Insert == "Operator")
                {
                    ModelState.AddModelError("Oper1", "Please enter your Operator");
                }
                if (string.IsNullOrEmpty(form.Oper2) && form.Op2 && form.Insert == "Operator")
                {
                    ModelState.AddModelError("Oper2", "Please enter your Operator in the 2nd Box (Remember to Check the Box again)");
                }
                if (string.IsNullOrEmpty(form.Num1) && !form.N2 && form.Insert == "Number")
                {
                    ModelState.AddModelError("Num1", "Please enter your Number");
                }
                if (string.IsNullOrEmpty(form.Num2) && form.N2 && form.Insert == "Number")
                {
                    ModelState.AddModelError("Num", "Please enter your Number in the 2nd Box (Remember to Check the Box again)");
                }
            }
        }
    }
}
