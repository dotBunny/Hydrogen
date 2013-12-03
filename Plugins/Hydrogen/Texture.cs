#region Copyright Notice & License Information
//
// Texture.cs
//
// Author:
//       Matthew Davey <matthew.davey@dotbunny.com>
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
		/// Additional static functions, constants and classes used to extend existing Texture support inside of Unity.
		/// </summary>
		//TODO: Need proper documentation
		public static class Texture
		{
				// simple bilinear filtering
				public static Texture2D Resize (this Texture2D texture, int newWidth, int newHeight)
				{
						var resizedTexture = new Texture2D (newWidth, newHeight);
						var resizeFactor = new Vector2 ((float)texture.width / (float)newWidth, (float)texture.height / (float)newHeight);

						Vector2 fraction;
						Vector2 oneMinusFraction;
						var one = new Vector2 (1f, 1f);

						Color c1;
						Color c2;
						Color c3;
						Color c4;

						Color[] image = texture.GetPixels ();
						Color[] result = resizedTexture.GetPixels ();

						for (var y = 0; y < newHeight; y++) {
								for (var x = 0; x < newWidth; x++) {
										var floorX = (int)Mathf.Floor (x * resizeFactor.x);
										var floorY = (int)Mathf.Floor (y * resizeFactor.y);

										var ceilingX = floorX + 1;

										if (ceilingX >= texture.width)
												ceilingX = floorX;

										var ceilingY = floorY + 1;
										if (ceilingY >= texture.height)
												ceilingY = floorY;

										fraction = new Vector2 (x * resizeFactor.x - floorX, y * resizeFactor.y - floorY);
										oneMinusFraction = one - fraction;

										c1 = image [floorY * texture.width + floorX];
										c2 = image [floorY * texture.width + ceilingX];
										c3 = image [ceilingY * texture.width + floorX];
										c4 = image [ceilingY * texture.width + ceilingX];

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

				public static Texture2D Tile (this Texture2D texture, int horizontalTiles, int verticalTiles)
				{
						// @TODO Ensure horizontalTiles and verticalTiles sanity
						var tiledTexture = new Texture2D (texture.width * horizontalTiles, texture.height * verticalTiles);
						var texturePixels = texture.GetPixels ();
						for (var i = 0; i < horizontalTiles; i++) {
								for (var j = 0; j < verticalTiles; j++)
										tiledTexture.SetPixels (i * texture.width, j * texture.height, texture.width, texture.height, texturePixels);
						}

						return tiledTexture;
				}
				//TODO error checking and assertions :)
				public static Texture2D Crop (this Texture2D texture, int x, int y, int width, int height)
				{

						var result = new Texture2D (width, height);

						result.SetPixels (texture.GetPixels (x, y, width, height));

						return result;
				}

				static bool AreColorsApproximatelyEqual (Color a, Color b)
				{
						return (Mathf.Approximately (a.r, b.r) && Mathf.Approximately (a.g, b.g) && Mathf.Approximately (a.b, b.b) && Mathf.Approximately (a.a, b.a));
				}

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
										if (!AreColorsApproximatelyEqual (image [y * width + x], borderColor))
												left = x;
								}
								x++;
						}

						x = width - 1;

						while (right == -1 && x >= 0) {
								for (y = 0; y < height; y++) {
										if (!AreColorsApproximatelyEqual (image [y * width + x], borderColor))
												right = x;
								}
								x--;
						}

						y = 0;

						while (top == -1 && y < height) {
								for (x = 0; x < width; x++) {
										if (!AreColorsApproximatelyEqual (image [y * width + x], borderColor))
												top = y;
								}
								y++;
						}

						y = height - 1;

						while (bottom == -1 && y >= 0) {
								for (x = 0; x < width; x++) {
										if (!AreColorsApproximatelyEqual (image [y * width + x], borderColor))
												bottom = y;
								}
								y--;
						}

						return Crop (texture, left, top, right - left, bottom - top);
				}

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

				public static Texture2D Transpose (Texture2D texture)
				{
						var image = texture.GetPixels ();
						var newImage = new Color[image.Length];

						var width = texture.width;
						var height = texture.height;

						for (int y = 0; y < height; y++) {
								for (int x = 0; x < width; x++) {
										newImage [y * width + x] = image [x * width + y];
								}
						}

						var result = new Texture2D (width, height);

						result.SetPixels (newImage);

						return result;
				}

				public static Texture2D TransposePerpendicular (Texture2D texture)
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

				public static void Blit (Texture2D dst, Texture2D src, int offsetX, int offsetY)
				{
						var srcWidth = src.width;
						//		int srcHeight = src.height;
						var dstWidth = dst.width;
						//		int dstHeight = dst.height;
						Color[] dstp = dst.GetPixels ();
						Color[] srcp = src.GetPixels ();

						for (int y = 0; y < src.height; y++) {
								for (int x = 0; x < src.width; x++) {
										dstp [(y + offsetY) * dstWidth + x + offsetX] = srcp [y * srcWidth + x];
								}
						}

						dst.SetPixels (dstp);
				}

				public static void BlitAlpha (Texture2D dstTexture, Texture2D srcTexture, int offsetX, int offsetY)
				{
						try {
								var srcWidth = srcTexture.width;
								var srcHeight = srcTexture.height;
								var dstWidth = dstTexture.width;
								//		int dstHeight = dstTexture.height;
								var dstps = dstTexture.GetPixels ();
								var srcps = srcTexture.GetPixels ();
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

								dstTexture.SetPixels (dstps);
						} catch (System.Exception e) {
								Debug.Log ("[H] There was a problem Blitting the texture. " + e);
						}
				}
		}
}
