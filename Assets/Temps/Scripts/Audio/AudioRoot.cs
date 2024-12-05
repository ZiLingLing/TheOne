using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguelike
{
    public class AudioRoot : MonoBehaviour
    {
        public static Dictionary<string, AudioClip> s_clips = new Dictionary<string, AudioClip>(); //����һ���ֵ�洢������Ƶ
        public static Dictionary<string, List<AudioSource>> s_audioSources = new Dictionary<string, List<AudioSource>>(); //�������е�AudioSource

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
            Debug.Log("��Ч�ֵ��е���Ч��Ŀ��" + s_clips.Count);

            for(int i = 0; i < 2; i++)
            {
                AudioSource audioSource = this.gameObject.AddComponent<AudioSource>();
                audioSource.loop = true;
                audioSource.playOnAwake = false;
                s_bgm.Add(audioSource);
            }
            s_audioSources.Add("Background", s_bgm);
            Debug.Log("BGMͨ��������" + s_audioSources["Background"].Count);

            for (int i = 0; i < 4; i++)
            {
                AudioSource audioSource = this.gameObject.AddComponent<AudioSource>();
                audioSource.loop = false;
                audioSource.playOnAwake = false;
                s_sound.Add(audioSource);
            }
            s_audioSources.Add("Sound", s_sound);
            Debug.Log("������ͨ��������" + s_audioSources["Sound"].Count);
        }
    }

}
