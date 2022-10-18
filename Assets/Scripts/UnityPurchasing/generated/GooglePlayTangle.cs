// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("7G9hbl7sb2Rs7G9vbqR62k+D24J0HIel0HJPS3TIIhtwDM6UESL/817sb0xeY2hnROgm6Jljb29va25tGmjgcWdwdQJ3H4bbh0jkVmD+SGXh3xLns5OpLQs4cpFm9MbDgtZws6/N4Atn7xRfDY++xA0/7wIMKFOiCDsE0w+o5g2T5z9/1Kxnv1XpyOhNXJHMJGRcQPm1N7OL4A4QilChEkExZsIygJButYxXy8vSClp0J8WrKtcQ+uheip5qyzXhwsjrXCgR7sIL3XnYTE9s+VVrrM0QwnFEIj3eWHdr9iOyoEUKDi4MQ06wtqiRS8tT0Oi4rObtNGxaJqOzslu/fChZtObX/xgiqxyDLiYFt1xSpp+rMZ82GxoIcT5w9Vcn3Wxtb25v");
        private static int[] order = new int[] { 1,13,13,11,6,10,8,13,12,12,12,12,12,13,14 };
        private static int key = 110;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
