using UnityEngine;
using System.Collections;

public class ColorList
{
    public bool isAvailable = true;
    public Color _color;

    public ColorList(bool avail, Color col)
    {
        isAvailable = avail;
        _color = col;

    }
}

public class ColorManager : MonoBehaviour {
    public ColorList[] _colorlist = new ColorList[10];
	// Use this for initialization
	void Start ()
    {
        _colorlist[0] = new ColorList(false, Color.red);
        _colorlist[1] = new ColorList(false, Color.blue);
        _colorlist[2] = new ColorList(false, Color.green);
        _colorlist[3] = new ColorList(false, Color.magenta);
        _colorlist[4] = new ColorList(true, Color.yellow); //need to change due to it being too close to the crystal's colour
        _colorlist[5] = new ColorList(true, Color.cyan);
        _colorlist[6] = new ColorList(true, new Color(0.29f,0.35f,0.67f,1f));
        _colorlist[7] = new ColorList(true, new Color(0.95f,0.62f,0f,1f));
        _colorlist[8] = new ColorList(true, new Color(0f, 0.95f, 0.7f,1f));
        _colorlist[9] = new ColorList(true, new Color(0.65f,0f,0.73f,1f));

    }
	
	// Update is called once per frame
	void Update ()
    {
	
	}
}
