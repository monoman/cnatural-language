using java.lang;
using java.util;

public class For2 {
	public static string test() {
		var result = "";
		var l1 = new ArrayList<string> { "A", "B", "C" };
		var l2 = new ArrayList<string> { "A", "C" };
		for (int i = 0, j = 0; i < l1.size() && j < l2.size(); i++, j++) {
			if (l1[i].equals(l2[j])) {
				result += l1[i];
			} else {
				break;
			}
		}
		for (int i = l1.size() - 1, j = l2.size() - 1; i >= 0 && j >= 0; --i, --j) {
			if (l1[i].equals(l2[j])) {
				result += l1[i];
			} else {
				break;
			}
		}
		return result;
	}
}
