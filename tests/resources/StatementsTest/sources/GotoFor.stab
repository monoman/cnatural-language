public class GotoFor {
	public static int test() {
	    int n = 0;
        int prev = -1;
        label: for (int i = 1; i < 10; i++) {
            if (prev == i) {
                break;
            }
            if (i % 2 == 0) {
                prev = i;
                goto label;
            }
            n += i;
        }
		return n;
	}
}
