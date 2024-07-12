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
    // TODO: Maby add padding?
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
        private static int _savedCursorVisualX = -1;
        private static int _highlightChar = -1;

        private int _cursorCharacter => Text[_cursorPosition];
        private bool _doDrawCursor => InputManager.CheckSelected(this);

        public Wrapping Wrapping = 0;

        private Vector2 _mouseScrollVel = new();
        private Vector2 _scroll = new();
        private Vector2 _scrollRoom = new();

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

            if (IsHovered)
            {
                if (_scrollRoom.Y > 0)
                {
                    float totalTextPercent = (_scrollRoom.Y + Dimensions.H) / Dimensions.H;

                    int scrollBarCoverage = (int)(Dimensions.H * (1f / totalTextPercent));

                    float rest = Dimensions.H - scrollBarCoverage;

                    int extraY = (int)(rest * (_scroll.Y / _scrollRoom.Y));

                    DrawRectangle(Dimensions.X + Dimensions.W - 4, Dimensions.Y + extraY, 4, scrollBarCoverage, Color.Gray);
                }

                if (_scrollRoom.X > 0)
                {
                    float totalTextPercent = (_scrollRoom.X + Dimensions.W) / Dimensions.W;

                    int scrollBarCoverage = (int)(Dimensions.W * (1f / totalTextPercent));

                    float rest = Dimensions.W - scrollBarCoverage;

                    int extraX = (int)(rest * (_scroll.X / _scrollRoom.X));

                    DrawRectangle(Dimensions.X + extraX, Dimensions.Y + Dimensions.H - 4, scrollBarCoverage, 4, Color.Gray);
                }
            }

            if (InputManager.CheckSelected(this))
            {
                DrawRectangle(Dimensions.X + _cursorVisualX - (int)_scroll.X, Dimensions.Y + _cursorVisualY - (int)_scroll.Y, 1, FontSize, Color.Gray);
            }
        }

        public override void Update()
        {
            if (IsHovered && IsMouseButtonPressed(MouseButton.Left))
            {
                _cursorPosition = 0;
                _cursorVisualX = -1;
                _cursorVisualY = -1;
                _savedCursorVisualX = -1;
                _highlightChar = -1;
                InputManager.SetInput(this);
                PrepTextboxDraw();
            }

            Vector2 mouseWheelMove = new(0f, GetMouseWheelMoveV().Y);

            if (IsKeyDown(KeyboardKey.LeftShift) || IsKeyDown(KeyboardKey.RightShift))
            {
                mouseWheelMove.X = mouseWheelMove.Y;
                mouseWheelMove.Y = 0f;
            }

            _mouseScrollVel *= Settings.MouseScrollVelocityDropoff;
            _mouseScrollVel += mouseWheelMove * Settings.mouseScrollSensitivity;

            _scroll.X = MathF.Max(MathF.Min(_scroll.X - _mouseScrollVel.X, _scrollRoom.X), 0f);
            _scroll.Y = MathF.Max(MathF.Min(_scroll.Y - _mouseScrollVel.Y, _scrollRoom.Y), 0f);
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

            Vector2? updateFakeCursorPosition = null;
            if (key == KeyboardKey.Up || key == KeyboardKey.Down)
            {
                if (_savedCursorVisualX == -1)
                {
                    _savedCursorVisualX = _cursorVisualX;
                }

                if (key == KeyboardKey.Up)
                {
                    updateFakeCursorPosition = new(_savedCursorVisualX, _cursorVisualY - FontSize * 0.5f);
                }
                if (key == KeyboardKey.Down)
                {
                    updateFakeCursorPosition = new(_savedCursorVisualX, _cursorVisualY + FontSize * 1.5f);
                }
            }
            else
            {
                _savedCursorVisualX = -1;
            }


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

            PrepTextboxDraw(updateFakeCursorPosition);
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
                        DrawTextCodepoint(font, cp, new Vector2(Dimensions.X + textOffsetX, Dimensions.Y + textOffsetY) - _scroll, FontSize, Color.Black);
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

        private void InsertBuffer(List<int> lineBuffer, List<Tuple<int, float>> codepointBuffer, Vector2 mPos, ref int curIndex, ref float textOffsetX, ref bool setCursor, float textOffsetY)
        {
            foreach (Tuple<int, float> cpNWidth in codepointBuffer)
            {
                if (setCursor)
                {
                    if (mPos.Y < textOffsetY + LineSpacing + FontSize && mPos.X < textOffsetX + cpNWidth.Item2 / 2f)
                    {
                        _cursorPosition = curIndex;
                        setCursor = false;
                    }
                }

                if (curIndex == _cursorPosition && _cursorVisualX == -1 && InputManager.CheckSelected(this))
                {
                    _cursorVisualX = (int)textOffsetX;
                    _cursorVisualY = (int)textOffsetY;
                }

                curIndex++;
                textOffsetX += cpNWidth.Item2;

                lineBuffer.Add(cpNWidth.Item1);
            }

            Console.WriteLine("x; " + textOffsetX);
        }

        // figure type out later
        /// <summary>
        /// Handles some logic and prepeares the text to be drawn.
        /// </summary>
        unsafe private void PrepTextboxDraw(Vector2? cursorPos = null)
        {
            Console.WriteLine("Clear");
            _lines.Clear();
            Console.WriteLine(_lines.Count);
            Font font = FontManager.GetFont(FontType, FontSize);
            sbyte* cText = Text.ToUtf8Buffer().AsPointer();

            uint size = TextLength(cText);    // Total size in bytes of the text, scanned by codepoints in loop

            List<Tuple<int, float>> codepointBuffer = [];
            List<int> lineBuffer = [];
            float codepointBufferWidth = 0;
            float curLineWidth = 0;

            int curIndex = 0;

            float textOffsetY = 0;
            float textOffsetX = 0;
            float peakTextOffsetX = 0;

            int codepoint;
            int cpIndex;
            float charWidth;

            int nextByteCount = 0;

            int nextCodepoint = GetCodepointNext(cText, &nextByteCount);
            int nextCodepointIndex = GetGlyphIndex(font, nextCodepoint);
            float nextCharWidth = GetCodepointWidth(nextCodepointIndex);

            float prevCursorX = _cursorVisualX;
            float prevCursorY = _cursorVisualY;

            if (InputManager.CheckSelected(this))
            {
                _cursorVisualX = -1;
                _cursorVisualY = -1;
            }

            bool setCursor = IsHovered && IsMouseButtonPressed(MouseButton.Left) || cursorPos is not null;
            cursorPos ??= GetMousePosition() - new Vector2(Dimensions.X, Dimensions.Y) + _scroll;
            Vector2 mPos = cursorPos.Value;

            if (setCursor)
            {
                _cursorPosition = -1;
            }

            for (int i = 0; i < size;)
            {
                // Get next codepoint from byte string and glyph index in font
                int byteCount = nextByteCount;
                codepoint = nextCodepoint;
                cpIndex = nextCodepointIndex;
                charWidth = nextCharWidth;

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
                    InsertBuffer(lineBuffer, codepointBuffer, mPos, ref curIndex, ref textOffsetX, ref setCursor, textOffsetY);
                    codepointBuffer = [];

                    if (setCursor)
                    {
                        if (mPos.Y < textOffsetY + LineSpacing + FontSize)
                        {
                            _cursorPosition = curIndex;
                            setCursor = false;

                            _cursorVisualX = (int)textOffsetX;
                            _cursorVisualY = (int)textOffsetY;
                        }
                    }

                    InsertBuffer(lineBuffer, [new(' ', 0)], mPos, ref curIndex, ref textOffsetX, ref setCursor, textOffsetY);
                    //DrawCodepointAt(origin, codepointBuffer, curIndex, textOffsetX, textOffsetY, didDrawCursor);
                    _lines.Add(lineBuffer);
                    lineBuffer = [];
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

                    curLineWidth = 0;
                    textOffsetY += LineSpacing + FontSize;
                    peakTextOffsetX = MathF.Max(peakTextOffsetX, textOffsetX);
                    textOffsetX = 0;
                }
                else
                {
                    codepointBuffer.Add(new(codepoint, charWidth));
                    codepointBufferWidth += charWidth;

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
                                InsertBuffer(lineBuffer, codepointBuffer, mPos, ref curIndex, ref textOffsetX, ref setCursor, textOffsetY);
                                _lines.Add(lineBuffer);
                                lineBuffer = [];
                                codepointBuffer = [];
                                codepointBufferWidth = 0;
                                curLineWidth = 0;
                            }

                            if (setCursor)
                            {
                                if (mPos.Y < textOffsetY + LineSpacing + FontSize)
                                {
                                    _cursorPosition = curIndex;
                                    setCursor = false;

                                    _cursorVisualX = (int)textOffsetX;
                                    _cursorVisualY = (int)textOffsetY;
                                }
                            }

                            textOffsetY += LineSpacing + FontSize;
                            textOffsetX = 0;
                            peakTextOffsetX = MathF.Max(peakTextOffsetX, textOffsetX);


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
                            InsertBuffer(lineBuffer, codepointBuffer, mPos, ref curIndex, ref textOffsetX, ref setCursor, textOffsetY);
                            codepointBuffer = [];
                            curLineWidth += codepointBufferWidth;
                            codepointBufferWidth = 0;

                        }
                    }
                }

                i += byteCount;
            }

            InsertBuffer(lineBuffer, codepointBuffer, mPos, ref curIndex, ref textOffsetX, ref setCursor, textOffsetY);
            _lines.Add(lineBuffer);
            curLineWidth += codepointBufferWidth;
            codepointBufferWidth = 0;

            if (setCursor)
            {
                _cursorPosition = Text.Length;
            }

            Console.WriteLine(_cursorVisualX + " - " + _cursorVisualY + " | " + _cursorPosition);

            if (_cursorVisualX == -1 && InputManager.CheckSelected(this))
            {
                _cursorVisualX = (int)textOffsetX;
                _cursorVisualY = (int)textOffsetY;
            }

            peakTextOffsetX = MathF.Max(peakTextOffsetX, textOffsetX);
            _scrollRoom = new(peakTextOffsetX - Dimensions.W, textOffsetY + FontSize - Dimensions.H);
            Console.WriteLine(_scrollRoom);

            if (prevCursorX != _cursorVisualX || prevCursorY != _cursorVisualY)
            {
                Console.WriteLine("ScrollCorrectionOnCursorMoved");
                if ((_cursorVisualY + FontSize - _scroll.Y) > Dimensions.H)
                {
                    _scroll.Y = (_cursorVisualY + FontSize) - Dimensions.H;
                }
                if ((_cursorVisualY - _scroll.Y) < 0)
                {
                    _scroll.Y = _cursorVisualY;
                }

                if ((_cursorVisualX + FontSize - _scroll.X) > Dimensions.W)
                {
                    _scroll.X = (_cursorVisualX + FontSize) - Dimensions.W;
                }
                if ((_cursorVisualX - _scroll.X) < 0)
                {
                    _scroll.X = _cursorVisualX;
                }
                Console.WriteLine(_scroll.Y + " | " + _scrollRoom.Y);
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
