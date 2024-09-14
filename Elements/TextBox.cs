using Raylib_cs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
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

    internal class TextBox : TextContainer, IInput // Also add a porgramming TextBox that auto corrects and displays possible values when writing lua.
    {
        [ConfigAttributes.SubElement]
        public Box Box;

        private static Dictionary<string, List<Func<char, bool>>> RepeatSets = new()
        {
            { "Note taking", [(char c) => { return char.IsLetterOrDigit(c); }, (char c) => { return c == '\t'; } ] },
            { "Programming", [] }

            // custom sets/set loading?
        };

        // only one textbox has a cursor at a time so its less memory use if i do this, right..? (not that i think it matters much...)
        private static int _cursorPosition = 0;
        private static int _cursorVisualX = -1;
        private static int _cursorVisualY = -1;
        private static int _savedCursorVisualX = -1;
        private static int _highlightPosition = -1;

        private static int _savedLeftHighlight = -1;
        private static int _savedRightHighlight = -1;

        private static bool justSelected = true;

        private enum HeldMode
        {
            Nothing,
            ScrollY,
            ScrollX,
            SelectTextChar,
            SelectTextWord,
            SelectTextLine,
        }

        private HeldMode heldMode = HeldMode.Nothing;

        private char _cursorCharacter => _cursorPosition == Text.Length || _cursorPosition == -1 ? ' ' : Text[_cursorPosition];
        private bool _doDrawCursor => InputManager.CheckSelected(this);

        public string setType = "Note taking";

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

        internal override void PostRecalculate(int x, int y, int w, int h)
        {
            PrepareTexbox(new Vector2(int.MinValue, int.MinValue));
        }

        protected override void DrawElement()
        {
            Box.Draw();
            // DrawTextEx(FontManager.GetFont(FontType, FontSize), Text, new Vector2(Dimensions.X, Dimensions.Y), FontSize, Spacing, Color);
            DrawCodepoints();

            if (IsHovered && _scrollRoom.Y > 0 && heldMode == 0 || heldMode == HeldMode.ScrollY)
            {
                float totalTextPercent = (_scrollRoom.Y + Dimensions.H) / Dimensions.H;
                int scrollBarCoverage = (int)(Dimensions.H * (1f / totalTextPercent));
                float rest = Dimensions.H - scrollBarCoverage;
                int extraY = (int)(rest * (_scroll.Y / _scrollRoom.Y));

                DrawRectangle(Dimensions.X + Dimensions.W - 4, Dimensions.Y + extraY, 4, scrollBarCoverage, Color.Gray);
            }

            if (IsHovered && _scrollRoom.X > 0 && heldMode == 0 || heldMode == HeldMode.ScrollX)
            {
                float totalTextPercent = (_scrollRoom.X + Dimensions.W) / Dimensions.W;
                int scrollBarCoverage = (int)(Dimensions.W * (1f / totalTextPercent));
                float rest = Dimensions.W - scrollBarCoverage;
                int extraX = (int)(rest * (_scroll.X / _scrollRoom.X));

                DrawRectangle(Dimensions.X + extraX, Dimensions.Y + Dimensions.H - 4, scrollBarCoverage, 4, Color.Gray);
            }

            if (InputManager.CheckSelected(this))
            {
                DrawRectangle(Dimensions.X + _cursorVisualX - (int)_scroll.X, Dimensions.Y + _cursorVisualY - (int)_scroll.Y, 1, FontSize, Color.Gray);
            }
        }

        internal override void Update()
        {
            base.Update(); //not really neccecary?

            if (!IsMouseButtonDown(MouseButton.Left))
            {
                if (heldMode == HeldMode.SelectTextChar)
                {
                    if (_cursorPosition == _highlightPosition)
                    {
                        _highlightPosition = -1;
                    }
                }

                heldMode = HeldMode.Nothing;
            }
            else if (heldMode != 0 && InputManager.CheckSelected(this))
            {
                if (heldMode == HeldMode.ScrollY)
                {
                    float totalTextPercent = (_scrollRoom.Y + Dimensions.H) / Dimensions.H;
                    _scroll.Y += MouseManager.MouseVelocity.Y * totalTextPercent;
                }
                if (heldMode == HeldMode.ScrollX)
                {
                    float totalTextPercent = (_scrollRoom.X + Dimensions.W) / Dimensions.W;
                    _scroll.X += MouseManager.MouseVelocity.X * totalTextPercent;
                }
                if (heldMode == HeldMode.SelectTextChar || heldMode == HeldMode.SelectTextWord || heldMode == HeldMode.SelectTextLine)
                {
                    // set cursor each frame
                    PrepareTexbox(MouseManager.MousePosition - new Vector2(Dimensions.X, Dimensions.Y) + _scroll);

                    if (heldMode != HeldMode.SelectTextChar)
                    {
                        if (heldMode == HeldMode.SelectTextWord)
                            SelectWord();
                        else if (heldMode == HeldMode.SelectTextLine)
                            SelectLine();

                        int p1 = Math.Max(Math.Max(Math.Max(_savedLeftHighlight, _savedRightHighlight), _cursorPosition), _highlightPosition);
                        int p2 = Math.Min(Math.Min(Math.Min(_savedLeftHighlight, _savedRightHighlight), _cursorPosition), _highlightPosition);

                        _cursorPosition = p1;
                        _highlightPosition = p2;
                    }
                }
            }

            if (IsHovered && IsMouseButtonDown(MouseButton.Left) && heldMode == 0)
            {
                Vector2 mousePos = MouseManager.MousePosition - new Vector2(Dimensions.X, Dimensions.Y);

                if (_scrollRoom.X > 0)
                {
                    float totalTextPercent = (_scrollRoom.X + Dimensions.W) / Dimensions.W;
                    int scrollBarCoverage = (int)(Dimensions.W * (1f / totalTextPercent));
                    float rest = Dimensions.W - scrollBarCoverage;
                    int extraX = (int)(rest * (_scroll.X / _scrollRoom.X));

                    if (mousePos.Y >= Dimensions.H - 4 && mousePos.Y <= Dimensions.H && mousePos.X >= extraX && mousePos.X <= extraX + scrollBarCoverage)
                    {
                        heldMode = HeldMode.ScrollX;
                    }
                }

                if (_scrollRoom.Y > 0)
                {
                    float totalTextPercent = (_scrollRoom.Y + Dimensions.H) / Dimensions.H;
                    int scrollBarCoverage = (int)(Dimensions.H * (1f / totalTextPercent));
                    float rest = Dimensions.H - scrollBarCoverage;
                    int extraY = (int)(rest * (_scroll.Y / _scrollRoom.Y));

                    if (mousePos.X >= Dimensions.W - 4 && mousePos.X <= Dimensions.W && mousePos.Y >= extraY && mousePos.Y <= extraY + scrollBarCoverage)
                    {
                        heldMode = HeldMode.ScrollY;
                    }
                }

                if (heldMode == HeldMode.ScrollY || heldMode == HeldMode.ScrollX)
                {
                    InputManager.SetInput(this);
                }
            }

            if (IsHovered && IsMouseButtonPressed(MouseButton.Left) && heldMode == HeldMode.Nothing)
            {
                justSelected = false;
                _cursorPosition = 0;
                _cursorVisualX = -1;
                _cursorVisualY = -1;
                _savedCursorVisualX = -1;

                InputManager.SetInput(this);
                PrepareTexbox();

                int click = MouseManager.RepeatedStillClicks[0];

                if (click == 1)
                {
                    heldMode = HeldMode.SelectTextChar;
                    _highlightPosition = _cursorPosition;
                }

                if (click > 1)
                {
                    click = (click) % 2;

                    switch (click)
                    {
                        case 0:
                            SelectWord();
                            heldMode = HeldMode.SelectTextWord;
                            break;
                        case 1:
                            SelectLine();
                            heldMode = HeldMode.SelectTextLine;
                            break;
                    }

                    _savedLeftHighlight = _highlightPosition;
                    _savedRightHighlight = _cursorPosition;

                    justSelected = true;
                }
            };

            Vector2 mouseWheelMove = new(0f, GetMouseWheelMoveV().Y);

            if (IsKeyDown(KeyboardKey.LeftShift) || IsKeyDown(KeyboardKey.RightShift))
            {
                mouseWheelMove.X = mouseWheelMove.Y;
                mouseWheelMove.Y = 0f;
            }

            _mouseScrollVel *= Settings.MouseScrollVelocityDropoff;
            _mouseScrollVel += mouseWheelMove * Settings.MouseScrollSensitivity;

            _scroll.X = MathF.Max(MathF.Min(_scroll.X - _mouseScrollVel.X, _scrollRoom.X), 0f);
            _scroll.Y = MathF.Max(MathF.Min(_scroll.Y - _mouseScrollVel.Y, _scrollRoom.Y), 0f);
        }

        private void SelectWord()
        {
            int sPos = _cursorPosition;
            if (!RepeatActionTillChange(Left))
                Right();
            _highlightPosition = _cursorPosition;
            _cursorPosition = sPos;
            RepeatActionTillChange(Right);
        }

        private void SelectLine()
        {
            int sPos = _cursorPosition;

            bool onlyLeft = false;
            if (_cursorCharacter == '\n')
            {
                Left();
                onlyLeft = true;
            }

            int pos = _cursorPosition == -1 ? 0 : -1;

            bool goingLeft = true;

            while (true)
            {
                if (_cursorCharacter == '\n' || pos == _cursorPosition)
                {
                    if (goingLeft == false)
                        return;

                    if (_cursorCharacter == '\n')
                        Right();

                    if (onlyLeft)
                    {
                        _highlightPosition = _cursorPosition;
                        _cursorPosition = sPos + 1;
                        return;
                    }

                    goingLeft = false;
                    _highlightPosition = _cursorPosition;
                    _cursorPosition = sPos;
                    pos = _cursorPosition == -1 ? 0 : -1;
                }

                pos = _cursorPosition;

                if (goingLeft)
                    Left();
                else
                    Right();
            }

        }

        // return true if movement stopped "by wall"
        private bool RepeatActionTillChange(Action action)
        {
            char c = _cursorCharacter;

            bool isSpace = c == ' ';
            bool isNLine = c == '\t';
            Func<char, bool> selectedSet = null;
            RepeatSets[setType].ForEach(inSet => {
                if (inSet(c))
                    selectedSet = inSet;
            });
            bool isInSet = selectedSet is null ? false : true;

            int position = _cursorPosition == -1 ? 0 : -1; // for tracking that it actually changed - make sure its different, pre first action

            while (position != _cursorPosition)
            {
                position = _cursorPosition;

                action();

                if ((isSpace && _cursorCharacter != ' ') || (isNLine && _cursorCharacter != '\n') || (isInSet && !selectedSet(_cursorCharacter)))
                    return false;
                else if (!isInSet && !isSpace && !isNLine)
                {
                    if (_cursorCharacter == ' ' || _cursorCharacter == '\n')
                        return false;

                    bool found = false; // space characters operate outside the sets
                    RepeatSets[setType].ForEach(inSet => {
                        if (inSet(_cursorCharacter))
                            found = true;
                    });

                    if (found)
                        return false;
                }
            }

            return true;
        }

        void IInput.IncommingCharacter(char character)
        {
            if (heldMode == HeldMode.SelectTextChar || heldMode == HeldMode.SelectTextWord || heldMode == HeldMode.SelectTextLine)
            {
                return;
            }

            if (_highlightPosition != -1)
            {
                ClearSelected();
            }

            Text = Text.Insert(_cursorPosition, character.ToString());
            _cursorPosition++;

            PrepareTexbox();
        }

        void IInput.IncommingSpecialKey(KeyboardKey key, List<KeyAddition> additions)
        {
            if (heldMode == HeldMode.SelectTextChar || heldMode == HeldMode.SelectTextWord || heldMode == HeldMode.SelectTextLine)
            {
                return;
            }

            if (key == KeyboardKey.Left || key == KeyboardKey.Right || key == KeyboardKey.Up || key == KeyboardKey.Down)
            {
                if (additions.Contains(KeyAddition.Shift))
                {
                    if (justSelected)
                    {
                        if (key == KeyboardKey.Left || key == KeyboardKey.Up)
                        {
                            int saveP = _highlightPosition;
                            _highlightPosition = _cursorPosition;
                            _cursorPosition = saveP;
                        }

                        justSelected = false;
                    }

                    if (_highlightPosition == -1)
                    {
                        _highlightPosition = _cursorPosition;
                    }
                }
                else if (_highlightPosition != -1)
                {
                    if (key == KeyboardKey.Left)
                    {
                        if (_highlightPosition < _cursorPosition)
                        {
                            _cursorPosition = _highlightPosition;
                        }
                    }
                    if (key == KeyboardKey.Right)
                    {
                        if (_highlightPosition > _cursorPosition)
                        {
                            _cursorPosition = _highlightPosition;
                        }
                    }

                    _highlightPosition = -1;

                    key = 0;
                }

                if (key == KeyboardKey.Left)
                {
                    CtrlRepeatedAction(Left, additions, true);
                }
                if (key == KeyboardKey.Right)
                {
                    CtrlRepeatedAction(Right, additions, false);
                }
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
                    updateFakeCursorPosition = Up(additions);
                }
                if (key == KeyboardKey.Down)
                {
                    updateFakeCursorPosition = Down(additions);
                }
            }
            else
            {
                _savedCursorVisualX = -1;
            }


            if (key == KeyboardKey.Backspace && (_cursorPosition != 0 || _highlightPosition != -1))
            {
                if (_highlightPosition == -1)
                {
                    CtrlRepeatedAction(Backspace, additions, true);
                }
                else
                {
                    if (additions.Contains(KeyAddition.Ctrl))
                    {
                        // ctrl Backspace() from lowest of _cursorPosition and _highlightPosition
                    }
                    else
                    {
                        ClearSelected();
                    }
                }
            }

            if (key == KeyboardKey.Delete && (_cursorPosition != 0 || _highlightPosition != -1))
            {
                if (_highlightPosition == -1)
                {
                    CtrlRepeatedAction(Delete, additions, false);
                }
                else
                {
                    if (additions.Contains(KeyAddition.Ctrl))
                    {
                        // ctrl Delete() from lowest of _cursorPosition and _highlightPosition
                    }
                    else
                    {
                        ClearSelected(); // TODO: this needs to know it cant delete quite that much
                    }
                }
            }

            if (key == KeyboardKey.Enter)
            {
                if (_highlightPosition != -1)
                {
                    ClearSelected();
                }
                
                Enter();
            }

            PrepareTexbox(updateFakeCursorPosition);

            if (_highlightPosition == _cursorPosition)
            {
                _highlightPosition = -1;
            }
        }

        private bool IsSpaceChar(char c)
        {
            return c == ' ';
        }

        private void CtrlRepeatedAction(Action action, List<KeyAddition> additions, bool left)
        {
            int offset = left ? -1 : 0;

            if (!additions.Contains(KeyAddition.Ctrl))
            {
                action();
                return;
            }

            Func<char, bool> selectedSet = null;

            char startChar = _cursorPosition == 0 ? ' ' : Text[_cursorPosition + offset];
            bool allowingSpace = IsSpaceChar(startChar) && left; // only when you start at space and are going left
                                                         // or activate when you dont start at space but encounter a space and are going right.

            int position = _cursorPosition == -1 ? 0 : -1; // for tracking that it actually changed - make sure its different, pre first action

            RepeatSets[setType].ForEach(inSet => {
                if (inSet(startChar))
                    selectedSet = inSet;
            });
            bool isInSet = selectedSet is null ? false : true;

            while (position != _cursorPosition)
            {
                position = _cursorPosition;

                action();

                char nChar = _cursorPosition == 0 ? ' ' : Text[_cursorPosition + offset];

                if (nChar == '\n')
                    return;

                if (!left && !allowingSpace && IsSpaceChar(nChar))
                {
                    selectedSet = a => false;
                    isInSet = true;

                    allowingSpace = true;
                }

                if (allowingSpace && IsSpaceChar(nChar))
                    continue;

                if (allowingSpace && !IsSpaceChar(nChar))
                {
                    if (left)
                    {
                        selectedSet = null;
                        RepeatSets[setType].ForEach(inSet => {
                            if (inSet(nChar))
                                selectedSet = inSet;
                        });
                        isInSet = selectedSet is null ? false : true;
                    }

                    allowingSpace = false;
                }

                if (!isInSet)
                {
                    bool found = IsSpaceChar(nChar); // space characters operate outside the sets
                    RepeatSets[setType].ForEach(inSet => {
                        if (inSet(nChar))
                            found = true;
                    });

                    if (found)
                        return;
                }
                else
                {
                    if (!selectedSet(nChar))
                    {
                        // not in the set anymore
                        return;
                    }
                }
            }
        }

        private void Left()
        {
            _cursorPosition = _cursorPosition == 0 ? 0 : _cursorPosition - 1;
        }

        private void Right()
        {
            _cursorPosition = _cursorPosition == Text.Length ? _cursorPosition : _cursorPosition + 1;
        }

        private Vector2? Up(List<KeyAddition> additions)
        {
            return new(_savedCursorVisualX, _cursorVisualY - FontSize * 0.5f);
        }

        private Vector2? Down(List<KeyAddition> additions)
        {
            return new(_savedCursorVisualX, _cursorVisualY + FontSize * 1.5f);
        }

        private void Backspace()
        {
            Text = Text.Remove(_cursorPosition - 1, 1);
            _cursorPosition--;
        }

        private void Delete()
        {
            Text = Text.Remove(_cursorPosition, 1);
        }

        private void Enter()
        {
            Text = Text.Insert(_cursorPosition, "\n");
            _cursorPosition++;
        }

        private void ClearSelected()
        {
            int mpos = Math.Min(_cursorPosition, _highlightPosition);
            int size = Math.Max(_cursorPosition, _highlightPosition) - mpos;
            Text = Text.Remove(mpos, size);
            _cursorPosition = mpos;
            _highlightPosition = -1;
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

                    if (InputManager.CheckSelected(this) && _highlightPosition != _cursorPosition && _highlightPosition != -1 && InBetween(curIndex, _highlightPosition, _cursorPosition))
                    {
                        DrawRectangle(Dimensions.X + (int)Math.Floor(textOffsetX) - (int)_scroll.X, Dimensions.Y + (int)Math.Floor(textOffsetY) - (int)_scroll.Y, (int)Math.Ceiling(cWidth), FontSize, Color.Gray);
                    }

                    if ((cp != ' ') && (cp != '\t'))
                    {
                        DrawTextCodepoint(font, cp, new Vector2(Dimensions.X + textOffsetX, Dimensions.Y + textOffsetY) - _scroll, FontSize, Color.Black);
                    }

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
        }

        // figure type out later
        /// <summary>
        /// Handles some logic and prepeares the text to be drawn.
        /// </summary>
        unsafe private void PrepareTexbox(Vector2? cursorPos = null)
        {
            _lines.Clear();
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
            cursorPos ??= MouseManager.MousePosition - new Vector2(Dimensions.X, Dimensions.Y) + _scroll;
            Vector2 mPos = cursorPos.Value;

            if (setCursor)
            {
                if (mPos.Y < 0)
                {
                    _savedCursorVisualX = -1;
                    _cursorPosition = 0;
                    setCursor = false;

                    _cursorVisualX = (int)textOffsetX;
                    _cursorVisualY = (int)textOffsetY;
                }
                else
                {
                    _cursorPosition = -1;
                }
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
                    _lines.Add(lineBuffer);
                    lineBuffer = [];
                    codepointBufferWidth = 0;

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
                        }
                        else
                        {
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

            if (_cursorVisualX == -1 && InputManager.CheckSelected(this))
            {
                _savedCursorVisualX = -1;
                _cursorVisualX = (int)textOffsetX;
                _cursorVisualY = (int)textOffsetY;
            }

            peakTextOffsetX = MathF.Max(peakTextOffsetX, textOffsetX);
            _scrollRoom = new(peakTextOffsetX - Dimensions.W, textOffsetY + FontSize - Dimensions.H);

            int SODown = heldMode == HeldMode.Nothing ? Settings.ScrollOffDown : 0;
            int SOUp = heldMode == HeldMode.Nothing ? Settings.ScrollOffUp : 0;

            if (prevCursorX != _cursorVisualX || prevCursorY != _cursorVisualY)
            {
                if ((_cursorVisualY + FontSize - _scroll.Y + FontSize * SODown) > Dimensions.H)
                {
                    _scroll.Y = (_cursorVisualY + FontSize) - Dimensions.H + FontSize * SODown;
                }
                if ((_cursorVisualY - _scroll.Y - FontSize * SOUp) < 0)
                {
                    _scroll.Y = _cursorVisualY - FontSize * SOUp;
                }

                if ((_cursorVisualX + FontSize - _scroll.X) > Dimensions.W)
                {
                    _scroll.X = (_cursorVisualX + FontSize) - Dimensions.W;
                }
                if ((_cursorVisualX - _scroll.X) < 0)
                {
                    _scroll.X = _cursorVisualX;
                }
            }
        }
    }
}
