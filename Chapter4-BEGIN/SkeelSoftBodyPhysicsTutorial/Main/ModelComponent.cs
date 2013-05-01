using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace SkeelSoftBodyPhysicsTutorial.Main
{
    public interface IModelComponent { }

    /// <summary>
    /// A component class that helps in loading and displaying of models
    /// </summary>
    public sealed class ModelComponent : Microsoft.Xna.Framework.DrawableGameComponent, IModelComponent
    {
        private Game game;
        private List<GameModel> models = new List<GameModel>();

        public ModelComponent(Game game)
            : base(game)
        {
            this.game = game;

            //add itself as a game service
            game.Services.AddService(typeof(IModelComponent), this);
        }

        public GameModel LoadGameModel(string modelFile)
        {
            GameModel model = new GameModel(game, modelFile);
            model.Initialize();
            model.LoadContent();
            models.Add(model);
            return model;
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (GameModel model in models)
            {
                model.Draw(gameTime);
            }
            base.Draw(gameTime);
        }
    }
}