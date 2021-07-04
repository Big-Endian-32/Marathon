﻿// Marathon is licensed under the MIT License:
/* 
 * MIT License
 * 
 * Copyright (c) 2021 HyperBE32
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using System;
using System.Drawing;
using System.Collections.Generic;
using Marathon.Helpers;

namespace Marathon
{
    public class Cache
    {
        private static Dictionary<string, Bitmap> BitmapCache = new Dictionary<string, Bitmap>();

        /// <summary>
        /// Loads and caches a Bitmap from .NET resources.
        /// </summary>
        /// <param name="resource">Name of .NET resource.</param>
        public static Bitmap LoadBitmapResource(string resource)
        {
            /* This entire function is skipped if we're running in Design View - takes too long to process
               and slows things down horrifically, so it'll be easier and faster to return the input bitmap. */
            if (DesignHelper.RunningInDesigner())
                return (Bitmap)Properties.Resources.ResourceManager.GetObject(resource);

            if (BitmapCache.ContainsKey(resource))
            {
                // Collect garbage from last bitmap instance.
                GC.Collect(GC.GetGeneration(BitmapCache[resource]), GCCollectionMode.Forced);

                return BitmapCache[resource];
            }

            // Get the bitmap data from the name of the input resource.
            Bitmap fromResource = (Bitmap)Properties.Resources.ResourceManager.GetObject(resource);

            // Add current bitmap to the dictionary.
            BitmapCache.Add(resource, fromResource);

            return fromResource;
        }
    }
}
