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
            if (_context != null)
            {
                _context.Session.SetString("formula", formula);
                int undoptr = 0;
                _context.Session.SetInt32("undoptr", undoptr);
                string ident = "";
                _context.Session.SetString("indent", ident);
                string oper = "";
                _context.Session.SetString("oper", oper);
                string num = "";
                _context.Session.SetString("num", num);
                string space = "";
                _context.Session.SetString("space", space);
                string text = "";
                _context.Session.SetString("text", text);
                _context.Session.CommitAsync();
            }

            ViewBag.matrix = "false";

            FormulaEditViewModel model = new FormulaEditViewModel
            {
                NodeID = id,
                ClearFormula = false,
                Reverse = false,
                ClearNumerator = false,
                ClearDenominator = false,
                BothIdent = false,
                BoldIdent = false,
                Ident1 = "",
                Ident2 = "",
                Oper1 = "",
                Oper2 = "",
                BothOper = false,
                Num1 = "",
                Num2 = "",
                BoldNum = false,
                Space = "",
                BoldText = false,
                BothText = false,
                Text = "",
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

            return View(model);
        }

        [HttpPost]
        public ViewResult Edit(FormulaEditViewModel form, string clear = "none", bool undo = false)
        {
            string Id, Op, Num;
            string searchfor = " </math>";
            bool insert = true;
            StringBuilder sb;

            if (ViewBag.matrix == "true")
            {
                form.Matrix = true;
            }
            else
            {
                form.Matrix = false;
            }

            if (form.Algebraic == "None" && form.Calculus == "None" && form.Ellipses == "None" && form.Geometric == "None" && form.GreekLower == "None" && form.GreekUpper == "None" && form.Logic == "None" && form.Set == "None" && form.Vector == "None" && !undo)
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

            if (form.Insert == "Matrix" && !undo)
            {
                if (string.IsNullOrEmpty(form.Row.ToString()))
                {
                    ModelState.AddModelError("Row", "Please enter Matrix Row");
                }

                if (string.IsNullOrEmpty(form.Column.ToString()))
                {
                    ModelState.AddModelError("Column", "Please enter Matrix Column");
                }
            }


            //if (ModelState.IsValid)
            //{
            if (_context != null)
            {
                if (undo && _context.Session.GetInt32("undoptr") > 0)
                {
                    sb = new StringBuilder(_context.Session.GetString("undo" + _context.Session.GetInt32("undoptr").ToString()));
                    _context.Session.SetInt32("undoptr", (_context.Session.GetInt32("undoptr") ?? 0) + 1);
                    _context.Session.CommitAsync();
                    ViewBag.formula = sb.ToString();
                }
                else
                {
                    sb = new StringBuilder(_context.Session.GetString("formula"));
                    ViewBag.formula = sb.ToString();
                }
            }
            else
            {
                sb = new StringBuilder(form.Formula);
            }

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
                default:
                    break;
            }

            if (form.ClearFormula)
            {
                if (_context != null)
                {
                    _context.Session.SetString("formula", "<math xmlns=" + '"' + "http://www.w3.org/1998/Math/MathML" + '"' + " display='inline'> </math>");
                    //_context.Session.SetString("undo", _context.Session.GetString("formula"));
                    //_context.Session.SetString("indent", _context.Session.GetString("ident"));
                    //_context.Session.SetString("oper", _context.Session.GetString("oper"));
                    //_context.Session.SetString("num", _context.Session.GetString("num"));
                    //_context.Session.SetString("space", _context.Session.GetString("space"));
                    //_context.Session.SetString("text", _context.Session.GetString("text"));
                    //_context.Session.CommitAsync();
                    int undoptr = 0;
                    _context.Session.SetInt32("undoptr", undoptr);
                    string ident = "";
                    _context.Session.SetString("indent", ident);
                    string oper = "";
                    _context.Session.SetString("oper", oper);
                    string num = "";
                    _context.Session.SetString("num", num);
                    string space = "";
                    _context.Session.SetString("space", space);
                    string text = "";
                    _context.Session.SetString("text", text);
                    _context.Session.CommitAsync();
                    sb = new StringBuilder(_context.Session.GetString("formula"));
                    ViewBag.formula = sb.ToString();
                }
                else
                {
                    string formula = "<math xmlns=" + '"' + "http://www.w3.org/1998/Math/MathML" + '"' + " display='inline'> </math>";
                    sb = new StringBuilder(formula);
                    ViewBag.formula = sb.ToString();
                }
                form.Reverse = false;
                form.ClearNumerator = false;
                form.ClearNumerator = false;
                form.BothIdent = false;
                form.BoldIdent = false;
                form.Ident1 = "";
                form.Ident2 = "";
                form.Oper1 = "";
                form.Oper2 = "";
                form.BothOper = false;
                form.Num1 = "";
                form.Num2 = "";
                form.BoldNum = false;
                form.Space = "";
                form.BoldText = false;
                form.BothText = false;
                form.Text = "";
                form.Matrix = false;
                form.Row = null;
                form.Column = null;
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
            else
            {
                if (form.Matrix)
                {
                    if (form.Column <= Int32.Parse(ViewBag.cols.ToString()) && form.Row <= Int32.Parse(ViewBag.rows.ToString()) && form.Column > 0 && form.Row > 0)
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
                        if (form.Column <= Int32.Parse(ViewBag.cols.ToString()) && form.Row <= Int32.Parse(ViewBag.rows.ToString()) && form.Column > 0 && form.Row > 0)
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
                    case "Fenced":
                        searchfor = " #fenced";
                        insert = true;
                        break;
                    case "Row":
                        searchfor = " #row";
                        insert = true;
                        break;
                    case "Clear Subscript Row1":
                        form.Insert = "None";
                        break;
                    case "Clear Subscript Row2":
                        form.Insert = "None";
                        break;
                    case "Clear Matrix":
                        form.Insert = "None";
                        break;
                    case "Clear Superscript Row1":
                        form.Insert = "None";
                        break;
                    case "Clear Superscript Row2":
                        form.Insert = "None";
                        break;
                    case "Clear Square Root Row":
                        form.Insert = "None";
                        break;
                    case "Clear Root Row":
                        form.Insert = "None";
                        break;
                    case "Clear Over Row":
                        form.Insert = "None";
                        break;
                    case "Clear Under Row":
                        form.Insert = "None";
                        break;
                    case "Clear Row":
                        form.Insert = "None";
                        break;
                    case "Clear Fenced":
                        form.Insert = "None";
                        break;
                    default:
                        searchfor = " </math>";
                        insert = true;
                        break;
                }

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

                if (undo)
                {
                    insert = false;
                }

                if (insert)
                {
                    if (form.Algebraic != "None")
                    {
                        form.Oper1 = "&" + form.Algebraic;
                        form.Insert = "Operator";
                        form.Algebraic = "None";
                    }
                    if (form.Calculus != "None")
                    {
                        if (form.Calculus == "&infin;")
                        {
                            form.Num1 = "&" + "infin;";
                            form.Insert = "Number";
                            form.Calculus = "None";
                        }
                        else
                        {
                            form.Oper1 = "&" + form.Calculus;
                            form.Ident1 = "&" + form.Calculus;
                            form.Insert = "Operator";
                            form.Calculus = "None";
                        }
                    }
                    if (form.Ellipses != "None")
                    {
                        form.Oper1 = "&" + form.Ellipses;
                        form.Insert = "Operator";
                        form.Ellipses = "None";
                    }
                    if (form.Logic != "None")
                    {
                        form.Oper1 = "&" + form.Logic;
                        form.Insert = "Operator";
                        form.Logic = "None";
                    }
                    if (form.Vector != "None")
                    {
                        form.Oper1 = "&" + form.Vector;
                        form.Insert = "Operator";
                        form.Vector = "None";
                    }
                    if (form.Set != "None")
                    {
                        form.Oper1 = "&" + form.Set;
                        form.Insert = "Operator";
                        form.Set = "None";
                    }
                    if (form.Geometric != "None")
                    {
                        form.Oper1 = "&" + form.Geometric;
                        form.Insert = "Operator";
                        form.Geometric = "None";
                    }
                    if (form.GreekUpper != "None")
                    {
                        form.Ident1 = "&" + form.GreekUpper;
                        form.Insert = "Identifier";
                        form.GreekUpper = "None";
                    }
                    if (form.GreekLower != "None")
                    {
                        form.Ident1 = "&" + form.GreekLower;
                        form.Insert = "Identifier";
                        form.GreekLower = "None";
                    }

                    if (form.Id2)
                    {
                        Id = form.Ident2;
                    }
                    else
                    {
                        Id = form.Ident1;
                    }
                    if (form.Op2)
                    {
                        Op = form.Oper2;
                    }
                    else
                    {
                        Op = form.Oper1;
                    }
                    if (form.N2)
                    {
                        Num = form.Num2;
                    }
                    else
                    {
                        Num = form.Num1;
                    }

                    if (form.Insert1 == "None")
                    {
                        switch (form.Insert)
                        {
                            case "Identifier":
                                if (sb.ToString().Contains(searchfor))
                                {
                                    if (form.BothNum)
                                    {
                                        sb.Insert(sb.ToString().IndexOf(searchfor), " <mn>" + Num + "</mn>");
                                    }
                                    if (!form.BothIdent)
                                    {
                                        if (form.BoldIdent)
                                        {
                                            sb.Insert(sb.ToString().IndexOf(searchfor), " <mi mathvariant='bold'>" + Id + "</mi>");
                                        }
                                        else
                                        {
                                            sb.Insert(sb.ToString().IndexOf(searchfor), " <mi>" + Id + "</mi>");
                                        }
                                    }
                                    else
                                    {
                                        if (form.BoldIdent)
                                        {
                                            sb.Insert(sb.ToString().IndexOf(searchfor), " <mi mathvariant='bold'>" + form.Ident1 + "</mi> <mi mathvariant='bold'>" + form.Ident2 + "</mi>");
                                        }
                                        else
                                        {
                                            sb.Insert(sb.ToString().IndexOf(searchfor), " <mi>" + form.Ident1 + "</mi> <mi>" + form.Ident2 + "</mi>");
                                        }
                                    }
                                }
                                form.Insert = "Operator";
                                break;
                            case "Operator":
                                if (!form.BothOper && sb.ToString().Contains(searchfor))
                                {
                                    sb.Insert(sb.ToString().IndexOf(searchfor), " <mo>" + Op + "</mo>");
                                }
                                if (form.BothOper && sb.ToString().Contains(searchfor))
                                {
                                    sb.Insert(sb.ToString().IndexOf(searchfor), " <mo>" + form.Oper1 + "</mo> <mo>" + form.Oper2 + "</mo>");
                                }
                                form.Insert = "Identifier";
                                break;
                            case "Number":
                                if (sb.ToString().Contains(searchfor))
                                {
                                    if (form.BoldNum)
                                    {
                                        sb.Insert(sb.ToString().IndexOf(searchfor), " <mn mathvariant='bold'>" + Num + "</mn>");
                                    }
                                    else
                                    {
                                        sb.Insert(sb.ToString().IndexOf(searchfor), " <mn>" + Num + "</mn>");
                                    }

                                }
                                form.Insert = "Operator";
                                break;
                            case "Matrix":
                                if (sb.ToString().Contains(searchfor))
                                {
                                    if (form.Oper1 == "(")
                                    {
                                        sb.Insert(sb.ToString().IndexOf(searchfor), " <mspace width=.2em /> <mrow> <mo>(</mo> <mtable>");

                                    }
                                    else if (form.Oper1 == "|")
                                    {
                                        sb.Insert(sb.ToString().IndexOf(searchfor), " <mspace width=.2em /> <mrow> <mo>|</mo> <mtable>");

                                    }
                                    else if (form.Oper1 == "{")
                                    {
                                        sb.Insert(sb.ToString().IndexOf(searchfor), " <mspace width=.2em /> <mrow> <mo>{</mo> <mtable>");

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

                                    }
                                    else if (form.Oper1 == "|")
                                    {
                                        sb.Insert(sb.ToString().IndexOf(searchfor), " </mtable> <mo>|</mo> </mrow> <mspace width=.2em /> ");

                                    }
                                    else if (form.Oper1 == "{")
                                    {
                                        sb.Insert(sb.ToString().IndexOf(searchfor), " </mtable> <mo>}</mo> </mrow> <mspace width=.2em /> ");

                                    }
                                    else
                                    {
                                        sb.Insert(sb.ToString().IndexOf(searchfor), " </mtable> <mo>]</mo> </mrow> <mspace width=.2em /> ");
                                    }
                                }
                                form.Target = "Matrix";
                                form.Insert = "Number";
                                form.Matrix = true;
                                TempData["matrix"] = "true";
                                TempData["rows"] = form.Row;
                                TempData["cols"] = form.Column;
                                TempData["row"] = 0;
                                TempData["col"] = 0;
                                break;
                            default:
                                break;
                        }

                    }
                    else
                    {
                        switch (form.Insert1)
                        {
                            case "Subscript (Identifier, Number)":
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
                                form.Insert = "Operator";
                                break;
                            case "Subscript (Identifier, Identifier)":
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
                                form.Insert = "Operator";
                                break;
                            case "Subscript (Identifier, Operator)":
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
                                form.Insert = "Operator";
                                break;
                            case "Subscript (Identifier, Row)":
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
                                form.Target = "Subscript Row1";
                                form.Insert = "Identifier";
                                break;
                            case "Subscript (Row, Identifier)":
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
                                form.Target = "Subscript Row1";
                                form.Insert = "Identifier";
                                break;
                            case "Subscript (Row, Number)":
                                if (form.BothNum && sb.ToString().Contains(searchfor))
                                {
                                    sb.Insert(sb.ToString().IndexOf(searchfor), " <mn>" + form.Num2 + "</mn>");
                                }
                                if (sb.ToString().Contains(searchfor))
                                {
                                    sb.Insert(sb.ToString().IndexOf(searchfor), " <msub> <mrow> #subrow1 </mrow> <mn>" + Num + "</mn> </msub>");
                                }
                                form.Target = "Subscript Row1";
                                form.Insert = "Identifier";
                                break;
                            case "Subscript (Row, Row)":
                                if (form.BothNum && sb.ToString().Contains(searchfor))
                                {
                                    sb.Insert(sb.ToString().IndexOf(searchfor), " <mn>" + form.Num1 + "</mn>");
                                }
                                if (sb.ToString().Contains(searchfor)) { sb.Insert(sb.ToString().IndexOf(searchfor), " <msub> <mrow> #subrow1 </mrow> <mrow> #subrow2 </mrow> </msub>"); }
                                form.Target = "Subscript Row1";
                                form.Insert = "Identifier";
                                break;
                            case "Superscript (Identifier, Number)":
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
                                form.Insert = "Operator";
                                break;
                            case "Superscript (Operator, Number)":
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
                                form.Insert = "Identifier";
                                break;
                            case "Superscript (Number, Number)":
                                if (form.BoldIdent && sb.ToString().Contains(searchfor))
                                {
                                    sb.Insert(sb.ToString().IndexOf(searchfor), " <msup> <mn mathvariant='bold'>" + form.Num1 + "</mni> <mn>" + form.Num2 + "</mn> </msup>");
                                }
                                else
                                {
                                    sb.Insert(sb.ToString().IndexOf(searchfor), " <msup> <mn>" + form.Num1 + "</mn> <mn>" + form.Num2 + "</mn> </msup>");
                                }
                                form.Insert = "Operator";
                                break;
                            case "Superscript (Number, Row)":
                                if (form.BoldIdent && sb.ToString().Contains(searchfor))
                                {
                                    sb.Insert(sb.ToString().IndexOf(searchfor), " <msup> <mn mathvariant='bold'>" + Num + "</mn> <mrow> #suprow1 </mrow> </msup>");
                                }
                                else
                                {
                                    sb.Insert(sb.ToString().IndexOf(searchfor), " <msup> <mn>" + Num + "</mn> <mrow> #suprow1 </mrow> </msup>");
                                }
                                form.Target = "Superscript Row1";
                                form.Insert = "Identifier";
                                break;
                            case "Superscript (Identifier, Identifier)":
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
                                form.Insert = "Operator";
                                break;
                            case "Superscript (Identifier, Row)":
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
                                form.Target = "Superscript Row1";
                                form.Insert = "Identifier";
                                break;
                            case "Superscript (Row, Identifier)":
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
                                    }
                                    else if (form.Oper1 == "[")
                                    {
                                        sb.Insert(sb.ToString().IndexOf(searchfor), " <msup> <mrow> <mo>[</mo> #suprow1 <mo>]</mo> </mrow> <mi>" + Id + "</mi>  </msup>");
                                    }
                                    else if (form.Oper1 == "|")
                                    {
                                        sb.Insert(sb.ToString().IndexOf(searchfor), " <msup> <mrow> <mo>|</mo> #suprow1 <mo>|</mo> </mrow> <mi>" + Id + "</mi>  </msup>");
                                    }
                                    else
                                    {
                                        sb.Insert(sb.ToString().IndexOf(searchfor), " <msup> <mrow> #suprow1 </mrow> <mi>" + Id + "</mi>  </msup>");
                                    }
                                }
                                form.Target = "Superscript Row1";
                                form.Insert = "Identifier";
                                break;
                            case "Superscript (Row, Number)":
                                if (form.BothNum && sb.ToString().Contains(searchfor))
                                {
                                    sb.Insert(sb.ToString().IndexOf(searchfor), " <mn>" + form.Num2 + "</mn>");
                                }
                                if (sb.ToString().Contains(searchfor))
                                {
                                    if (form.Oper1 == "(")
                                    {
                                        sb.Insert(sb.ToString().IndexOf(searchfor), " <msup> <mrow> <mo>(</mo> #suprow1 <mo>)</mo> </mrow> <mn>" + Num + "</mn>  </msup>");
                                    }
                                    else if (form.Oper1 == "[")
                                    {
                                        sb.Insert(sb.ToString().IndexOf(searchfor), " <msup> <mrow> <mo>[</mo> #suprow1 <mo>]</mo> </mrow> <mn>" + Num + "</mn>  </msup>");
                                    }
                                    else if (form.Oper1 == "|")
                                    {
                                        sb.Insert(sb.ToString().IndexOf(searchfor), " <msup> <mrow> <mo>|</mo> #suprow1 <mo>|</mo> </mrow> <mn>" + Num + "</mn>  </msup>");
                                    }
                                    else
                                    {
                                        sb.Insert(sb.ToString().IndexOf(searchfor), " <msup> <mrow> #suprow1 </mrow> <mn>" + Num + "</mn>  </msup>");
                                    }
                                }
                                form.Target = "Superscript Row1";
                                form.Insert = "Identifier";
                                break;
                            case "Superscript (Row, Row)":
                                if (form.BothNum && sb.ToString().Contains(searchfor))
                                {
                                    sb.Insert(sb.ToString().IndexOf(searchfor), " <mn>" + form.Num1 + "</mn>");
                                }
                                if (sb.ToString().Contains(searchfor))
                                {
                                    if (form.Oper1 == "(")
                                    {
                                        sb.Insert(sb.ToString().IndexOf(searchfor), " <msup> <mrow> <mo>(</mo> #suprow1 <mo>)</mo> </mrow> <mrow> #suprow2 </mrow> </msup>");
                                    }
                                    else if (form.Oper1 == "[")
                                    {
                                        sb.Insert(sb.ToString().IndexOf(searchfor), " <msup> <mrow> <mo>[</mo> #suprow1 <mo>]</mo> </mrow> <mrow> #suprow2 </mrow> </msup>");
                                    }
                                    else if (form.Oper1 == "|")
                                    {
                                        sb.Insert(sb.ToString().IndexOf(searchfor), " <msup> <mrow> <mo>|</mo> #suprow1 <mo>|</mo> </mrow> <mrow> #suprow2 </mrow> </msup>");
                                    }
                                    else
                                    {
                                        sb.Insert(sb.ToString().IndexOf(searchfor), " <msup> <mrow> #suprow1 </mrow> <mrow> #suprow2 </mrow> </msup>");
                                    }
                                }
                                form.Target = "Superscript Row1";
                                form.Insert = "Identifier";
                                break;
                            case "SubSup (Identifier, Number, Number)":
                                if (form.BoldIdent && sb.ToString().Contains(searchfor))
                                {
                                    sb.Insert(sb.ToString().IndexOf(searchfor), " <msubsup> <mi mathvariant='bold'>" + form.Ident1 + "</mi> <mn>" + form.Num1 + "</mn> <mn>" + form.Num2 + "</mn> </msubsup>");
                                }
                                else
                                {
                                    sb.Insert(sb.ToString().IndexOf(searchfor), " <msubsup> <mi>" + form.Ident1 + "</mi> <mn>" + form.Num1 + "</mn> <mn>" + form.Num2 + "</mn> </msubsup>");
                                }
                                form.Insert = "Operator";
                                break;
                            case "SubSup (Identifier, Number, Identifier)":
                                if (form.BothNum && sb.ToString().Contains(searchfor))
                                {
                                    sb.Insert(sb.ToString().IndexOf(searchfor), " <mn>" + form.Num2 + "</mn>");
                                }
                                if (form.BoldIdent && sb.ToString().Contains(searchfor))
                                {
                                    sb.Insert(sb.ToString().IndexOf(searchfor), " <msubsup> <mi mathvariant='bold'>" + form.Ident1 + "</mi> <mn>" + Num + "</mn> <mi>" + form.Ident2 + "</mi> </msubsup>");
                                }
                                else
                                {
                                    sb.Insert(sb.ToString().IndexOf(searchfor), " <msubsup> <mi>" + form.Ident1 + "</mi> <mn>" + Num + "</mn> <mi>" + form.Ident2 + "</mi> </msubsup>");
                                }
                                form.Insert = "Operator";
                                break;
                            case "SubSup (Identifier, Identifier, Number)":
                                if (form.BothNum && sb.ToString().Contains(searchfor))
                                {
                                    sb.Insert(sb.ToString().IndexOf(searchfor), " <mn>" + form.Num2 + "</mn>");
                                }
                                if (form.BoldIdent && sb.ToString().Contains(searchfor))
                                {
                                    sb.Insert(sb.ToString().IndexOf(searchfor), " <msubsup> <mi mathvariant='bold'>" + form.Ident1 + "</mi> <mi>" + form.Ident2 + "</mi> <mn>" + Num + "</mn> </msubsup>");
                                }
                                else
                                {
                                    sb.Insert(sb.ToString().IndexOf(searchfor), " <msubsup> <mi>" + form.Ident1 + "</mi> <mi>" + form.Ident2 + "</mi> <mn>" + Num + "</mn> </msubsup>");
                                }
                                form.Insert = "Operator";
                                break;
                            case "SubSup (Identifier, Identifier, Identifier)":
                                if (form.BothNum && sb.ToString().Contains(searchfor))
                                {
                                    sb.Insert(sb.ToString().IndexOf(searchfor), " <mn>" + form.Num2 + "</mn>");
                                }
                                if (form.BoldIdent && sb.ToString().Contains(searchfor))
                                {
                                    sb.Insert(sb.ToString().IndexOf(searchfor), " <msubsup> <mi mathvariant='bold'>" + form.Ident1 + "</mi> <mi>" + form.Num1 + "</mi> <mi>" + form.Ident2 + "</mi> </msubsup>");
                                }
                                else
                                {
                                    sb.Insert(sb.ToString().IndexOf(searchfor), " <msubsup> <mi>" + form.Ident1 + "</mi> <mi>" + form.Num1 + "</mi> <mi>" + form.Ident2 + "</mi> </msubsup>");
                                }
                                form.Insert = "Operator";
                                break;
                            case "SubSup (Identifier, Row, Row)":
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
                                form.Target = "Subscript Row1";
                                form.Insert = "Identifier";
                                break;
                            case "Row":
                                if (sb.ToString().Contains(searchfor)) { sb.Insert(sb.ToString().IndexOf(searchfor), " <mrow> #row </mrow>"); }
                                form.Target = "Row";
                                form.Insert = "Identifier";
                                break;
                            case "Fraction":
                                if (sb.ToString().Contains(searchfor)) { sb.Insert(sb.ToString().IndexOf(searchfor), " <mstyle mathsize='1.2em'> <mfrac> <mrow> #numerator </mrow> <mrow> #denominator </mrow> </mfrac> </mstyle>"); }
                                form.Target = "Numerator";
                                form.Insert = "Identifier";
                                break;
                            case "Square Root":
                                if (sb.ToString().Contains(searchfor)) { sb.Insert(sb.ToString().IndexOf(searchfor), " <msqrt> <mrow> #sqrtrow </mrow> </msqrt>"); }
                                form.Target = "Square Root Row";
                                form.Insert = "Identifier";
                                break;
                            case "Root":
                                if (sb.ToString().Contains(searchfor))
                                {
                                    sb.Insert(sb.ToString().IndexOf(searchfor), " <mroot> <mrow> #rootrow </mrow> <mn>" + form.Num1 + "</mn></mroot>");
                                }
                                form.Target = "Root Row";
                                form.Insert = "Identifier";
                                break;
                            case "Under Over":
                                if (sb.ToString().Contains(searchfor))
                                {
                                    sb.Insert(sb.ToString().IndexOf(searchfor), " <munderover> <mo>" + Op + "</mo> <mrow> #underrow </mrow> <mrow> #overrow </mrow> </munderover>");
                                }
                                form.Target = "Under Row";
                                form.Insert = "Identifier";
                                break;
                            case "Under":
                                if (sb.ToString().Contains(searchfor))
                                {
                                    sb.Insert(sb.ToString().IndexOf(searchfor), " <munder> <mo>" + Op + "</mo> <mrow> #underrow </mrow> </munder>");
                                }
                                form.Target = "Under Row";
                                form.Insert = "Identifier";
                                break;
                            case "Limit":
                                if (sb.ToString().Contains(searchfor))
                                {
                                    sb.Insert(sb.ToString().IndexOf(searchfor), " <munder> <mo>lim</mo> <mrow> #underrow </mrow> </munder>");
                                }
                                form.Target = "Under Row";
                                form.Insert = "Identifier";
                                break;
                            case "Over":
                                if (form.BoldIdent && sb.ToString().Contains(searchfor))
                                {
                                    sb.Insert(sb.ToString().IndexOf(searchfor), " <mover> <mi mathvariant='bold'>" + Id + "</mi> <mo>" + Op + "</mo> </mover>");
                                }
                                else
                                {
                                    sb.Insert(sb.ToString().IndexOf(searchfor), " <mover> <mi>" + Id + "</mi> <mo>" + Op + "</mo> </mover>");
                                }
                                form.Insert = "Operator";
                                break;
                            case "Over Row":
                                if (sb.ToString().Contains(searchfor))
                                {
                                    sb.Insert(sb.ToString().IndexOf(searchfor), " <mover> <mrow> #overrow </mrow> <mo>" + form.Oper1 + "</mo> </mover>");
                                }
                                form.Target = "Over Row";
                                form.Insert = "Identifier";
                                break;
                            case "Fenced 0":
                                if (form.Oper1 == "[")
                                {
                                    sb.Insert(sb.ToString().IndexOf(searchfor), " <mo>[</mo> #fenced <mo>]</mo> ");
                                }
                                else if (form.Oper1 == "{")
                                {
                                    sb.Insert(sb.ToString().IndexOf(searchfor), " <mo>{</mo> #fenced <mo>}</mo> ");
                                }
                                else if (form.Oper1 == "|")
                                {
                                    sb.Insert(sb.ToString().IndexOf(searchfor), " <mo>|</mo> #fenced <mo>|</mo> ");
                                }
                                else
                                {
                                    sb.Insert(sb.ToString().IndexOf(searchfor), " <mo>(</mo> #fenced <mo>)</mo> ");
                                }
                                form.Target = "Fenced";
                                form.Insert = "Identifier";
                                break;
                            case "Fenced 1":
                                sb.Insert(sb.ToString().IndexOf(searchfor), " <mrow> <mo>(</mo> <mi>" + Id + "</mi> <mo>)</mo> </mrow>");
                                form.Insert = "Operator";
                                break;
                            case "Fenced 2":
                                sb.Insert(sb.ToString().IndexOf(searchfor), " <mrow> <mo>(</mo> <mi>" + form.Ident1 + "</mi> <mo>,</mo> <mi>" + form.Ident2 + "</mi> <mo>)</mo> </mrow>");
                                form.Insert = "Operator";
                                break;
                            case "Fenced 3":
                                sb.Insert(sb.ToString().IndexOf(searchfor), " <mrow> <mo>(</mo> <mi>" + form.Ident1 + "</mi> <mo>,</mo> <mi>" + form.Ident2 + "</mi> <mo>,</mo> <mi>" + form.Num1 + "</mi> <mo>)</mo> </mrow>");
                                form.Insert = "Operator";
                                break;
                            case "Space":
                                if (sb.ToString().Contains(searchfor)) { sb.Insert(sb.ToString().IndexOf(searchfor), " <mspace width=" + form.Space + "em />"); }
                                form.Insert = "Operator";
                                break;
                            case "Text":
                                if (form.BothText && sb.ToString().Contains(searchfor))
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
                                form.Insert = "Identifier";
                                break;
                            case "Line Break 1":
                                if (sb.ToString().Contains("</math>")) { sb.Replace("</math>", " </math><br/>"); }
                                break;
                            case "Line Break 2":
                                if (sb.ToString().Contains("</math>")) { sb.Replace("</math>", " </math><br/><br/>"); }
                                break;
                            case "New Line =":
                                if (sb.ToString().Contains(searchfor)) { sb.Insert(sb.ToString().IndexOf(searchfor), "<mspace width=2em /> <mo>=</mo> "); }
                                break;
                            default:
                                break;
                        }
                        form.Insert1 = "None";
                    }
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

                    switch (form.Target)
                    {
                        case "Clear Numerator":
                            if (sb.ToString().Contains("#numerator")) { sb.Remove(sb.ToString().IndexOf("#numerator"), 10); };
                            insert = false;
                            form.Target = "Denominator";
                            form.Insert = "Identifier";
                            break;
                        case "Clear Denominator":
                            if (sb.ToString().Contains("#denominator")) { sb.Remove(sb.ToString().IndexOf("#denominator"), 12); };
                            insert = false;
                            if (sb.ToString().Contains("#numerator"))
                            {
                                form.Target = "Numerator";
                                form.Insert = "Identifier";
                            }
                            else if (sb.ToString().Contains("#denominator"))
                            {
                                form.Target = "Denominator";
                                form.Insert = "Identifier";
                            }
                            else if (sb.ToString().Contains("#suprow1"))
                            {
                                form.Target = "Superscript Row1";
                                form.Insert = "Identifier";
                            }
                            else if (sb.ToString().Contains("#suprow2"))
                            {
                                form.Target = "Superscript Row2";
                                form.Insert = "Identifier";
                            }
                            else if (sb.ToString().Contains("#subrow1"))
                            {
                                form.Target = "Subscript Row1";
                                form.Insert = "Identifier";
                            }
                            else if (sb.ToString().Contains("#subrow2"))
                            {
                                form.Target = "Subscript Row2";
                                form.Insert = "Identifier";
                            }
                            else if (sb.ToString().Contains("#fenced"))
                            {
                                form.Target = "Fenced";
                                form.Insert = "Operator";
                            }
                            else
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
                            break;
                        case "Clear Subscript Row1":
                            if (sb.ToString().Contains("#subrow1")) { sb.Remove(sb.ToString().IndexOf("#subrow1"), 8); };
                            insert = false;
                            if (sb.ToString().Contains("#subrow2"))
                            {
                                form.Target = "Subscript Row2";
                                form.Insert = "Identifier";
                            }
                            else if (sb.ToString().Contains("#subrow1"))
                            {
                                form.Target = "Subscript Row1";
                                form.Insert = "Identifier";
                            }
                            else if (sb.ToString().Contains("#numerator"))
                            {
                                form.Target = "Numerator";
                                form.Insert = "Identifier";
                            }
                            else if (sb.ToString().Contains("#denominator"))
                            {
                                form.Target = "Denominator";
                                form.Insert = "Identifier";
                            }
                            else if (sb.ToString().Contains("#fenced"))
                            {
                                form.Target = "Fenced";
                                form.Insert = "Operator";
                            }
                            else
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
                            break;
                        case "Clear Subscript Row2":
                            if (sb.ToString().Contains("#subrow2")) { sb.Remove(sb.ToString().IndexOf("#subrow2"), 8); };
                            insert = false;
                            if (sb.ToString().Contains("#numerator"))
                            {
                                form.Target = "Numerator";
                                form.Insert = "Identifier";
                            }
                            else if (sb.ToString().Contains("#denominator"))
                            {
                                form.Target = "Denominator";
                                form.Insert = "Identifier";
                            }
                            else if (sb.ToString().Contains("#fenced"))
                            {
                                form.Target = "Fenced";
                                form.Insert = "Operator";
                            }
                            else
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
                            insert = false;
                            form.Insert = "Operator";
                            form.Target = "Append";
                            form.Matrix = false;
                            TempData["matrix"] = "false";
                            break;
                        case "Clear Superscript Row1":
                            if (sb.ToString().Contains("#suprow")) { sb.Remove(sb.ToString().IndexOf("#suprow1"), 8); };
                            insert = false;
                            if (sb.ToString().Contains("#suprow2"))
                            {
                                form.Target = "Superscript Row2";
                                form.Insert = "Identifier";
                            }
                            else if (sb.ToString().Contains("#suprow1"))
                            {
                                form.Target = "Superscript Row1";
                                form.Insert = "Identifier";
                            }
                            else if (sb.ToString().Contains("#numerator"))
                            {
                                form.Target = "Numerator";
                                form.Insert = "Identifier";
                            }
                            else if (sb.ToString().Contains("#denominator"))
                            {
                                form.Target = "Denominator";
                                form.Insert = "Identifier";
                            }
                            else if (sb.ToString().Contains("#fenced"))
                            {
                                form.Target = "Fenced";
                                form.Insert = "Operator";
                            }
                            else
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
                            break;
                        case "Clear Superscript Row2":
                            if (sb.ToString().Contains("#suprow2")) { sb.Remove(sb.ToString().IndexOf("#suprow2"), 8); };
                            insert = false;
                            if (sb.ToString().Contains("#numerator"))
                            {
                                form.Target = "Numerator";
                                form.Insert = "Identifier";
                            }
                            else if (sb.ToString().Contains("#denominator"))
                            {
                                form.Target = "Denominator";
                                form.Insert = "Identifier";
                            }
                            else if (sb.ToString().Contains("#fenced"))
                            {
                                form.Target = "Fenced";
                                form.Insert = "Operator";
                            }
                            else
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
                            break;
                        case "Clear Square Root Row":
                            if (sb.ToString().Contains("#sqrtrow")) { sb.Remove(sb.ToString().IndexOf("#sqrtrow"), 8); };
                            insert = false;
                            form.Insert = "Operator";
                            if (sb.ToString().Contains("#numerator"))
                            {
                                form.Target = "Numerator";
                                form.Insert = "Identifier";
                            }
                            else if (sb.ToString().Contains("#denominator"))
                            {
                                form.Target = "Denominator";
                                form.Insert = "Identifier";
                            }
                            else if (sb.ToString().Contains("#suprow1"))
                            {
                                form.Target = "Superscript Row1";
                                form.Insert = "Identifier";
                            }
                            else if (sb.ToString().Contains("#suprow2"))
                            {
                                form.Target = "Superscript Row2";
                                form.Insert = "Identifier";
                            }
                            else if (sb.ToString().Contains("#subrow1"))
                            {
                                form.Target = "Subscript Row1";
                                form.Insert = "Identifier";
                            }
                            else if (sb.ToString().Contains("#subrow2"))
                            {
                                form.Target = "Subscript Row2";
                                form.Insert = "Identifier";
                            }
                            else if (sb.ToString().Contains("#fenced"))
                            {
                                form.Target = "Fenced";
                                form.Insert = "Operator";
                            }
                            else
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
                            break;
                        case "Clear Fenced":
                            if (sb.ToString().Contains("#fenced")) { sb.Remove(sb.ToString().IndexOf("#fenced"), 7); };
                            insert = false;
                            if (sb.ToString().Contains("#numerator"))
                            {
                                form.Target = "Numerator";
                                form.Insert = "Identifier";
                            }
                            else if (sb.ToString().Contains("#denominator"))
                            {
                                form.Target = "Denominator";
                                form.Insert = "Identifier";
                            }
                            else if (sb.ToString().Contains("#suprow1"))
                            {
                                form.Target = "Superscript Row1";
                                form.Insert = "Identifier";
                            }
                            else if (sb.ToString().Contains("#suprow2"))
                            {
                                form.Target = "Superscript Row2";
                                form.Insert = "Identifier";
                            }
                            else if (sb.ToString().Contains("#subrow1"))
                            {
                                form.Target = "Subscript Row1";
                                form.Insert = "Identifier";
                            }
                            else if (sb.ToString().Contains("#subrow2"))
                            {
                                form.Target = "Subscript Row2";
                                form.Insert = "Identifier";
                            }
                            else if (sb.ToString().Contains("#fenced"))
                            {
                                form.Target = "Fenced";
                                form.Insert = "Operator";
                            }
                            else
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
                            break;
                        case "Clear Root Row":
                            if (sb.ToString().Contains("#rootrow")) { sb.Remove(sb.ToString().IndexOf("#rootrow"), 8); };
                            insert = false;
                            form.Insert = "Operator";
                            break;
                        case "Clear Over Row":
                            if (sb.ToString().Contains("#overrow")) { sb.Remove(sb.ToString().IndexOf("#overrow"), 8); };
                            insert = false;
                            form.Target = "Append";
                            form.Insert = "Identifier";
                            break;
                        case "Clear Under Row":
                            if (sb.ToString().Contains("#underrow")) { sb.Remove(sb.ToString().IndexOf("#underrow"), 9); };
                            insert = false;
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
                            insert = false;
                            form.Insert = "Operator";
                            break;
                    }
                }

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

                if (form.ClearFormula) { form.ClearFormula = false; }
                if (form.BoldIdent) { form.BoldIdent = false; }
                if (form.BoldNum) { form.BoldNum = false; }
                if (form.BoldText) { form.BoldText = false; }
                if (form.BothIdent) { form.BothIdent = false; }
                if (form.BothOper) { form.BothOper = false; }
                if (form.BothNum) { form.BothNum = false; }
                if (form.BothText) { form.BothText = false; }
                if (form.Id2) { form.Id2 = false; }
                if (form.Op2) { form.Op2 = false; }
                if (form.N2) { form.N2 = false; }

                if (_context != null)
                {
                    if (!undo)
                    {
                        _context.Session.SetInt32("undoptr", (_context.Session.GetInt32("undoptr") ?? 0) + 1);
                        _context.Session.SetString("undo" + (_context.Session.GetInt32("undoptr") ?? 0).ToString(), _context.Session.GetString("formula"));
                    }
                    _context.Session.SetString("formula", sb.ToString());
                    _context.Session.CommitAsync();
                }
                ViewBag.formula = sb.ToString();
                form.Formula = sb.ToString();


            }
            return View(form);
            //}
            //else
            //{
            // there is something wrong with the data values
            //    return View(form);
            //}
        }
    }
}
