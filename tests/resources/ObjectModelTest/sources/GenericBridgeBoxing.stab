using java.lang;

public class GenericBridgeBoxing : GenericBridgeBoxingAux<Integer>, GenericBridgeBoxingAux2<Integer> {
	public static Integer test() {
		var obj = new GenericBridgeBoxing();
		return obj.method(1);
	}
	
	public Integer method(Integer i) {
		return this[i];
	}
	
	int this[int i] {
		get {
			return i;
		}
	}
}

public interface GenericBridgeBoxingAux<T> : GenericBridgeBoxingAux2<T> {
	T method(T t);
}

public interface GenericBridgeBoxingAux2<T> : GenericBridgeBoxingAux3<T> {
	T method(T t);
}

public interface GenericBridgeBoxingAux3<T> {
	T method(T t);
}
