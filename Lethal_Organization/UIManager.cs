using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using System.IO;


namespace Lethal_Organization
{
    public enum Type
    {
        NewText,
        LoadText,
        OptionsText,
        ExitText,

        AmmoBar,
        HealthBar,

        HealthBarPlaceholder,
        BarPlaceholder,
        MedButtonPlaceholder,
       
        MedButton,
        TextBox,

        SmallButton,
        SmallButtonPress,
        SmallButtonHover,
        
        HomeIcon,
       
        PauseIcon,
        
        SaveText,
        
        BackText,
       
        CreditsText,
       
        SoundText,
       
        PlayerIcon,
       
        LargeButton,
       
        LargeButtonHover,
      
        LargeButtonPress,
       
        PauseMenu
    }

    internal class UIManager
    {
        private List<UI> _UIs;

        private GameManager _gameManager;

        private Dictionary<Type, Rectangle> _textureMap;

        private List<(Rectangle textPos, Rectangle buttonPos)> _menuItems;

        private Type[] _types;

        private Texture2D _UISPriteSheet;


        public UIManager(GameManager gameManager,
            Texture2D spriteSheet, Texture2D openTheme, Texture2D loadGame, Texture2D startGame, Texture2D exit, Texture2D option,
            int screenWidth, int screenHeight,
            string textureMapFile, Action<GameManager.GameState> changeState, int scale = 5)
        {

            _gameManager = gameManager;

            _gameManager.OnStateChange += OnStateChange;

            _UIs = new List<UI>();

           
            _textureMap = new Dictionary<Type, Rectangle>();

            LoadContent(spriteSheet, textureMapFile);

            _UIs.Add(new Menu(changeState, openTheme, loadGame, startGame, exit, option, screenWidth, screenHeight));

            _UIs.Add(new HeatlhBar(spriteSheet, _textureMap[Type.HealthBar], _textureMap[Type.HealthBarPlaceholder]));
        }

       
        private void OnStateChange(GameManager.GameState state)
        {
            switch (state)
            {
                case GameManager.GameState.Menu:
                    ShowMenu<Menu>();
                    break;

                case GameManager.GameState.Game:
                    HideMenu<Menu>();
                    ShowMenu<HeatlhBar>();
                    break;

                case GameManager.GameState.GameOver:

                   
                    break;
                case GameManager.GameState.Pause:
                 

                    break;
                case GameManager.GameState.Debug:
                    
                    break;
            }
        }

        public void Update(GameTime gameTime)
        {
            foreach (UI ui in _UIs)
            {
                if (ui.Visible)
                {
                    ui.Update(gameTime);

                }
            }
        }
        public void Draw(SpriteBatch sb)
        {
            foreach (UI ui in _UIs)
            {
                if(ui.Visible)
                {
                   ui.Draw(sb);
                }
            }
        }

        private void ShowMenu<T>() where T : UI
        {
            for (int i = 0; i < _UIs.Count; i++)
            {
                UI menu = _UIs[i];
                if (menu is T)
                {
                    menu.Show();
                }

            }
        }

        private void HideMenu<T>() where T : UI
        {
            for (int i = 0; i < _UIs.Count; i++)
            {
                UI menu = _UIs[i];
                if (menu is T)
                {
                    menu.Hide();
                }

            }
        }


        public void LoadContent(Texture2D spriteSheet, string textureMapFile)
        {
            _UISPriteSheet = spriteSheet;
            LoadTextureMap(textureMapFile);
        }

        private void LoadTextureMap(string textureMapFile)
        {
            StreamReader reader = null;
            try
            {
                reader = new StreamReader(textureMapFile);
                string line = "";

                int currentWidth = 0;
                int currentHeight = 0;

                while ((line = reader.ReadLine()) != null)
                {
                    //Skip the line start by '/' or '=' because it is description, not data
                    if (line[0] == '/' || line[0] == '=')
                    {
                        continue;
                    }

                    string[] data = line.Split(',');

                    //If it is size data
                    if (data.Length == 2)
                    {
                        currentWidth = int.Parse(data[0]);
                        currentHeight = int.Parse(data[1]);
                    }
                    else if (data.Length == 3)
                    {
                        _textureMap.Add(
                            (Type)Enum.Parse(typeof(Type), data[0]), //Tile Name
                            new Rectangle( //Source Rect
                                int.Parse(data[2]) * 16, //X-pivot = ColIndex * 16 (16 is standard pixel scale, no space between tile)
                                int.Parse(data[1]) * 16, //Y-pivot = RowIndex * 16
                                currentWidth,
                                currentHeight)
                        );
                    }
                }
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine("ERROR: Can not find the text file!");
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }
        }

    }
}
