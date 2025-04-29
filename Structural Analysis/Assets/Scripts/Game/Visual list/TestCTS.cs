using System.IO;
using System.Reflection;

using UnityEngine;

public class TestCTS : MonoBehaviour
{
    private const int COUNT_TEST = 3;

    private void Start()
    {
        /*Debug.Log(Assembly.GetExecutingAssembly().Location);
        DirectoryInfo directory = new(Assembly.GetExecutingAssembly().Location.Replace(@"\Library\ScriptAssemblies\Assembly-CSharp.dll", ""));
        if (directory.GetFiles().Length <= 0) return;
        foreach (FileInfo file in directory.GetFiles())
        {
            if (file.Name.EndsWith(".txt"))
            {
                CalculateMatrix(file.Name.Replace(".txt", ""));
            }
        }*/
    }
    public void CalculateMatrix(string path)
    {
        int[,] matrix = ReadMatrix(path);

        string text = GetOrderCalculation(matrix.Transposition());

        FileStream f = new($"{path}.xls", FileMode.Create);
        f.Close();
        using StreamWriter writer = new($"{path}.xls");
        writer.Write(text);
    }

    public static int[,] ReadMatrix(string path)
    {
        using StreamReader reader = new($"{path}.txt");
        
        int count = -1;
        int y = 0;
        int[,] matrix = null;
        while (!reader.EndOfStream)
        {
            string[] line = reader.ReadLine()!.Split('\t');
            if (count == -1)
            {
                count = line.Length;
                matrix = new int[count, count];
            }

            for (int x = 0; x < count; x++)
                matrix![x, y] = line[x].ToInt();
            y++;
        }

        return matrix;
    }

    private string GetOrderCalculation(int[,] matrix) => new StructuralAnalysisCts().StructuralAnalysisChemicalTechnologicalSystems(matrix);

}
