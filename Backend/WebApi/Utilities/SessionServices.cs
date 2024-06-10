using System.Reflection.Emit;

namespace WebApi.Utilities
{
    public static class SessionServices
    {
        static Random generator = new Random();
        public static string GetNewSessionCode(IEnumerable<string> existingCodes) {
            string newCode = generator.Next(0, 1000000).ToString("D6");
            while (existingCodes.Any(x => x == newCode))
            {
                newCode = generator.Next(0, 1000000).ToString("D6");
            }
            return newCode;
        }
    }
}
