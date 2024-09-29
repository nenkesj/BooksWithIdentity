using Books.Controllers;
using Books.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Book.Tests
{
    public class FormulaControllerTest
    {
        [Fact]
        public void EditGet()
        {
            // Arrange
            IHttpContextAccessor context = new HttpContextAccessor();
            var controller = new FormulaController(context);
            string formula = "<math xmlns=" + '"' + "http://www.w3.org/1998/Math/MathML" + '"' + " display='inline'> </math>";
            // Act
            var result = controller.Edit(1) as ViewResult;
            var form = (FormulaEditViewModel)result.ViewData.Model;
            // Assert
            Assert.Equal(formula, form.Formula);
        }
        [Fact]
        public void EditPost()
        {
            // Arrange
            IHttpContextAccessor context = new HttpContextAccessor();
            var controller = new FormulaController(context);
            string formula = "<math xmlns=" + '"' + "http://www.w3.org/1998/Math/MathML" + '"' + " display='inline'> <mi>a</mi> </math>";
            // Act
            var result = controller.Edit(1) as ViewResult;
            var form = (FormulaEditViewModel)result.ViewData.Model;
            form.Ident1 = "a";
            result = controller.Edit(form) as ViewResult;
            form = (FormulaEditViewModel)result.ViewData.Model;
            // Assert
            Assert.Equal(formula, form.Formula);
        }
        [Fact]
        public void Superscript_Identifier_Number()
        {
            // Arrange
            IHttpContextAccessor context = new HttpContextAccessor();
            var controller = new FormulaController(context);
            string formula = "<math xmlns=" + '"' + "http://www.w3.org/1998/Math/MathML" + '"' + " display='inline'> <msup> <mi>a</mi> <mn>2</mn> </msup> </math>";
            // Act
            var result = controller.Edit(1) as ViewResult;
            var form = (FormulaEditViewModel)result.ViewData.Model;
            form.Ident1 = "a";
            form.Num1 = "2";
            form.Insert1 = "Superscript (Identifier, Number)";
            result = controller.Edit(form) as ViewResult;
            form = (FormulaEditViewModel)result.ViewData.Model;
            // Assert
            Assert.Equal(formula, form.Formula);
        }
        [Fact]
        public void Superscript_Identifier_Identifier()
        {
            // Arrange
            IHttpContextAccessor context = new HttpContextAccessor();
            var controller = new FormulaController(context);
            string formula = "<math xmlns=" + '"' + "http://www.w3.org/1998/Math/MathML" + '"' + " display='inline'> <msup> <mi>a</mi> <mi>b</mi> </msup> </math>";
            // Act
            var result = controller.Edit(1) as ViewResult;
            var form = (FormulaEditViewModel)result.ViewData.Model;
            form.Ident1 = "a";
            form.Ident2 = "b";
            form.Insert1 = "Superscript (Identifier, Identifier)";
            result = controller.Edit(form) as ViewResult;
            form = (FormulaEditViewModel)result.ViewData.Model;
            // Assert
            Assert.Equal(formula, form.Formula);
        }
        [Fact]
        public void Superscript_Identifier_Row()
        {
            // Arrange
            IHttpContextAccessor context = new HttpContextAccessor();
            var controller = new FormulaController(context);
            string formula = "<math xmlns=" + '"' + "http://www.w3.org/1998/Math/MathML" + '"' + " display='inline'> <msup> <mi>a</mi> <mrow> #suprow1 </mrow> </msup> </math>";
            // Act
            var result = controller.Edit(1) as ViewResult;
            var form = (FormulaEditViewModel)result.ViewData.Model;
            form.Ident1 = "a";
            form.Insert1 = "Superscript (Identifier, Row)";
            result = controller.Edit(form) as ViewResult;
            form = (FormulaEditViewModel)result.ViewData.Model;
            // Assert
            Assert.Equal(formula, form.Formula);
        }
        [Fact]
        public void Subscript_Identifier_Number()
        {
            // Arrange
            IHttpContextAccessor context = new HttpContextAccessor();
            var controller = new FormulaController(context);
            string formula = "<math xmlns=" + '"' + "http://www.w3.org/1998/Math/MathML" + '"' + " display='inline'> <msub> <mi>a</mi> <mn>2</mn> </msub> </math>";
            // Act
            var result = controller.Edit(1) as ViewResult;
            var form = (FormulaEditViewModel)result.ViewData.Model;
            form.Ident1 = "a";
            form.Num1 = "2";
            form.Insert1 = "Subscript (Identifier, Number)";
            result = controller.Edit(form) as ViewResult;
            form = (FormulaEditViewModel)result.ViewData.Model;
            // Assert
            Assert.Equal(formula, form.Formula);
        }
        [Fact]
        public void Subscript_Identifier_Identifier()
        {
            // Arrange
            IHttpContextAccessor context = new HttpContextAccessor();
            var controller = new FormulaController(context);
            string formula = "<math xmlns=" + '"' + "http://www.w3.org/1998/Math/MathML" + '"' + " display='inline'> <msub> <mi>a</mi> <mi>b</mi> </msub> </math>";
            // Act
            var result = controller.Edit(1) as ViewResult;
            var form = (FormulaEditViewModel)result.ViewData.Model;
            form.Ident1 = "a";
            form.Ident2 = "b";
            form.Insert1 = "Subscript (Identifier, Identifier)";
            result = controller.Edit(form) as ViewResult;
            form = (FormulaEditViewModel)result.ViewData.Model;
            // Assert
            Assert.Equal(formula, form.Formula);
        }
        [Fact]
        public void Subscript_Identifier_Row()
        {
            // Arrange
            IHttpContextAccessor context = new HttpContextAccessor();
            var controller = new FormulaController(context);
            string formula = "<math xmlns=" + '"' + "http://www.w3.org/1998/Math/MathML" + '"' + " display='inline'> <msub> <mi>a</mi> <mrow> #subrow1 </mrow> </msub> </math>";
            // Act
            var result = controller.Edit(1) as ViewResult;
            var form = (FormulaEditViewModel)result.ViewData.Model;
            form.Ident1 = "a";
            form.Insert1 = "Subscript (Identifier, Row)";
            result = controller.Edit(form) as ViewResult;
            form = (FormulaEditViewModel)result.ViewData.Model;
            // Assert
            Assert.Equal(formula, form.Formula);
        }
        [Fact]
        public void Fraction()
        {
            // Arrange
            IHttpContextAccessor context = new HttpContextAccessor();
            var controller = new FormulaController(context);
            string formula = "<math xmlns=" + '"' + "http://www.w3.org/1998/Math/MathML" + '"' + " display='inline'> <mstyle mathsize='1.2em'> <mfrac> <mrow> #numerator </mrow> <mrow> #denominator </mrow> </mfrac> </mstyle> </math>";
            // Act
            var result = controller.Edit(1) as ViewResult;
            var form = (FormulaEditViewModel)result.ViewData.Model;
            form.Insert1 = "Fraction";
            result = controller.Edit(form) as ViewResult;
            form = (FormulaEditViewModel)result.ViewData.Model;
            // Assert
            Assert.Equal(formula, form.Formula);
        }
        [Fact]
        public void Fenced0()
        {
            // Arrange
            IHttpContextAccessor context = new HttpContextAccessor();
            var controller = new FormulaController(context);
            string formula = "<math xmlns=" + '"' + "http://www.w3.org/1998/Math/MathML" + '"' + " display='inline'> <mo>(</mo> #fenced <mo>)</mo>  </math>";
            // Act
            var result = controller.Edit(1) as ViewResult;
            var form = (FormulaEditViewModel)result.ViewData.Model;
            form.Insert1 = "Fenced 0";
            result = controller.Edit(form) as ViewResult;
            form = (FormulaEditViewModel)result.ViewData.Model;
            // Assert
            Assert.Equal(formula, form.Formula);
        }
        [Fact]
        public void Fenced2()
        {
            // Arrange
            IHttpContextAccessor context = new HttpContextAccessor();
            var controller = new FormulaController(context);
            string formula = "<math xmlns=" + '"' + "http://www.w3.org/1998/Math/MathML" + '"' + " display='inline'> <mrow> <mo>(</mo> <mi>x</mi> <mo>,</mo> <mi>y</mi> <mo>)</mo> </mrow> </math>";
            // Act
            var result = controller.Edit(1) as ViewResult;
            var form = (FormulaEditViewModel)result.ViewData.Model;
            form.Insert1 = "Fenced 2";
            form.Ident1 = "x";
            form.Ident2 = "y";
            result = controller.Edit(form) as ViewResult;
            form = (FormulaEditViewModel)result.ViewData.Model;
            // Assert
            Assert.Equal(formula, form.Formula);
        }
        [Fact]
        public void Fenced1()
        {
            // Arrange
            IHttpContextAccessor context = new HttpContextAccessor();
            var controller = new FormulaController(context);
            string formula = "<math xmlns=" + '"' + "http://www.w3.org/1998/Math/MathML" + '"' + " display='inline'> <mrow> <mo>(</mo> <mi>x</mi> <mo>)</mo> </mrow> </math>";
            // Act
            var result = controller.Edit(1) as ViewResult;
            var form = (FormulaEditViewModel)result.ViewData.Model;
            form.Insert1 = "Fenced 1";
            form.Ident1 = "x";
            result = controller.Edit(form) as ViewResult;
            form = (FormulaEditViewModel)result.ViewData.Model;
            // Assert
            Assert.Equal(formula, form.Formula);
        }
        [Fact]
        public void Fenced3()
        {
            // Arrange
            IHttpContextAccessor context = new HttpContextAccessor();
            var controller = new FormulaController(context);
            string formula = "<math xmlns=" + '"' + "http://www.w3.org/1998/Math/MathML" + '"' + " display='inline'> <mrow> <mo>(</mo> <mi>x</mi> <mo>,</mo> <mi>y</mi> <mo>,</mo> <mi>z</mi> <mo>)</mo> </mrow> </math>";
            // Act
            var result = controller.Edit(1) as ViewResult;
            var form = (FormulaEditViewModel)result.ViewData.Model;
            form.Insert1 = "Fenced 3";
            form.Ident1 = "x";
            form.Ident2 = "y";
            form.Num1 = "z";
            result = controller.Edit(form) as ViewResult;
            form = (FormulaEditViewModel)result.ViewData.Model;
            // Assert
            Assert.Equal(formula, form.Formula);
        }
        [Fact]
        public void SquareRoot()
        {
            // Arrange
            IHttpContextAccessor context = new HttpContextAccessor();
            var controller = new FormulaController(context);
            string formula = "<math xmlns=" + '"' + "http://www.w3.org/1998/Math/MathML" + '"' + " display='inline'> <msqrt> <mrow> #sqrtrow </mrow> </msqrt> </math>";
            // Act
            var result = controller.Edit(1) as ViewResult;
            var form = (FormulaEditViewModel)result.ViewData.Model;
            form.Insert1 = "Square Root";
            result = controller.Edit(form) as ViewResult;
            form = (FormulaEditViewModel)result.ViewData.Model;
            // Assert
            Assert.Equal(formula, form.Formula);
        }
        [Fact]
        public void UnderOver()
        {
            // Arrange
            IHttpContextAccessor context = new HttpContextAccessor();
            var controller = new FormulaController(context);
            string formula = "<math xmlns=" + '"' + "http://www.w3.org/1998/Math/MathML" + '"' + " display='inline'> <munderover> <mo>&int</mo> <mrow> #underrow </mrow> <mrow> #overrow </mrow> </munderover> </math>";
            // Act
            var result = controller.Edit(1) as ViewResult;
            var form = (FormulaEditViewModel)result.ViewData.Model;
            form.Insert1 = "Under Over";
            form.Calculus = "int";
            result = controller.Edit(form) as ViewResult;
            form = (FormulaEditViewModel)result.ViewData.Model;
            // Assert
            Assert.Equal(formula, form.Formula);
        }
    }
}
