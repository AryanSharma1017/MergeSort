using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Vector
{
    public class Vector<T> where T : IComparable<T>
    {

        // This constant determines the default number of elements in a newly created vector.
        // It is also used to extended the capacity of the existing vector
        private const int DEFAULT_CAPACITY = 10;

        // This array represents the internal data structure wrapped by the vector class.
        // In fact, all the elements are to be stored in this private  array. 
        // You will just write extra functionality (methods) to make the work with the array more convenient for the user.
        private T[] data;

        // This property represents the number of elements in the vector
        public int Count { get; private set; } = 0;

        // This property represents the maximum number of elements (capacity) in the vector
        public int Capacity
        {
            get { return data.Length; }
        }

        // This is an overloaded constructor
        public Vector(int capacity)
        {
            data = new T[capacity];
        }

        // This is the implementation of the default constructor
        public Vector() : this(DEFAULT_CAPACITY) { }

        // An Indexer is a special type of property that allows a class or structure to be accessed the same way as array for its internal collection. 
        // For example, introducing the following indexer you may address an element of the vector as vector[i] or vector[0] or ...
        public T this[int index]
        {
            get
            {
                if (index >= Count || index < 0) throw new IndexOutOfRangeException();
                return data[index];
            }
            set
            {
                if (index >= Count || index < 0) throw new IndexOutOfRangeException();
                data[index] = value;
            }
        }

        // This private method allows extension of the existing capacity of the vector by another 'extraCapacity' elements.
        // The new capacity is equal to the existing one plus 'extraCapacity'.
        // It copies the elements of 'data' (the existing array) to 'newData' (the new array), and then makes data pointing to 'newData'.
        private void ExtendData(int extraCapacity)
        {
            T[] newData = new T[Capacity + extraCapacity];
            for (int i = 0; i < Count; i++) newData[i] = data[i];
            data = newData;
        }

        // This method adds a new element to the existing array.
        // If the internal array is out of capacity, its capacity is first extended to fit the new element.
        public void Add(T element)
        {
            if (Count == Capacity) ExtendData(DEFAULT_CAPACITY);
            data[Count++] = element;
        }

        // This method searches for the specified object and returns the zero‐based index of the first occurrence within the entire data structure.
        // This method performs a linear search; therefore, this method is an O(n) runtime complexity operation.
        // If occurrence is not found, then the method returns –1.
        // Note that Equals is the proper method to compare two objects for equality, you must not use operator '=' for this purpose.
        public int IndexOf(T element)
        {
            for (var i = 0; i < Count; i++)
            {
                if (data[i].Equals(element)) return i;
            }
            return -1;
        }

        public ISorter Sorter { set; get; } = new DefaultSorter();

        internal class DefaultSorter : ISorter
        {
            public void Sort<K>(K[] sequence, IComparer<K> comparer) where K : IComparable<K>
            {
                if (comparer == null) comparer = Comparer<K>.Default;
                Array.Sort(sequence, comparer);
            }
        }

        public void Sort()
        {
            if (Sorter == null) Sorter = new DefaultSorter();
            Array.Resize(ref data, Count);
            Sorter.Sort(data, null);
        }

        public void Sort(IComparer<T> comparer)
        {
            if (Sorter == null) Sorter = new DefaultSorter();
            Array.Resize(ref data, Count);
            if (comparer == null) Sorter.Sort(data, null);
            else Sorter.Sort(data, comparer);
        }

        

    }


    class RandomizedQuickSort : ISorter  // Quick Sort (Randomised)
    {

        private Random rnd {get;set;} = new Random(); //Declaring an instyance of the random class

        private int rndPivot(int Start, int End) => rnd.Next(Start, End); // Returns a random number in between start(inclusive) and End(exclusive)


        public RandomizedQuickSort()
        {

        }


        public void Sort<K>(K[] sequence, IComparer<K> comparer) where K : IComparable<K>
        {
            if (comparer == null) comparer = Comparer<K>.Default;
            Sort(sequence, comparer, 0, sequence.Length-1);
        }

        private void swap<K>(K[] sequence, int swapA, int swapB) // creating a swap method with basic swap algorithm
        {
            K C = sequence[swapA];
            sequence[swapA] = sequence[swapB];
            sequence[swapB] = C;
        }

        private int Partition<K>(K[] sequence, IComparer<K> comparer, int Start, int End) where K : IComparable<K>
        {
            // a partition method is made to place the pivot at its original position and make sure that all the elements on the left are smaller then pivot whereas all on the right are bigger
            int PivotIndex = rndPivot(Start,End);

            swap(sequence, PivotIndex, End);

            K Final_Pivot = sequence[End];

            int count = Start;
            for(int i = Start;i<End;i++)
            {
                if(comparer.Compare(sequence[i],Final_Pivot)<=0)
                {
                    swap(sequence, i, count);
                    count++;
                }
            }

            sequence[End] = sequence[count];
            sequence[count] = Final_Pivot;
            return count;
        }
        public void Sort<K>(K[] sequence, IComparer<K> comparer, int Start, int End) where K : IComparable<K>
        {
            if(Start >= End)
            {
                return;
            }

            if(Start<End)
            {
                int Sorting = Partition(sequence,comparer,Start,End);
                Sort(sequence,comparer,Start,Sorting-1); // recursive call
                Sort(sequence,comparer,Sorting+1,End);
            }
        }
    }



    class MergeSortTopDown : ISorter // Top Down Merge Sort -Reccursive Approach
    {
        public MergeSortTopDown()
        {
             
        }


        private void Merging<K>(K[]first_half,K[] second_half, K[] sequence, IComparer<K> comparer) where K : IComparable<K>
        {
            int Length1 = 0, Length2 = 0, resuting =0;
// basic code for adding up of 2 sorted arrays
            while(Length1 < first_half.Length && Length2 < second_half.Length)
            {
                if(comparer.Compare(first_half[Length1], second_half[Length2]) <=0)
                {
                    sequence[resuting++] = first_half[Length1++];
                }

                else
                {
                    sequence[resuting++] = second_half[Length2++];
                }
                
            }

            while(Length1 < first_half.Length)
            {
                sequence[resuting++] = first_half[Length1++];
              
            }

            while(Length2 < second_half.Length)
            {
                sequence[resuting++] = second_half[Length2++];
                
            }
        }

        public void Sort<K>(K[] sequence, IComparer<K> comparer) where K : IComparable<K>
        {
            int n = sequence.Length;
            //Checking the base case, that means the array has 1 element so their is no need for sorting
            if(n<=1)
            {
                return;
            }

            int middle = n/2; //getting the middle element

            K[] arr1 = new K[middle]; //Creating 2 new array for the copy of the previous array to divide in 2
            K[] arr2 = new K[n-middle];

            Array.Copy(sequence,0,arr1,0,middle);  // here the first halp is being copied
            Array.Copy(sequence,middle,arr2,0,n-middle); // here the second half is being copied

            Sort(arr1,comparer);
            Sort(arr2,comparer);
            // now we will merge all the arrays
            Merging(arr1,arr2,sequence,comparer); // merging all the subarrays
        }  
    }


    class MergeSortBottomUp : ISorter  // Bottom Up merge Sort -Iterative Approach
    {
        public MergeSortBottomUp()
        {

        }

        // In this sort we will first breakdown the array into subarrays of element 1, and will start from their only

        private void Merge<K>(K[] sequence ,K[] temporary, int First, int Mid, int Last, IComparer<K> comparer) where K : IComparable<K>
        {
            int L1 = Mid+1;
            int L2 = First;
            int L3 = First;
         
            while(L2 <= Mid && L1 <= Last)
            {
                if(comparer.Compare(sequence[L2],sequence[L1])<=0)
                {
                    temporary[L3] = sequence[L2];
                    L2++;
                }

                else
                {
                    temporary[L3] = sequence[L1];
                    L1++;
                }

                L3++;
            }

            while(L2<= Mid)
            {
                temporary[L3] = sequence[L2];
                L2++;
                L3++;
            }

            while(L1<= Last)
            {
                temporary[L3] = sequence[L1];
                L1++;
                L3++;
            }

            for(int a = First; a<= Last; a++)
            {
                sequence[a] = temporary[a];
            }


        }

        public void Sort<K>(K[] sequence, IComparer<K> comparer) where K : IComparable<K>
        {
            int n = sequence.Length;

            K[] temporary = new K[n];  // A temporary array for storing the value in case of sub arrays

            for(int i =1; i<n; i*=2)  // till i *=2 because for every round the merging elements grow 2x, so for first round in total 2 elements were merged then 4 then 8 then so on
            {
                for(int j =0; j<n-i; j +=2*i) // this loop is taking a span of 2 lists
                {
                    int mid = j + i -1;
                    int end = Math.Min(j+2*i-1, n-1);
                    Merge(sequence,temporary,j,mid,end,comparer); // here the merge is performed

                }
            }
        }
    }
}