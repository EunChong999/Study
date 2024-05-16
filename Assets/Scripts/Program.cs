using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using Unity.Burst.Intrinsics;
using static UnityEngine.GraphicsBuffer;

public class Program : MonoBehaviour {

    public AudioSource audioSource; 
    public GameObject cube_prefab;
    public GameObject cube_group;
    private bool instantiated = false;
    public Slider slider;
    private GameObject[] cube_array;
    public float[] heights;
    public Dropdown algorithms;
    private bool clicked = false, sorted = false, merged = false, test = false, reverse = false;
    private int accesses = 0;
    public Material mat_red, mat_white;
    public Text swap_count_text, slider_text;

    private float[,] states;
    private float waitForSwapTime = 0.1f;
    private float waitForSubstituteTime = 0.1f;
    private float waitForColorTime = 0.1f;
    private int state_index = 0;

    private List<int> dummy;

    private Thread t1;

    private void Start() {
        RenderBars((int)slider.value);
    }

    private void Update() {

        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }
    }

    public void OnClickSort() {

        if (sorted)
            return;

        clicked = true;
        string algorithm;
        algorithm = algorithms.options[algorithms.value].text;
        switch (algorithm) {
            case "Bubble Sort":
                BubbleSort();
                break;
            case "Insertion Sort":
                InsertionSort();
                break;
            case "Selection Sort":
                SelectionSort();
                break;
            case "Heap Sort":
                HeapSort();
                break;
            case "Merge Sort":
                MergeSort();
                break;
            case "Quick Sort":
                QuickSort();
                break;
            default:
                break;
        }
     }

    //***Sorting Algorithms***

    private void BubbleSort() {
        float[] temp = new float[heights.Length];
        Array.Copy(heights, 0, temp, 0, heights.Length);
        Array.Sort(temp);  

        int n = heights.Length;
        bool swapped;

        for (int i = 0; i < n - 1; i++)
        {
            swapped = false;
            for (int j = 0; j < n - i - 1; j++)
            {
                if (heights[j] > heights[j + 1])
                {
                    float tempVal = heights[j];
                    heights[j] = heights[j + 1];
                    heights[j + 1] = tempVal;
                    StartCoroutine(Swap(j, j + 1));
                    swapped = true;
                }

                //StartCoroutine(ChangeColor(temp));
            }

            if (!swapped)
            {
                break;
            }
        }

        sorted = true;
    }

    private void InsertionSort() {
        float[] temp = new float[heights.Length];
        Array.Copy(heights, 0, temp, 0, heights.Length);
        Array.Sort(temp);  

        int n = heights.Length;

        for (int i = 1; i < n; i++)
        {
            float key = heights[i];
            int j = i - 1;

            while (j >= 0 && heights[j] > key)
            {
                heights[j + 1] = heights[j];
                j = j - 1;
                StartCoroutine(Swap(j + 1, j + 2));
                //StartCoroutine(ChangeColor(temp));
            }
            heights[j + 1] = key;

            //StartCoroutine(ChangeColor(temp));
        }

        sorted = true;
    }

    private void SelectionSort() {
        float[] temp = new float[heights.Length];
        Array.Copy(heights, 0, temp, 0, heights.Length);
        Array.Sort(temp);  // 정렬된 배열 생성

        int n = heights.Length;

        for (int i = 0; i < n - 1; i++)
        {
            // Find the minimum element in unsorted array
            int minIndex = i;
            for (int j = i + 1; j < n; j++)
            {
                if (heights[j] < heights[minIndex])
                {
                    minIndex = j;
                }
            }

            // Swap the found minimum element with the first element
            if (minIndex != i)
            {
                float tempVal = heights[i];
                heights[i] = heights[minIndex];
                heights[minIndex] = tempVal;
                StartCoroutine(Swap(i, minIndex));
            }

            //StartCoroutine(ChangeColor(temp));
        }

        sorted = true;
    }

    #region HeapSort
    public void HeapSort()
    {
        float[] temp = new float[heights.Length];
        Array.Copy(heights, 0, temp, 0, heights.Length);
        Array.Sort(temp);

        // Build a max heap
        for (int i = temp.Length / 2 - 1; i >= 0; i--)
        {
            Heapify(heights, temp.Length, i);
        }

        // One by one extract an element from heap
        for (int i = temp.Length - 1; i > 0; i--)
        {
            // Move current root to end
            float tempo = heights[0];
            heights[0] = heights[i];
            heights[i] = tempo;
            StartCoroutine(Swap(0, i));

            // Reduce the heap size by one and heapify the root element
            Heapify(heights, i, 0);
            //StartCoroutine(ChangeColor(heights));
        }

        sorted = true;
    }
    private void Heapify(float[] array, int heapSize, int rootIndex)
    {
        int largest = rootIndex;
        int leftChild = 2 * rootIndex + 1;
        int rightChild = 2 * rootIndex + 2;

        // If left child is larger than root
        if (leftChild < heapSize && array[leftChild] > array[largest])
        {
            largest = leftChild;
        }

        // If right child is larger than largest so far
        if (rightChild < heapSize && array[rightChild] > array[largest])
        {
            largest = rightChild;
        }

        // If largest is not root
        if (largest != rootIndex)
        {
            float swap = array[rootIndex];
            array[rootIndex] = array[largest];
            array[largest] = swap;
            StartCoroutine(Swap(rootIndex, largest));

            // Recursively heapify the affected sub-tree
            Heapify(array, heapSize, largest);
        }
    }
    #endregion

    #region MergeSort
    private void MergeSort()
    {
        float[] temp = new float[heights.Length];
        Array.Copy(heights, 0, temp, 0, heights.Length);
        Array.Sort(temp);  

        MergeSortHelper(heights, 0, heights.Length - 1);

        sorted = true;
    }

    private void MergeSortHelper(float[] array, int beginIndex, int endIndex)
    {
        if (beginIndex < endIndex)
        {
            int midIndex = (beginIndex + endIndex) / 2;
            MergeSortHelper(array, beginIndex, midIndex);
            MergeSortHelper(array, midIndex + 1, endIndex);
            Merge(array, beginIndex, midIndex, endIndex);
            StartCoroutine(Substitute(array));
            //StartCoroutine(ChangeColor(array));
        }
    }

    private void Merge(float[] array, int beginIndex, int midIndex, int endIndex)
    {
        int leftSize = midIndex - beginIndex + 1;
        int rightSize = endIndex - midIndex;
        float[] leftHalf = new float[leftSize];
        float[] rightHalf = new float[rightSize];

        Array.Copy(array, beginIndex, leftHalf, 0, leftSize);
        Array.Copy(array, midIndex + 1, rightHalf, 0, rightSize);

        int i = 0, j = 0, k = beginIndex;

        while (i < leftSize && j < rightSize)
        {
            if (leftHalf[i] <= rightHalf[j])
            {
                array[k] = leftHalf[i];
                i++;
            }
            else
            {
                array[k] = rightHalf[j];
                j++;
            }
            k++;
        }

        while (i < leftSize)
        {
            array[k] = leftHalf[i];
            i++;
            k++;
        }

        while (j < rightSize)
        {
            array[k] = rightHalf[j];
            j++;
            k++;
        }
    }
    #endregion

    #region QuickSort
    private void QuickSort()
    {
        float[] temp = new float[heights.Length];
        Array.Copy(heights, 0, temp, 0, heights.Length);
        Array.Sort(temp);

        QuickSortHelper(heights, 0, temp.Length - 1);

        sorted = true;
    }

    public void QuickSortHelper(float[] arr, int low, int high)
    {
        if (low < high)
        {
            int pivotIndex = Partition(arr, low, high);
            QuickSortHelper(arr, low, pivotIndex - 1);
            QuickSortHelper(arr, pivotIndex + 1, high);
            //StartCoroutine(ChangeColor(arr));
        }
    }

    private int Partition(float[] arr, int low, int high)
    {
        float pivot = arr[high];
        int i = low - 1;

        for (int j = low; j < high; j++)
        {
            if (arr[j] <= pivot)
            {
                i++;
                Swap(arr, i, j);
            }
        }
        Swap(arr, i + 1, high);
        return i + 1;
    }

    private void Swap(float[] arr, int a, int b)
    {
        float temp = arr[a];
        arr[a] = arr[b];
        arr[b] = temp;
        StartCoroutine(Swap(a, b));
    }
    #endregion

    //*** Helper functions***
    IEnumerator Swap(int a, int b) {
        WaitForSeconds waitForSeconds = new WaitForSeconds(waitForSwapTime);
        waitForSwapTime += 0.05f;
        yield return waitForSeconds;
        audioSource.Play();
        GameObject t = cube_array[a];
        cube_array[a] = cube_array[b];
        cube_array[b] = t;
        Vector3 v = cube_array[a].transform.position;
        cube_array[a].transform.position = new Vector3(cube_array[b].transform.position.x, v.y, 0f);
        cube_array[b].transform.position = new Vector3(v.x, cube_array[b].transform.position.y, 0f);
        accesses += 1;
        swap_count_text.text = "Accesses: " + accesses;
    }

    IEnumerator Substitute(float[] array)
    {
        WaitForSeconds waitForSeconds = new WaitForSeconds(waitForSubstituteTime);
        waitForSubstituteTime += 0.05f;
        yield return waitForSeconds;
        audioSource.Play();
        for (int i = 0; i < array.Length; i++)
        {
            // 오브젝트의 스케일을 조정합니다.
            Vector3 newScale = cube_array[i].transform.localScale;
            newScale.y = array[i];
            cube_array[i].transform.localScale = newScale;

            // 오브젝트의 위치를 조정합니다.
            Vector3 newPosition = cube_array[i].transform.position;
            newPosition.y = (array[i] / 2) - 35f; // 예를 들어, y 좌표를 배열의 값으로 설정합니다.
            cube_array[i].transform.position = newPosition;
        }
        accesses += 1;
        swap_count_text.text = "Accesses: " + accesses;
    }

    //IEnumerator ChangeColor(float[] s) {
    //    WaitForSeconds waitForSeconds = new WaitForSeconds(waitForColorTime);
    //    waitForColorTime += 0.25f;
    //    for (int k = 0; k < heights.Length; k++) {
    //        yield return waitForSeconds;
    //        Renderer rend = cube_array[k].GetComponent<Renderer>();
    //        if (heights[k] == s[k])
    //            rend.material = mat_red;
    //        else
    //            rend.material = mat_white;
    //    }
    //}
    
    public void OnSliderMoved() {
        Reset();
        RenderBars((int)slider.value);
    }

    public void RenderBars(int n)
    {
        slider_text.text = "Number of items: " + n;
        if (instantiated)
        {
            for (int i = 0; i < cube_array.Length; i++)
            {
                Destroy(cube_array[i]);
                heights[i] = -1;
            }
        }
        cube_array = new GameObject[n];
        heights = new float[n];

        // 전체 두께의 합
        float totalThickness = 130f;

        // 각 큐브의 두께 계산
        float thickness = totalThickness / n;

        cube_group.transform.position = new Vector3(-(thickness / 2), 0, 0);

        float start = -65f;
        float z = 0f;
        for (int i = 0; i < n; i++)
        {
            heights[i] = UnityEngine.Random.Range(5, 75);

            // 큐브의 높이에 따라 위치를 조정하지 않고, 두께만 설정
            cube_array[i] = Instantiate(cube_prefab, new Vector3(start + (i * (thickness)), (heights[i] / 2) - 35f, z), Quaternion.identity);
            cube_array[i].transform.parent = cube_group.transform;
            cube_array[i].transform.localScale = new Vector3(thickness * 0.9f, heights[i], 1f);
        }
        instantiated = true;

        cube_group.transform.position = new Vector3(0, 0, 0);
    }

    public void Reset() {
        StopAllCoroutines();
        waitForSwapTime = 0.1f;
        waitForSubstituteTime = 0.1f;
        waitForColorTime = 0.1f;
        accesses = 0;
        sorted = false;
        clicked = false;
        reverse = false;
        swap_count_text.text = "Accesses: ";
        for (int k = 0; k < heights.Length; k++) {
            Renderer rend = cube_array[k].GetComponent<Renderer>();
            rend.material = mat_white;
        }
    }
}
