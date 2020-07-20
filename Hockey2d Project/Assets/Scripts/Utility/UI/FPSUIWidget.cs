using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FPSUIWidget : MonoBehaviour
{
    [SerializeField]
    private Text text;
    
    [SerializeField]
    private float frequency = 0.5F; // Tempo de Refresh dos FPS
	
    [SerializeField]
    private int numDecimal = 1; // Quantos decimais queres mostrar ?
 
    [SerializeField]
    private float yellowFps = 50f;
    
    [SerializeField]
    private float greenFps = 60f;

    [SerializeField]
    private Color redColor = Color.red;

    [SerializeField]
    private Color yellowColor = Color.yellow;

    [SerializeField]
    private Color greenColor = Color.green;
 
	private float accum = 0f; // FPS acumulados durante um x Tempo.
	private int frames = 0; // Frames mostrados durante um x Tempo
 
	private void Awake()
	{
	    this.StartCoroutine(this.UpdateCoroutine());
	}
 
	private void Update()
	{
	    this.accum += Time.timeScale/Time.deltaTime;
        this.frames++;
	}
 
	private IEnumerator UpdateCoroutine()
	{
		while (true)
		{
			yield return new WaitForSeconds(this.frequency);
            
		    var fps = this.accum / this.frames;

		    this.text.text = fps.ToString("f" + Mathf.Clamp(numDecimal, 0, 10));
            this.text.color = (fps >= this.greenFps) ? this.greenColor : (fps >= this.yellowFps) ? this.yellowColor : this.redColor;
 
	        this.accum = 0f;
	        this.frames = 0;
		}
	}
}
