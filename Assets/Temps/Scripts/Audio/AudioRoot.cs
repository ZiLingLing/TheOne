using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguelike
{
    public class AudioRoot : MonoBehaviour
    {
        public static Dictionary<string, AudioClip> s_clips = new Dictionary<string, AudioClip>(); //定义一个字典存储所有音频
        public static Dictionary<string, List<AudioSource>> s_audioSources = new Dictionary<string, List<AudioSource>>(); //定义所有的AudioSource

        private static List<AudioSource> s_bgm = new List<AudioSource>();
        private static List<AudioSource> s_sound = new List<AudioSource>();

        private void Awake()
        {
            var clips = Resources.LoadAll<AudioClip>("Audio");
            foreach (AudioClip clip in clips)
            {
                if (s_clips.ContainsKey(clip.name) == false)
                {
                    s_clips.Add(clip.name, clip);
                }
            }
            Debug.Log("音效字典中的音效数目：" + s_clips.Count);

            for(int i = 0; i < 2; i++)
            {
                AudioSource audioSource = this.gameObject.AddComponent<AudioSource>();
                audioSource.loop = true;
                audioSource.playOnAwake = false;
                s_bgm.Add(audioSource);
            }
            s_audioSources.Add("Background", s_bgm);
            Debug.Log("BGM通道数量：" + s_audioSources["Background"].Count);

            for (int i = 0; i < 4; i++)
            {
                AudioSource audioSource = this.gameObject.AddComponent<AudioSource>();
                audioSource.loop = false;
                audioSource.playOnAwake = false;
                s_sound.Add(audioSource);
            }
            s_audioSources.Add("Sound", s_sound);
            Debug.Log("触发音通道数量：" + s_audioSources["Sound"].Count);
        }
    }

}
