using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeapSort : MonoBehaviour
{
    int swapCount = 0;

    public void Sort(float[] arr)
    {
        int n = arr.Length;

        // Build heap (rearrange array)
        for (int i = n / 2 - 1; i >= 0; i--)
        {
            Heapify(arr, n, i);
        }

        // One by one extract an element from heap
        for (int i = n - 1; i >= 0; i--)
        {
            // Move current root to end
            float temp1 = arr[0];
            arr[0] = arr[i];
            arr[i] = temp1;

            // call max heapify on the reduced heap
            Heapify(arr, i, 0);
        }
    }

    // To heapify a subtree rooted with node i
    void Heapify(float[] arr, int n, int i)
    {
        int largest = i;  // Initialize largest as root
        int left = 2 * i + 1;  // left = 2*i + 1
        int right = 2 * i + 2;  // right = 2*i + 2

        // If left child is larger than root
        if (left < n && arr[left] > arr[largest])
            largest = left;

        // If right child is larger than largest so far
        if (right < n && arr[right] > arr[largest])
            largest = right;

        // If largest is not root
        if (largest != i)
        {
            // Swap
            float swap = arr[i];
            arr[i] = arr[largest];
            arr[largest] = swap;
            swapCount++;

            // Recursively heapify the affected sub-tree
            Heapify(arr, n, largest);
        }
    }

    private void Start()
    {
        float[] temp = new float[] { 57, 49, 74, 15, 31, 55, 93, 50, 25, 33, 22, 27, 1, 86, 7, 48, 89, 46, 30, 41, 94, 98, 44, 43, 68, 83, 32, 100, 0, 9, 64, 23, 92, 18, 72, 97, 66, 51, 99, 12, 88, 91, 11, 39, 76, 53, 2, 13, 54, 3, 52, 28, 5, 34, 81, 36, 67, 82, 69, 4, 71, 77, 14, 59, 45, 38, 47, 70, 65, 19, 96, 90, 84, 58, 37, 79, 29, 26, 75, 20, 16, 6, 40, 17, 62, 42, 63, 24, 80, 35, 95, 8, 56, 10, 85, 73, 60, 87, 61, 21 };

        // 배열 요소를 한 줄에 출력
        string arrayString = string.Join(", ", temp);
        Debug.Log(arrayString);

        Sort(temp);

        // 배열 요소를 한 줄에 출력
        arrayString = string.Join(", ", temp);
        Debug.Log(arrayString);

        Debug.Log(swapCount);
    }
}
