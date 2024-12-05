using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguelike
{
    public class AudioManager
    {

        public static Dictionary<string, AudioClip> s_clips = new Dictionary<string, AudioClip>(); //定义一个字典存储所有音频
        public static Dictionary<string, List<AudioSource>> s_audioSources = new Dictionary<string, List<AudioSource>>(); //定义所有的AudioSource

        private static int index = 0;

        public AudioManager()
        {
            s_clips = AudioRoot.s_clips;
            s_audioSources = AudioRoot.s_audioSources;
        }

        /// <summary>
        /// 播放音效
        /// </summary>
        /// <param name="audioChannelName"></param>
        /// <param name="audioClipName"></param>
        /// <param name="isOverlap"></param>
        public static void PlaySound(string audioClipName,bool isOverlap)
        {
            if (audioClipName == null)
            {
                return;
            }

            AudioClip clip = s_clips[audioClipName];
            List<AudioSource> curAudioSources = s_audioSources["Sound"];

            //不允许重叠播放的音效
            if (isOverlap == false)
            {
                foreach(var audioSource in curAudioSources)
                {
                    if (clip.Equals(audioSource.clip) == true)
                    {
                        return;
                    }
                }
            }

            curAudioSources[index].clip = clip;
            curAudioSources[index].Play();
            index++;
            if (index == curAudioSources.Count)
            {
                index = 0;
            }
        }



        //播放背景音乐
        public static void PlayBackgroundMusic(string audioClipName)
        {
            //foreach(var item in s_clips)
            //{
            //    Debug.Log(item.Key);
            //}
            List<AudioSource> curAudioSources = s_audioSources["Background"];

            if (audioClipName == null)
            {
                curAudioSources[0].clip = null;
            }
            else
            {
                AudioClip clip = s_clips[audioClipName];

                curAudioSources[0].clip = clip;
                curAudioSources[0].Play();
            }

        }
    }
}

