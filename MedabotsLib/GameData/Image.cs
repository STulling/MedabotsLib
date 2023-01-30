using MedabotsLib.Utils;

namespace MedabotsLib.Data
{
    /// <summary>
    /// A struct that represents an image
    /// the image is stored as a 3D array of bytes [Height, Width, Color]
    /// </summary>
    public class Image : IByteable, ICanGetDirty
    {
        private bool isDirty = false;
        public bool IsDirty { get => isDirty; }
        byte[,,] imageData {
            get
            {
                return imageData;
            }
            set
            {
                imageData = value;
                isDirty = true;
            }
        }

        public Image(byte[,,] imageData)
        {
            this.imageData = imageData;
        }

        public byte[] ToBytes()
        {
            byte[] bytes = new byte[imageData.Length];
            int i = 0;
            foreach (byte b in imageData)
            {
                bytes[i] = b;
                i++;
            }
            return bytes;
        }
        
    }
}
