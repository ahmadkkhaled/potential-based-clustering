using UnityEngine;
using UnityEngine.Assertions;

public class FileExplorer : MonoBehaviour
{
    public string Path { get; private set; }

    public void OpenExplorer()
    {
        Path = UnityEditor.EditorUtility.OpenFilePanel("Select Dataset", "", "csv");
        Assert.IsFalse(string.IsNullOrEmpty(Path));
    }

    public void ClearPath() { Path = ""; }
}
