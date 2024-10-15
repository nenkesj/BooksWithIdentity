using Books.Infrastructure;
namespace Books.Tests
{
    public class TextTests
    {
        [Fact]
        public void AllBlanks_BlankCharactersOnly_True()
        {
            // Arrange
            Text txt = new Text();
            // Act
            bool result = txt.AllBlanks("           ");
            //Assert
            Assert.Equal(true, result);
        }

        [Fact]
        public void AllBlanks_NonBlank_False()
        {
            // Arrange
            Text txt = new Text();
            // Act
            bool result = txt.AllBlanks(" a         ");
            //Assert
            Assert.Equal(false, result);
        }

        [Fact]
        public void AllBlanks_EmptyString_False()
        {
            // Arrange
            Text txt = new Text();
            // Act
            bool result = txt.AllBlanks("");
            //Assert
            Assert.Equal(true, result);
        }

        [Fact]
        public void AllBlanks_SingleNonBlankChar_False()
        {
            // Arrange
            Text txt = new Text();
            // Act
            bool result = txt.AllBlanks("a");
            //Assert
            Assert.Equal(false, result);
        }

        [Fact]
        public void AllBlanks_SingleBlankChar_False()
        {
            // Arrange
            Text txt = new Text();
            // Act
            bool result = txt.AllBlanks(" ");
            //Assert
            Assert.Equal(true, result);
        }

        [Fact]
        public void LastNonBlankChar_SemiColon_True()
        {
            // Arrange
            Text txt = new Text();
            // Act
            char result = txt.lastNonBlankChar("   NewBook.Author = 'John Paul Mueller';\r\n");
            //Assert
            Assert.Equal(';', result);
        }

        [Fact]
        public void WheresNextEndOfLine_EmptyString_minusone()
        {
            // Arrange
            Text txt = new Text();
            // Act
            int result = txt.WheresNextEndOfLine("");
            //Assert
            Assert.Equal(-1, result);
        }

        [Fact]
        public void WheresNextEndOfLine_CRLFOnly_one()
        {
            // Arrange
            Text txt = new Text();
            // Act
            int result = txt.WheresNextEndOfLine("\r\n");
            //Assert
            Assert.Equal(1, result);
        }

        [Fact]
        public void WheresNextEndOfLine_LFAt4_four()
        {
            // Arrange
            Text txt = new Text();
            // Act
            int result = txt.WheresNextEndOfLine("012\r\n5678\r\n12345\r\n");
            //Assert
            Assert.Equal(4, result);
        }

        [Fact]
        public void WheresNextEndOfLine_NoLF_four()
        {
            // Arrange
            Text txt = new Text();
            // Act
            int result = txt.WheresNextEndOfLine("01234");
            //Assert
            Assert.Equal(4, result);
        }

        [Fact]
        public void AnalyseWord_WordEndsWithDelimiter_True()
        {
            // Arrange
            bool WordEndsWithDelimiter, WordEndsWithFullStop, WordIsAInteger, WordIsAHexNumber, WordAllCapitals;
            Text txt = new Text();
            // Act
            txt.AnalyseWord("Hello:", out WordEndsWithDelimiter, out WordEndsWithFullStop, out WordIsAInteger, out WordIsAHexNumber, out WordAllCapitals);
            //Assert
            Assert.Equal(true, WordEndsWithDelimiter);
        }

        [Fact]
        public void AnalyseWord_WordEndsWithFullStop_True()
        {
            // Arrange
            bool WordEndsWithDelimiter, WordEndsWithFullStop, WordIsAInteger, WordIsAHexNumber, WordAllCapitals;
            Text txt = new Text();
            // Act
            txt.AnalyseWord("Hello.", out WordEndsWithDelimiter, out WordEndsWithFullStop, out WordIsAInteger, out WordIsAHexNumber, out WordAllCapitals);
            //Assert
            Assert.Equal(true, WordEndsWithFullStop);
        }

        [Fact]
        public void AnalyseWord_WordIsAInteger_True()
        {
            // Arrange
            bool WordEndsWithDelimiter, WordEndsWithFullStop, WordIsAInteger, WordIsAHexNumber, WordAllCapitals;
            Text txt = new Text();
            // Act
            txt.AnalyseWord("12345", out WordEndsWithDelimiter, out WordEndsWithFullStop, out WordIsAInteger, out WordIsAHexNumber, out WordAllCapitals);
            //Assert
            Assert.Equal(true, WordIsAInteger);
        }

        [Fact]
        public void AnalyseWord_WordIsAHexNumber_True()
        {
            // Arrange
            bool WordEndsWithDelimiter, WordEndsWithFullStop, WordIsAInteger, WordIsAHexNumber, WordAllCapitals;
            Text txt = new Text();
            // Act
            txt.AnalyseWord("2E45F", out WordEndsWithDelimiter, out WordEndsWithFullStop, out WordIsAInteger, out WordIsAHexNumber, out WordAllCapitals);
            //Assert
            Assert.Equal(true, WordIsAHexNumber);
        }

        [Fact]
        public void AnalyseWord_WordAllCapitals_True()
        {
            // Arrange
            bool WordEndsWithDelimiter, WordEndsWithFullStop, WordIsAInteger, WordIsAHexNumber, WordAllCapitals;
            Text txt = new Text();
            // Act
            txt.AnalyseWord("HELLO", out WordEndsWithDelimiter, out WordEndsWithFullStop, out WordIsAInteger, out WordIsAHexNumber, out WordAllCapitals);
            //Assert
            Assert.Equal(true, WordAllCapitals);
        }

        [Fact]
        public void AnalyseWord_WordEndsWithDelimiter_False()
        {
            // Arrange
            bool WordEndsWithDelimiter, WordEndsWithFullStop, WordIsAInteger, WordIsAHexNumber, WordAllCapitals;
            Text txt = new Text();
            // Act
            txt.AnalyseWord("Hello", out WordEndsWithDelimiter, out WordEndsWithFullStop, out WordIsAInteger, out WordIsAHexNumber, out WordAllCapitals);
            //Assert
            Assert.Equal(false, WordEndsWithDelimiter);
        }

        [Fact]
        public void AnalyseWord_WordEndsWithFullStop_False()
        {
            // Arrange
            bool WordEndsWithDelimiter, WordEndsWithFullStop, WordIsAInteger, WordIsAHexNumber, WordAllCapitals;
            Text txt = new Text();
            // Act
            txt.AnalyseWord("Hello", out WordEndsWithDelimiter, out WordEndsWithFullStop, out WordIsAInteger, out WordIsAHexNumber, out WordAllCapitals);
            //Assert
            Assert.Equal(false, WordEndsWithFullStop);
        }

        [Fact]
        public void AnalyseWord_WordIsAInteger_False()
        {
            // Arrange
            bool WordEndsWithDelimiter, WordEndsWithFullStop, WordIsAInteger, WordIsAHexNumber, WordAllCapitals;
            Text txt = new Text();
            // Act
            txt.AnalyseWord("12345F", out WordEndsWithDelimiter, out WordEndsWithFullStop, out WordIsAInteger, out WordIsAHexNumber, out WordAllCapitals);
            //Assert
            Assert.Equal(false, WordIsAInteger);
        }

        [Fact]
        public void AnalyseWord_WordIsAHexNumber_False()
        {
            // Arrange
            bool WordEndsWithDelimiter, WordEndsWithFullStop, WordIsAInteger, WordIsAHexNumber, WordAllCapitals;
            Text txt = new Text();
            // Act
            txt.AnalyseWord("2E4FG", out WordEndsWithDelimiter, out WordEndsWithFullStop, out WordIsAInteger, out WordIsAHexNumber, out WordAllCapitals);
            //Assert
            Assert.Equal(false, WordIsAHexNumber);
        }

        [Fact]
        public void AnalyseWord_WordAllCapitals_False()
        {
            // Arrange
            bool WordEndsWithDelimiter, WordEndsWithFullStop, WordIsAInteger, WordIsAHexNumber, WordAllCapitals;
            Text txt = new Text();
            // Act
            txt.AnalyseWord("HELLo", out WordEndsWithDelimiter, out WordEndsWithFullStop, out WordIsAInteger, out WordIsAHexNumber, out WordAllCapitals);
            //Assert
            Assert.Equal(false, WordAllCapitals);
        }

        [Fact]
        public void AnalyseFirstWord_Everything_True()
        {
            // Arrange
            bool FirstWordAllCapitals, FirstWordHasDelimiter;
            int FirstWordPtr, SecondWordPtr, FirstWordLength, SecondWordLength;
            Text txt = new Text();
            // Act
            txt.AnalyseFirstWord("  HELLO: There", out FirstWordPtr, out SecondWordPtr, out FirstWordLength, out SecondWordLength, out FirstWordAllCapitals, out FirstWordHasDelimiter);
            //Assert
            Assert.Equal(true, FirstWordAllCapitals);
            Assert.Equal(true, FirstWordHasDelimiter);
            Assert.Equal(2, FirstWordPtr);
            Assert.Equal(9, SecondWordPtr);
            Assert.Equal(5, FirstWordLength);
            Assert.Equal(5, SecondWordLength);
        }

        [Fact]
        public void AnalyseFirstWord_WithCRLF1_True()
        {
            // Arrange
            bool FirstWordAllCapitals, FirstWordHasDelimiter;
            int FirstWordPtr, SecondWordPtr, FirstWordLength, SecondWordLength;
            Text txt = new Text();
            // Act
            txt.AnalyseFirstWord("  HELLO: There\r\n", out FirstWordPtr, out SecondWordPtr, out FirstWordLength, out SecondWordLength, out FirstWordAllCapitals, out FirstWordHasDelimiter);
            //Assert
            Assert.Equal(true, FirstWordAllCapitals);
            Assert.Equal(true, FirstWordHasDelimiter);
            Assert.Equal(2, FirstWordPtr);
            Assert.Equal(9, SecondWordPtr);
            Assert.Equal(5, FirstWordLength);
            Assert.Equal(5, SecondWordLength);
        }

        [Fact]
        public void AnalyseFirstWord_WithCRLF2_True()
        {
            // Arrange
            bool FirstWordAllCapitals, FirstWordHasDelimiter;
            int FirstWordPtr, SecondWordPtr, FirstWordLength, SecondWordLength;
            Text txt = new Text();
            // Act
            txt.AnalyseFirstWord("  HELLO: There \r\n", out FirstWordPtr, out SecondWordPtr, out FirstWordLength, out SecondWordLength, out FirstWordAllCapitals, out FirstWordHasDelimiter);
            //Assert
            Assert.Equal(true, FirstWordAllCapitals);
            Assert.Equal(true, FirstWordHasDelimiter);
            Assert.Equal(2, FirstWordPtr);
            Assert.Equal(9, SecondWordPtr);
            Assert.Equal(5, FirstWordLength);
            Assert.Equal(5, SecondWordLength);
        }


        [Fact]
        public void AnalyseFirstWord_AndSecond_True()
        {
            // Arrange
            bool FirstWordAllCapitals, FirstWordHasDelimiter;
            int FirstWordPtr, SecondWordPtr, FirstWordLength, SecondWordLength;
            Text txt = new Text();
            // Act
            txt.AnalyseFirstWord("private void btnAdd_Click(object sender, EventArgs e)", out FirstWordPtr, out SecondWordPtr, out FirstWordLength, out SecondWordLength, out FirstWordAllCapitals, out FirstWordHasDelimiter);
            //Assert
            Assert.Equal(0, FirstWordPtr);
            Assert.Equal(8, SecondWordPtr);
            Assert.Equal(7, FirstWordLength);
            Assert.Equal(4, SecondWordLength);
        }

        [Fact]
        public void AnalyseFirstWord_NoDelimETC_False()
        {
            // Arrange
            bool FirstWordAllCapitals, FirstWordHasDelimiter;
            int FirstWordPtr, SecondWordPtr, FirstWordLength, SecondWordLength;
            Text txt = new Text();
            // Act
            txt.AnalyseFirstWord("  HELLo There", out FirstWordPtr, out SecondWordPtr, out FirstWordLength, out SecondWordLength, out FirstWordAllCapitals, out FirstWordHasDelimiter);
            //Assert
            Assert.Equal(false, FirstWordAllCapitals);
            Assert.Equal(false, FirstWordHasDelimiter);
            Assert.Equal(2, FirstWordPtr);
            Assert.Equal(8, SecondWordPtr);
            Assert.Equal(5, FirstWordLength);
            Assert.Equal(5, SecondWordLength);
        }

        [Fact]
        public void AnalyseLastWord_NoDelimETC_False()
        {
            // Arrange
            bool LastWordHasDelimiter, LastWordIsAInteger, LastWordIsAHexNumber;
            int LastWordPtr, LastWordLength;
            Text txt = new Text();
            // Act
            txt.AnalyseLastWord("  hello 12A45  ", out LastWordPtr, out LastWordLength, out LastWordHasDelimiter, out LastWordIsAInteger, out LastWordIsAHexNumber);
            //Assert
            Assert.Equal(false, LastWordHasDelimiter);
            Assert.Equal(false, LastWordIsAInteger);
            Assert.Equal(true, LastWordIsAHexNumber);
            Assert.Equal(8, LastWordPtr);
            Assert.Equal(5, LastWordLength);
        }

        [Fact]
        public void AnalyseLastWord_Everything_True()
        {
            // Arrange
            bool LastWordHasDelimiter, LastWordIsAInteger, LastWordIsAHexNumber;
            int LastWordPtr, LastWordLength;
            Text txt = new Text();
            // Act
            txt.AnalyseLastWord("  hello 12345:", out LastWordPtr, out LastWordLength, out LastWordHasDelimiter, out LastWordIsAInteger, out LastWordIsAHexNumber);
            //Assert
            Assert.Equal(true, LastWordHasDelimiter);
            Assert.Equal(true, LastWordIsAInteger);
            Assert.Equal(true, LastWordIsAHexNumber);
            Assert.Equal(8, LastWordPtr);
            Assert.Equal(5, LastWordLength);
        }

        [Fact]
        public void DivideAfterChar_Everything_True()
        {
            // Arrange
            int SegmentsNoOf;
            List<String> Segments = new List<string>();
            Text txt = new Text();
            txt.TheText = "The following example does#not compile, because doInitialize does not#assign a value to param: ";
            // Act
            txt.DivideAfterChar('#', out SegmentsNoOf, ref Segments);
            //Assert
            Assert.Equal("The following example does#", Segments[0]);
            Assert.Equal("not compile, because doInitialize does not#", Segments[1]);
            Assert.Equal("assign a value to param: ", Segments[2]);
            Assert.Equal(3, SegmentsNoOf);
        }

        [Fact]
        public void DivideAfterChar_WithCRLFs_True()
        {
            // Arrange
            int SegmentsNoOf;
            List<String> Segments = new List<string>();
            Text txt = new Text();
            txt.TheText = "The following example does#not compile,\r\n because doInitialize does\r\nnot#assign a value to param: ";
            // Act
            txt.DivideAfterChar('#', out SegmentsNoOf, ref Segments);
            //Assert
            Assert.Equal("The following example does#", Segments[0]);
            Assert.Equal("not compile,\r\n because doInitialize does\r\nnot#", Segments[1]);
            Assert.Equal("assign a value to param: ", Segments[2]);
            Assert.Equal(3, SegmentsNoOf);
        }

        [Fact]
        public void DivideIntoWord_Everything_True()
        {
            // Arrange
            int WordsNoOf;
            List<String> Words = new List<string>();
            Text txt = new Text();
            txt.TheText = "The following example does not compile, because doInitialize does not assign a value to param: ";
            // Act
            txt.DivideIntoWords(txt.TheText, out WordsNoOf, ref Words);
            //Assert
            Assert.Equal("example", Words[2]);
            Assert.Equal("compile", Words[5]);
            Assert.Equal("param", Words[14]);
            Assert.Equal(15, WordsNoOf);
        }

        [Fact]
        public void DivideText_Everything_True()
        {
            // Arrange
            bool Debug, splitOnColon, InsertIndicators;
            string DebugText;
            List<string> Paragrphs = new List<string>();
            List<string> Sentences = new List<string>();
            List<int> SentenceInParagraph = new List<int>();
            List<string> Lines = new List<string>();
            Paragraphs Paragraphs = new Paragraphs();
            Debug = true;
            splitOnColon = false;
            InsertIndicators = true;
            Text txt = new Text();
            txt.TheText = "Paragraph one Sentence one. Paragraph one Sentence two Line one.\r\nParagraph two Sentence three Line two.\r\nParagraph three Sentence four Line three.\r\n";
            // Act
            txt.DivideText(out int ParagraphsNoOf, ref Paragrphs, out int SentencesNoOf, ref Sentences, ref SentenceInParagraph, out int LinesNoOf, ref Lines, out DebugText, Debug, splitOnColon, InsertIndicators);
            //Assert
            Assert.Equal("Paragraph one Sentence one. Paragraph one Sentence two Line one. ", Paragrphs[0]);
            Assert.Equal("Paragraph two Sentence three Line two. ", Paragrphs[1]);
            Assert.Equal("Paragraph three Sentence four Line three. ", Paragrphs[2]);
            Assert.Equal("Paragraph one Sentence one. Paragraph one Sentence two Line one. ", Lines[0]);
            Assert.Equal("Paragraph two Sentence three Line two. ", Lines[1]);
            Assert.Equal("Paragraph three Sentence four Line three. ", Lines[2]);
            Assert.Equal("Paragraph one Sentence one.", Sentences[0]);
            Assert.Equal("Paragraph one Sentence two Line one. ", Sentences[1]);
            Assert.Equal("Paragraph two Sentence three Line two. ", Sentences[2]);
            Assert.Equal("Paragraph three Sentence four Line three. ", Sentences[3]);
            Assert.Equal(3, ParagraphsNoOf);
            Assert.Equal(4, SentencesNoOf);
            Assert.Equal(3, LinesNoOf);
        }
        [Fact]
        public void DivideText_Tables_True()
        {
            // Arrange
            bool Debug, splitOnColon, InsertIndicators;
            string DebugText;
            List<string> Paragrphs = new List<string>();
            List<string> Sentences = new List<string>();
            List<int> SentenceInParagraph = new List<int>();
            List<string> Lines = new List<string>();
            Paragraphs Paragraphs = new Paragraphs();
            Debug = true;
            splitOnColon = false;
            InsertIndicators = true;
            Text txt = new Text();
            txt.TheText = "In Chapter 22, I describe some of the other features that views support. Table 21-1 puts Razor views in context.\r\nTable 21-1 Putting Razor Views in Context\r\nQuestion\tAnswer\r\nWhat are they?\tViews are files that contain a mix of static HTML content and C# expressions.\r\nWhy are they useful?\tViews are used to create HTML responses for HTTP requests. The C# expressions are evaluated and combined with the HTML content to create a response.\r\nHow are they used?\tThe View method defined by the Controller class creates an action response that uses a view.\r\nAre there any pitfalls or limitations?\tIt can take a little time to get used to the syntax of view files and the way they combine code and content.\r\nAre there any alternatives?\tThere are third-party view engines that can be used in ASP.NET Core MVC, but their use is limited.\r\n";
            // Act
            txt.DivideText(out int ParagraphsNoOf, ref Paragrphs, out int SentencesNoOf, ref Sentences, ref SentenceInParagraph, out int LinesNoOf, ref Lines, out DebugText, Debug, splitOnColon, InsertIndicators);
            //Assert
            Assert.Equal(8, ParagraphsNoOf);
            Assert.Equal(12, SentencesNoOf);
            Assert.Equal(8, LinesNoOf);
        }
        [Fact]
        public void DivideText_UnorderedList_True()
        {
            // Arrange
            bool Debug, splitOnColon, InsertIndicators;
            string DebugText;
            List<string> Paragrphs = new List<string>();
            List<string> Sentences = new List<string>();
            List<int> SentenceInParagraph = new List<int>();
            List<string> Lines = new List<string>();
            Paragraphs Paragraphs = new Paragraphs();
            Debug = true;
            splitOnColon = false;
            InsertIndicators = true;
            Text txt = new Text();
            txt.TheText = "She has asked for these four key features:\r\n\r\n• A home page that shows information about the party\r\n\r\n• A form that can be used to RSVP \r\n\r\n• Validation for the RSVP form, which will display a thank-you page\r\n\r\n• A summary page that shows who is coming to the party \r\n\r\nIn this chapter, I create an ASP.NET Core project and use it to create a simple application that contains these features; once everything works, I’ll apply some styling to improve the appearance of the finished application.\r\n";
            // Act
            txt.DivideText(out int ParagraphsNoOf, ref Paragrphs, out int SentencesNoOf, ref Sentences, ref SentenceInParagraph, out int LinesNoOf, ref Lines, out DebugText, Debug, splitOnColon, InsertIndicators);
            //Assert
            Assert.Equal(6, ParagraphsNoOf);
            Assert.Equal(6, SentencesNoOf);
            Assert.Equal(6, LinesNoOf);
        }
        [Fact]
        public void DivideText_OrderedList_True()
        {
            // Arrange
            bool Debug, splitOnColon, InsertIndicators;
            string DebugText;
            List<string> Paragrphs = new List<string>();
            List<string> Sentences = new List<string>();
            List<int> SentenceInParagraph = new List<int>();
            List<string> Lines = new List<string>();
            Paragraphs Paragraphs = new Paragraphs();
            Debug = true;
            splitOnColon = false;
            InsertIndicators = true;
            Text txt = new Text();
            txt.TheText = "She has asked for these four key features:\r\n\r\n1. A home page that shows information about the party\r\n\r\n2. A form that can be used to RSVP \r\n\r\n3. Validation for the RSVP form, which will display a thank-you page\r\n\r\n4. A summary page that shows who is coming to the party \r\n\r\nIn this chapter, I create an ASP.NET Core project and use it to create a simple application that contains these features; once everything works, I’ll apply some styling to improve the appearance of the finished application.\r\n";
            // Act
            txt.DivideText(out int ParagraphsNoOf, ref Paragrphs, out int SentencesNoOf, ref Sentences, ref SentenceInParagraph, out int LinesNoOf, ref Lines, out DebugText, Debug, splitOnColon, InsertIndicators);
            //Assert
            Assert.Equal(6, ParagraphsNoOf);
            Assert.Equal(6, SentencesNoOf);
            Assert.Equal(6, LinesNoOf);
        }
        [Fact]
        public void DivideText_Code_True()
        {
            // Arrange
            bool Debug, splitOnColon, InsertIndicators;
            string DebugText;
            List<string> Paragrphs = new List<string>();
            List<string> Sentences = new List<string>();
            List<int> SentenceInParagraph = new List<int>();
            List<string> Lines = new List<string>();
            Paragraphs Paragraphs = new Paragraphs();
            Debug = true;
            splitOnColon = false;
            InsertIndicators = true;
            Text txt = new Text();
            txt.TheText = "Listing 3-7 adds a new action method to the Home controller:\r\n\r\nusing Microsoft.AspNetCore.Mvc;\r\nnamespace PartyInvites.Controllers {\r\n    public class HomeController : Controller {\r\n        public IActionResult Index() {\r\n            return View();\r\n        }\r\n        public ViewResult RsvpForm() {\r\n            return View();\r\n        }\r\n    }\r\n}\r\n\r\nListing 3-7 Adding an Action Method in the HomeController.cs File in the Controllers Folder \r\n\r\nBoth action methods invoke the View method without arguments, which may seem odd, but remember that the Razor view engine will use the name of the action method when looking for a view file, as explained in Chapter 2.\r\n";
            // Act
            txt.DivideText(out int ParagraphsNoOf, ref Paragrphs, out int SentencesNoOf, ref Sentences, ref SentenceInParagraph, out int LinesNoOf, ref Lines, out DebugText, Debug, splitOnColon, InsertIndicators);
            //Assert
            Assert.Equal(4, ParagraphsNoOf);
            Assert.Equal(4, SentencesNoOf);
            Assert.Equal(15, LinesNoOf);
        }
        [Fact]
        public void DivideText_TabsInOrderedList_True()
        {
            // Arrange
            bool Debug, splitOnColon, InsertIndicators;
            string DebugText;
            List<string> Paragrphs = new List<string>();
            List<string> Sentences = new List<string>();
            List<int> SentenceInParagraph = new List<int>();
            List<string> Lines = new List<string>();
            Paragraphs Paragraphs = new Paragraphs();
            Debug = true;
            splitOnColon = false;
            InsertIndicators = true;
            Text txt = new Text();
            txt.TheText = "She has asked for these four key features:\r\n\r\n1. A home page that shows information about the party\r\n\r\n2. A form that can be used to RSVP \r\n\r\n3. Validation for the RSVP form, which will display a thank-you page\r\n\r\n4. A summary page\tthat shows who is coming to the party \r\n\r\nIn this chapter, I create an ASP.NET Core project and use it to create a simple application that contains these features; once everything works, I’ll apply some styling to improve the appearance of the finished application.\r\n";
            // Act
            txt.DivideText(out int ParagraphsNoOf, ref Paragrphs, out int SentencesNoOf, ref Sentences, ref SentenceInParagraph, out int LinesNoOf, ref Lines, out DebugText, Debug, splitOnColon, InsertIndicators);
            //Assert
            Assert.Equal("§4. A summary page that shows who is coming to the party \r\n\r\n", Paragrphs[4]);
            Assert.Equal(6, ParagraphsNoOf);
            Assert.Equal(6, SentencesNoOf);
            Assert.Equal(7, LinesNoOf);
        }
        [Fact]
        public void DivideText_OddCharacter_True()
        {
            // Arrange
            bool Debug, splitOnColon, InsertIndicators;
            string DebugText;
            List<string> Paragrphs = new List<string>();
            List<string> Sentences = new List<string>();
            List<int> SentenceInParagraph = new List<int>();
            List<string> Lines = new List<string>();
            Paragraphs Paragraphs = new Paragraphs();
            Debug = true;
            splitOnColon = false;
            InsertIndicators = true;
            Text txt = new Text();
            txt.TheText = "Fig. 9–1. A small displacement of an object.\r\n";
            // Act
            txt.DivideText(out int ParagraphsNoOf, ref Paragrphs, out int SentencesNoOf, ref Sentences, ref SentenceInParagraph, out int LinesNoOf, ref Lines, out DebugText, Debug, splitOnColon, InsertIndicators);
            //Assert
            Assert.Equal("Fig. 9-1. A small displacement of an object. ", Paragrphs[0]);
            Assert.Equal(1, ParagraphsNoOf);
            Assert.Equal(3, SentencesNoOf);
            Assert.Equal(1, LinesNoOf);
        }
    }
}