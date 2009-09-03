using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace UltimaXNA.Graphics.UI
{
    public interface IGump 
    {
        Serial Serial { get; }
        bool Closable { get; }
    }

    public interface IGumpAlpha
    {
        float Opacity { get; }
    }

    public interface IGumpBackground 
    {
        Texture2D TopLeftTexture { get; }
        Texture2D TopTexture { get; }
        Texture2D TopRightTexture { get; }
        Texture2D MiddleLeftTexture { get; }
        Texture2D MiddleTexture { get; }
        Texture2D MiddleRightTexture { get; }
        Texture2D BottomLeftTexture { get; }
        Texture2D BottomTexture { get; }
        Texture2D BottomRightTexture { get; }
    }

    public interface IGumpButton 
    {
        bool Pressed { get; }
        Texture2D NormalTexture { get; }
        Texture2D PressedTexture { get; }
    }

    public interface IGumpCheckbox 
    {
        bool Checked { get; }
        Texture2D UncheckedTexture { get; }
        Texture2D CheckedTexture { get; }
    }

    public interface IGumpRadioButton : IGumpCheckbox 
    {

    }

    public interface IGumpExpander 
    {
    
    }

    public interface IGumpHtml 
    {
    
    }

    public interface IGumpLabel  
    {
        string Text { get; }
    }

    public interface IGumpProgressBar
    {
        float Value { get; }
        float MinValue { get; }
        float MaxValue { get; }
    }

    public interface IGumpSlider : IGumpProgressBar
    {
    
    }

    public interface IUIContainer
    {
        UIContainer Children { get; }
        UINode FindFirstChildAt(Vector2 position);
        UINode FindFirstChildAt(float x, float y);
    }
}
