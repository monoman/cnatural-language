public class Destructor {
	public static void test() {
        #pragma warning disable 281
		var obj = new Destructor();
	}
	
	~Destructor() {
		cleanup();
	}
	
	private void cleanup() {
	}
}
