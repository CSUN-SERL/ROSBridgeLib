namespace ROSBridgeLib.Extensions
{
    public static class BoolExtensions
    {
        public static string ToStringLower(this bool value)
        {
            return value ? "true" : "false";
        }
    }
}