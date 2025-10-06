﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameJam_KoganDev.Scripts
{
    internal class SoundManager
    {
        Microsoft.Xna.Framework.Audio.SoundEffect sound;
        SoundEffectInstance soundInstance;
        List<SoundEffectInstance> MusicList = new List<SoundEffectInstance>(); //List of all music
        List<SoundEffectInstance> EffectList = new List<SoundEffectInstance>();
        List<List<SoundEffectInstance>> masterList = new List<List<SoundEffectInstance>>();

        float MasterVolume;
        float EffectVolume;
        float MusicVolume;

        List<List<SoundEffectInstance>> MasterList
        {
            get
            {
                masterList.Clear();
                masterList.Add(MusicList);
                masterList.Add(EffectList);

                return masterList;
            }
        }
        ContentManager content;
        bool canLoop = false;
        public string currEffectName = "";


        public SoundManager(string effectName, ContentManager content, bool canLoop, float masterVolume, float effectVolume, float musicVolume)
        {
            sound = content.Load<Microsoft.Xna.Framework.Audio.SoundEffect>(@"Audio\" + effectName);
            this.content = content;
            soundInstance = sound.CreateInstance();
            soundInstance.IsLooped = canLoop;
            currEffectName = effectName;

            this.MasterVolume = masterVolume;
            EffectVolume = effectVolume;
            MusicVolume = musicVolume;
        }
        public SoundManager(ContentManager content, float masterVolume, float effectVolume, float musicVolume)
        {
            this.content = content;
            MasterVolume = masterVolume;
            EffectVolume = effectVolume;
            MusicVolume = musicVolume;
        }
        public void ChangeVolume(float master, float effect, float music)
        {
            MasterVolume = master;
            EffectVolume = effect;
            MusicVolume = music;

            for (int i = 0; i < MasterList.Count; i++)
            {
                for (int j = 0; j < MasterList[i].Count; j++)
                {
                    MasterList[i][j].Volume = master;
                }
            }

            for (int i = 0; i < EffectList.Count; i++)
            {

                if (master + (effect - 1) < 0)
                    EffectList[i].Volume = 0;
                else
                    EffectList[i].Volume = master + (effect - 1);
            }
            for (int i = 0; i < MusicList.Count; i++)
            {
                if (master + (music - 1) < 0)
                    MusicList[i].Volume = 0;
                else
                    MusicList[i].Volume = master + (music - 1);

            }
        }

        public void AddSound(string effectName, bool canLoop, float pitch = -1001100)
        {
            sound = content.Load<Microsoft.Xna.Framework.Audio.SoundEffect>(@"Audio\" + effectName);
            soundInstance = sound.CreateInstance();
            soundInstance.IsLooped = canLoop;
            currEffectName = effectName;


            if (pitch != -1001100)
            {
                soundInstance.Pitch = pitch;
            }

        }

        public void AddAndReplaceSound(string effectName)
        {
            sound = content.Load<Microsoft.Xna.Framework.Audio.SoundEffect>(@"Audio\" + effectName);
            //soundInstance.
        }

        public void ClearSounds()
        {

            StopCurrSounds();
            EffectList.Clear();
            MusicList.Clear();
            currEffectName = "";

        }

        public void StopCurrSounds()
        {
            for (int i = 0; i < MasterList.Count; i++)
            {
                for (int j = 0; j < MasterList[i].Count; j++)
                {
                    MasterList[i][j].Stop();
                }
            }
        }

        public void PlaySound()
        {

            if (currEffectName.Contains("Theme") || currEffectName.Contains("Level"))//Music list
            {
                MusicList.Add(soundInstance);
                MusicList[MusicList.Count - 1].Play();
            }
            else if (currEffectName.Contains("SoundEffects"))
            {
                EffectList.Add(soundInstance);
                EffectList[EffectList.Count - 1].Play();
            }

            ChangeVolume(MasterVolume, EffectVolume, MusicVolume);
        }

        public void Update(GameTime gameTime)
        {
            for (int i = 0; i < MasterList.Count; i++)
            {
                for (int j = MasterList[i].Count - 1; j >= 0; j--)
                {
                    if (MasterList[i][j].IsLooped)
                    {

                    }
                    else
                    {
                        if (MasterList[i][j].State == SoundState.Stopped)
                        {
                            MasterList[i].RemoveAt(j);
                        }
                    }
                }
            }
        }
    }
}
