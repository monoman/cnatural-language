public class BitCount {
	public static int test(int algo, int n) {
		switch (algo) {
		case 1:
			return bitcount1(n);
		case 2:
			return bitcount2(n);
		case 3:
			return bitcount3(n);
		default:
			return bitcount4(n);
		}
	}

	public static int bitcount1(int n) {
		int count = 0;
   		while (n != 0) {
      		count += n & 0x1;
      		n >>= 1;
   		}
   		return count;
	}
	
	public static int bitcount2(int n)  {
   		int count = 0 ;
   		while (n != 0)  {
      		count++;
      		n &= (n - 1) ; // sets the rightmost 1 bit in n to 0
   		}
   		return count ;
	}

	public static int bitcount3(int n)  {
		int count = 32;
   		n ^= -1;
   		while (n != 0) {
      		count--;
      		n &= (n - 1);
   		}
   		return count;
	}	
	
	private static int[] BitsInChar;
	private static void initializeBitsInChar() {
		BitsInChar = new int[256];
		for (int i = 0; i < 256; i++) {
			BitsInChar[i] = bitcount2(i);
		}
	}
	
	public static int bitcount4(int n) {
		if (BitsInChar == null) {
			initializeBitsInChar();
		}
		return BitsInChar[n  & 0xff]
      		+  BitsInChar[(n >>  8) & 0xff]
      		+  BitsInChar[(n >> 16) & 0xff]
      		+  BitsInChar[(n >> 24) & 0xff];
	}
	
}