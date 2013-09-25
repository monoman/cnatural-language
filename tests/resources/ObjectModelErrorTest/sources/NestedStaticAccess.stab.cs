class C {
    void m() {}
    class D {
        void n() {
            C.m();
        }
    }
}
