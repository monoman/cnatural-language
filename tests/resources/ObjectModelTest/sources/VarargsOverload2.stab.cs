public class VarargsOverload2 {
	    public static int m(int a, int b)
        {
                return 1;
        }

        public static int m(int x0, params int[] xr)
        {
                return 2;
        }
	
		public static int test()
		{
			return m(1, 2);
		}
}
