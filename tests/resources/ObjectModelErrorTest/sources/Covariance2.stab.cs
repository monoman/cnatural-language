using java.lang;

public interface Covariance {
	Covariance copy();
}

public class CovarianceAux : Covariance {
	public Object copy() {
		return null;
	}
}
