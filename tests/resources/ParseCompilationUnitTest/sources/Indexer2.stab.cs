class C {
    private int[][] t;
    public int this[int i, int j] {
        get {
            return t[i][j];
        }
        set {
            t[i][j] = value;
        }
    }
}