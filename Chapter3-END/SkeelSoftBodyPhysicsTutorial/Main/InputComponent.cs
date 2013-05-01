using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SkeelSoftBodyPhysicsTutorial.Main
{
    public interface IInputComponent { }

    public enum MouseButtonType { Left, Middle, Right }

    /// <summary>
    /// A component class that helps in detecting different keyboard and mouse input events
    /// </summary>
    public sealed class InputComponent : Microsoft.Xna.Framework.GameComponent, IInputComponent
    {
        private Game game;
        private KeyboardState prevKeyboardState;
        private KeyboardState keyboardState;

        private MouseState mouseState;
        private MouseState prevMouseState;
        private Vector2 mouseMoved;
        private Vector2 mousePosition;

        public Vector2 MouseMoved
        {
            get
            {
                mouseMoved.X = prevMouseState.X - mouseState.X;
                mouseMoved.Y = prevMouseState.Y - mouseState.Y;
                return mouseMoved;
            }
        }

        public Vector2 MousePosition
        {
            get 
            {
                mousePosition.X = mouseState.X;
                mousePosition.Y = mouseState.Y;
                return mousePosition;
            }
        }

        public InputComponent(Game game, bool isMouseVisible)
            : base(game)
        {
            this.game = game;
            game.IsMouseVisible = isMouseVisible;

            //add itself as a game service
            game.Services.AddService(typeof(IInputComponent), this);

            prevKeyboardState = Keyboard.GetState();
        }

        public bool IsKeyHeldDown(Keys key)
        {
            return (keyboardState.IsKeyDown(key) && prevKeyboardState.IsKeyDown(key));
        }

        public bool IsKeyJustDown(Keys key)
        {
            return (keyboardState.IsKeyDown(key) && prevKeyboardState.IsKeyUp(key));
        }

        public bool IsKeyJustUp(Keys key)
        {
            return (keyboardState.IsKeyUp(key) && prevKeyboardState.IsKeyDown(key));
        }

        public bool IsMouseHeldDown(MouseButtonType button)
        {
            switch (button)
            {
                case MouseButtonType.Left:
                    return mouseState.LeftButton == ButtonState.Pressed && prevMouseState.LeftButton == ButtonState.Pressed;
                case MouseButtonType.Middle:
                    return mouseState.MiddleButton == ButtonState.Pressed && prevMouseState.MiddleButton == ButtonState.Pressed;
                case MouseButtonType.Right:
                    return mouseState.RightButton == ButtonState.Pressed && prevMouseState.RightButton == ButtonState.Pressed;
                default:
                    throw new ArgumentException();
            }
        }

        public bool IsMouseJustDown(MouseButtonType button)
        {
            switch (button)
            {
                case MouseButtonType.Left:
                    return mouseState.LeftButton == ButtonState.Pressed && prevMouseState.LeftButton == ButtonState.Released;
                case MouseButtonType.Middle:
                    return mouseState.MiddleButton == ButtonState.Pressed && prevMouseState.MiddleButton == ButtonState.Released;
                case MouseButtonType.Right:
                    return mouseState.RightButton == ButtonState.Pressed && prevMouseState.RightButton == ButtonState.Released;
                default:
                    throw new ArgumentException();
            }
        }

        public bool IsMouseJustUp(MouseButtonType button)
        {
            switch (button)
            {
                case MouseButtonType.Left:
                    return mouseState.LeftButton == ButtonState.Released && prevMouseState.LeftButton == ButtonState.Pressed;
                case MouseButtonType.Middle:
                    return mouseState.MiddleButton == ButtonState.Released && prevMouseState.MiddleButton == ButtonState.Pressed;
                case MouseButtonType.Right:
                    return mouseState.RightButton == ButtonState.Released && prevMouseState.RightButton == ButtonState.Pressed;
                default:
                    throw new ArgumentException();
            }
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            //check for exit
            if (keyboardState.IsKeyDown(Keys.Escape))
            {
                game.Exit();
            }

            //update the state variables
            prevKeyboardState = keyboardState;
            keyboardState = Keyboard.GetState();
            prevMouseState = mouseState;
            mouseState = Mouse.GetState();

            base.Update(gameTime);
        }
    }
}