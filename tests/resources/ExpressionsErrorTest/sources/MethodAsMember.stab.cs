public class C {
	public void m1(int i) {
	}
	
	public int getX() {
		return 0;
	}
	
	public void m() {
		m1(this.getX);
	}
}