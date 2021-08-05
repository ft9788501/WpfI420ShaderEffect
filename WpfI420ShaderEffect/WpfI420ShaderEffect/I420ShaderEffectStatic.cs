using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace WpfI420ShaderEffect
{
    public class I420ShaderEffectStatic : ShaderEffect
    {
        #region DependencyProperty

        public static readonly DependencyProperty InputProperty = RegisterPixelShaderSamplerProperty("Input", typeof(I420ShaderEffectStatic), 0);
        public static readonly DependencyProperty TextureYProperty = RegisterPixelShaderSamplerProperty("TextureY", typeof(I420ShaderEffectStatic), 1);
        public static readonly DependencyProperty TextureUProperty = RegisterPixelShaderSamplerProperty("TextureU", typeof(I420ShaderEffectStatic), 2);
        public static readonly DependencyProperty TextureVProperty = RegisterPixelShaderSamplerProperty("TextureV", typeof(I420ShaderEffectStatic), 3);
        public static readonly DependencyProperty ViewboxProperty = DependencyProperty.Register(
            nameof(Viewbox),
            typeof(Rect),
            typeof(I420ShaderEffectStatic),
            new FrameworkPropertyMetadata(Rect.Empty, new PropertyChangedCallback((s, e) =>
            {
                if (s is I420ShaderEffectStatic i420ShaderEffectStatic && e.NewValue is Rect viewBox)
                {
                    i420ShaderEffectStatic.TextureY = new ImageBrush(YuvTextures.Instance.WriteableBitmapY)
                    {
                        Stretch= Stretch.None,
                        Viewbox = viewBox,
                        ViewboxUnits = BrushMappingMode.Absolute
                    };
                    i420ShaderEffectStatic.TextureU = new ImageBrush(YuvTextures.Instance.WriteableBitmapU)
                    {
                        Stretch = Stretch.None,
                        Viewbox = viewBox == Rect.Empty ? Rect.Empty : new Rect(viewBox.X/2, viewBox.Y/2 , viewBox.Width, viewBox.Height ),
                        ViewboxUnits = BrushMappingMode.Absolute
                    };
                    i420ShaderEffectStatic.TextureV = new ImageBrush(YuvTextures.Instance.WriteableBitmapV)
                    {
                        Stretch = Stretch.None,
                        Viewbox = viewBox == Rect.Empty ? Rect.Empty : new Rect(viewBox.X /2, viewBox.Y/2 , viewBox.Width, viewBox.Height),
                        ViewboxUnits = BrushMappingMode.Absolute
                    };
                }
            })));

        public Brush Input
        {
            get => (Brush)GetValue(InputProperty);
            set => SetValue(InputProperty, value);
        }
        public ImageBrush TextureY
        {
            get => (ImageBrush)GetValue(TextureYProperty);
            set => SetValue(TextureYProperty, value);
        }
        public ImageBrush TextureU
        {
            get => (ImageBrush)GetValue(TextureUProperty);
            set => SetValue(TextureUProperty, value);
        }
        public ImageBrush TextureV
        {
            get => (ImageBrush)GetValue(TextureVProperty);
            set => SetValue(TextureVProperty, value);
        }
        public Rect Viewbox
        {
            get { return (Rect)GetValue(ViewboxProperty); }
            set { SetValue(ViewboxProperty, value); }
        }

        #endregion

        public I420ShaderEffectStatic()
        {
            PixelShader = YuvTextures.Instance.PixelShader;
        }
    }
}
