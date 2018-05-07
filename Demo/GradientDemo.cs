using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class GradientDemo : MonoBehaviour 
{
    public KR.Graphics.Gradient gradient = new KR.Graphics.Gradient();

    [Range(0, 1)]
    public float evaluateTime;
    public Color evaluateColor;

    public void Evaluate()
    {
        evaluateColor = gradient.Evaluate(evaluateTime);
    }

    void Update()
    {
        Evaluate();
    }
}
