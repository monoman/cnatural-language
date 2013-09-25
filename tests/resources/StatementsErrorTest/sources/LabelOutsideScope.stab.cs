public class C {
	public int method(int i) {
		if (i > 0) {
			goto label;
		}
		if (i < 0) {
			label:
			i = -i;
		}
		return 1;
	}
}