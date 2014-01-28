#region Copyright Notice & License Information
//
// Texture.cs
//
// Author:
//       Matthew Davey <matthew.davey@dotbunny.com>
//       Robin Southern <betajaen@ihoed.com>
//
// Copyright (c) 2013 dotBunny Inc. (http://www.dotbunny.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
#endregion

using UnityEngine;

namespace Hydrogen
{
		/// <summary>
		/// Additional static functions used to extend existing Texture support inside of Unity.
		/// </summary>
		public static class Texture
		{
				/// <summary>
				/// Handy 1/255, useful for calculating color values. 
				/// </summary>
				public const float HexInverse = 0.00392156863f;

				/// <summary>
				/// Web Colors
				/// </summary>
				public enum WebColor
				{
						None = 0,
						AliceBlue = 0xf0f8ff,
						Gainsboro = 0xdcdcdc,
						MistyRose = 0xffe4e1,
						AntiqueWhite = 0xfaebd7,
						GhostWhite = 0xf8f8ff,
						Moccasin = 0xffe4b5,
						Aqua = 0x00ffff,
						Gold = 0xffd700,
						NavajoWhite = 0xffdead,
						Aquamarine = 0x7fffd4,
						Goldenrod = 0xdaa520,
						Navy = 0x000080,
						Azure = 0xf0ffff,
						Gray = 0x808080,
						OldLace = 0xfdf5e6,
						Beige = 0xf5f5dc,
						Green = 0x008000,
						Olive = 0x808000,
						Bisque = 0xffe4c4,
						GreenYellow = 0xadff2f,
						OliveDrab = 0x6b8e23,
						Black = 0x000000,
						Grey = 0x808080,
						Orange = 0xffa500,
						BlanchedAlmond = 0xffebcd,
						Honeydew = 0xf0fff0,
						OrangeRed = 0xff4500,
						Blue = 0x0000ff,
						HotPink = 0xff69b4,
						Orchid = 0xda70d6,
						BlueViolet = 0x8a2be2,
						IndianRed = 0xcd5c5c,
						PaleGoldenrod = 0xeee8aa,
						Brown = 0xa52a2a,
						Indigo = 0x4b0082,
						PaleGreen = 0x98fb98,
						Burlywood = 0xdeb887,
						Ivory = 0xfffff0,
						PaleTurquoise = 0xafeeee,
						CadetBlue = 0x5f9ea0,
						Khaki = 0xf0e68c,
						PaleVioletRed = 0xdb7093,
						Chartreuse = 0x7fff00,
						Lavender = 0xe6e6fa,
						PapayaWhip = 0xffefd5,
						Chocolate = 0xd2691e,
						LavenderBlush = 0xfff0f5,
						PeachPuff = 0xffdab9,
						Coral = 0xff7f50,
						LawnGreen = 0x7cfc00,
						Peru = 0xcd853f,
						CornflowerBlue = 0x6495ed,
						LemonChiffon = 0xfffacd,
						Pink = 0xffc0cb,
						Cornsilk = 0xfff8dc,
						LightBlue = 0xadd8e6,
						Plum = 0xdda0dd,
						Crimson = 0xdc143c,
						LightCoral = 0xf08080,
						PowderBlue = 0xb0e0e6,
						Cyan = 0x00ffff,
						LightCyan = 0xe0ffff,
						Purple = 0x800080,
						DarkBlue = 0x00008b,
						LightGoldenrodyellow = 0xfafad2,
						Red = 0xff0000,
						DarkCyan = 0x008b8b,
						LightGray = 0xd3d3d3,
						RosyBrown = 0xbc8f8f,
						DarkGoldenrod = 0xb8860b,
						LightGreen = 0x90ee90,
						RoyalBlue = 0x4169e1,
						DarkGray = 0xa9a9a9,
						LightGrey = 0xd3d3d3,
						SaddleBrown = 0x8b4513,
						DarkGreen = 0x006400,
						LightPink = 0xffb6c1,
						Salmon = 0xfa8072,
						DarkGrey = 0xa9a9a9,
						LightSalmon = 0xffa07a,
						SandyBrown = 0xf4a460,
						DarkKhaki = 0xbdb76b,
						LightSeagreen = 0x20b2aa,
						SeaGreen = 0x2e8b57,
						DarkMagenta = 0x8b008b,
						LightSkyblue = 0x87cefa,
						SeaShell = 0xfff5ee,
						DarkOlivegreen = 0x556b2f,
						LightSlategray = 0x778899,
						Sienna = 0xa0522d,
						DarkOrange = 0xff8c00,
						LightSlategrey = 0x778899,
						Silver = 0xc0c0c0,
						DarkOrchid = 0x9932cc,
						LightSteelblue = 0xb0c4de,
						SkyBlue = 0x87ceeb,
						DarkRed = 0x8b0000,
						LightYellow = 0xffffe0,
						SlateBlue = 0x6a5acd,
						DarkSalmon = 0xe9967a,
						Lime = 0x00ff00,
						SlateGray = 0x708090,
						DarkSeagreen = 0x8fbc8f,
						LimeGreen = 0x32cd32,
						SlateGrey = 0x708090,
						DarkSlateblue = 0x483d8b,
						Linen = 0xfaf0e6,
						Snow = 0xfffafa,
						DarkSlategray = 0x2f4f4f,
						Magenta = 0xff00ff,
						SpringGreen = 0x00ff7f,
						DarkSlategrey = 0x2f4f4f,
						Maroon = 0x800000,
						SteelBlue = 0x4682b4,
						DarkTurquoise = 0x00ced1,
						MediumAquamarine = 0x66cdaa,
						Tan = 0xd2b48c,
						DarkViolet = 0x9400d3,
						MediumBlue = 0x0000cd,
						Teal = 0x008080,
						DeepPink = 0xff1493,
						MediumOrchid = 0xba55d3,
						Thistle = 0xd8bfd8,
						DeepSkyblue = 0x00bfff,
						MediumPurple = 0x9370db,
						Tomato = 0xff6347,
						DimGray = 0x696969,
						MediumSeaGreen = 0x3cb371,
						Turquoise = 0x40e0d0,
						DimGrey = 0x696969,
						MediumSlateBlue = 0x7b68ee,
						Violet = 0xee82ee,
						DodgerBlue = 0x1e90ff,
						MediumSpringGreen = 0x00fa9a,
						Wheat = 0xf5deb3,
						FireBrick = 0xb22222,
						MediumTurquoise = 0x48d1cc,
						White = 0xffffff,
						FloralWhite = 0xfffaf0,
						MediumBioletRed = 0xc71585,
						WhiteSmoke = 0xf5f5f5,
						ForestGreen = 0x228b22,
						MidnightBlue = 0x191970,
						Yellow = 0xffff00,
						Fuchsia = 0xff00ff,
						MintCream = 0xf5fffa,
						YellowGreen = 0x9acd32
				}

				/// <summary>
				/// Convert a WebColor to a UnityEngine.Color
				/// </summary>
				/// <returns>UnityEngine.Color.</returns>
				/// <param name="color">The web color you wish to have returned in Unity format.</param>
				public static Color ToColor (this WebColor color)
				{

						return new Color (
								(((int)color >> 16) & 0xFF) * HexInverse,
								(((int)color >> 8) & 0xFF) * HexInverse,
								((int)color & 0xFF) * HexInverse

						);
				}

				/// <summary>
				/// Convert a WebColor to a UnityEngine.Color
				/// </summary>
				/// <returns>UnityEngine.Color.</returns>
				/// <param name="color">The web color you wish to have returned in Unity format.</param>
				/// <param name="alpha">The alpha value of the color you want returned.</param>
				public static Color ToColor (WebColor color, float alpha)
				{
						Color newColor = ToColor (color);
						newColor = new Color (newColor.r, newColor.g, newColor.b, alpha);
						return newColor;
				}

				/// <summary>
				/// Autocrop the texture removes the borders from the texture. It searches the active layer for the 
				/// largest possible border area that's color matches borderColor, and then crops this area from the 
				/// image, as if you had used the Crop tool.
				/// </summary>
				/// <returns>The cropped image.</returns>
				/// <param name="texture">The source texture.</param>
				/// <param name="borderColor">The border color used in the crop calculations.</param>
				public static Texture2D AutoCrop (this Texture2D texture, Color borderColor)
				{
						var image = texture.GetPixels ();
						var width = texture.width;
						var height = texture.height;

						var left = -1;
						var right = -1;
						var top = -1;
						var bottom = -1;
						var x = 0;
						int y;

						while (left == -1 && x < width) {
								for (y = 0; y < height; y++) {
										if (!image [y * width + x].IsColorApproximatelySame (borderColor))
												left = x;
								}
								x++;
						}

						x = width - 1;

						while (right == -1 && x >= 0) {
								for (y = 0; y < height; y++) {
										if (!image [y * width + x].IsColorApproximatelySame (borderColor))
												right = x;
								}
								x--;
						}

						y = 0;

						while (top == -1 && y < height) {
								for (x = 0; x < width; x++) {
										if (!image [y * width + x].IsColorApproximatelySame (borderColor))
												top = y;
								}
								y++;
						}

						y = height - 1;

						while (bottom == -1 && y >= 0) {
								for (x = 0; x < width; x++) {
										if (!image [y * width + x].IsColorApproximatelySame (borderColor))
												bottom = y;
								}
								y--;
						}

						return Crop (texture, left, top, right - left, bottom - top);
				}

				/// <summary>
				/// The pixels (RGB) of both the destinationTexture and the sourceTexture are combined bitwise according to 
				/// the simplified overwrite raster operation, and the result is then written to the destinationTexture. 
				/// </summary>
				/// <param name="destinationTexture">The destination texture.</param>
				/// <param name="sourceTexture">The source texture.</param>
				/// <param name="offsetX">Offset X.</param>
				/// <param name="offsetY">Offset Y.</param>
				public static void Blit (Texture2D destinationTexture, Texture2D sourceTexture, int offsetX, int offsetY)
				{
						var srcWidth = sourceTexture.width;
						//		int srcHeight = src.height;
						var dstWidth = destinationTexture.width;
						//		int dstHeight = dst.height;
						Color[] dstp = destinationTexture.GetPixels ();
						Color[] srcp = sourceTexture.GetPixels ();

						for (int y = 0; y < sourceTexture.height; y++) {
								for (int x = 0; x < sourceTexture.width; x++) {
										dstp [(y + offsetY) * dstWidth + x + offsetX] = srcp [y * srcWidth + x];
								}
						}

						destinationTexture.SetPixels (dstp);
				}

				/// <summary>
				/// The pixels (RGBA) of both the destinationTexture and the sourceTexture are combined bitwise according to 
				/// the simplified overwrite raster operation, and the result is then written to the destinationTexture. 
				/// </summary>
				/// <param name="destinationTexture">The destination texture.</param>
				/// <param name="sourceTexture">The source texture.</param>
				/// <param name="offsetX">Offset X.</param>
				/// <param name="offsetY">Offset Y.</param>
				public static void BlitAlpha (Texture2D destinationTexture, Texture2D sourceTexture, int offsetX, int offsetY)
				{
						try {
								var srcWidth = sourceTexture.width;
								var srcHeight = sourceTexture.height;
								var dstWidth = destinationTexture.width;
								var dstps = destinationTexture.GetPixels ();
								var srcps = sourceTexture.GetPixels ();
								Color srcp;

								for (int y = 0; y < srcHeight; y++) {
										for (int x = 0; x < srcWidth; x++) {
												int dstOffset = (y + offsetY) * dstWidth + x + offsetX;
												int srcOffset = y * srcWidth + x;
												srcp = srcps [srcOffset];
												if (srcp.a > 0.001f) {
														dstps [dstOffset] = Color.Lerp (dstps [dstOffset], srcp, srcp.a);
												}
										}
								}

								destinationTexture.SetPixels (dstps);
						} catch (System.Exception e) {
								Debug.Log ("[H] There was a problem Blitting the texture. " + e);
						}
				}

				/// <summary>
				/// Crop the specified texture starting at (X,Y) and extending out based on the width and height.
				/// </summary>
				/// <param name="sourceTexture">The texture to crop.</param>
				/// <param name="offsetX">The X offset coordinate.</param>
				/// <param name="offsetY">The Y offset coordinate.</param>
				/// <param name="width">Target Width</param>
				/// <param name="height">Target Height</param>
				public static Texture2D Crop (this Texture2D sourceTexture, int offsetX, int offsetY, int width, int height)
				{
						var result = new Texture2D (width, height);

						result.SetPixels (sourceTexture.GetPixels (offsetX, offsetY, width, height));

						return result;
				}

				/// <summary>
				/// Flip texture horizontally.
				/// </summary>
				/// <returns>The horizontally flipped texture.</returns>
				/// <param name="texture">The source texture.</param>
				public static Texture2D FlipHorizontally (this Texture2D texture)
				{
						var image = texture.GetPixels ();
						var newImage = new Color[image.Length];

						var width = texture.width;
						var height = texture.height;

						for (var y = 0; y < height; y++) {
								for (var x = 0; x < width; x++) {
										newImage [y * width + (x - width - 1)] = image [y * width + x];
								}
						}

						var result = new Texture2D (width, height);

						result.SetPixels (newImage);

						return result;
				}

				/// <summary>
				/// Flip texture vertically.
				/// </summary>
				/// <returns>The vertically flipped texture.</returns>
				/// <param name="texture">The source texture..</param>
				public static Texture2D FlipVertically (this Texture2D texture)
				{
						var image = texture.GetPixels ();
						var newImage = new Color[image.Length];

						var width = texture.width;
						var height = texture.height;

						for (var y = 0; y < height; y++) {
								for (var x = 0; x < width; x++) {
										newImage [(height - y - 1) * width + x] = image [y * width + x];
								}
						}

						var result = new Texture2D (width, height);

						result.SetPixels (newImage);

						return result;
				}

				/// <summary>
				/// Resize the texture to a specific width and height using bilinear filtering.
				/// </summary>
				/// <param name="sourceTexture">The source texture.</param>
				/// <param name="newWidth">The new resized width.</param>
				/// <param name="newHeight">The new resized height.</param>
				public static Texture2D Resize (this Texture2D sourceTexture, int newWidth, int newHeight)
				{
						var resizedTexture = new Texture2D (newWidth, newHeight);
						var resizeFactor = new Vector2 ((float)sourceTexture.width / (float)newWidth, (float)sourceTexture.height / (float)newHeight);

						Vector2 fraction;
						Vector2 oneMinusFraction;
						var one = new Vector2 (1f, 1f);

						Color c1;
						Color c2;
						Color c3;
						Color c4;

						Color[] image = sourceTexture.GetPixels ();
						Color[] result = resizedTexture.GetPixels ();

						for (var y = 0; y < newHeight; y++) {
								for (var x = 0; x < newWidth; x++) {
										var floorX = (int)Mathf.Floor (x * resizeFactor.x);
										var floorY = (int)Mathf.Floor (y * resizeFactor.y);

										var ceilingX = floorX + 1;

										if (ceilingX >= sourceTexture.width)
												ceilingX = floorX;

										var ceilingY = floorY + 1;
										if (ceilingY >= sourceTexture.height)
												ceilingY = floorY;

										fraction = new Vector2 (x * resizeFactor.x - floorX, y * resizeFactor.y - floorY);
										oneMinusFraction = one - fraction;

										c1 = image [floorY * sourceTexture.width + floorX];
										c2 = image [floorY * sourceTexture.width + ceilingX];
										c3 = image [ceilingY * sourceTexture.width + floorX];
										c4 = image [ceilingY * sourceTexture.width + ceilingX];

										// Blue
										float b1 = (oneMinusFraction.x * c1.b + fraction.x * c2.b);
										float b2 = (oneMinusFraction.x * c3.b + fraction.x * c4.b);

										float blue = (oneMinusFraction.y * b1 + fraction.y * b2);

										// Green
										b1 = (oneMinusFraction.x * c1.g + fraction.x * c2.g);
										b2 = (oneMinusFraction.x * c3.g + fraction.x * c4.g);

										float green = (oneMinusFraction.y * b1 + fraction.y * b2);

										// Red
										b1 = (oneMinusFraction.x * c1.r + fraction.x * c2.r);
										b2 = (oneMinusFraction.x * c3.r + fraction.x * c4.r);

										float red = (oneMinusFraction.y * b1 + fraction.y * b2);

										// Alpha
										b1 = (oneMinusFraction.x * c1.a + fraction.x * c2.a);
										b2 = (oneMinusFraction.x * c3.a + fraction.x * c4.a);

										float alpha = (oneMinusFraction.y * b1 + fraction.y * b2);

										result [y * resizedTexture.width + x] = new Color (red, green, blue, alpha);
								}
						}

						resizedTexture.SetPixels (result);

						return resizedTexture;
				}

				/// <summary>
				/// Rotate texture clockwise, 90 degrees.
				/// </summary>
				/// <returns>The rotated texture.</returns>
				/// <param name="texture">The source texture.</param>
				public static Texture2D RotateClockwise90Degrees (this Texture2D texture)
				{
						var image = texture.GetPixels ();
						var newImage = new Color[image.Length];

						var width = texture.width;
						var height = texture.height;

						for (int y = 0; y < height; y++) {
								for (int x = 0; x < width; x++) {
										newImage [y * width + x] = image [(x * width) + (height - 1 - y)];
								}
						}

						var result = new Texture2D (width, height);

						result.SetPixels (newImage);

						return result;
				}

				/// <summary>
				/// Rotate texture counter clockwise, 90 degrees.
				/// </summary>
				/// <returns>The rotated texture.</returns>
				/// <param name="texture">The source texture.</param>
				public static Texture2D RotateCounterClockwise90Degrees (this Texture2D texture)
				{
						var image = texture.GetPixels ();
						var newImage = new Color[image.Length];

						var width = texture.width;
						var height = texture.height;

						for (int y = 0; y < height; y++) {
								for (int x = 0; x < width; x++) {
										newImage [y * width + x] = image [(width - 1 - x) * width + y];
								}
						}

						var result = new Texture2D (width, height);

						result.SetPixels (newImage);

						return result;
				}

				/// <summary>
				/// Tile the specified texture as outlined.
				/// </summary>
				/// <returns>The tiled texture.</returns>
				/// <param name="texture">The texture to tile.</param>
				/// <param name="horizontalTiles">Number of horizontal tiles.</param>
				/// <param name="verticalTiles">Number of vertical tiles.</param>
				public static Texture2D Tile (this Texture2D texture, int horizontalTiles, int verticalTiles)
				{
						// Sanity Check
						if (horizontalTiles < 1)
								horizontalTiles = 1;
						if (verticalTiles < 1)
								verticalTiles = 1;

						var tiledTexture = new Texture2D (texture.width * horizontalTiles, texture.height * verticalTiles);
						var texturePixels = texture.GetPixels ();
						for (var i = 0; i < horizontalTiles; i++) {
								for (var j = 0; j < verticalTiles; j++)
										tiledTexture.SetPixels (i * texture.width, j * texture.height, texture.width, texture.height, texturePixels);
						}

						return tiledTexture;
				}

				/// <summary>
				/// Transpose texture.
				/// </summary>
				/// <returns>The transposed texture.</returns>
				/// <param name="texture">The source texture.</param>
				public static Texture2D Transpose (this Texture2D texture)
				{
						var image = texture.GetPixels ();
						var newImage = new Color[image.Length];

						var width = texture.width;
						var height = texture.height;

						for (var y = 0; y < height; y++) {
								for (var x = 0; x < width; x++) {
										newImage [y * width + x] = image [x * width + y];
								}
						}

						var result = new Texture2D (width, height);

						result.SetPixels (newImage);

						return result;
				}

				/// <summary>
				/// Transpose texture perpendicularily.
				/// </summary>
				/// <returns>The transponsed texture.</returns>
				/// <param name="texture">The source texture.</param>
				public static Texture2D TransposePerpendicular (this Texture2D texture)
				{
						var image = texture.GetPixels ();
						var newImage = new Color[image.Length];

						var width = texture.width;
						var height = texture.height;

						for (int y = 0; y < height; y++) {
								for (int x = 0; x < width; x++) {
										newImage [y * width + x] = image [(width - 1 - x) * width + (height - 1 - y)];
								}
						}

						var result = new Texture2D (width, height);

						result.SetPixels (newImage);

						return result;
				}
		}
}
