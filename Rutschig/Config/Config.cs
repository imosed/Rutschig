namespace Rutschig
{
    public class Config
    {
        public string AppName { get; set; }
        public byte ShortenedLength { get; set; }
        public int MaxPinLength { get; set; }
        public int MaxExpirationDelta { get; set; }
    }
}