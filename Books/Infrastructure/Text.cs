using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Books.Infrastructure
{
    public class Text
    {

        int txtNoOfChars, txtAlteredNoOfChars;
        string txtLetter, txtTheText, txtAlteredText, txtSearchText;
        bool textHasPicture;

        public Text()
        {
            txtNoOfChars = 0;
            txtAlteredNoOfChars = 0;
        }

        public string TheText { get; set; }
        public string SearchText { get; set; }
        public int NoOfChars { get; set; }

        public bool AnySpaces(string txt)
        {
            // Any Spaces in the sting true || false?
            bool Any;
            int txtPtr;
            Any = false;
            txtPtr = 0;
            do
            {
                if (txt.Substring(txtPtr, 1) == " ")
                {
                    Any = true;
                }
                txtPtr += 1;
            }
            while (txtPtr <= txt.Length - 1 || Any != true);
            return Any;
        }

        public bool AllBlanks(string txt)
        {
            // Is the string all blank true || false?
            bool AllBlank;
            int txtPtr;
            char currChar;
            AllBlank = true;
            txtPtr = 0;
            if (txt.Length > 0)
            {
                do
                {
                    currChar = txt.Substring(txtPtr, 1).ToCharArray()[0];
                    if ((int)(currChar) > 32)
                    {
                        AllBlank = false;
                    }
                    txtPtr += 1;
                }
                while (txtPtr <= txt.Length - 1 && AllBlank == true);
            }
            return AllBlank;
        }

        public char lastNonBlankChar(string txt)
        {
            char lastNonBlankChar;
            int txtPtr;
            char currChar;
            lastNonBlankChar = (Char)10;
            txtPtr = 0;
            if (txt.Length > 0)
                txtPtr = txt.Length - 1;
            {
                do
                {
                    currChar = txt.Substring(txtPtr, 1).ToCharArray()[0];
                    if ((int)(currChar) > 32)
                    {
                        lastNonBlankChar = currChar;
                    }
                    txtPtr -= 1;
                }
                while (txtPtr >= 0 && lastNonBlankChar == (Char)10);
            }
            return lastNonBlankChar;
        }

        public int WheresNextEndOfLine(string txt)
        {
            // Note: CHAR(10) is the character represented by ASCII code 10, which is a Line Feed (\n) so its a new line. 
            // (Although its not the windows standard new line which is Carriage Return + Line Feed CHAR(13)+CHAR(10) )
            // So this returns an integer pointer to the Line Feed Character or -1 if it doesnt exist
            int ptrChr10, txtPtr;
            char currChar;
            ptrChr10 = -1;
            txtPtr = 0;
            if (txt.Length > 0)
            {
                do
                {
                    currChar = txt.Substring(txtPtr, 1).ToCharArray()[0];
                    if (currChar == (Char)10)
                    {
                        ptrChr10 = txtPtr;
                    }
                    txtPtr += 1;
                }
                while (txtPtr <= txt.Length - 1 && ptrChr10 == -1);
                if (txtPtr >= txt.Length)
                {
                    ptrChr10 = txt.Length - 1;
                }
            }
            return ptrChr10;
        }

        public void AnalyseWord(string txt, out bool WordEndsWithDelimiter, out bool WordEndsWithFullStop, out bool WordIsAInteger, out bool WordIsAHexNumber, out bool WordAllCapitals)
        {
            // 'Determines if (the string has the above qualities i.e. ends in delimiter or full stop etc.
            int charPtr, charsToCheck, wordLength;
            string lastChar;
            char ptrChar;
            WordEndsWithDelimiter = false;
            WordEndsWithFullStop = false;
            WordIsAInteger = false;
            WordIsAHexNumber = false;
            WordAllCapitals = false;
            wordLength = txt.Length;
            if (wordLength > 1)
            {
                lastChar = txt.Substring(wordLength - 1, 1);
                if ((lastChar == ".") || (lastChar == ":") || (lastChar == ";") || (lastChar == "-"))
                {
                    WordEndsWithDelimiter = true;
                    if (lastChar == ".")
                    {
                        WordEndsWithFullStop = true;
                    }
                }
            }
            if (wordLength > 0)
            {
                if (WordEndsWithDelimiter)
                {
                    charsToCheck = wordLength - 1;
                }
                else
                {
                    charsToCheck = wordLength;
                }
                WordIsAInteger = true;
                WordIsAHexNumber = true;
                WordAllCapitals = true;
                charPtr = 0;
                do
                {
                    ptrChar = txt.Substring(charPtr, 1).ToCharArray()[0];
                    if (!((int)ptrChar >= (int)'0' && (int)ptrChar <= (int)'9'))
                    {
                        WordIsAInteger = false;
                    }
                    if (!((int)ptrChar >= (int)'0' && (int)ptrChar <= (int)'9') &&
                        !((int)ptrChar >= (int)'A' && (int)ptrChar <= (int)'F'))
                    {
                        WordIsAHexNumber = false;
                    }
                    if (((int)ptrChar < (int)'A') || ((int)ptrChar > (int)'Z'))
                    {
                        WordAllCapitals = false;
                    }
                    charPtr += 1;
                }
                while (charPtr <= charsToCheck - 1);
            }
        }

        public void AnalyseFirstWord(string txt, out int FirstWordPtr, out int SecondWordPtr, out int FirstWordLength, out int SecondWordLength, out bool FirstWordAllCapitals, out bool FirstWordHasDelimiter)
        {
            int charPtr;
            char ptrChar;
            bool atFirstWord, pastFirstWord, atSecondWord, pastSecondWord, nothingYet;
            FirstWordLength = 0;
            FirstWordPtr = 0;
            SecondWordLength = 0;
            SecondWordPtr = 0;
            charPtr = 0;
            FirstWordHasDelimiter = false;
            FirstWordAllCapitals = true;
            if (txt.Length > 0)
            {
                ptrChar = txt.Substring(charPtr, 1).ToCharArray()[0];
                nothingYet = true;
                atFirstWord = false;
                pastFirstWord = false;
                atSecondWord = false;
                pastSecondWord = false;
                do
                {
                    // GT Chr(32) is all the printable characters weve arrived at the first word
                    if (nothingYet && (int)ptrChar > 32)
                    {
                        nothingYet = false;
                        atFirstWord = true;
                        FirstWordPtr = charPtr;
                        FirstWordAllCapitals = true;
                    }
                    if (atFirstWord)
                    {
                        if (ptrChar == ':')
                        {
                            FirstWordHasDelimiter = true;
                            atFirstWord = false;
                            pastFirstWord = true;
                        }
                        else if (ptrChar == '-')
                        {
                            // if this isnt the last character in the string and the next character is a space 
                            // treat it as a delimiter
                            if (charPtr < txt.Length - 2 && txt.Substring(charPtr + 1, 1) == " ")
                            {
                                FirstWordHasDelimiter = true;
                                atFirstWord = false;
                                pastFirstWord = true;
                            }
                        }
                        // Everything less than Chr(32) i.e. a space is a non printable character e.g. null or esc
                        // that is we've reached the end of the 1st word
                        // if atFirstWord is false these characters are before the 1st word so skip them
                        else if ((int)ptrChar <= 32)
                        {
                            atFirstWord = false;
                            pastFirstWord = true;
                        }
                        else
                        {
                            if (FirstWordAllCapitals && ((int)txt.Substring(charPtr, 1).ToCharArray()[0] < (int)'A' || (int)txt.Substring(charPtr, 1).ToCharArray()[0] > (int)'Z'))
                            {
                                FirstWordAllCapitals = false;
                            }
                            FirstWordLength += 1;
                        }
                    }
                    if (pastFirstWord && !atSecondWord && txt.Substring(charPtr, 1).ToCharArray()[0] > 32 && txt.Substring(charPtr, 1) != ":" && txt.Substring(charPtr, 1) != "-")
                    {
                        atSecondWord = true;
                        SecondWordPtr = charPtr;
                    }
                    if (atSecondWord)
                    {
                        if ((int)ptrChar <= 32)
                        {
                            pastSecondWord = true;
                            atSecondWord = false;
                        }
                        else
                        {
                            SecondWordLength += 1;
                        }
                    }
                    charPtr += 1;
                    if (charPtr < txt.Length - 1)
                    {
                        ptrChar = txt.Substring(charPtr, 1).ToCharArray()[0];
                    }
                }
                while (!pastSecondWord && charPtr <= txt.Length - 1);
            }
        }

        public void AnalyseLastWord(string txt, out int LastWordPtr, out int LastWordLength, out bool LastWordHasDelimiter, out bool LastWordIsAInteger, out bool LastWordIsAHexNumber)
        {
            int charPtr;
            char ptrChar;
            bool atLastWord, beforeLastWord, nothingYet;
            LastWordLength = 0;
            LastWordPtr = 0;
            LastWordHasDelimiter = false;
            LastWordIsAInteger = false;
            LastWordIsAHexNumber = false;
            charPtr = txt.Length - 1;
            if (txt.Length > 0)
            {
                ptrChar = txt.Substring(charPtr, 1).ToCharArray()[0];
                atLastWord = false;
                beforeLastWord = false;
                nothingYet = true;
                LastWordHasDelimiter = false;
                do
                {
                    if ((int)ptrChar <= 32)
                    {
                        if (atLastWord)
                        {
                            atLastWord = false;
                            beforeLastWord = true;
                            LastWordPtr = charPtr + 1;
                        }
                    }
                    else
                    {
                        if (nothingYet)
                        {
                            if (ptrChar == ':' || ptrChar == '-')
                            {
                                LastWordHasDelimiter = true;
                            }
                            else
                            {
                                LastWordIsAInteger = true;
                                LastWordIsAHexNumber = true;
                                atLastWord = true;
                                nothingYet = false;
                            }
                        }
                        if (atLastWord)
                        {
                            LastWordLength += 1;
                            if (!((int)ptrChar >= (int)'0' && (int)ptrChar <= (int)'9'))
                            {
                                LastWordIsAInteger = false;
                            }
                            if (!((int)ptrChar >= (int)'0' && (int)ptrChar <= (int)'9') && !((int)ptrChar >= (int)'A' && (int)ptrChar <= (int)'F'))
                            {
                                LastWordIsAHexNumber = false;
                            }
                        }
                    }
                    if (!beforeLastWord)
                    {
                        charPtr -= 1;
                        if (charPtr >= 0)
                        {
                            ptrChar = txt.Substring(charPtr, 1).ToCharArray()[0];
                        }
                    }
                }
                while (!beforeLastWord && charPtr >= 0);
            }
        }

        public void AnalyseNewLine(char lineCurrChar, char lineNextChar, char lineLastChar, ref char firstWord1stChr, ref bool nothingYet, ref bool atFirstWord, ref bool pastFirstWord, ref bool atSecondWord, ref bool pastSecondWord, ref bool firstWordAllCapitals, ref bool firstWordHasDelimiter, ref bool firstWordIsAInteger, ref bool firstWordIsAHexNumber, ref bool firstWordEndsWithFullStop, ref bool ItsAList, ref bool ItsAnUnorderedList, ref bool ItsAnOrderedList, ref bool ListItemEndsWithFullStop, ref int txtCtr, ref int firstWordPtr, ref int firstWordLength, ref int secondWordPtr, ref int secondWordLength, ref string firstWord, ref string secondWord, bool Format)
        {
            if (!pastSecondWord)
            {
                // GT Chr(32) is all the printable characters weve arrived at the first word
                if (nothingYet && (int)lineCurrChar > 32)
                {
                    nothingYet = false;
                    atFirstWord = true;
                    firstWordPtr = txtCtr;
                    firstWord1stChr = lineCurrChar;
                    firstWordHasDelimiter = false;
                    firstWordAllCapitals = true;
                    firstWordIsAInteger = true;
                    firstWordIsAHexNumber = true;
                    firstWordEndsWithFullStop = false;
                }

                if (atFirstWord)
                {
                    if ((lineCurrChar == ':') || (lineCurrChar == ';') || (lineCurrChar == '-') ||
                        (lineCurrChar == '-' && (int)lineNextChar <= 32) ||
                        (lineCurrChar == '.' && (int)lineNextChar <= 32)
                       )
                    {
                        firstWordHasDelimiter = true;
                        atFirstWord = false;
                        pastFirstWord = true;
                        if (lineCurrChar == '.')
                        {
                            firstWordEndsWithFullStop = true;
                            if (lineLastChar == '.')
                            {
                                firstWordLength += 1;
                                firstWord += lineCurrChar;
                            }
                        }
                    }
                    // Everything less than Chr(32) i.e. a space is a non printable character e.g. null or esc
                    // that is we've reached the end of the 1st word
                    // if atFirstWord is false these characters are before the 1st word so skip them
                    else if ((int)lineCurrChar <= 32)
                    {
                        atFirstWord = false;
                        pastFirstWord = true;
                    }
                    else
                    {
                        firstWordLength += 1;
                        firstWord += lineCurrChar;
                        if (firstWordAllCapitals && ((int)lineCurrChar < (int)'A' || (int)lineCurrChar > (int)'Z'))
                        {
                            firstWordAllCapitals = false;
                        }

                        if (!((int)lineCurrChar >= (int)'0' && (int)lineCurrChar <= (int)'9'))
                        {
                            firstWordIsAInteger = false;
                        }

                        if (!((int)lineCurrChar >= (int)'0' && (int)lineCurrChar <= (int)'9') &&
                            !((int)lineCurrChar >= (int)'A' && (int)lineCurrChar <= (int)'F'))
                        {
                            firstWordIsAHexNumber = false;
                        }
                    }
                }

                if (pastFirstWord && !atSecondWord && (int)lineCurrChar > 32 && lineCurrChar != ':' && lineCurrChar != ';' && lineCurrChar != '-' && lineCurrChar != '.')
                {
                    atSecondWord = true;
                    secondWordPtr = txtCtr;

                    // Is It A List (need to check even if were not formatting as this is a condition to start formatting again)

                    ItsAList = false;
                    ItsAnUnorderedList = false;
                    ItsAnOrderedList = false;
                    ListItemEndsWithFullStop = false;

                    // Dont treat "o" or "v" as the start of a list in code

                    if ((firstWord == "o" || firstWord == "-" || firstWord == "v" || firstWord == "*") && (Format))
                    {
                        ItsAList = true;
                        ItsAnUnorderedList = true;
                        ItsAnOrderedList = false;
                    }

                    if (firstWord == "•")
                    {
                        ItsAList = true;
                        ItsAnUnorderedList = true;
                        ItsAnOrderedList = false;
                    }
                    if (
                        (firstWordIsAInteger && firstWordEndsWithFullStop) ||
                        ((int)firstWord1stChr >= (int)'a' && (int)firstWord1stChr <= (int)'z' && firstWordEndsWithFullStop && firstWordLength == 1)
                       )
                    {
                        ItsAList = true;
                        ItsAnOrderedList = true;
                        ItsAnUnorderedList = false;
                    }

                    if (ItsAList && firstWordEndsWithFullStop) { ListItemEndsWithFullStop = true; }
                }

                if (atSecondWord)
                {
                    if ((int)lineCurrChar <= 32)
                    {
                        pastSecondWord = true;
                        atSecondWord = false;
                    }
                    else
                    {
                        secondWordLength += 1;
                        secondWord += lineCurrChar;
                    }
                }
            }
        }        

        public void IncludeSegments(int SegmentsNoOf, int SegmentsAlteredNoOf, string MatchText, ref List<string> Segments, string AlteredText)
        {
            string seg;
            AlteredText = "";
            SegmentsAlteredNoOf = 0;
            for (int segPtr = 0; segPtr <= SegmentsNoOf - 1; segPtr++)
            {
                seg = Segments[segPtr];
                if (seg.Length >= MatchText.Length)
                {
                    if (seg.IndexOf(MatchText) >= 0)
                    {
                        AlteredText += seg;
                        SegmentsAlteredNoOf += 1;
                    }
                }
            }
        }

        public void ExcludeSegments(int SegmentsNoOf, int SegmentsAlteredNoOf, string MatchText, ref List<string> Segments, string AlteredText)
        {
            string seg;
            AlteredText = "";
            SegmentsAlteredNoOf = 0;
            for (int segPtr = 0; segPtr <= SegmentsNoOf - 1; segPtr++)
            {
                seg = Segments[segPtr];
                if (seg.Length >= MatchText.Length)
                {
                    if (seg.IndexOf(MatchText) < 0)
                    {
                        AlteredText += seg;
                        SegmentsAlteredNoOf += 1;
                    }
                }
            }
        }

        public void DivideAfterChar(char Divider, out int SegmentsNoOf, ref List<string> Segments)
        {
            // This divides the text into Segments at he defined by a single character divider
            // Segments appear to include the divider at the end of the segment
            int noOfChars, segStart, segLength;
            string theText;
            char txtLetter;
            theText = this.TheText;
            noOfChars = this.TheText.Length;
            segStart = 0;
            SegmentsNoOf = 0;
            for (int txtPtr = 0; txtPtr <= noOfChars - 1; txtPtr++)
            {
                txtLetter = theText.Substring(txtPtr, 1).ToCharArray()[0];
                if (txtLetter == Divider || txtPtr == noOfChars - 1)
                {
                    SegmentsNoOf += 1;
                    segLength = txtPtr - segStart + 1;
                    Segments.Add(theText.Substring(segStart, segLength));
                    segStart = txtPtr + 1;
                }
            }
        }

        public void DivideByFieldWithNoSpaces(int DividerStart, int DividerLength, int LinesNoOf, int SegmentsNoOf, ref List<string> Segments)
        {
            // Seems to create segments where segments are continuously concatenated lines until a line has no spaces in the
            // divider area in which case a new segment is started!! What is this used for??
            int noOfChars;
            string theText, fldTime;
            List<string> Lines;
            Lines = new List<string>();
            noOfChars = this.NoOfChars;
            theText = this.TheText;
            //// First up divide text into lines
            LinesNoOf = 0;
            // (Char)10 is the line feed character which is the last character on every line
            DivideAfterChar((Char)10, out LinesNoOf, ref Lines);
            SegmentsNoOf = 0;
            foreach (string line in Lines)
            {
                if (line.Length >= DividerLength)
                {
                    fldTime = line.Substring(DividerStart, DividerLength);
                    if (AnySpaces(fldTime))
                    {
                        Segments[SegmentsNoOf] += line;
                    }
                    else
                    {
                        SegmentsNoOf += 1;
                        Segments[SegmentsNoOf] = line;
                    }
                }
            }
        }

        public void DivideIntoWords(string txtText, out int WordsNoOf, ref List<string> Words)
        {
            int noOfChars, wordStart, wordLength;
            char currChar, lastChar;
            noOfChars = txtText.Length;
            wordStart = 0;
            wordLength = 0;
            WordsNoOf = 0;
            lastChar = ' ';
            for (int txtPtr = 0; txtPtr <= noOfChars - 1; txtPtr++)
            {
                currChar = txtText.Substring(txtPtr, 1).ToCharArray()[0];
                // if the current char isnt alphabetic || numeric but the last char is weve reached
                // the end of the word
                if (!(((int)currChar >= (int)'a' && (int)currChar <= (int)'z') ||
                        ((int)currChar >= (int)'A' && (int)currChar <= (int)'Z') ||
                        ((int)currChar >= (int)'0' && (int)currChar <= (int)'9') ||
                        currChar == '\\' ||
                        currChar == '-' ||
                        currChar == '/' ||
                        currChar == '’') &&
                       (((int)lastChar >= (int)'a' && (int)lastChar <= (int)'z') ||
                        ((int)lastChar >= (int)'A' && (int)lastChar <= (int)'Z') ||
                        ((int)lastChar >= (int)'0' && (int)lastChar <= (int)'9')) ||
                        txtPtr == noOfChars - 1)
                {
                    if (wordStart != -1)
                    {
                        WordsNoOf += 1;
                        wordLength = txtPtr - wordStart;
                        Words.Add(txtText.Substring(wordStart, wordLength).ToLower());
                        wordStart = -1;
                    }
                }
                if ((((int)currChar >= (int)'a' && (int)currChar <= (int)'z') ||
                        ((int)currChar >= (int)'A' && (int)currChar <= (int)'Z') ||
                        ((int)currChar >= (int)'0' && (int)currChar <= (int)'9')) &&
                    (!(((int)lastChar >= (int)'a' && (int)lastChar <= (int)'z') ||
                        ((int)lastChar >= (int)'A' && (int)lastChar <= (int)'Z') ||
                        ((int)lastChar >= (int)'0' && (int)lastChar <= (int)'9') ||
                        (lastChar == '\\' ||
                        lastChar == '-' ||
                        lastChar == '/' ||
                        lastChar == '’'))))
                {
                    wordStart = txtPtr;
                }
                lastChar = currChar;
            }
        }

        public void StartNewParagraph(ref int ParagraphsNoOf, ref List<string> Paragraphs, string sentence, ref string paragraph, char currChar, ref int ParagraphLength, bool Debug, string DebugText, bool Format, bool ItsAnOrderedList, bool ItsAnUnorderedList, bool ItsGotHTML, bool SplittingOnColon, bool InsertIndicators, bool allCapitals)
        {
            if (Debug)
            {
                DebugText += (Char)13 + (Char)10 + (Char)13 + (Char)10 + " - Its a new paragraph - Old Paragraph Length = " + ParagraphLength.ToString() + (Char)13 + (Char)10 + (Char)13 + (Char)10;
                DebugText += currChar;
            }
            // Emit existing list heading and this will be formated using HTML (e.g. include the "¥" and skip the "1." etc.)
            if (paragraph.Length > 0)
            {
                if (paragraph.Substring(0, 1) == "¥")
                {
                    // Fell Over when remaining text was "¤W" so add a space
                    paragraph = paragraph + " ";
                    paragraph = paragraph.Substring(0, 1) + paragraph.Substring(paragraph.IndexOf(" "));
                }
            }
            Paragraphs.Add(paragraph);
            ParagraphsNoOf += 1;

            //
            // What to do with the First Char of a new paragraph
            //
            // Any HTML is difficult as alot of times it is sample HTML in the manual and isnt intended to be formatted
            // This "code" example HTML is indicated by the preceding paragraph ending in a Semi Colon (i.e. Splitting on a Colon)
            // when we are formatting the text (the colon has to be inserted manually).
            //
            // In general text that we dont want formatted e.g. programming language example code is preceded with a "§" indicator
            //							
            //Format    Indicators	AllCapitals SpltOnColon OL	UL	HTML    Other   Action                  Comment
            //	
            // Y    	Y	        Y            N       	Y	N	N   	N   	Append "¤" & currChar   (Format Ordered List)
            //			                                    N   Y	N   	N   	Append "¥" & currChar   (Format Unordered List)
            //			                                    N   N	Y   	N   	Append "Ÿ" & currChar   (Format AllCap Heading)
            //			                                    N   N	N   	Y   	Append "Ÿ" & currChar   (Format AllCap Heading)
            //		                             Y          Y   N	N   	N   	Append "¤" & currChar   (Format Ordered List)
            //			                                    N   Y	N   	N   	Append "¥" & currChar   (Format Unordered List)
            //			                                    N   N	Y   	N   	Append "§" & currChar   (Treat HTML as code)
            //			                                    N   N	N   	Y   	Append "§" & currChar   (Treat as code)
            //      	 	        N            N       	Y	N	N   	N   	Append "¤" & currChar   (Format Ordered List)
            //			                                    N   Y	N   	N   	Append "¥" & currChar   (Format Unordered List)
            //			                                    N   N	Y   	N   	Append currChar         (Process any HTML)
            //			                                    N   N	N   	Y   	Append currChar         (Normal append char)
            //		                             Y          Y   N	N   	N   	Append "¤" & currChar   (Format Ordered List)
            //			                                    N   Y	N   	N   	Append "¥" & currChar   (Format Unordered List)
            //			                                    N   N	Y   	N   	Append "§" & currChar   (Treat HTML as code)
            //			                                    N   N	N   	Y   	Append "§" & currChar   (Treat as code)
            //          N   	    Y            N	        Y   N	N   	N   	Append currChar         (Normal append char)
            //			                                    N   Y	N   	N   	Append currChar         (Normal append char)
            //			                                    N   N	Y   	N   	Append currChar         (Process any HTML)
            //			                                    N   N	N   	Y   	Append currChar         (Normal append char)
            //		                             Y	        Y   N	N   	N   	Append currChar         (Dont treat as code)
            //			                                    N   Y	N   	N   	Append currChar         (Dont treat as code)
            //			                                    N   N	Y   	N   	Append currChar         (Process any HTML)
            //			                                    N   N	N   	Y   	Append currChar         (Dont treat as code)
            //              	    N            N	        Y   N	N   	N   	Append currChar         (Normal append char)
            //			                                    N   Y	N   	N   	Append currChar         (Normal append char)
            //			                                    N   N	Y   	N   	Append currChar         (Process any HTML)
            //			                                    N   N	N   	Y   	Append currChar         (Normal append char)
            //		                             Y	        Y   N	N   	N   	Append currChar         (Dont treat as code)
            //			                                    N   Y	N   	N   	Append currChar         (Dont treat as code)
            //			                                    N   N	Y   	N   	Append currChar         (Process any HTML)
            //			                                    N   N	N   	Y   	Append currChar         (Dont treat as code)
            //N	        Y	        Y           N           Y	N	N   	N   	Append "§" & currChar   (Treat paragraph as code)
            //			                                    N   Y	N   	N   	Append "§" & currChar   (Treat paragraph as code)
            //			                                    N   N	Y   	N   	Append currChar         (Process HTML unfmt'd)
            //			                                    N   N	N   	Y   	Append "§" & currChar   (Treat paragraph as code)
            //		                            Y	        Y   N	N   	N   	Append "§" & currChar   (Treat paragraph as code)
            //			                                    N   Y	N   	N   	Append "§" & currChar   (Treat paragraph as code)
            //			                                    N   N	Y   	N   	Append "§" & currChar   (Treat paragraph as code)
            //			                                    N   N	N   	Y   	Append "§" & currChar   (Treat paragraph as code)
            // 	        	        N           N           Y	N	N   	N   	Append "§" & currChar   (Treat paragraph as code)
            //			                                    N   Y	N   	N   	Append "§" & currChar   (Treat paragraph as code)
            //			                                    N   N	Y   	N   	Append currChar         (Process HTML unfmt'd)
            //			                                    N   N	N   	Y   	Append "§" & currChar   (Treat paragraph as code)
            //		                            Y	        Y   N	N   	N   	Append "§" & currChar   (Treat paragraph as code)
            //			                                    N   Y	N   	N   	Append "§" & currChar   (Treat paragraph as code)
            //			                                    N   N	Y   	N   	Append "§" & currChar   (Treat paragraph as code)
            //			                                    N   N	N   	Y   	Append "§" & currChar   (Treat paragraph as code)
            //	        N   	    Y           N	        Y   N	N   	N   	Append currChar         (Normal append char)
            //			                                    N   Y	N   	N   	Append currChar         (Normal append char)
            //			                                    N   N	Y   	N   	Append currChar         (Process HTML unfmt'd)
            //			                                    N   N	N   	Y   	Append currChar         (Normal append char)
            //		                            Y	        Y   N	N   	N   	Append currChar         (Dont treat as code)
            //			                                    N   Y	N   	N   	Append currChar         (Dont treat as code)
            //			                                    N   N	Y   	N   	Append currChar         (Dont treat as code)
            //			                                    N   N	N   	Y   	Append currChar         (Dont treat as code)
            //	            	    N            N	        Y   N	N   	N   	Append currChar         (Normal append char)
            //			                                    N   Y	N   	N   	Append currChar         (Normal append char)
            //			                                    N   N	Y   	N   	Append currChar         (Process HTML unfmt'd)
            //			                                    N   N	N   	Y   	Append currChar         (Normal append char)
            //		                            Y	        Y   N	N   	N   	Append currChar         (Dont treat as code)
            //			                                    N   Y	N   	N   	Append currChar         (Dont treat as code)
            //			                                    N   N	Y   	N   	Append currChar         (Dont treat as code)
            //			                                    N   N	N   	Y   	Append currChar         (Dont treat as code)
            //
            if (Format && InsertIndicators && ItsAnOrderedList)
            {
                paragraph = "¤" + currChar.ToString();
                ParagraphLength = 2;
            }
            if (Format && InsertIndicators && ItsAnUnorderedList)
            {
                paragraph = "¥" + currChar.ToString();
                ParagraphLength = 2;
            }
            if (Format && InsertIndicators && allCapitals)
            {
                paragraph = "Ÿ" + currChar.ToString();
                ParagraphLength = 2;
            }
            if ((!Format && InsertIndicators && !SplittingOnColon && !ItsGotHTML) ||
                (!Format && InsertIndicators && SplittingOnColon) ||
                (Format && InsertIndicators && !allCapitals && SplittingOnColon && !ItsAnOrderedList && !ItsAnUnorderedList))
                {
                paragraph = "§" + currChar.ToString();
                ParagraphLength = 2;
            }
            if ((Format && InsertIndicators && !allCapitals && !SplittingOnColon && !ItsAnOrderedList && !ItsAnUnorderedList) ||
                (!Format && InsertIndicators && !SplittingOnColon && ItsGotHTML) ||
                (!InsertIndicators))
            {
                paragraph = currChar.ToString();
                ParagraphLength = 1;
            }
        }

        public void StartNewSentence(int ParagraphsNoOf, ref int SentencesNoOf, ref List<string> Sentences, ref List<int> SentenceInParagraph, ref string sentence, ref int SentenceLength, char currChar, bool Debug, string DebugText, bool Format)
        {
            if (Debug)
            {
                DebugText += " - Its a new sentence - Old Sentence Length = " + SentenceLength.ToString() + " ";
            }
            SentencesNoOf += 1;
            Sentences.Add(sentence);
            SentenceInParagraph.Add(ParagraphsNoOf);
            sentence = currChar.ToString();
            SentenceLength = 1;
        }


        public void StartNewLine(ref int LinesNoOf, ref List<string> Lines, ref string line, char currChar, ref int LineLength, bool Debug, string DebugText)
        {
            if (Debug)
            {
                DebugText += " - Its a new line - Old Line Length = " + LineLength.ToString() + " ";
            }
            LinesNoOf += 1;
            Lines.Add(line);
            line = currChar.ToString();
            LineLength = 1;
        }

        public bool Formatting(ref bool format, string firstWord, char firstWordChar, string secondWord, char lastCharInLine, bool ItsAList, ref bool SplittingOnAColon, string newLine, bool allCapitals)
        {

        // Generally make sure the last char on formatted line end in a semicolon to indicate it is preceded by code which won't 
        // be formatted i.e. wont have the lines run togeather and we start formatting again when the newline starts with a capital
        // letter. The following are exceptions to this general rule

            if (format)
            {
                // The following are examples of lines that indicate we have lines of code which means we dont want to 
                // run the lines togeather
                // 
                // "> db.users.find({'username' : 'joe'})",
                // "{",
                // "$ mongod",
                // "...",
                // "//",
                // "namespace SportsStore.WebUI.Models",
                // "@using",
                // "@model SportsStore.WebUI.Models.ProductsListViewModel",
                // "@{",
                if (firstWord == ">" || firstWord == "{" || firstWord == "$" || firstWord == "..." || firstWord == "//" || firstWord == "namespace" || firstWord == "@using" || firstWord == "@model" || firstWord == "@{" || firstWord == "git" || firstWord == "USE" || firstWord == "SELECT" || firstWord == "FROM" || firstWord == "ORDER" || firstWord == "GROUP" || firstWord == "INTO" || firstWord == "WHERE" || firstWord == "INSERT" || firstWord == "VALUES" || firstWord == "DELETE" || firstWord == "CREATE" || firstWord == "DROP" || firstWord == "INNER" || firstWord == "OUTER" || firstWord == "LEFT" || firstWord == "JOIN" || firstWord == "UPDATE" || firstWord == "SET" || firstWord == "WHEN" || firstWord == "MERGE" || firstWord == "OFFSET" || firstWord == "GO" || firstWord == "CONSTRAINT" || firstWord == "AS" || firstWord == "RETURNS" || firstWord == "BEGIN")
                {
                    format = false;
                }
                // < for HTML
                if (firstWordChar == '<' && firstWord != "<hr>")
                {
                    format = false;
                }
                // We are going to presume the following first words on a line are code if the last character matches (this is mainly JavaScript and C#)
                if ((firstWord == "function" && lastCharInLine == '{') ||
                    (firstWord == "var" && lastCharInLine == '{') ||
                    (firstWord == "for" && lastCharInLine == '{') ||
                    (firstWord == "if" && lastCharInLine == '{') ||
                    (firstWord == "for" && lastCharInLine == ')') ||
                    (firstWord == "if" && lastCharInLine == ')') ||
                    (firstWord == "void" && lastCharInLine == ')') ||
                    (firstWord == "public" && lastCharInLine == ')') ||
                    (firstWordChar == '.' && lastCharInLine == '{') ||
                    (firstWordChar == '#' && lastCharInLine == '{'))
                {
                    format = false;
                }
                // Also well presume the following 1st and 2nd word of a line are the start of code
                if (firstWord == "public" || firstWord == "Public" || firstWord == "Private" || firstWord == "END" || firstWord == "private" || firstWord == "protected" || firstWord == "using" || firstWord == "CREATE")
                {
                    if (secondWord == "class" || secondWord == "Class" || secondWord == "SUB" || secondWord == "Sub" || secondWord == "void" || secondWord == "int" || secondWord == "bool" || secondWord == "string" || secondWord == "override" || secondWord == "interface" || secondWord == "TABLE")
                    {
                        format = false;
                    }
                    if (secondWord.Length > 5)
                    {
                        if (secondWord.Substring(0, 6) == "System")
                        {
                            format = false;
                        }
                    }
                    if (lastCharInLine == ';')
                    {
                        format = false;
                    }
                }
                if (newLine.IndexOf((Char)9) > 0)
                {
                    format = false;
                }
                if (SplittingOnAColon && !ItsAList)
                {
                    format = false;
                    // Some text precede figures with a line that ends in a semicolon dont treat the semicolon as a code indicator
                    if (firstWord.Length > 2)
                    {
                        if (firstWord.Substring(0, 3) == "Fig")
                        {
                            SplittingOnAColon = false;
                            format = true;
                        }
                    }

                }
            }
            else
            {
                //
                // These are the main triggers to start formatting again basically if the first word of a newline starts with a
                // capital letter surprisingly this works most of the time.
                // 
                // Added newLine.Substring(0, 1).ToCharArray()[0] == firstWordChar refinement this only starts formatting again
                // if the capital is the 1st character of the line. This gets around formatting code starting with a capital
                // because it is usually indented
                //
                // if the line ends in ; { or > we assume it is still code or HTML so we make an exception
                //
                if ((int)firstWordChar >= (int)'A' && (int)firstWordChar <= (int)'Z')
                {
                    if (lastCharInLine != ';' && lastCharInLine != '{' && lastCharInLine != '>' && newLine.Substring(0, 1).ToCharArray()[0] == firstWordChar && firstWord != "USE" && firstWord != "SELECT" && firstWord != "FROM" && firstWord != "ORDER" && firstWord != "GROUP" && firstWord != "INTO" && firstWord != "WHERE" && firstWord != "INSERT" && firstWord != "VALUES" && firstWord != "DELETE" && firstWord != "CREATE" && firstWord != "DROP" && firstWord != "INNER" && firstWord != "OUTER" && firstWord != "LEFT" && firstWord != "JOIN" && firstWord != "UPDATE" && firstWord != "SET" && firstWord != "WHEN" && firstWord != "MERGE" && firstWord != "OFFSET" && firstWord != "GO" && firstWord != "CONSTRAINT" && firstWord != "AS" && firstWord != "RETURNS" && firstWord != "BEGIN" && firstWord != "C#" && firstWord != "VB" && firstWord != "Public" && firstWord != "Private" && firstWord != "END" && firstWord != "End" && firstWord != "Sub" && firstWord != "Dim")
                    {
                        format = true;
                    }
                }
                // Usually the beginning of a list indicates we should start formatting again 
                if (ItsAList)
                {
                    format = true;
                }
                // Format Figure headings (you would think it would automatically do this not sure why this is here)
                if (firstWord.Length > 2)
                {
                    if (firstWord.Substring(0,3) == "Fig")
                    {
                        format = true;
                    }
                }
            }

            return format;
        }

        public bool AddSpace(char lastChar, char currChar, char nextChar, bool Debug, ref string DebugText)
        {
            bool Space = false;
            // Insert a space if appropriate
            //
            // if hit end of line and line doesnt end in a space add one to prevent words running togeather
            if (currChar != ':' && nextChar == (Char)13)
            {
                if (currChar != ' ')
                {
                    Space = true;
                    if (Debug)
                    {
                        DebugText += " - Add a space, newline not preceded by space ";
                    }
                }
            }
            // ensure commas and semicolons are followed by a space if there not there already (not colons)
            if (currChar == ',' || currChar == ';')
            {
                // not if followed by a full stop or closing bracket e.g.  "For example, font-family: geneva;." 
                // not if ; followed by , e.g. "those with display: inline-block;, you may also " 
                // not if followed by a digit e.g. " 1,999,999"
                // not if &lt; or &gt; e.g. <code>&lt;h2&gt</code> that is text contains HTML that we dont want to be treated as HTML
                if (nextChar != ' ' && nextChar != '.' && nextChar != '>' && nextChar != ')' && nextChar != '}' && nextChar != ']' && nextChar != '"' && nextChar != ';' && nextChar != ',' &&
                  !((int)nextChar >= (int)'0' && (int)nextChar <= (int)'9') && lastChar != 't')
                {
                    Space = true;
                    if (Debug)
                    {
                        DebugText += " - Add a space ,; not followed by a space ";
                    }

                }
            }
            // ensure closing bracket is followed by space unless followed by a comma, semicolon, colon or fullstop, exclammation mark or question mark or arithmetic operator or another bracket (not > because of HTML)
            if (currChar == '}' || currChar == ']')
            {
                if (nextChar != ' ' && nextChar != ',' && nextChar != ';' && nextChar != ':' && nextChar != '.' && nextChar != '!' && nextChar != '?' && nextChar != '*' && nextChar != '/' && nextChar != '+' && nextChar != '-' && nextChar != '>' && nextChar != '<' && nextChar != ')' && nextChar != '}' && nextChar != ']' && nextChar != '%')
                {
                    Space = true;
                    if (Debug)
                    {
                        DebugText += " - Add a space )}] not followed by a space ,;: ";
                    }

                }
            }
            return Space;
        }

        public void OldDivideText(out int ParagraphsNoOf, ref List<string> Paragraphs, out int SentencesNoOf, ref List<string> Sentences, ref List<int> SentenceInParagraph, out int LinesNoOf, ref List<string> Lines, out string DebugText, int LineWidth, bool Debug, bool EliminateWhiteSpace, bool tabs, bool SplitHeaders, bool SplitOnColon, bool SplitOnLF, bool InsertIndicators)
        {
            int txtPtr, noOfChars, oldLineLength, LineLength, SentenceLength, ParagraphLength, firstWordPtr, secondWordPtr, firstWordLength, secondWordLength, lineEnd;
            char currChar, lastChar, nextChar, firstWord1stChr, lastCharInLine;
            string line, newLine, sentence, paragraph, firstWord, secondWord, txtText;
            bool endOfSentence, endOfParagraph, endOfLine, ItsAList, ItsAnUnorderedList, ItsAnOrderedList, ItsGotATab, ItsGotHTML, ListItemEndsWithFullStop, SplittingOnColon, FirstLine, firstWordAllCapitals, delimited, WordEndsWithDelimiter, WordEndsWithFullStop, WordIsAInteger, WordIsAHexNumber, WordAllCapitals, Format, FormatBefore, AddASpace, allCapitals;

            // Initial values

            FirstLine = true;
            endOfLine = false;
            endOfSentence = false;
            endOfParagraph = false;
            SplittingOnColon = false;
            ItsAList = false;
            ItsAnOrderedList = false;
            ItsAnUnorderedList = false;
            ItsGotATab = false;
            ItsGotHTML = false;
            ListItemEndsWithFullStop = false;
            Format = true;
            FormatBefore = true;
            lastChar = (Char)10;
            paragraph = "";
            sentence = "";
            line = "";
            oldLineLength = 0;
            LineLength = 0;
            ParagraphsNoOf = 0;
            SentencesNoOf = 0;
            LinesNoOf = 0;
            firstWordPtr = 0;
            secondWordPtr = 0;
            firstWordLength = 0;
            secondWordLength = 0;
            firstWordAllCapitals = false;
            delimited = false;
            WordEndsWithDelimiter = false;
            WordEndsWithFullStop = false;
            WordIsAInteger = false;
            WordIsAHexNumber = false;
            WordAllCapitals = false;
            SentenceLength = 0;
            ParagraphLength = 0;
            lastCharInLine = (Char)10;
            DebugText = "";
            allCapitals = false;

            // Get the text that were formatting

            txtText = this.TheText;
            noOfChars = txtText.Length;

            // First up ")." is the preferred end of a sentence not ".)" etc not ?} causes problems in MVC Routes

            txtText = txtText.Replace(".)", ").");
            txtText = txtText.Replace("?)", ")?");
            txtText = txtText.Replace("!)", ")!");
            txtText = txtText.Replace(".}", "}.");
            txtText = txtText.Replace("!}", "}!");
            txtText = txtText.Replace(".]", "].");
            txtText = txtText.Replace("?]", "]?");
            txtText = txtText.Replace("!]", "]!");
            txtText = txtText.Replace(".”", "”.");
            txtText = txtText.Replace(",”", "”,");
            txtText = txtText.Replace("?”", "”?");
            txtText = txtText.Replace("!”", "”!");

            // Each Character one at a time

            for (txtPtr = 0; txtPtr <= noOfChars - 1; txtPtr++)
            {
                currChar = txtText.Substring(txtPtr, 1).ToCharArray()[0];

                // Some of the decisions below need to know what the next char is

                if (txtPtr < noOfChars - 1)
                {
                    nextChar = txtText.Substring(txtPtr + 1, 1).ToCharArray()[0];
                }
                else
                {
                    nextChar = ' ';
                }

                // Skip the character in the following situations
                //   Skip Line Feeds and Carriage returns if were formatting 
                //   Skip whitespace

                if (!(
                      (currChar == (Char)10 && Format) ||
                      (currChar == (Char)13 && Format) ||
                      (currChar == ' ' && lastChar == ' ' && Format)
                     ))
                {

                    // if (lastChar is LF were now at a new line

                    if (lastChar == (Char)10)
                    {
                        lineEnd = WheresNextEndOfLine(txtText.Substring(txtPtr));
                        newLine = txtText.Substring(txtPtr, lineEnd + 1);
                        lastCharInLine = (Char)10;
                        lastCharInLine = lastNonBlankChar(newLine);
                        if (newLine.IndexOf((Char)9) > 0) { ItsGotATab = true; } else { ItsGotATab = false; }
                        if (newLine.Count<Char>(x => x == '<') > 1 && newLine.Contains(">") && newLine.Contains("</"))
                        { ItsGotHTML = true; }
                        else { ItsGotHTML = false; }
                        AnalyseFirstWord(newLine, out firstWordPtr, out secondWordPtr, out firstWordLength, out secondWordLength, out firstWordAllCapitals, out delimited);
                        // these are the positions in the whole text not just the next line
                        firstWordPtr += txtPtr;
                        firstWord = " ";
                        firstWord1stChr = ' ';
                        if (firstWordLength > 0)
                        {
                            firstWord = txtText.Substring(firstWordPtr, firstWordLength);
                            firstWord1stChr = txtText.Substring(firstWordPtr, 1).ToCharArray()[0];
                        }
                        AnalyseWord(firstWord, out WordEndsWithDelimiter, out WordEndsWithFullStop, out WordIsAInteger, out WordIsAHexNumber, out WordAllCapitals);
                        secondWordPtr += txtPtr;
                        secondWord = "";
                        if (secondWordLength > 0) { secondWord = txtText.Substring(secondWordPtr, secondWordLength); }

                        // Is It A List (need to check even if were not formatting as this is a condition to start formatting again)

                        ItsAList = false;
                        ItsAnUnorderedList = false;
                        ItsAnOrderedList = false;
                        ListItemEndsWithFullStop = false;

                        if ((firstWord == "•" || firstWord == "o" || firstWord == "-" || firstWord == "v" || firstWord == "*"))
                        {
                            ItsAList = true;
                            ItsAnUnorderedList = true;
                            ItsAnOrderedList = false;
                        }
                        if (WordIsAInteger && WordEndsWithFullStop)
                        {
                            ItsAList = true;
                            ItsAnOrderedList = true;
                            ItsAnUnorderedList = false;
                        }
                        if ((int)firstWord1stChr >= (int)'a' && (int)firstWord1stChr <= (int)'z' && delimited && Format && firstWordLength == 1) // <=== Shouldnt this be 1 need to test!!
                        {
                            ItsAList = true;
                            ItsAnOrderedList = true;
                            ItsAnUnorderedList = false;
                        }

                        if (ItsAList && WordEndsWithFullStop) { ListItemEndsWithFullStop = true; }

                        // Decide whether were formatting this line or treating it as preformatted code

                        FormatBefore = Format;
                        Format = Formatting(ref Format, firstWord, firstWord1stChr, secondWord, lastCharInLine, ItsAList, ref SplittingOnColon, newLine, allCapitals);

                        // Is this new line a new paragraph (and hence a new sentence)
                        //
                        // This is the logic:-
                        // First line isnt a new paragraph (the logic assumes their was a preceding paragraph)
                        // If weve just gone from normal paragraphs to code thats pre-formatted or visa versa or
                        // If were formatting (normal paragraphs) 
                        // wait till the character pointer is at the 1st non blank character of the new line 
                        // (i.e. skip over whitespace after the end of the last paragraph) and
                        // 1st letter of the new paragraph is a capital A through to Z or " or [ ([ for Euclid) or
                        // its a list item or
                        // A single word line or
                        // If were not formatting and the line contains a tab (each row of table needs to be separate)

                        if (!FirstLine &&
                            (
                              (FormatBefore != Format) ||
                              (
                               (
                                Format && (int)currChar > 32 &&
                                (
                                 ((int)firstWord1stChr >= (int)'A' && (int)firstWord1stChr <= (int)'Z') ||
                                 firstWord1stChr == '"' ||
                                 firstWord1stChr == '[' ||
                                 ItsAList ||
                                 (secondWordLength < 1 && !WordEndsWithFullStop)
                                )
                               ) ||
                               (
                                !Format && ItsGotATab
                               )
                              )
                             )
                            )
                        {
                            endOfParagraph = true;
                            endOfSentence = true;
                        }

                        // Next LF wont be on the First Line and becuase lastChar was LF this is the end of a line

                        if (FirstLine)
                        {
                            // Insert List indicator prior to adding 1st char to the paragraph 
                            // (otherwise it ignores list items starting on 1st line)
                            if (ParagraphLength == 0)
                            {
                                if (ItsAnOrderedList && InsertIndicators)
                                {
                                    paragraph = "¤";
                                    ParagraphLength = 1;
                                }
                                if (ItsAnUnorderedList && InsertIndicators)
                                {
                                    paragraph = "¥";
                                    ParagraphLength = 1;
                                }
                            }
                            FirstLine = false;
                        }
                        else
                        {
                            endOfLine = true;
                            oldLineLength = LineLength;
                            // Turning off the effect of splitting on a colon here ensure at least on line of unformatted code
                            // SplittingOnColon = false;
                        }
                    }

                    // End of Last Char was a LF so we do the following for every character

                    // Turn off Formatting and start a new paragraph after a line that terminates with a ":"  
                    // This allows us to manually turn off formatting for the text that follows

                    if (!FirstLine && Format && SplitOnColon &&
                        (currChar == ':' && nextChar == (Char)13) ||
                        (lastChar == ':' && currChar == ' ' && nextChar == (Char)13)
                       )
                    {
                        SplittingOnColon = true;
                    }

                    // In list change tabs to spaces

                    if (currChar == (Char)9 && ItsAList)
                    {
                        currChar = ' ';
                    }

                    // There seem to numerous examples where a space needs to be added so created a function that decides
                    // if this is necessary, dont do it if were splitting on a colon causes no end of difficulties

                    AddASpace = false;
                    if (Format)
                    {
                        if ((currChar != ':' && nextChar == (Char)13) ||
                            (currChar == ',' || currChar == ';' || currChar == ')' || currChar == '}' || currChar == ']'))
                        {
                            AddASpace = AddSpace(lastChar, currChar, nextChar, Debug, ref DebugText);
                        }
                    }

                    // Start Newline if (we've reached the end of a line)

                    if (endOfLine)
                    {
                        endOfLine = false;
                        StartNewLine(ref LinesNoOf, ref Lines, ref line, currChar, ref LineLength, Debug, DebugText);
                    }
                    else
                    {
                        line += currChar;
                        LineLength += 1;
                        if (AddASpace)
                        {
                            line += " ";
                            LineLength += 1;
                        }
                    }

                    // Start new Sentence if we've hit a fullstop and we've now hit non-blank text following the full stop

                    if (endOfSentence && (int)currChar > 32)
                    {
                        endOfSentence = false;
                        StartNewSentence(ParagraphsNoOf, ref SentencesNoOf, ref Sentences, ref SentenceInParagraph, ref sentence, ref SentenceLength, currChar, Debug, DebugText, Format);
                    }
                    else
                    {
                        if (!endOfSentence)
                        {
                            sentence += currChar;
                            SentenceLength += 1;
                            if (AddASpace)
                            {
                                sentence += " ";
                                SentenceLength += 1;
                            }
                        }
                    }

                    // Start New Pragraph if we've reached the 1st printable character after the end of the last paragraph

                    if (endOfParagraph && (int)currChar > 32)
                    {
                        endOfParagraph = false;
                        StartNewParagraph(ref ParagraphsNoOf, ref Paragraphs, sentence, ref paragraph, currChar, ref ParagraphLength, Debug, DebugText, Format, ItsAnOrderedList, ItsAnUnorderedList, ItsGotHTML, SplittingOnColon, InsertIndicators, allCapitals);
                    }
                    else
                    {
                        if (!endOfParagraph)
                        {
                            paragraph += currChar;
                            ParagraphLength += 1;
                            if (AddASpace)
                            {
                                paragraph += " ";
                                ParagraphLength += 1;
                            }
                        }
                    }

                    // Have we reached the end of a sentence

                    // Basically if we've hit a full stop the next char is a space (or non-printable) and the previous char 
                    // was alphanumeric or a closing bracket weve reached the end of a sentence

                    // The one case where this isnt the end of a sentence is the full-stop follows a list item in a  
                    // list (i.e. the 1st word in a line is delimited by a fullstop)
                    // so dont indicate its an end of sentence in this case??

                    if (Format && currChar == '.' && (int)nextChar <= 32 &&
                       ((int)lastChar >= (int)'a' && (int)lastChar <= (int)'z' ||
                       (int)lastChar >= (int)'A' && (int)lastChar <= (int)'Z' ||
                       (int)lastChar >= (int)'0' && (int)lastChar <= (int)'9' ||
                       lastChar == '>' ||
                       lastChar == ')' ||
                       lastChar == '}' ||
                       lastChar == ']'))
                    {
                        if (ListItemEndsWithFullStop)
                        {
                            ListItemEndsWithFullStop = false;
                        }
                        else
                        {
                            endOfSentence = true;
                        }
                    }
                    // So that lastChar is correct, for eliminating white space etc.
                    if (AddASpace)
                    {
                        currChar = ' ';
                    }
                }
                // End of Dont skip this character
                lastChar = currChar;
            }
            // End of Character Loop i.e. there are no more characters in the text

            // Add remaining characters (if text didnt end neatly on the end of a paragraph, sentence or line)

            if (line.Length > 0)
            {
                LinesNoOf += 1;
                Lines.Add(line);
            }
            if (sentence.Length > 0)
            {
                SentencesNoOf += 1;
                Sentences.Add(sentence);
                SentenceInParagraph.Add(ParagraphsNoOf);
            }
            if (paragraph.Length > 0)
            {
                if (paragraph.Substring(0, 1) == "¤" || paragraph.Substring(0, 1) == "¥")
                {
                    paragraph = paragraph.Substring(0, 1) + paragraph.Substring(paragraph.IndexOf(" "));
                }
                ParagraphsNoOf += 1;
                Paragraphs.Add(paragraph);
            }
        }

        public void DivideText(out int ParagraphsNoOf, ref List<string> Paragraphs, out int SentencesNoOf, ref List<string> Sentences, ref List<int> SentenceInParagraph, out int LinesNoOf, ref List<string> Lines, out string DebugText, int LineWidth, bool Debug, bool EliminateWhiteSpace, bool tabs, bool SplitHeaders, bool SplitOnColon, bool SplitOnLF, bool InsertIndicators)
        {
            int txtPtr, txtCtr, noOfLTs, noOfChars, oldLineLength, LineLength, SentenceLength, ParagraphLength, firstWordPtr, secondWordPtr, firstWordLength, secondWordLength, lineEnd, charNo;
            char currChar, lastChar, nextChar, lineCurrChar, lineLastChar, lineNextChar, firstWord1stChr, lastCharInLine;
            string line, newLine, sentence, paragraph, firstWord, secondWord, txtText;
            bool endOfSentence, endOfParagraph, endOfLine, ItsAList, ItsAnUnorderedList, ItsAnOrderedList, ItsGotATab, ItsGotHTML, ItsGotAGT, ItsGotALTSlash, ItsGotACapital, ListItemEndsWithFullStop, SplittingOnColon, FirstLine, firstWordHasDelimiter, firstWordAllCapitals, WordEndsWithFullStop, WordIsAInteger, WordIsAHexNumber, Format, FormatBefore, AddASpace, nothingYet, atFirstWord, pastFirstWord, atSecondWord, pastSecondWord, allCapitals;
            char[] txtArr;

            // Initial values

            FirstLine = true;
            endOfLine = false;
            endOfSentence = false;
            endOfParagraph = false;
            SplittingOnColon = false;
            ItsAList = false;
            ItsAnOrderedList = false;
            ItsAnUnorderedList = false;
            ItsGotATab = false;
            ItsGotHTML = false;
            ItsGotAGT = false;
            ItsGotALTSlash = false;
            ListItemEndsWithFullStop = false;
            Format = true;
            FormatBefore = true;
            lastChar = (Char)10;
            paragraph = "";
            sentence = "";
            line = "";
            newLine = "";
            oldLineLength = 0;
            LineLength = 0;
            ParagraphsNoOf = 0;
            SentencesNoOf = 0;
            LinesNoOf = 0;
            noOfLTs = 0;
            firstWordLength = 0;
            firstWordPtr = 0;
            firstWord = "";
            firstWord1stChr = ' ';
            secondWord = "";
            secondWordLength = 0;
            secondWordPtr = 0;
            firstWordHasDelimiter = false;
            firstWordAllCapitals = true;
            nothingYet = true;
            atFirstWord = false;
            pastFirstWord = false;
            atSecondWord = false;
            pastSecondWord = false;
            WordEndsWithFullStop = false;
            WordIsAInteger = false;
            WordIsAHexNumber = false;
            SentenceLength = 0;
            ParagraphLength = 0;
            lastCharInLine = (Char)10;
            DebugText = "";
            AddASpace = false;
            allCapitals = false;

            // Get the text that were formatting

            txtText = this.TheText;

            // First up ")." is the preferred end of a sentence not ".)" etc not ?} causes problems in MVC Routes

            txtText = txtText.Replace(".)", ").");
            txtText = txtText.Replace("?)", ")?");
            txtText = txtText.Replace("!)", ")!");
            txtText = txtText.Replace(".}", "}.");
            txtText = txtText.Replace("!}", "}!");
            txtText = txtText.Replace(".]", "].");
            txtText = txtText.Replace("?]", "]?");
            txtText = txtText.Replace("!]", "]!");
            txtText = txtText.Replace(".”", "”.");
            txtText = txtText.Replace(",”", "”,");
            txtText = txtText.Replace("?”", "”?");
            txtText = txtText.Replace("!”", "”!");
            // mfenced in MathsML has be deprecated (this saves having to change individual nodes)
            if (txtText.Contains("<mfenced open='[' close=']' separators=''>"))
            {
                txtText = txtText.Replace("<mfenced open='[' close=']' separators=''>", "<mrow> <mo>[</mo>");
                txtText = txtText.Replace("</mfenced>", "<mo>]</mo> </mrow>");
            }
            if (txtText.Contains("<mfenced>"))
            {
                txtText = txtText.Replace("<mfenced>", "<mrow> <mo>(</mo>");
                txtText = txtText.Replace("</mfenced>", "<mo>)</mo> </mrow>");
            }

            noOfChars = txtText.Length;

            // Each Character one at a time

            txtArr = txtText.ToCharArray();

            for (txtPtr = 0; txtPtr <= noOfChars - 1; txtPtr++)
            {
                currChar = txtArr[txtPtr];
                charNo = (int)currChar;

                // Some of the decisions below need to know what the next char is

                if (txtPtr < noOfChars - 1)
                {
                    nextChar = txtArr[txtPtr + 1];
                }
                else
                {
                    nextChar = ' ';
                }

                // Change square list char to circular list char causes problems otherwise

                if (currChar == (Char)61607)
                {
                    currChar = '•';
                }

                // Skip the character in the following situations
                //   Skip Line Feeds and Carriage returns if were formatting 
                //   Skip whitespace

                if (!(
                      (currChar == (Char)10 && Format) ||
                      (currChar == (Char)13 && Format) ||
                      (currChar == ' ' && lastChar == ' ' && Format) 
                     ))
                {

                    // if (lastChar is LF were now at a new line

                    if (lastChar == (Char)10)
                    {
                        ItsGotATab = false;
                        ItsGotAGT = false;
                        ItsGotALTSlash = false;
                        ItsGotACapital = false;
                        noOfLTs = 0;
                        lastCharInLine = (Char)10;
                        firstWordLength = 0;
                        firstWordPtr = 0;
                        secondWordLength = 0;
                        secondWordPtr = 0;
                        nothingYet = true;
                        atFirstWord = false;
                        pastFirstWord = false;
                        atSecondWord = false;
                        pastSecondWord = false;
                        firstWord = "";
                        firstWord1stChr = ' ';
                        secondWord = "";
                        lineEnd = -1;
                        txtCtr = txtPtr;
                        lineCurrChar = ' ';
                        lineLastChar = ' ';
                        lineNextChar = ' ';
                        allCapitals = true;
                        ItsAList = false;
                        ItsAnOrderedList = false;
                        ItsAnUnorderedList = false;

                        // Scan the new line and decide if its to be formatted or not

                        do
                        {
                            lineCurrChar = txtArr[txtCtr];
                            if (txtCtr < noOfChars - 1)
                            {
                                lineNextChar = txtArr[txtCtr + 1];
                            }
                            else
                            {
                                lineNextChar = ' ';
                            }

                            if (lineCurrChar == (Char)9) { ItsGotATab = true; }
                            if (lineCurrChar == '<') { noOfLTs++; }
                            if (lineCurrChar == '>') { ItsGotAGT = true; }
                            if (lineLastChar == '<' && lineCurrChar == '/') { ItsGotALTSlash = true; }
                            if (!ItsGotACapital)
                            {
                                if ((int)lineCurrChar >= (int)'A' && (int)lineCurrChar <= (int)'Z')
                                {
                                    ItsGotACapital = true;
                                }
                            }
                            if (lineCurrChar == (Char)10) { lineEnd = txtCtr; }
                            if ((int)lineCurrChar > 32) { lastCharInLine = lineCurrChar; }
                            if (lineCurrChar == (Char)61607) { lineCurrChar = '•'; }
                            if (!pastSecondWord)
                            {
                                AnalyseNewLine(lineCurrChar, lineNextChar, lineLastChar, ref firstWord1stChr, ref nothingYet, ref atFirstWord, ref pastFirstWord, ref atSecondWord, ref pastSecondWord, ref firstWordAllCapitals, ref firstWordHasDelimiter, ref WordIsAInteger, ref WordIsAHexNumber, ref WordEndsWithFullStop, ref ItsAList, ref ItsAnUnorderedList, ref ItsAnOrderedList, ref ListItemEndsWithFullStop, ref txtCtr, ref firstWordPtr, ref firstWordLength, ref secondWordPtr, ref secondWordLength, ref firstWord, ref secondWord, Format);
                            }
                            if (allCapitals) 
                            {
                                // figure headings are never all Capitals (the actual picture won't go in the right place)
                                if (firstWord.Length > 2)
                                {
                                    if (firstWord.Substring(0,3).ToLower() == "fig")
                                    {
                                        allCapitals = false; 
                                    }
                                }
                                // if it contains a lower case letter its not all Capitals
                                if ((int)lineCurrChar >= (int)'a' && (int)lineCurrChar <= (int)'z')
                                {
                                    allCapitals = false; 
                                }
                                if (ItsAList)
                                {
                                    allCapitals = false;
                                } 
                            }

                            lineLastChar = lineCurrChar;
                            txtCtr += 1;
                        }
                        while (txtCtr < noOfChars && lineEnd == -1);

                        newLine = txtText.Substring(txtPtr, txtCtr - txtPtr);

                        if (firstWord == "<hr>") { ItsAList = false; ItsAnOrderedList = false; ItsAnUnorderedList = false; }

                        if (noOfLTs > 1 && ItsGotAGT && ItsGotALTSlash) { ItsGotHTML = true; } else { ItsGotHTML = false; }

                        if (allCapitals && !ItsGotACapital ) 
                        { 
                            allCapitals = false; 
                        }

                        // Decide whether were formatting this line or treating it as preformatted code

                        FormatBefore = Format;
                        Format = Formatting(ref Format, firstWord, firstWord1stChr, secondWord, lastCharInLine, ItsAList, ref SplittingOnColon, newLine, allCapitals);

                        // Next LF wont be on the First Line and becuase lastChar was LF this is the end of a line

                        if (FirstLine && (ParagraphLength == 0))
                        {
                            // Insert List indicator prior to adding 1st char to the paragraph 
                            // (otherwise it ignores list items starting on 1st line)
                                if (ItsAnOrderedList && InsertIndicators)
                                {
                                    paragraph = "¤";
                                    ParagraphLength = 1;
                                }
                                if (ItsAnUnorderedList && InsertIndicators)
                                {
                                    paragraph = "¥";
                                    ParagraphLength = 1;
                                }
                                if (allCapitals && InsertIndicators)
                                {
                                    paragraph = "Ÿ";
                                    ParagraphLength = 1;
                                }
                        }

                        // if this is the first line and last char is LF and paragraph isnt empty were not at the
                        // beginning of the very first line (very 1st lastChar is set to LF) were at the end of the first line

                        if (FirstLine && (ParagraphLength > 1))
                        {
                            FirstLine = false;
                        }

                        // if lastChar is LF (except on very 1st char) were at the beginning of a new line

                        if (!FirstLine)
                        {
                            endOfLine = true;
                            oldLineLength = LineLength;
                        }

                        // Is this new line a new paragraph (and hence a new sentence)
                        //
                        // This is the logic:-
                        // First line isnt a new paragraph (the logic assumes their was a preceding paragraph)
                        // If weve just gone from normal paragraphs to code thats pre-formatted or visa versa or
                        // If were formatting (normal paragraphs) 
                        // wait till the character pointer is at the 1st non blank character of the new line 
                        // (i.e. skip over whitespace after the end of the last paragraph) and
                        // 1st letter of the new paragraph is a capital A through to Z or " or [ ([ for Euclid) or
                        // its a list item or
                        // A single word line or
                        // If were not formatting and the line contains a tab (each row of table needs to be separate)
                        //
                        // Then its a new paragraph and hence a new sentence

                        if (!FirstLine &&
                            (
                              (FormatBefore != Format) ||
                              (
                               (
                                Format && (int)currChar > 32 &&
                                (
                                 ((int)firstWord1stChr >= (int)'A' && (int)firstWord1stChr <= (int)'Z') ||
                                 firstWord1stChr == '"' ||
                                 firstWord1stChr == '[' ||
                                 firstWord1stChr == '(' ||
                                 ItsAList ||
                                 (secondWordLength < 1 && !WordEndsWithFullStop)
                                )
                               ) ||
                               (
                                !Format && ItsGotATab 
                               )
                              )
                             )
                            )
                        {
                            endOfParagraph = true;
                            endOfSentence = true;
                        }

                    }
                    // End of Last Char was a LF so we do the following for every character 

                    // Turn off Formatting and start a new paragraph if this line terminates with a ":"  
                    // This allows us to manually turn off formatting for the text that follows

                    if (currChar == ':' && Format && SplitOnColon)
                    {
                        if (nextChar == (Char)13 || AllBlanks(newLine.Substring(newLine.IndexOf(":") + 1)))
                        {
                            SplittingOnColon = true;
                        }
                    }

                    // In list change tabs to spaces

                    if (currChar == (Char)9 && ItsAList)
                    {
                        currChar = ' ';
                    }

                    // There seem to numerous examples where a space needs to be added, so created a function that decides
                    // if this is necessary, dont do it if were splitting on a colon causes no end of difficulties

                    AddASpace = false;
                    if (Format)
                    {
                        if ((currChar != ':' && nextChar == (Char)13) ||
                            (currChar == ',' || currChar == ';' || currChar == ')' || currChar == '}' || currChar == ']'))
                        {
                            AddASpace = AddSpace(lastChar, currChar, nextChar, Debug, ref DebugText);
                        }
                    }

                    // Start Newline if (we've reached the end of a line)

                    if (endOfLine)
                    {
                        endOfLine = false;
                        StartNewLine(ref LinesNoOf, ref Lines, ref line, currChar, ref LineLength, Debug, DebugText);
                    }
                    else
                    {
                        line += currChar;
                        LineLength += 1;
                        if (AddASpace)
                        {
                            line += " ";
                            LineLength += 1;
                        }
                    }

                    // Start new Sentence if we've hit a fullstop and we've now hit non-blank text following the full stop

                    if (endOfSentence && (int)currChar > 32)
                    {
                        endOfSentence = false;
                        StartNewSentence(ParagraphsNoOf, ref SentencesNoOf, ref Sentences, ref SentenceInParagraph, ref sentence, ref SentenceLength, currChar, Debug, DebugText, Format);
                    }
                    else
                    {
                        if (!endOfSentence)
                        {
                            sentence += currChar;
                            SentenceLength += 1;
                            if (AddASpace)
                            {
                                sentence += " ";
                                SentenceLength += 1;
                            }
                        }
                    }

                    // Start New Pragraph if we've reached the 1st printable character after the end of the last paragraph

                    if (endOfParagraph && (int)currChar > 32)
                    {
                        endOfParagraph = false;
                        StartNewParagraph(ref ParagraphsNoOf, ref Paragraphs, sentence, ref paragraph, currChar, ref ParagraphLength, Debug, DebugText, Format, ItsAnOrderedList, ItsAnUnorderedList, ItsGotHTML, SplittingOnColon, InsertIndicators, allCapitals);
                        // Turn off Splitting On A Colon indicator here as weve now started a new Paragraph
                        SplittingOnColon = false;
                    }
                    else
                    {
                        if (!endOfParagraph)
                        {
                            paragraph += currChar;
                            ParagraphLength += 1;
                            if (AddASpace)
                            {
                                paragraph += " ";
                                ParagraphLength += 1;
                            }
                        }
                    }

                    // Have we reached the end of a sentence

                    // Basically if we've hit a full stop the next char is a space (or non-printable) and the previous char 
                    // was alphanumeric or a closing bracket weve reached the end of a sentence

                    // The one case where this isnt the end of a sentence is the full-stop follows a list item in a  
                    // list (i.e. the 1st word in a line is delimited by a fullstop)
                    // so dont indicate its an end of sentence in this case??

                    // Place here rather than before end of sentence above as we want to include the full stop... so currChar 
                    // will move onto the next char prior to starting the new sentence

                    if (Format && (currChar == '.' || currChar == '?') && (int)nextChar <= 32 &&
                       ((int)lastChar >= (int)'a' && (int)lastChar <= (int)'z' ||
                       (int)lastChar >= (int)'A' && (int)lastChar <= (int)'Z' ||
                       (int)lastChar >= (int)'0' && (int)lastChar <= (int)'9' ||
                       lastChar == '>' ||
                       lastChar == ')' ||
                       lastChar == '}' ||
                       lastChar == ']'))
                    {
                        if (ListItemEndsWithFullStop)
                        {
                            ListItemEndsWithFullStop = false;
                        }
                        else
                        {
                            endOfSentence = true;
                        }
                    }
                    // So that lastChar is correct, for eliminating white space etc.
                    if (AddASpace)
                    {
                        currChar = ' ';
                    }
                }
                // End of Dont skip this character
                lastChar = currChar;
            }
            // End of Character Loop i.e. there are no more characters in the text

            // Add remaining characters (if text didnt end neatly on the end of a paragraph, sentence or line)

            if (line.Length > 0)
            {
                LinesNoOf += 1;
                Lines.Add(line);
            }
            if (sentence.Length > 0)
            {
                SentencesNoOf += 1;
                Sentences.Add(sentence);
                SentenceInParagraph.Add(ParagraphsNoOf);
            }
            if (paragraph.Length > 0)
            {
                if (paragraph.Substring(0, 1) == "¥")
                {
                    // Fell Over when remaining text was "¤W" so add a space
                    paragraph = paragraph + " ";
                    paragraph = paragraph.Substring(0, 1) + paragraph.Substring(paragraph.IndexOf(" "));
                }
                ParagraphsNoOf += 1;
                Paragraphs.Add(paragraph);
            }
        }
    }
}

