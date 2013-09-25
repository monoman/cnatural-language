public class SwitchBlock {
	public static int test() {
		var c = 'a';
		var res = 1;
		if (c < 'b') 
			switch (c) {
			case 'a': {
				res++;
				break;
			}
			}
		
		return res;
	}
}
