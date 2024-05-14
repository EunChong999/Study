using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
public class Program : MonoBehaviour {

    public GameObject cube_prefab;
    public GameObject cube_group;
    private bool instantiated = false;
    public Slider slider;
    private GameObject[] cube_array;
    private float[] heights;
    public Dropdown algorithms;
    private bool clicked = false, sorted = false, merged = false, test = false, reverse = false;
    private int i_b, i_i, j_b, j_i, i_s, i_g, i_c, ms_count = 0 ,swaps = 0, middle = 0;
    public Material mat_blue, mat_white;
    public Text swap_count_text, slider_text;

    private float[,] states;
    private int state_index = 0;

    private List<int> dummy;

    private Thread t1;

    private void Start() {

        i_b = 0;
        j_b = 0;
        i_i = 1;
        j_i = i_i - 1;
        i_s = 0;
        i_g = 0;
        i_c = 0;

        RenderBars((int)slider.value);
    }

    private void Update() {

        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }

        if (!sorted && clicked)
            OnClickSort();
    }

    public void OnClickSort() {        
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
                SelectionSort();
                break;
            case "Quick Sort":
                SelectionSort();
                break;
            case "Radix Sort (LSD)":
                SelectionSort();
                break;
            case "Radix Sort (MSD)":
                SelectionSort();
                break;
            case "Shell Sort":
                SelectionSort();
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

        if (i_b < heights.Length - 1) {
            if (j_b < heights.Length - i_b - 1) {
                if (heights[j_b] > heights[j_b + 1]) {
                    float t = heights[j_b];
                    heights[j_b] = heights[j_b + 1];
                    heights[j_b + 1] = t;
                    Swap(j_b, j_b + 1);
                }
                j_b += 1;
            }
            else {
                j_b = 0;
                i_b += 1;
            }
            ChangeColor(temp);
        }
    }


    private void InsertionSort() {  
        float[] temp = new float[heights.Length];
        Array.Copy(heights, 0, temp, 0, heights.Length);
        Array.Sort(temp);

        if (i_i < heights.Length){
            if (j_i >= 0 && heights[j_i] > heights[j_i + 1]) {
                float t = heights[j_i];
                heights[j_i] = heights[j_i + 1];
                heights[j_i + 1] = t;
                Swap(j_i, j_i + 1);
                j_i -= 1;
            }
            else {
                i_i += 1;
                j_i = i_i - 1;
            }
            ChangeColor(temp);
        }
    }

    private void SelectionSort() {  

        float[] temp = new float[heights.Length];
        Array.Copy(heights, 0, temp, 0, heights.Length);
        Array.Sort(temp);

        if (i_s < heights.Length) {
            float min = heights[i_s];
            int min_index = i_s;
            for (int i = i_s; i < heights.Length; i++) {
                if (heights[i] < min) {
                    min = heights[i];
                    min_index = i;
                }
            }
            if (i_s != min_index) {
                float t = heights[i_s];
                heights[i_s] = heights[min_index];
                heights[min_index] = t;
                Swap(i_s, min_index);
            }
            i_s += 1;
            ChangeColor(temp);
        }
    }

    private void HeapSort()
    {
        float[] temp = new float[heights.Length];
        Array.Copy(heights, 0, temp, 0, heights.Length);
        Array.Sort(temp);

        for (int i = heights.Length / 2 - 1; i >= 0; i--)
        {
            Heapify(heights, heights.Length, i);
        }

        for (int i = heights.Length - 1; i > 0; i--)
        {
            float t = heights[0];
            heights[0] = heights[i];
            heights[i] = t;
            Swap(0, i);
            Heapify(heights, i, 0);
        }

        ChangeColor(temp);
    }

    private void Heapify(float[] arr, int heapSize, int rootIndex)
    {
        int largest = rootIndex;
        int leftChild = 2 * rootIndex + 1;
        int rightChild = 2 * rootIndex + 2;

        if (leftChild < heapSize && arr[leftChild] > arr[largest])
        {
            largest = leftChild;
        }

        if (rightChild < heapSize && arr[rightChild] > arr[largest])
        {
            largest = rightChild;
        }

        if (largest != rootIndex)
        {
            float t = arr[rootIndex];
            arr[rootIndex] = arr[largest];
            arr[largest] = t;
            Heapify(arr, heapSize, largest);
        }
    }


    //*** Helper functions***
    private void Swap(int a, int b) {
        GameObject t = cube_array[a];
        cube_array[a] = cube_array[b];
        cube_array[b] = t;
        Vector3 v = cube_array[a].transform.position;
        cube_array[a].transform.position = new Vector3(cube_array[b].transform.position.x, v.y, 0f);
        cube_array[b].transform.position = new Vector3(v.x, cube_array[b].transform.position.y, 0f);
        swaps += 1;
        swap_count_text.text = "Swaps: " + swaps;
    }

    private void ChangeColor(float[] s) {
        for (int k = 0; k < heights.Length; k++) {
            Renderer rend = cube_array[k].GetComponent<Renderer>();
            if (heights[k] == s[k])
                rend.material = mat_blue;
            else
                rend.material = mat_white;
        }
    }
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
            cube_array[i].transform.localScale = new Vector3(thickness, heights[i], 1f);
        }
        instantiated = true;

        cube_group.transform.position = new Vector3(0, 0, 0);
    }




    public void Reset() {
        i_b = 0;
        j_b = 0;
        i_i = 1;
        j_i = i_i - 1;
        i_s = 0;
        i_g = 0;
        i_c = 0;
        swaps = 0;
        sorted = false;
        clicked = false;
        reverse = false;
        swap_count_text.text = "Swaps: ";
        for (int k = 0; k < heights.Length; k++) {
            Renderer rend = cube_array[k].GetComponent<Renderer>();
            rend.material = mat_white;
        }
    }
}
