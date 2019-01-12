using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SadConsole;

namespace SadRogue.UI
{
    // A scrollable window that displays messages using a FIFO (first-in-first-out) queue data structure.
    public class MessageLogWindow : Window
    {
        private static readonly int _maxLines = 100;

        private SadConsole.Controls.ScrollBar _messageScrollBar;

        private int _scrollBarCurrentPosition;

        // Account for thickness of window border to prevent UI element spillover.
        private int _windowBorderThickness = 2;

        // A queue works using a FIFO structure, where the first line added is the first line removed when maxLines is exceeded.
        private readonly Queue<string> _lines;

        private SadConsole.Console _messageConsole;

        public MessageLogWindow(int width, int height, string title) : base(width, height)
        {
            _lines = new Queue<string>();
            Dragable = false;
            //Theme.FillStyle.Background = Color.Black;

            Title = title.Align(HorizontalAlignment.Center, title.Length + 2);

            _messageConsole = new SadConsole.Console(width - _windowBorderThickness, _maxLines)
            {
                Position = new Point(1, 1),
                ViewPort = new Rectangle(0, 0, width - 1, height - _windowBorderThickness)
            };

            _messageScrollBar = SadConsole.Controls.ScrollBar.Create(Orientation.Vertical, height - _windowBorderThickness);
            _messageScrollBar.Position = new Point(_messageConsole.Width + 1, _messageConsole.Position.X);
            _messageScrollBar.IsEnabled = false;
            _messageScrollBar.ValueChanged += MessageScrollBar_ValueChanged;
            Add(_messageScrollBar);

            UseMouse = true;

            Children.Add(_messageConsole);
        }

        // Controls the position of the message log viewport based on the scrollbar position using an event handler.
        void MessageScrollBar_ValueChanged(object sender, EventArgs e)
        {
            _messageConsole.ViewPort = new Rectangle(0, _messageScrollBar.Value + _windowBorderThickness,
                _messageConsole.Width, _messageConsole.ViewPort.Height);
        }

        public void Add(string message)
        {
            _lines.Enqueue(message);
            // When exceeding the max number of lines remove the oldest one.
            if (_lines.Count > _maxLines)
            {
                _lines.Dequeue();
            }

            _messageConsole.Cursor.Position = new Point(1, _lines.Count);
            _messageConsole.Cursor.PrintAppearance.Background = DefaultBackground;
            _messageConsole.Cursor.Print("\n" + message);
        }

        public override void Draw(TimeSpan drawTime)
        {
            base.Draw(drawTime);
        }

        public override void Update(TimeSpan time)
        {
            base.Update(time);

            // Ensure that the scrollbar tracks the current position of the Message Console.
            if (_messageConsole.TimesShiftedUp != 0 | _messageConsole.Cursor.Position.Y >=
                _messageConsole.ViewPort.Height + _scrollBarCurrentPosition)
            {
                // Enable the scrollbar once the Message Log has filled up with enough text to warrant scrolling.
                _messageScrollBar.IsEnabled = true;

                // Make sure we've never scrolled the entire size of the buffer.
                if (_scrollBarCurrentPosition < _messageConsole.Height - _messageConsole.ViewPort.Height)
                    // Record how much we've scrolled to enable how far back the bar can see.
                    _scrollBarCurrentPosition += _messageConsole.TimesShiftedUp != 0 ? _messageConsole.TimesShiftedUp : 1;

                _messageScrollBar.Maximum = (_messageConsole.Height + _scrollBarCurrentPosition) - _messageConsole.Height - _windowBorderThickness;

                // This will follow the cursor since we move the render area in the event.
                _messageScrollBar.Value = _scrollBarCurrentPosition;

                // Reset the shift amount.
                _messageConsole.TimesShiftedUp = 0;
            }
        }
    }
}