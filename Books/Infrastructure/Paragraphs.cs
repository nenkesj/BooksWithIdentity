using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics; 

namespace Books.Infrastructure
{
    public class Paragraphs : Text
    {
        public Paragraphs()
            : base()
        {
        }

        public int NoOfParagraphs { get; set; }
        public string TheAlteredText { get; set; }
        public int AlteredNoOfParagraphs { get; set; }

        int txtPtr, linePtr, paragraphStart, paragraphEnd, paragraphLength;
        string txtLetter, paragraph, debugText;
        Text Txt;

        public void Words(int WordsNoOf, List<string> Words)
        {
            // Subs called:- None
            // Properties Altered:- None
            int wordPtr;
            string word;
            SortedList<string, int> wordsDistinct;
            wordsDistinct = new SortedList<string, int>();
            this.TheAlteredText = "";
            WordsNoOf = 0;
            this.DivideIntoWords(this.TheText, out WordsNoOf, ref Words);
            for (wordPtr = 0; wordPtr <= Words.Count - 1; wordPtr++)
            {
                word = Words[wordPtr].ToString();
                if (wordsDistinct.ContainsKey(word))
                {
                    wordsDistinct[word] += 1;
                }
                else
                {
                    wordsDistinct[word] = 1;
                }
            }
            this.TheAlteredText = "There were " + Words.Count + " words in text with the following frequencies:-" + (Char)13 + (Char)10 + (Char)13 + (Char)10;
            foreach (KeyValuePair<string, int> element in wordsDistinct)
            {
                string wrd = element.Key;
                int cnt = element.Value;
                this.TheAlteredText += wrd + ": " + cnt + (Char)13 + (Char)10;
            }
        }

        public void Sentences(out int SentencesNoOf, ref List<string> Sentences, ref List<int> SentenceInParagraph, int LineWidth, bool Debug, bool EliminateWhiteSpace, bool tabs, bool SplitOnLF)
        {
            // Subs called:- None
            // Properties Altered:- None
            int ParagraphsNoOf, LinesNoOf;
            string debugText, SentenceNo, Sentence, Paragraph;
            List<string> Paragraphs, Lines;
            Paragraphs = new List<string>();
            Lines = new List<string>();
            SentenceInParagraph = new List<int>();
            debugText = "";
            ParagraphsNoOf = 0;
            LinesNoOf = 0;
            this.DivideText(out ParagraphsNoOf, ref Paragraphs, out SentencesNoOf, ref Sentences, ref SentenceInParagraph, out LinesNoOf, ref Lines, out debugText, LineWidth, Debug, true, true, true, false, SplitOnLF, false);
            this.TheAlteredText = "There were " + SentencesNoOf.ToString() + " sentences in text these are:-" + (Char)13 + (Char)10;
            this.TheAlteredText = "There were " + Sentences.Count + " Sentences in text" + (Char)13 + (Char)10 + (Char)13 + (Char)10;
            for (int i = 0; i <= SentencesNoOf - 1; i++)
            {
                SentenceNo = i.ToString();
                Sentence = Sentences[i];
                Paragraph = SentenceInParagraph[i].ToString();
                this.TheAlteredText += SentenceNo + " - " + Sentence + (Char)13 + (Char)10 + "This Sentence is in paragraph # " + Paragraph + (Char)13 + (Char)10 + (Char)13 + (Char)10;
            }
            if (Debug)
            {
                this.TheAlteredText += "Debug Text : -" + (Char)13 + (Char)10 + (Char)13 + (Char)10 + debugText + (Char)13 + (Char)10 + (Char)13 + (Char)10;
            }
        }

        public void Paragrphs(out int ParagraphsNoOf, ref List<string> Paragraphs, out int SentencesNoOf, ref List<string> Sentences, ref List<int> SentenceInParagraph, out int LinesNoOf, ref List<string> Lines, int LineWidth, bool debug, bool EliminateWhiteSpace, bool tabs, bool SplitHeaders, bool SplitOnColon, bool SplitOnLF, bool InsertIndicators)
        {
            // Subs called:- DivideText
            // Properties Altered:- TheAlteredText
            int paragraphPtr, sentencePtr, linePtr;
            string debugText;
            linePtr = 0;
            sentencePtr = 0;
            paragraphPtr = 0;
            //Stopwatch sw = new Stopwatch();
            //Stopwatch sw1 = new Stopwatch();
            //sw.Reset();
            //sw.Start();
            //this.OldDivideText(out ParagraphsNoOf, ref Paragraphs, out SentencesNoOf, ref Sentences, ref SentenceInParagraph, out LinesNoOf, ref Lines, out debugText, LineWidth, debug, EliminateWhiteSpace, tabs, SplitHeaders, SplitOnColon, SplitOnLF, InsertIndicators);
            //sw.Stop();
            //Debug.WriteLine("Elapsed ms Old DivideText: {0}", sw.ElapsedTicks);
            //sw1.Reset();
            //sw1.Start();
            this.DivideText(out ParagraphsNoOf, ref Paragraphs, out SentencesNoOf, ref Sentences, ref SentenceInParagraph, out LinesNoOf, ref Lines, out debugText, LineWidth, debug, EliminateWhiteSpace, tabs, SplitHeaders, SplitOnColon, SplitOnLF, InsertIndicators);
            //sw1.Stop();
            //Debug.WriteLine("Elapsed ms New DivideText: {0}", sw1.ElapsedTicks);

            this.TheAlteredText = "";

            if (debug)
            {
                this.TheAlteredText += "There were " + Paragraphs.Count + " Paragraphs in text" + (Char)13 + (Char)10 + (Char)13 + (Char)10;
            }

            foreach (string paragraph in Paragraphs)
            {
                paragraphPtr += 1;
                if (debug)
                {
                    this.TheAlteredText += paragraphPtr.ToString() + " - " + paragraph;
                }
                else
                {
                    this.TheAlteredText += paragraph;
                }
                if (this.TheAlteredText.Length > 3)
                {
                    // Havent got any CR LF's add two lots to separate paragraphs with a blank line
                    if (this.TheAlteredText.Substring(this.TheAlteredText.Length - 3, 1).ToCharArray()[0] != (Char)10 && this.TheAlteredText.Substring(this.TheAlteredText.Length - 1, 1).ToCharArray()[0] != (Char)10)
                    {
                        this.TheAlteredText = this.TheAlteredText + (Char)13 + (Char)10 + (Char)13 + (Char)10;
                    }
                    // Only got one CR LF so add one so we have a blank line between paragraphs
                    if (this.TheAlteredText.Substring(this.TheAlteredText.Length - 3, 1).ToCharArray()[0] != (Char)10 && this.TheAlteredText.Substring(this.TheAlteredText.Length - 1, 1).ToCharArray()[0] == (Char)10)
                    {
                        this.TheAlteredText = this.TheAlteredText + (Char)13 + (Char)10;
                    }
                }
                if (this.TheAlteredText.Length > 5)
                {
                    // Eliminate double blank lines between paragraphs
                    if (this.TheAlteredText.Substring(this.TheAlteredText.Length - 5, 1).ToCharArray()[0] == (Char)10 && this.TheAlteredText.Substring(this.TheAlteredText.Length - 3, 1).ToCharArray()[0] == (Char)10 && this.TheAlteredText.Substring(this.TheAlteredText.Length - 1, 1).ToCharArray()[0] == (Char)10)
                    {
                        this.TheAlteredText = this.TheAlteredText.Substring(0, this.TheAlteredText.Length - 2);
                    }
                }
                if (this.TheAlteredText.Length > 7)
                {
                    // Eliminate double blank lines between paragraphs
                    if (this.TheAlteredText.Substring(this.TheAlteredText.Length - 7, 1).ToCharArray()[0] == (Char)10 && this.TheAlteredText.Substring(this.TheAlteredText.Length - 5, 1).ToCharArray()[0] == (Char)10 && this.TheAlteredText.Substring(this.TheAlteredText.Length - 3, 1).ToCharArray()[0] == (Char)10 && this.TheAlteredText.Substring(this.TheAlteredText.Length - 1, 1).ToCharArray()[0] == (Char)10)
                    {
                        this.TheAlteredText = this.TheAlteredText.Substring(0, this.TheAlteredText.Length - 4);
                    }
                }
                linePtr = 0;
            }
            if (debug)
            {
                this.TheAlteredText += "Debug Text : -" + (Char)13 + (Char)10 + (Char)13 + (Char)10 + debugText + (Char)13 + (Char)10 + (Char)13 + (Char)10;
                this.TheAlteredText += "There were " + Sentences.Count + " Sentences in text" + (Char)13 + (Char)10 + (Char)13 + (Char)10;
                foreach (string sentence in Sentences)
                {
                    sentencePtr += 1;
                    this.TheAlteredText += sentencePtr.ToString() + " - " + sentence.ToString() + (Char)13 + (Char)10 + (Char)13 + (Char)10;
                }
                this.TheAlteredText += "There were " + Lines.Count + " Lines in text" + (Char)13 + (Char)10 + (Char)13 + (Char)10;
                foreach (string line in Lines)
                {
                    linePtr += 1;
                    this.TheAlteredText += linePtr.ToString() + " - " + line.ToString() + (Char)13 + (Char)10 + (Char)13 + (Char)10;
                }
            }
        }

        public void ListsAndTables(int ParagraphsNoOf, List<string> Paragraphs, out int newNoOfParagraphs, out List<string> newParagraphs, out bool hasnofigpara, out bool hasnotabpara)
        {
            string p;

            bool itHasATableHeading = false;
            bool tableStarted = false;
            int orderedListStartedAt = -1;
            int unorderedListStartedAt = -1;
            int tableStartedAt = -1;

            newParagraphs = new List<string>();

            hasnofigpara = true;
            hasnotabpara = true;

            newNoOfParagraphs = 0;
            for (int i = 0; i < ParagraphsNoOf; i++)
            {
                p = Paragraphs.ToArray()[i];
                if (p.Length > 2)
                {
                    if (p.Substring(0, 3).ToLower() == "fig") { hasnofigpara = false; }
                    if (p.Substring(0, 3).ToLower() == "tab")
                    {
                        hasnotabpara = false;
                        itHasATableHeading = true;
                        tableStarted = false;
                    }
                }
                if (p.Substring(0, 1) == "¤" || p.Substring(0, 1) == "¥" || p.IndexOf((Char)9) > 0)
                {
                    if (p.Substring(0, 1) == "¤")
                    {
                        tableStartedAt = -1;
                        unorderedListStartedAt = -1;
                        if (orderedListStartedAt == -1)
                        {
                            orderedListStartedAt = newNoOfParagraphs;
                            newParagraphs.Add("");
                            newNoOfParagraphs++;
                        }
                        if (orderedListStartedAt > -1)
                        {
                            newParagraphs[orderedListStartedAt] += p;
                        }
                    }
                    if (p.Substring(0, 1) == "¥")
                    {
                        tableStartedAt = -1;
                        orderedListStartedAt = -1;
                        if (unorderedListStartedAt == -1)
                        {
                            unorderedListStartedAt = newNoOfParagraphs;
                            newParagraphs.Add("");
                            newNoOfParagraphs++;
                        }
                        if (unorderedListStartedAt > -1)
                        {
                            newParagraphs[unorderedListStartedAt] += p;
                        }
                    } 
                    if (p.IndexOf((Char)9) > 0)
                    {
                        if (itHasATableHeading)
                        {
                            orderedListStartedAt = -1;
                            unorderedListStartedAt = -1;
                            if (tableStartedAt == -1)
                            {
                                tableStarted = true;
                                tableStartedAt = newNoOfParagraphs;
                                newParagraphs.Add("");
                                newNoOfParagraphs++;
                            }
                            if (tableStartedAt > -1)
                            {
                                if (p.Substring(0, 1) == "§")
                                {
                                    newParagraphs[tableStartedAt] += "£" + p.Substring(1);
                                }
                                else
                                {
                                    newParagraphs[tableStartedAt] += "£" + p;
                                }
                            }
                        }
                        else
                        {
                            unorderedListStartedAt = -1;
                            orderedListStartedAt = -1;
                            tableStartedAt = -1;
                            if (p.Length > 2)
                            {
                                if (p != "§\r\n")
                                {
                                    newNoOfParagraphs++;
                                    newParagraphs.Add(p);
                                }
                            }
                            if (tableStarted)
                            {
                                itHasATableHeading = false;
                                tableStarted = false;
                            }
                        }
                    }
                }
                else
                {
                    unorderedListStartedAt = -1;
                    orderedListStartedAt = -1;
                    tableStartedAt = -1;
                    if (p.Length > 2)
                    {
                        if (p != "§\r\n")
                        {
                            newNoOfParagraphs++;
                            newParagraphs.Add(p);
                        }
                    }
                    if (tableStarted)
                    {
                        itHasATableHeading = false;
                        tableStarted = false;
                    }
                }
            }
        }
    }
}