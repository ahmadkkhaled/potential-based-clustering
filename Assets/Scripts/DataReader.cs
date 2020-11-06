using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class DataReader : MonoBehaviour
{

    private string dataPath = "";
    private int xColumn, yColumn, xStretchColumn, yStretchColumn, rotationColumn, massColumn, labelColumn;
    private double FIXED_X_STRETCH = 1, FIXED_Y_STRETCH, FIXED_ROTATION, FIXED_MASS;
    private string FIXED_LABEL;
    private bool isFixedXStretch, isFixedYStretch, isFixedRotation, isFixedMass, isNoneLabel;

    private enum DROPDOWN_INDEX : int
    {
        X_AXIS,
        Y_AXIS,
        X_STRETCH,
        Y_STRETCH,
        ROTATION,
        MASS,
        LABEL
    }
    private enum INPUT_FIELD_INDEX : int
    {
        X_SRETCH,
        Y_STRETCH,
        ROTATION,
        MASS
    }

    public Dropdown[] dropDown;
    public InputField[] inputField;
    public GameObject mappingPanel;
    public bool isDataReady = false;

    private void Initialize()
    {
        isDataReady = false;
        isFixedXStretch = isFixedYStretch = isFixedRotation = isFixedMass = isNoneLabel = false;
}
    private void PopulateDropdown(int nColumns)
    {
        List<string> options = new List<string>();
        for (int i = 0; i < nColumns; i++)
            options.Add("" + i);

        for (int i = 0; i < dropDown.Length; i++)
            dropDown[i].AddOptions(options);
    }

    private double GetFixedValue(InputField input)
    {
        return double.Parse(input.text);
    }
    private string GetSelectedOption(Dropdown dropdown)
    {
        return dropdown.options[dropdown.value].text;
    }

    public void ReadData() // ImportButton::OnClick()
    {
        dataPath = UnityEditor.EditorUtility.OpenFilePanel("Select Dataset", "", "csv");
        if (string.IsNullOrEmpty(dataPath))
        {
            isDataReady = false;
            Debug.Log("dataPath: " + dataPath);
            return;
        }

        mappingPanel.SetActive(true);
        Initialize();

        string line = File.ReadLines(dataPath).First();
        string[] columns = line.Split(',');

        PopulateDropdown(columns.Length);
        
    }

    // FIXME: This is one ugly implementation of data mapping, I am honestly disappointed in myself
    public void MapDataToColumns() // OKButton::OnClick()
    {
        xColumn = int.Parse(GetSelectedOption(dropDown[(int)DROPDOWN_INDEX.X_AXIS]));
        yColumn = int.Parse(GetSelectedOption(dropDown[(int)DROPDOWN_INDEX.Y_AXIS]));

        string option = GetSelectedOption(dropDown[(int)DROPDOWN_INDEX.X_STRETCH]);
        if (option.Equals("Fixed"))
        {
            isFixedXStretch = true;
            FIXED_X_STRETCH = GetFixedValue(inputField[(int)INPUT_FIELD_INDEX.X_SRETCH]);
        }
        else
        {
            isFixedXStretch = false;
            xStretchColumn = int.Parse(option);
        }

        option = GetSelectedOption(dropDown[(int)DROPDOWN_INDEX.Y_STRETCH]);
        if (option.Equals("Fixed"))
        {
            isFixedYStretch = true;
            FIXED_Y_STRETCH = GetFixedValue(inputField[(int)INPUT_FIELD_INDEX.Y_STRETCH]);
        }
        else
        {
            isFixedYStretch = false;
            yStretchColumn = int.Parse(option);
        }

        option = GetSelectedOption(dropDown[(int)DROPDOWN_INDEX.ROTATION]);
        if (option.Equals("Fixed"))
        {
            isFixedRotation = true;
            FIXED_ROTATION = GetFixedValue(inputField[(int)INPUT_FIELD_INDEX.ROTATION]);
        }
        else
        {
            isFixedRotation = false;
            rotationColumn = int.Parse(option);
        }

        option = GetSelectedOption(dropDown[(int)DROPDOWN_INDEX.MASS]);
        if(option.Equals("Fixed"))
        {
            isFixedMass = true;
            FIXED_MASS = GetFixedValue(inputField[(int)INPUT_FIELD_INDEX.MASS]);
        }
        else
        {
            isFixedMass = false;
            massColumn = int.Parse(option);
        }

        option = GetSelectedOption(dropDown[(int)DROPDOWN_INDEX.LABEL]);
        if (option.Equals("None"))
        {
            isNoneLabel = true;
            FIXED_LABEL = "None";
        }
        else
        {
            isNoneLabel = false;
            labelColumn = int.Parse(option);
        }

        Debug.Log("xColumn " + xColumn);
        Debug.Log("yColumn " + yColumn);
        Debug.Log("xStretchColumn " + xStretchColumn);
        Debug.Log("yStretchColumn " + yStretchColumn);
        Debug.Log("rotationColumn " + rotationColumn);
        Debug.Log("massColumn " + massColumn);
        Debug.Log("labelColumn " + labelColumn);

        Debug.Log("FIXED_X_STRETCH " + isFixedXStretch + " " + FIXED_X_STRETCH);
        Debug.Log("FIXED_Y_STRETCH " + isFixedYStretch + " " + FIXED_Y_STRETCH);
        Debug.Log("FIXED_ROTATION " + isFixedRotation + " " +  FIXED_ROTATION);
        Debug.Log("FIXED_MASS " + isFixedMass + " " + FIXED_MASS);
        Debug.Log("FIXED_LABEL " + isNoneLabel + " " + FIXED_LABEL);

        isDataReady = true;
        mappingPanel.SetActive(false);
    }

    public void Populate(List<DataPoint> dataPoints)
    {
        Assert.IsFalse(string.IsNullOrEmpty(dataPath));
        string[] lines = File.ReadAllLines(dataPath);
        foreach(string line in lines){
            string[] columns = line.Split(',');

            double x = double.Parse(columns[xColumn]);
            double y = double.Parse(columns[yColumn]);

            double x_stretch = (isFixedXStretch ? FIXED_X_STRETCH : double.Parse(columns[xStretchColumn]));
            double y_stretch = (isFixedYStretch ? FIXED_Y_STRETCH : double.Parse(columns[yStretchColumn]));
            double rotation = (isFixedRotation ? FIXED_ROTATION : double.Parse(columns[rotationColumn]));
            double mass = (isFixedMass ? FIXED_MASS : double.Parse(columns[massColumn]));
            string label = (isNoneLabel ? FIXED_LABEL : columns[labelColumn]);

            dataPoints.Add(new DataPoint(x, y, x_stretch, y_stretch, rotation, mass, label));
        }
    } 
}
