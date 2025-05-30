﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using System.IO;


namespace Lethal_Organization.UI
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

        EnemyHealthBarPlaceHolder,
        EnemyHealthBar,

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

        private Texture2D _UISPriteSheet;
        public Rectangle this[Type type]
        {
            get
            {
                if(_textureMap.ContainsKey(type))
                {
                    return _textureMap[type];

                }
                return Rectangle.Empty;
            }
        }

        public UIManager(AudioManager audioManager, GameManager gameManager, Player player, Boss boss,
            SpriteFont font,
            Texture2D spriteSheet, Texture2D openTheme, Texture2D startGame, Texture2D credit, Texture2D creditMenu, Texture2D tutorial, Texture2D tutorialMenu,
            Texture2D exit,
            Texture2D youDie,
            Texture2D endScreen, Texture2D again,
            Texture2D exitButton,
            int screenWidth, int screenHeight,
            string textureMapFile, Action<GameManager.GameState> changeState, Action exitGame, int scale = 5)
        {

            _gameManager = gameManager;

            boss.OnBossFightEvent += ShowBossHub;   

            _gameManager.OnStateChange += OnStateChange;

            _UIs = new List<UI>();

            _textureMap = new Dictionary<Type, Rectangle>();

            LoadContent(spriteSheet, textureMapFile);

            _UIs.Add(new Menu(audioManager,changeState, openTheme, startGame, credit, creditMenu,tutorial, tutorialMenu,exit,exitButton,screenWidth, screenHeight,
                ShowMenu<Credits>, ShowMenu<Tutorial>, exitGame));

            _UIs.Add(new PlayerHUB(player, spriteSheet, _textureMap[Type.HealthBar], _textureMap[Type.HealthBarPlaceholder], _textureMap[Type.PlayerIcon]));

            _UIs.Add(new EndScreen(audioManager,changeState, endScreen, again, screenWidth, screenHeight));

            _UIs.Add(new Setting(audioManager,changeState, spriteSheet, 
                _textureMap[Type.HomeIcon], _textureMap[Type.PauseIcon], _textureMap[Type.SmallButton], _textureMap[Type.SmallButtonPress], _textureMap[Type.SmallButtonHover],
                screenWidth, screenHeight));

            _UIs.Add(new BossHUB(boss, font, spriteSheet, _textureMap[Type.HealthBar], screenWidth, screenHeight));

            _UIs.Add(new YouDie(youDie, font, screenWidth, screenHeight));

            _UIs.Add(new Credits(audioManager,creditMenu, exitButton, screenWidth, screenHeight));

            _UIs.Add(new Tutorial(audioManager,tutorialMenu, exitButton, screenWidth, screenHeight));    
        }

       /// <summary>
       /// Show, and hide each UI element when state changes
       /// </summary>
       /// <param name="state"></param>
        private void OnStateChange(GameManager.GameState state)
        {
            switch (state)
            {
                case GameManager.GameState.Menu:
                    ShowMenu<Menu>();
                    HideMenu<PlayerHUB>();
                    HideMenu<EndScreen>();
                    HideMenu<Setting>();
                    HideMenu<BossHUB>();    
                    break;

                case GameManager.GameState.Game:
                    HideMenu<Menu>();
                    ShowMenu<PlayerHUB>();
                    ShowMenu<Setting>();
                    HideMenu<YouDie>();
                    HideMenu<EndScreen>();
                    break;

                case GameManager.GameState.GameOver:
                    HideMenu<PlayerHUB>();
                    HideMenu<Setting>();
                    HideMenu<BossHUB>();
                    ShowMenu<EndScreen>();
                    break;

                case GameManager.GameState.Die:
                    ShowMenu<YouDie>();
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

        /// <summary>
        /// Show UI element
        /// </summary>
        /// <typeparam name="T">type of UI</typeparam>
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

        /// <summary>
        /// Hide UI element
        /// </summary>
        /// <typeparam name="T">type of UI</typeparam>
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

        private void ShowBossHub()
        {
            ShowMenu<BossHUB>();
        }


        public void LoadContent(Texture2D spriteSheet, string textureMapFile)
        {
            _UISPriteSheet = spriteSheet;
            LoadTextureMap(textureMapFile);
        }

        /// <summary>
        /// Load texture in UI Sprite sheet, map rectangle (source image) to each UI type
        /// </summary>
        /// <param name="textureMapFile">UI sprite sheet</param>
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
