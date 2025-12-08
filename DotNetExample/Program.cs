using System;
using OtpNet;

namespace DotNetExample
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var secretKey = KeyGeneration.GenerateRandomKey(20);

            var totp = new Totp(secretKey);

            var time = DateTime.UtcNow;

            var totpCode = totp.ComputeTotp(time);

            Console.WriteLine(totpCode);

            var remainingSeconds = totp.RemainingSeconds(time);
            Console.WriteLine($"Time remaining for this code: {remainingSeconds} seconds");

            // This shows the remaining time for the current code.  It rolls over so it doesn't become negative over time.
            remainingSeconds = totp.RemainingSeconds(time.AddSeconds(5));
            Console.WriteLine($"Time remaining for this code after 5 seconds: {remainingSeconds} seconds");

            remainingSeconds = totp.RemainingSeconds(time.AddSeconds(25));
            Console.WriteLine($"Time remaining for this code after 25 seconds: {remainingSeconds} seconds");

            // The timeStepMatched out parameter shows which time step was used to verify the code.
            // It can be used for logging or auditing purposes to ensure that a code has not been reused.
            var verifyResult = totp.VerifyTotp(time, totpCode, out long timeStepMatched, VerificationWindow.RfcSpecifiedNetworkDelay);
            Console.WriteLine($"The verification result was {verifyResult}, and the time step matched was {timeStepMatched}.");

            verifyResult = totp.VerifyTotp(time.AddSeconds(15), totpCode, out timeStepMatched, VerificationWindow.RfcSpecifiedNetworkDelay);
            Console.WriteLine($"The verification result 15s later was {verifyResult}, and the time step matched was {timeStepMatched}.");

            verifyResult = totp.VerifyTotp(time.AddSeconds(60), totpCode, out timeStepMatched, VerificationWindow.RfcSpecifiedNetworkDelay);
            Console.WriteLine($"The verification result 60s later was {verifyResult}, and the time step matched was {timeStepMatched}.");
        }
    }
}
