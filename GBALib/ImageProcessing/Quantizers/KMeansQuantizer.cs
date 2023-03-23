using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GBALib.ImageProcessing.Quantizers
{
    internal class KMeansQuantizer : Quantizer
    {
        public override List<SKColor> Quantize(SKBitmap bitmap, int max_colors)
        {
            if (bitmap == null)
            {
                throw new ArgumentNullException(nameof(bitmap));
            }

            if (max_colors < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(max_colors));
            }

            List<SKColor> colors = GetColors(bitmap);
            List<SKColor> centroids = InitializeCentroids(colors, max_colors);
            List<SKColor> previousCentroids;

            do
            {
                Dictionary<SKColor, List<SKColor>> clusters = AssignToClusters(colors, centroids);
                previousCentroids = centroids;
                centroids = UpdateCentroids(clusters);
            } while (!AreCentroidsConverged(previousCentroids, centroids));

            return centroids;
        }

        private List<SKColor> GetColors(SKBitmap bitmap)
        {
            List<SKColor> colors = new List<SKColor>();

            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    colors.Add(bitmap.GetPixel(x, y));
                }
            }

            return colors;
        }

        private List<SKColor> InitializeCentroids(List<SKColor> colors, int max_colors)
        {
            Random random = new Random();
            List<SKColor> centroids = new List<SKColor>();

            for (int i = 0; i < max_colors; i++)
            {
                centroids.Add(colors[random.Next(colors.Count)]);
            }

            return centroids;
        }

        private Dictionary<SKColor, List<SKColor>> AssignToClusters(List<SKColor> colors, List<SKColor> centroids)
        {
            Dictionary<SKColor, List<SKColor>> clusters = new Dictionary<SKColor, List<SKColor>>();

            foreach (SKColor centroid in centroids)
            {
                clusters[centroid] = new List<SKColor>();
            }

            foreach (SKColor color in colors)
            {
                SKColor nearestCentroid = GetNearestCentroid(color, centroids);
                clusters[nearestCentroid].Add(color);
            }

            return clusters;
        }

        private SKColor GetNearestCentroid(SKColor color, List<SKColor> centroids)
        {
            SKColor nearestCentroid = centroids[0];
            double minDistance = ColorHelper.Instance.ColorDistance(color, nearestCentroid);

            foreach (SKColor centroid in centroids.Skip(1))
            {
                double distance = ColorHelper.Instance.ColorDistance(color, centroid);

                if (distance < minDistance)
                {
                    nearestCentroid = centroid;
                    minDistance = distance;
                }
            }

            return nearestCentroid;
        }

        private List<SKColor> UpdateCentroids(Dictionary<SKColor, List<SKColor>> clusters)
        {
            List<SKColor> newCentroids = new List<SKColor>();

            foreach (List<SKColor> cluster in clusters.Values)
            {
                int redSum = 0;
                int greenSum = 0;
                int blueSum = 0;

                foreach (SKColor color in cluster)
                {
                    redSum += color.Red;
                    greenSum += color.Green;
                    blueSum += color.Blue;
                }

                int count = cluster.Count;
                newCentroids.Add(new SKColor((byte)(redSum / count), (byte)(greenSum / count), (byte)(blueSum / count)));
            }

            return newCentroids;
        }

        private bool AreCentroidsConverged(List<SKColor> previousCentroids, List<SKColor> currentCentroids)
        {
            for (int i = 0; i < previousCentroids.Count; i++)
            {
                if (previousCentroids[i] != currentCentroids[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
