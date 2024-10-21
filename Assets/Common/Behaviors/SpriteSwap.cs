using UnityEngine;

public class SpriteSwap : MonoBehaviour
{
    public Sprite[] Sprites;
    public SpriteRenderer Renderer;
    private int _currentIndex;

    public int CurrentIndex
    {
        get => _currentIndex;
        set => _currentIndex = Mathf.Clamp(value, 0, Sprites.Length-1);
    }

    public void SetSprite()
    {
        Renderer.sprite = Sprites[CurrentIndex];
    }
    
    public void SetSprite(int index)
    {
        Renderer.sprite = Sprites[index];
    }
}
