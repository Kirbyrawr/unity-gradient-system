using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace KR.Graphics
{
    [System.Serializable]
    public class Gradient
    {
        [System.Serializable]
        public class GradientKey
        {
            public GradientKey(Color color, float time)
            {
                this.color = color;
                this.time = time;
            }

            public Color color;
            [Range(0, 1)]
            public float time;
        }

        public List<GradientKey> keys = new List<GradientKey>() { new GradientKey(Color.white, 0) };

        #if UNITY_EDITOR
        public Texture2D preview;
        #endif

        public Color Evaluate(float time)
        {   
            GradientKey lastKey = keys[keys.Count - 1]; 

            //If the time is over the time of the last key we return the last key color.
            if (time > lastKey.time)
            {
                return lastKey.color;
            }

            for (int i = 0; i < keys.Count - 1; i++)
            {
                GradientKey actualKey = keys[i];
                GradientKey nextKey = keys[i + 1];

                if (time >= actualKey.time && time <= keys[i + 1].time)
                {    
                    return Color.Lerp(actualKey.color, nextKey.color, (time - actualKey.time) / (nextKey.time - actualKey.time));
                }
            }

            return keys[0].color;
        }
        
        public void SortKeys()
        {
            keys.Sort((p1, p2) => (p1.time.CompareTo(p2.time)));
        }
    }
}



