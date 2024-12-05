using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguelike
{
    public class AudioManager
    {

        public static Dictionary<string, AudioClip> s_clips = new Dictionary<string, AudioClip>(); //����һ���ֵ�洢������Ƶ
        public static Dictionary<string, List<AudioSource>> s_audioSources = new Dictionary<string, List<AudioSource>>(); //�������е�AudioSource

        private static int index = 0;

        public AudioManager()
        {
            s_clips = AudioRoot.s_clips;
            s_audioSources = AudioRoot.s_audioSources;
        }

        /// <summary>
        /// ������Ч
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

            //�������ص����ŵ���Ч
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



        //���ű�������
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

