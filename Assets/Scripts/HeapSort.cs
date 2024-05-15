using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeapSort : MonoBehaviour
{
    [SerializeField] Program program;

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
        float[] temp = new float[program.heights.Length];
        Array.Copy(program.heights, 0, temp, 0, program.heights.Length);
        //Array.Sort(temp);

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
