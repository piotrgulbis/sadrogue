using System;
using Microsoft.Xna.Framework;
using SadConsole;
using SadRogue.Actions;
using SadRogue.Entities;
using SadRogue.UI;
using SharpDX.Direct3D11;

namespace SadRogue
{
    class GameLoop
    {
        //public static int GameWidth = 240;
        //public static int GameHeight = 75;
        public static int GameWidth = 120;
        public static int GameHeight = 50;


        static int oldWindowPixelsWidth;
        static int oldWindowPixelsHeight;

        public static EntityManager EntityManager;
        public static UIManager UIManager;
        public static ActionManager ActionManager;

        public static World World;

        static void Main()
        {
            SadConsole.Settings.ResizeMode = SadConsole.Settings.WindowResizeOptions.Center;

            // Setup the engine and create the main window.
            SadConsole.Game.Create("Fonts/IBM.font", GameWidth, GameHeight);

            // Hook the start event so we can add consoles to the system.
            SadConsole.Game.OnInitialize = Init;

            // Hook the update event that happens each frame so we can trap keys and respond.
            SadConsole.Game.OnUpdate = Update;

            // Start the game.
            SadConsole.Game.Instance.Run();

            //
            // Code here will not run until the game window closes.
            //

            SadConsole.Game.Instance.Dispose();
        }

        private static void Update(GameTime time)
        {
            // Called each logic update.
        }

        private static void Init()
        {
            //SadConsole.Settings.ToggleFullScreen();
            SadConsole.Themes.Colors.ControlHostBack = Color.Black;

            EntityManager = new EntityManager();

            UIManager = new UIManager();

            ActionManager = new ActionManager();

            World = new World();
            World.Init();

            UIManager.Init();

            SadConsole.Game.Instance.Window.ClientSizeChanged += Window_ClientSizeChanged;
        }

        private static void Window_ClientSizeChanged(object sender, EventArgs e)
        {
            // Get the size of the window.
            int windowPixelsWidth = SadConsole.Game.Instance.Window.ClientBounds.Width;
            int windowPixelsHeight = SadConsole.Game.Instance.Window.ClientBounds.Height;

            // If this is getting called because of the ApplyChanges, exit.
            if (windowPixelsWidth == oldWindowPixelsWidth && windowPixelsHeight == oldWindowPixelsHeight)
                return;

            // Store for later
            oldWindowPixelsWidth = windowPixelsWidth;
            oldWindowPixelsHeight = windowPixelsHeight;

            // Get the exact pixels we can fit in that window based on a font.
            int fontPixelsWidth = (windowPixelsWidth / SadConsole.Global.FontDefault.Size.X) * SadConsole.Global.FontDefault.Size.X;
            int fontPixelsHeight = (windowPixelsHeight / SadConsole.Global.FontDefault.Size.Y) * SadConsole.Global.FontDefault.Size.Y;

            // Resize the monogame rendering to match
            SadConsole.Global.GraphicsDeviceManager.PreferredBackBufferWidth = windowPixelsWidth;
            SadConsole.Global.GraphicsDeviceManager.PreferredBackBufferHeight = windowPixelsHeight;
            SadConsole.Global.GraphicsDeviceManager.ApplyChanges();

            // Tell sadconsole how much to render to the screen.
            Global.RenderWidth = fontPixelsWidth;
            Global.RenderHeight = fontPixelsHeight;
            Global.ResetRendering();

            // Get the total cells you can fit
            int totalCellsX = fontPixelsWidth / SadConsole.Global.FontDefault.Size.X;
            int totalCellsY = fontPixelsHeight / SadConsole.Global.FontDefault.Size.Y;

            // Resize your console based on totalCellsX/Y
            //GameWidth = totalCellsX;
            //GameHeight = totalCellsY;
        }
    }
}