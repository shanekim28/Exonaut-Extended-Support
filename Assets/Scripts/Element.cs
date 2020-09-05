using UnityEngine;

public class Element
{
	public Vector2 mPos;

	public float mWidth;

	public float mHeight;

	public Texture2D mElementTexture;

	public int mNumFrames;

	public int mCurrentFrame;

	public int mDir;

	public Element()
	{
	}

	public Element(Vector2 pos, float width, float height, string textureName)
	{
		mElementTexture = (Resources.Load(textureName) as Texture2D);
		mPos = pos;
		mWidth = width;
		mHeight = height;
		mNumFrames = 0;
		mCurrentFrame = 0;
		mDir = 0;
	}
}
