using Raylib_cs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GNSUsingCS.Elements
{
    enum Wrapping
    {
        CharWrapping, // normal(?) wrapping
        WordWrapping, // word wrappoing
        NoWrapping // no wrapping at all
    }

    internal class TextBox : TextContainer, IInput
    {
        public Box Box;

        // only one textpox has a cursor at a time so its less memory use if i do this, right..? (not that i think it matters much...)z\
        private static int _cursorPosition = 0;
        private static int _cursorVisualX = -1;
        private static int _cursorVisualY = -1;
        private static int _highlightChar = -1;

        private int _cursorCharacter => Text[_cursorPosition];
        private bool _doDrawCursor => InputManager.CheckSelected(this);

        public Wrapping Wrapping = 0;

        public override List<Element> Children => [Box];

        private List<List<int>> _lines = [];

        public TextBox()
        {
            Box = new Box();
            Box.Dimensions.Width.Set(0, 1);
            Box.Dimensions.Height.Set(0, 1);
        }

        protected override void DrawElement()
        {
            Box.Draw();
            // DrawTextEx(FontManager.GetFont(FontType, FontSize), Text, new Vector2(Dimensions.X, Dimensions.Y), FontSize, Spacing, Color);
            DrawCodepoints();
        }

        public override void Update()
        {
            if (IsHovered && IsMouseButtonPressed(MouseButton.Left))
            {
                _cursorPosition = 0;
                _cursorVisualX = -1;
                _cursorVisualY = -1;
                _highlightChar = -1;
                InputManager.SetInput(this);
                PrepTextboxDraw();
            }
        }

        public void IncommingCharacter(char character)
        {
            Text = Text.Insert(_cursorPosition, character.ToString());
            _cursorPosition++;

            PrepTextboxDraw();
        }

        public void IncommingSpecialKey(KeyboardKey key, List<KeyAddition> additions)
        {
            if (key == KeyboardKey.Right)
            {
                _cursorPosition = _cursorPosition == Text.Length ? _cursorPosition : _cursorPosition + 1;
            }

            if (key == KeyboardKey.Left)
            {
                _cursorPosition = _cursorPosition == 0 ? 0 : _cursorPosition - 1;
            }

            Console.WriteLine("S: " + key.ToString());
            if (key == KeyboardKey.Backspace && _cursorPosition != 0)
            {
                Text = Text.Remove(_cursorPosition - 1, 1);
                _cursorPosition--;
            }
            if (key == KeyboardKey.Delete && _cursorPosition != Text.Length)
            {
                Text = Text.Remove(_cursorPosition, 1);
            }

            if (key == KeyboardKey.Enter)
            {
                Text = Text.Insert(_cursorPosition, "\n");
                _cursorPosition++;
            }

            PrepTextboxDraw();
        }
        private bool InBetween(int val, int v1, int v2)
        {
            if (v1 > v2)
                return val < v1 && val > v2 - 1;
            else
                return val < v2 && val > v1 - 1;
        }

        unsafe private float GetCodepointWidth(int idx)
        {
            Font font = FontManager.GetFont(FontType, FontSize);
            float scaleFactor = FontSize / (float)font.BaseSize;
            return (font.Glyphs[idx].AdvanceX == 0 ? (float)font.Recs[idx].Width : (float)font.Glyphs[idx].AdvanceX) * scaleFactor + Spacing;
        }

        unsafe private void DrawCodepoints()
        {
            Font font = FontManager.GetFont(FontType, FontSize);

            int curIndex = 0;
            float textOffsetY = 0;
            float textOffsetX = 0;

            foreach (List<int> lineBuffer in _lines)
            {
                foreach (int cp in lineBuffer)
                {
                    int index = GetGlyphIndex(font, cp);
                    float cWidth = GetCodepointWidth(index);

                    if (_highlightChar != _cursorPosition && _highlightChar != -1 && InBetween(curIndex, _highlightChar, _cursorPosition))
                    {
                        DrawRectangle(Dimensions.X + (int)Math.Floor(textOffsetX), Dimensions.Y + (int)Math.Floor(textOffsetY), (int)Math.Ceiling(cWidth), FontSize, Color.Gray);
                    }

                    if ((cp != ' ') && (cp != '\t'))
                    {
                        DrawTextCodepoint(font, cp, new(Dimensions.X + textOffsetX, Dimensions.Y + textOffsetY), FontSize, Color.Black);
                    }

                    // std::cout << curIndex << " is " << (curIndex == cursorPosition) << std::endl;

                    /*
                    if (curIndex == cursorPosition && !didDrawCursor)
                    {
                        didDrawCursor = true;
                        int effectiveX = origin.x + (textOffsetX > this->size.x - 1 ? this->size.x - 1 : textOffsetX);
                        DrawRectangle(effectiveX, origin.y + textOffsetY, 1, fontSize, BLACK);
                        curX = effectiveX;
                        curY = textOffsetY + fontSize / 2;
                    }
                    */

                    curIndex++; // idk if this should be here or before, but whatever...
                    textOffsetX += cWidth;
                }
                textOffsetY += FontSize + LineSpacing;
                textOffsetX = 0;
            }
        }

        // figure type out later
        /// <summary>
        /// Handles some logic and prepeares the text to be drawn.
        /// </summary>
        unsafe private void PrepTextboxDraw()
        {
            Console.WriteLine("Clear");
            _lines.Clear();
            Console.WriteLine(_lines.Count);
            Font font = FontManager.GetFont(FontType, FontSize);
            sbyte* cText = Text.ToUtf8Buffer().AsPointer();

            uint size = TextLength(cText);    // Total size in bytes of the text, scanned by codepoints in loop

            List<int> codepointBuffer = [];
            List<int> lineBuffer = [];
            float codepointBufferWidth = 0;
            float curLineWidth = 0;

            float nextCharWidth = 0;

            int curIndex = 0;

            float textOffsetY = 0;       // Offset between lines (on linebreak '\n')
            float textOffsetX = 0;       // Offset X to next character to draw

            int codepoint;
            int cpIndex;

            int nextByteCount = 0;

            int nextCodepoint = GetCodepointNext(cText, &nextByteCount);
            int nextCodepointIndex = GetGlyphIndex(font, nextCodepoint);

            bool setCursor = IsHovered && IsMouseButtonPressed(MouseButton.Left);
            Vector2 mPos = GetMousePosition() - new Vector2(Dimensions.X, Dimensions.Y);

            for (int i = 0; i < size;)
            {
                // Get next codepoint from byte string and glyph index in font
                int byteCount = nextByteCount;
                codepoint = nextCodepoint;
                cpIndex = nextCodepointIndex;

                if (i + nextByteCount != size)
                {
                    nextCodepoint = GetCodepointNext(&cText[i + nextByteCount], &nextByteCount);
                    nextCodepointIndex = GetGlyphIndex(font, nextCodepoint);
                    nextCharWidth = GetCodepointWidth(nextCodepointIndex);
                }
                else
                {
                    nextCharWidth = 0;
                }

                if (codepoint == '\n') 
                {
                    //DrawCodepointAt(origin, codepointBuffer, curIndex, textOffsetX, textOffsetY, didDrawCursor);
                    _lines.Add(lineBuffer);
                    lineBuffer = [];
                    codepointBuffer = [];
                    codepointBufferWidth = 0;

                    /*
                    if (curIndex == cursorPosition && !didDrawCursor)
                    {
                        didDrawCursor = true;
                        int effectiveX = origin.x + (textOffsetX > this->size.x - 1 ? this->size.x - 1 : textOffsetX);
                        DrawRectangle(effectiveX, origin.y + textOffsetY, 1, fontSize, BLACK);
                        curX = effectiveX;
                        curY = textOffsetY + fontSize / 2;
                    }
                    curIndex++;
                    */

                    textOffsetY += LineSpacing + FontSize;
                    curLineWidth = 0;
                }
                else
                {
                    codepointBuffer.Add(codepoint);
                    codepointBufferWidth += GetCodepointWidth(cpIndex);

                    if (setCursor)
                    {
                        Console.WriteLine(mPos.Y + ">" + (textOffsetY + LineSpacing + FontSize) + " | " + mPos.X + "<" + (curLineWidth + nextCharWidth + codepointBufferWidth));
                        if (mPos.Y < textOffsetY + LineSpacing + FontSize && mPos.X < curLineWidth + codepointBufferWidth - nextCharWidth / 2f)
                        {
                            _cursorPosition = curIndex;
                            setCursor = false;
                        }
                    }

                    if (codepoint == ' ' || codepoint == '\t' || Wrapping == Wrapping.CharWrapping || Wrapping == Wrapping.NoWrapping || curLineWidth + nextCharWidth + codepointBufferWidth > Dimensions.W)
                    {
                        if (curLineWidth + nextCharWidth + codepointBufferWidth > Dimensions.W && Wrapping != Wrapping.NoWrapping && !(codepoint == ' ' || codepoint == '\t'))
                        {
                            //bool wasZero = curLineWidth == 0;

                            if (Wrapping == Wrapping.WordWrapping && curLineWidth != 0)
                            {
                                _lines.Add(lineBuffer);
                                lineBuffer = [];
                                curLineWidth = 0;
                            }
                            else
                            {
                                lineBuffer.AddRange(codepointBuffer);
                                _lines.Add(lineBuffer);
                                lineBuffer = [];
                                codepointBuffer = [];
                                codepointBufferWidth = 0;
                                curLineWidth = 0;
                            }

                            textOffsetY += LineSpacing + FontSize;


                            /*

                            //DrawCodepointAt(origin, codepointBuffer, curIndex, textOffsetX, textOffsetY, didDrawCursor);

                            if (wasZero || Wrapping != Wrapping.WordWrapping)
                            {
                                if (!charWrapNextLine)
                                {
                                    if (curIndex == cursorPosition && !didDrawCursor)
                                    {
                                        didDrawCursor = true;
                                        int effectiveX = origin.x + (textOffsetX > this->size.x - 1 ? this->size.x - 1 : textOffsetX);
                                        DrawRectangle(effectiveX, origin.y + textOffsetY, 1, fontSize, BLACK);
                                        curX = effectiveX;
                                        curY = textOffsetY + fontSize / 2;
                                    }
                                }
                            
                                textOffsetY += LineSpacing + FontSize;
                                textOffsetX = 0.0f;
                                curLineWidth = 0;
                            }
                            */
                        }
                        else
                        {
                            // DrawCodepointAt(origin, codepointBuffer, curIndex, textOffsetX, textOffsetY, didDrawCursor);
                            lineBuffer.AddRange(codepointBuffer);
                            codepointBuffer = [];
                            curLineWidth += codepointBufferWidth;
                            codepointBufferWidth = 0;

                        }
                    }
                }

                i += byteCount;
                curIndex++;
            }

            lineBuffer.AddRange(codepointBuffer);
            _lines.Add(lineBuffer);

            if (setCursor)
            {
                _cursorPosition = Text.Length;
            }

            /*
            if (!didDrawCursor)
            {
                int effectiveX = origin.x + (textOffsetX > this->size.x - 1 ? this->size.x - 1 : textOffsetX);
                DrawRectangle(effectiveX, origin.y + textOffsetY, 1, fontSize, BLACK);
                curX = effectiveX;
                curY = textOffsetY + fontSize / 2;
            }
            */

            // std::cout << " f: " << curLineWidth + codepointBufferWidth << " - " << this->size.x << std::endl;
        }
    }
}
