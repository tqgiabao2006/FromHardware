using Lethal_Organization;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;
using static Lethal_Organization.Player;
using System.Diagnostics;

internal class AudioManager
{
    public enum SongID
    {
        BossSong,
        EndSong,
        MainSong,
        OpenSong
    }

    public enum SFXID
    {
        BossJump,
        ButtonClick,
        IceCrack,
        SpikeSound,
        PlayerJump,
        PlayerShoot,
        Swoosh,
        DieSFX,
        GetHit
    }


    private readonly Dictionary<SongID, Song> _songs;

    private readonly Dictionary<SFXID, SoundEffect> _sfx;

    public AudioManager(
        GameManager gameManager,
        Song bossSong,
        Song endSong,
        Song mainSong,
        Song openSong,
        SoundEffect bossJump,
        SoundEffect buttonClick,
        SoundEffect iceCrack,
        SoundEffect spikeSound,
        SoundEffect playerJump,
        SoundEffect playerShoot,
        SoundEffect swoosh,
        SoundEffect dieSFX,
        SoundEffect getHit)
    {
        _songs = new Dictionary<SongID, Song>
        {
            { SongID.BossSong, bossSong },
            { SongID.EndSong, endSong },
            { SongID.MainSong, mainSong },
            { SongID.OpenSong, openSong }
        };

        _sfx = new Dictionary<SFXID, SoundEffect>
        {
            { SFXID.BossJump, bossJump },
            { SFXID.ButtonClick, buttonClick },
            { SFXID.IceCrack, iceCrack },
            { SFXID.SpikeSound, spikeSound },
            { SFXID.PlayerJump, playerJump },
            { SFXID.PlayerShoot, playerShoot },
            { SFXID.Swoosh, swoosh },
            { SFXID.DieSFX, dieSFX },
            { SFXID.GetHit, getHit },
        };

        MediaPlayer.Volume = 0.5f;

        gameManager.OnStateChange += OnStateChange;
    }

    /// <summary>
    /// Play theme song in each game state
    /// </summary>
    /// <param name="state"></param>
    public void OnStateChange(GameManager.GameState state)
    {
        switch (state)
        {
            case GameManager.GameState.Menu:
                MediaPlayer.Play(_songs[SongID.OpenSong]);
                break;


            case GameManager.GameState.Game:
                MediaPlayer.Play(_songs[SongID.MainSong]);
                break;

            case GameManager.GameState.GameOver:
                MediaPlayer.Play(_songs[SongID.EndSong]);
                break;

            case GameManager.GameState.Die:
                MediaPlayer.Play(_songs[SongID.EndSong]);
                break;

            case GameManager.GameState.Pause:
                
                break;
            case GameManager.GameState.Debug:
               
                break;
        }
    }

    /// <summary>
    /// Play song
    /// </summary>
    /// <param name="id">song id</param>
    /// <param name="loop">is loop</param>
    public void PlaySong(SongID id, bool loop = true)
    {
        if (_songs.TryGetValue(id, out var song))
        {
            MediaPlayer.IsRepeating = loop;
            MediaPlayer.Play(song);
        }
    }

    /// <summary>
    /// Play sfx
    /// </summary>
    /// <param name="id">SFX type</param>
    /// <param name="volume">volume</param>
    /// <param name="pitch">pitch</param>
    /// <param name="pan">pan</param>
    public void PlaySFX(SFXID id, float volume = 1f, float pitch = 0f, float pan = 0f)
    {
        if (_sfx.TryGetValue(id, out var effect))
        {
            effect.Play(volume, pitch, pan);
        }
    }
}
