using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguelike
{
    public class WaitSceneLoad : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            Transform player = EventManager.TriggerEvent<Transform>("GetPlayerTransform");
            player.transform.position = new Vector3(219.47f, 32f, 220.64f);
        }

        // Update is called once per frame
        void Update()
        {
            Transform player = EventManager.TriggerEvent<Transform>("GetPlayerTransform");
            player.transform.position = new Vector3(219.47f, 32f, 220.64f);
        }
    }

}
