public class Quicksort {
  
 	public static void test(int[] a) {  
  		quicksort(a, 0, sizeof(a) - 1);  
 	}
  
	private static void swap(int[] a, int i, int j) {  
  		int temp = a[i];  
  		a[i] = a[j];  
  		a[j] = temp;  
 	}  
  
 	private static void quicksort(int[] a, int L, int R) {  
  		int m = a[(L + R) / 2];  
  		int i = L;  
  		int j = R;  
  		while (i <= j) {  
   			while (a[i] < m)  
    			i++;  
   			while (a[j] > m)  
    			j--;  
 	  		if (i <= j) {  
    			swap(a, i, j);  
    			i++;  
    			j--;  
   			}  
  		}  
  		if (L < j)  
   			quicksort(a, L, j);  
  		if (R > i)  
   			quicksort(a, i, R);  
 	}  
}
