using System;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Themes;
using SadRogue.Entities;

namespace SadRogue.UI
{
    // Creates, Holds, Destroys all consoles used in the game and makes consoles easily addressable from a central place.
    public class UIManager : ConsoleContainer
    {
        public SadConsole.Console MapConsole;
        public Window MapWindow;
        public MessageLogWindow MessageLog;

        public UIManager()
        {
            // Must be set to true or it will not call each child's Draw method.
            IsVisible = true;
            IsFocused = true;

            // The UIManager becomes the only screen that SadConsole processes.
            Parent = Global.CurrentScreen;
        }

        // Creates all child consoles to be managed.
        // Make sure they are added as children so that they are updated and drawn.
        public void CreateConsoles()
        {
            MapConsole = new SadConsole.Console(GameLoop.World.CurrentMap.Width, GameLoop.World.CurrentMap.Height,
                Global.FontDefault, new Rectangle(0, 0, GameLoop.GameWidth, GameLoop.GameHeight),
                GameLoop.World.CurrentMap.Tiles);

            // MapConsole.DefaultBackground = Color.Black;

            // Add the Entity Manager to the MapConsole or we won't be able to keep track of our actors.
            MapConsole.Children.Add(GameLoop.EntityManager);
        }

        // Creates a window that encloses a Map Console of a specified height and width and displays a centered window title.
        // Make sure it is added as a child of the UIManager so it is updated and drawn.
        public void CreateMapWindow(int width, int height, string title)
        {
            MapWindow = new Window(width, height)
            {
                Dragable = false
            };

            MapWindow.Theme.FillStyle.Background = new Color(30, 30, 30);

            // Make the console short enough to show the window title and borders, and position it away from borders.
            var mapConsoleWidth = width - 2;
            var mapConsoleHeight = height - 2;

            // Resize the MapConsole's viewport to fit inside of the window's borders.
            MapConsole.ViewPort = new Rectangle(0, 0, mapConsoleWidth, mapConsoleHeight);

            // Reposition the MapConsole so that it does not overlap with the left/top window edges.
            MapConsole.Position = new Point(1, 1);

            // Add a CloseWindow button
            var closeButton = new SadConsole.Controls.Button(3)
            {
                Position = new Point(MapWindow.Width - 4, 0),
                Text = "[X]"
            };

            MapWindow.Add(closeButton);

            // Add a window title.
            MapWindow.Title = title.Align(HorizontalAlignment.Center, title.Length + 2);

            // Add the map viewer to the window.
            MapWindow.Children.Add(MapConsole);

            // The MapWindow now becomes a child console of the UIManager.
            Children.Add(MapWindow);

            // Without this, the window will never be visible onscreen.
            MapWindow.Show();
        }

        public void CenterOnActor(Actor actor)
        {
            MapConsole.CenterViewPortOnPoint(actor.Position);
        }

        public override void Update(TimeSpan timeElapsed)
        {
            CheckKeyboard();
            base.Update(timeElapsed);
        }

        // Scans SadConsole's Global Keyboard State and triggers behaviour based on the button pressed.
        private void CheckKeyboard()
        {
            // SadConsole.Global.KeyboardState.InitialRepeatDelay = 0.1F;
            // SadConsole.Global.KeyboardState.RepeatDelay = 0.1F;

            // As an example, we'll use the F5 key to make the game fullscreen.
            if (Global.KeyboardState.IsKeyReleased(Microsoft.Xna.Framework.Input.Keys.F5))
            {
                Settings.ToggleFullScreen();
            }
            else if (Global.KeyboardState.IsKeyReleased(Microsoft.Xna.Framework.Input.Keys.Escape))
            {
                SadConsole.Game.Instance.Exit();
            }

            // Keyboard movement for player.
            if (Global.KeyboardState.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Up))
            {
                GameLoop.ActionManager.MoveActorBy(GameLoop.World.Player, new Point(0, -1));
                CenterOnActor(GameLoop.World.Player);
            }

            if (Global.KeyboardState.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Down))
            {
                GameLoop.ActionManager.MoveActorBy(GameLoop.World.Player, new Point(0, 1));
                CenterOnActor(GameLoop.World.Player);
            }

            if (Global.KeyboardState.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Left))
            {
                GameLoop.ActionManager.MoveActorBy(GameLoop.World.Player, new Point(-1, 0));
                CenterOnActor(GameLoop.World.Player);
            }

            if (Global.KeyboardState.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Right))
            {
                GameLoop.ActionManager.MoveActorBy(GameLoop.World.Player, new Point(1, 0));
                CenterOnActor(GameLoop.World.Player);
            }

            // Undo last command: Z
            if (Global.KeyboardState.IsKeyReleased(Microsoft.Xna.Framework.Input.Keys.Z))
            {
                GameLoop.ActionManager.UndoMoveActorBy();
                CenterOnActor(GameLoop.World.Player);
            }

            //Redo last command: X
            if (Global.KeyboardState.IsKeyReleased(Microsoft.Xna.Framework.Input.Keys.X))
            {
                GameLoop.ActionManager.RedoMoveActorBy();
                CenterOnActor(GameLoop.World.Player);
            }
        }

        // Initializes all windows and consoles.
        public void Init()
        {
            CreateConsoles();

            MessageLog = new MessageLogWindow(GameLoop.GameWidth, 6, "Message Log");
            Children.Add(MessageLog);
            MessageLog.Show();
            MessageLog.Position = new Point(0, GameLoop.GameHeight - MessageLog.Height);

            MessageLog.Add("Welcome to the jungle!");
            var mapSizeMessage = new StringBuilder();
            mapSizeMessage.AppendFormat("Game Size: {0} x {1}", GameLoop.GameWidth, GameLoop.GameHeight);
            MessageLog.Add(mapSizeMessage.ToString());

            CreateMapWindow(GameLoop.GameWidth, GameLoop.GameHeight - MessageLog.Height, "Game Map");

            UseMouse = true;

            // Start the game with the view centered on the player.
            //CenterOnActor(GameLoop.World.Player);
        }
    }
}