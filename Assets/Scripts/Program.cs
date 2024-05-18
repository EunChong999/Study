using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using TMPro;

public class Program : MonoBehaviour {

    public AudioSource audioSource; 
    public GameObject cube_prefab;
    public GameObject cube_group;
    private bool instantiated = false;
    public Slider slider;
    private GameObject[] cube_array;
    public float[] heights;
    public Dropdown algorithms;
    private bool sorted = false;
    public int swaps = 0;
    public Material mat_green, mat_white, mat_red;
    public TextMeshProUGUI swap_count_text, slider_text;
    public float waitForSwapTime = 0;
    float waitForChangeTime = 0.025f;
    WaitForSeconds waitForSeconds;

    private void Start() {
        RenderBars((int)slider.value);
        waitForSeconds = new WaitForSeconds(waitForChangeTime);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }

        if (Mathf.Abs(waitForSwapTime - swaps * 0.05f) < 0.01f && swaps > 0) { 
            StartCoroutine(ChangeColor());
        }
    }

    public void OnClickSort() {

        if (sorted)
            return;

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

    #region Sorter
    private void BubbleSort()
    {
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
                    StartCoroutine(Swap(j, j + 1, temp));
                    swapped = true;
                }
            }

            if (!swapped)
            {
                break;
            }
        }

        sorted = true;
    }
    private void InsertionSort()
    {
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
                StartCoroutine(Swap(j + 1, j + 2, temp));
            }
            heights[j + 1] = key;
        }

        sorted = true;
    }
    private void SelectionSort()
    {
        float[] temp = new float[heights.Length];
        Array.Copy(heights, 0, temp, 0, heights.Length);
        Array.Sort(temp);  

        int n = heights.Length;

        for (int i = 0; i < n - 1; i++)
        {
            int minIndex = i;
            for (int j = i + 1; j < n; j++)
            {
                if (heights[j] < heights[minIndex])
                {
                    minIndex = j;
                }
            }

            if (minIndex != i)
            {
                float tempVal = heights[i];
                heights[i] = heights[minIndex];
                heights[minIndex] = tempVal;
                StartCoroutine(Swap(i, minIndex, temp));
            }
        }

        sorted = true;
    }
    #region HeapSort
    public void HeapSort()
    {
        float[] temp = new float[heights.Length];
        Array.Copy(heights, 0, temp, 0, heights.Length);
        Array.Sort(temp);

        for (int i = temp.Length / 2 - 1; i >= 0; i--)
        {
            Heapify(heights, temp.Length, i);
        }

        for (int i = temp.Length - 1; i > 0; i--)
        {
            float tempo = heights[0];
            heights[0] = heights[i];
            heights[i] = tempo;
            StartCoroutine(Swap(0, i, temp));

            Heapify(heights, i, 0);
        }

        sorted = true;
    }
    private void Heapify(float[] array, int heapSize, int rootIndex)
    {
        int largest = rootIndex;
        int leftChild = 2 * rootIndex + 1;
        int rightChild = 2 * rootIndex + 2;

        if (leftChild < heapSize && array[leftChild] > array[largest])
        {
            largest = leftChild;
        }

        if (rightChild < heapSize && array[rightChild] > array[largest])
        {
            largest = rightChild;
        }

        if (largest != rootIndex)
        {
            float swap = array[rootIndex];
            array[rootIndex] = array[largest];
            array[largest] = swap;
            StartCoroutine(Swap(rootIndex, largest, array));

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
                if (array[k] != leftHalf[i]) 
                {
                    int targetIndex = FindOriginalIndex(array, k, endIndex, leftHalf[i]); 
                    if (targetIndex != -1) Swap(array, k, targetIndex); 
                }
                array[k] = leftHalf[i];  
                i++;
            }
            else
            {
                if (array[k] != rightHalf[j]) 
                {
                    int targetIndex = FindOriginalIndex(array, k, endIndex, rightHalf[j]); 
                    if (targetIndex != -1) Swap(array, k, targetIndex); 
                }
                array[k] = rightHalf[j]; 
                j++;
            }
            k++;
        }

        while (i < leftSize)
        {
            if (array[k] != leftHalf[i]) 
            {
                int targetIndex = FindOriginalIndex(array, k, endIndex, leftHalf[i]);
                if (targetIndex != -1) Swap(array, k, targetIndex); 
            }
            array[k] = leftHalf[i]; 
            i++;
            k++;
        }

        while (j < rightSize)
        {
            if (array[k] != rightHalf[j]) 
            {
                int targetIndex = FindOriginalIndex(array, k, endIndex, rightHalf[j]); 
                if (targetIndex != -1) Swap(array, k, targetIndex); 
            }
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
        StartCoroutine(Swap(a, b, arr));
    }
    #endregion
    #endregion

    #region Helper
    IEnumerator Swap(int a, int b, float[] arr)
    {
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
        swaps += 1;
        swap_count_text.text = "Swaps : " + swaps;
    }

    IEnumerator ChangeColor()
    {
        foreach (GameObject cube in cube_array) 
        {
            yield return waitForSeconds;
            Renderer rend = cube.GetComponent<Renderer>();
            rend.material = mat_green;
        }
    }

    private int FindOriginalIndex(float[] array, int beginIndex, int endIndex, float value)
    {
        for (int i = beginIndex; i <= endIndex; i++)
        {
            if (array[i] == value)
            {
                return i;
            }
        }
        return -1; 
    }

    public void OnSliderMoved()
    {
        Reset();
        RenderBars((int)slider.value);
    }

    public void RenderBars(int n)
    {
        slider_text.text = "Number of items : " + n;
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

        cube_group.transform.position = new Vector3(-32.5f, 0, 0);
    }

    public void Reset()
    {
        StopAllCoroutines();
        waitForSwapTime = 0;
        swaps = 0;
        sorted = false;
        swap_count_text.text = "Swaps : 0";
        for (int k = 0; k < heights.Length; k++)
        {
            Renderer rend = cube_array[k].GetComponent<Renderer>();
            rend.material = mat_green;
        }
    }
    #endregion
}
